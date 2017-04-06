/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";
const net = require("net");
const utils = require("./Conf/SFUtils");
const commonConf = require("./Conf/SFCommonConf");
const socketData = require("./Data/SFSocketData");

/**
 * 写日志
 * @param msg 日志内容
 * @param level 等级，数字越大优先级越低，默认0
 */
const logInfo = function (msg, level = 0) {
    if (msg) {
        const msgObj = {
            type: "LOG",
            level: level,
            data: msg.toString()
        };
        process.send(msgObj);
    }
};

/**
 * 有新连接时调用
 * @param {object} socket socket连接
 */
const onSocket = function (socket) {
    socket.id = utils.getRandomString("skt_");
    socket.uid = "";
    socket.dataBuffer = "";
    socket.writeBuffer = "";
    socket.writeReady = true;
    socket.setTimeout(30 * 1000); // 30s超时
    socketData.socketMap[socket.id] = socket;

    logInfo(
        "有新的连接:\n" +
        "- address: " + socket.remoteAddress + "\n" +
        "-    port: " + socket.remotePort + "\n" +
        "-      id: " + socket.id
    );
    logInfo(socket.bufferSize.toString());

    socket.on("data", function (data) {
        socket.dataBuffer += data;
        while (true) {
            const idx = socket.dataBuffer.indexOf("\r\n\r\n");
            if (idx == -1) {
                break;
            }
            const req = socket.dataBuffer.substr(0, idx);
            socket.dataBuffer = socket.dataBuffer.substr(idx + 4);
            logInfo("request: " + req, 1);
            try {
                const reqObj = JSON.parse(req);
                const pid = reqObj.pid;
                const uid = reqObj.uid;
                // 防止重复登录
                let check = true;
                if (pid == 1 && reqObj["loginOrOut"] == 1) {
                    utils.traverse(socketData.socketMap, function (item) {
                        if (item.uid == uid) {
                            check = false;
                            return true;
                        }
                    });
                }
                if (check) {
                    processRequest(pid, uid, req);
                }
                else {
                    socket.write(`{"pid":1,"retCode":commonConf.errCode.duplicatedLogin}\r\n\r\n`);
                }
            }
            catch (e) {
                logInfo(
                    "不能解析的协议: " + req + "\n" +
                    "错误信息: " + e);
            }
        }
    });

    socket.on("end", function () {
        if (socketData.socketMap.hasOwnProperty(socket.id)) {
            let uid = socketData.socketMap[socket.id].uid;
            delete socketData.socketMap[socket.id];
            if (uid == "") {
                uid = "unknown";
            }
            else {
                // 找到了uid，说明这个user已经登陆但是意外掉线，告诉GameServer用户登出
                const data = `{"pid":1,"uid":${uid},"loginOrOut":2}`;
                processRequest(1, uid, data)
            }
        }
        logInfo("连接已断开: " + socket.id);
    });

    socket.on("timeout", function () {
        logInfo("连接超时: " + ((socket.uid == "") ? "unknown" : socket.uid));
        socket.end();
    });

    socket.on("drain", function () {
        socket.writeReady = true;
        logInfo("缓冲区已清空", 2);
    });

    socket.on("close", function (had_error) {
        if (had_error) {
            logInfo("Socket closed with error.")
        }
    });

    socket.on("error", function (err) {
        logInfo("Socket error: " + err)
    });

    socket.uid = "abc";
    setInterval(function () {
        responseWithUid("abc", utils.getRandomString("", 1996));
    }, 1);
};

/**
 * 根据指定的uid移除socket对象，并断开连接
 * @param {string} uid
 */
const removeSocketWithUid = function (uid) {
    if (uid) {
        utils.traverse(socketData.socketMap, function (item) {
            if (item.uid == uid) {
                item.end();
            }
        });
    }
};

/**
 * 处理请求
 * @param {number} pid 协议号
 * @param {string} uid 用户id
 * @param {string} jsonString 请求数据
 */
const processRequest = function (pid, uid, jsonString) {
    if (pid == undefined || uid == undefined) {
        throw "Unknown pid or uid";
    }

    if (pid > 0) {
        process.send({type: "REQ", data: jsonString});
    }
    else {
        // pid <= 0为socket层的协议，无需GameServer处理
        if (pid == 0) {
            // 心跳包
        }
    }
};

/**
 * 处理GameServer发来的响应
 * @param {string} jsonString 响应数据
 */
const processResponse = function (jsonString) {
    try {
        // jsonString的格式：{user_list:["userA", "userB", ...], response_data:"{}"}
        const respObj = JSON.parse(jsonString);
        const userList = respObj["user_list"];
        const respData = respObj["response_data"];
        let count = 0;
        for (let i = 0; i < userList.length; ++i) {
            const uid = userList[i];
            count += responseWithUid(uid, respData);
        }
        if (count > 1) {
            let pid = "";
            if (respData != "__KICK__") {
                pid = JSON.parse(respData).pid;
            }
            else {
                pid = respData;
            }
            logInfo(`Responded ${pid} to ${count} user(s)`);
        }
    }
    catch (e) {
        logInfo("处理请求信息出错: " + jsonString + "\n错误信息：\n" + e);
    }
};

/**
 * 根据指定的uid推送响应数据
 * @param {string} uid
 * @param {string} respJsonString
 */
const responseWithUid = function (uid, respJsonString) {
    if (respJsonString == "__KICK__") {
        removeSocketWithUid(uid);
        return 1;
    }
    let found = 0;
    utils.traverse(socketData.socketMap, function (item) {
        if (item.uid != uid) {
            return false;
        }
        item.writeBuffer += respJsonString + "\r\n\r\n";
        if (item.writeReady) {
            const ret = item.write(item.writeBuffer);
            logInfo(`写入了${item.writeBuffer.length}字节`, 2);
            if (!ret) {
                logInfo("不是所有的数据都写入了缓冲区", 2);
                logInfo(`当前缓冲区大小:${item.bufferSize}`,2);
                item.writeReady = false;
            }
            item.writeBuffer = "";
        }
        else {
            logInfo(`缓冲区还未清空，已排队:${item.writeBuffer.length}`,2);
        }
        found = 1;
        return true;
    });
    return found;
};

/**
 * 程序入口
 */
const main = function () {
    process.on("SIGINT", function () {
        console.log("SocketHandler即将退出");
        process.exit(0);
    });

    process.on("message", function (data) {
        if (data.type == "RESP") {
            // TODO: call socket.write()
        }
    });

    // 启动TCP服务器
    const server = net.createServer(onSocket);
    server.on("error", function (err) {
        logInfo("TCP Server error: " + err);
    });
    server.listen(commonConf.serverPort);
    logInfo("SocketHandler Started");
};

main();