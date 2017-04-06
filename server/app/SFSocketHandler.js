/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";
const net = require("net");
const commonConf = require("./Conf/SFCommonConf");
const socketData = require("./Data/SFSocketData");

const logInfo = function (msg) {
    if (msg) {
        const msgObj = {
            type: "LOG",
            data: msg.toString()
        };
        process.send(msgObj);
    }
};

const onSocket = function (socket) {
    logInfo("有新的连接:");
    logInfo("- address: " + socket.remoteAddress);
    logInfo("-    port: " + socket.remotePort);
    socket.uid = "";
    socket.dataBuffer = "";
    socket.setTimeout(30 * 1000); // 30s超时
    socketData.socketList.push(socket);

    socket.on("data", function (data) {
        console.log("request: " + data);
        socket.write('{"pid":1,"retCode":0}');
    });

    socket.on("end", function () {
        let uid = "";
        let found = false;
        for (let i = 0; i < socketData.socketList.length; ++i) {
            const item = socketData.socketList[i];
            if (socket == item) {
                found = true;
                uid = item.uid;
                socketData.socketList.splice(i, 1);
                break;
            }
        }
        if (found) {
            if (uid == "") {
                uid = "unknown";
            }
            else {
                // 找到了uid，说明这个user已经登陆但是意外掉线，告诉GameServer用户登出
                const data = `{"pid":1,"uid":${uid},"loginOrOut":2}`;
                processProtocol(1, uid, data)
            }
        }
        logInfo("连接已断开: " + uid);
    });

    socket.on("timeout", function () {
        logInfo("连接超时: " + ((socket.uid == "") ? "unknown" : socket.uid));
        socket.end();
    });

    socket.on("close", function (had_error) {
        if (had_error) {
            logInfo("Socket closed with error.")
        }
    });

    socket.on("error", function (err) {
        logInfo("Socket error: " + err)
    });
};

const removeSocketWithUid = function (uid) {
    if (uid) {
        for (let i = 0; i < socketData.socketList.length; ++i) {
            const item = socketData.socketList[i];
            if (item.uid == uid) {
                item.end();
            }
        }
    }
};

const processProtocol = function (pid, uid, jsonString) {
    if (pid > 0) {
        process.send({type: "REQ", data: jsonString});
    }
    else {
        // pid <= 0为socket层的协议，无需GameServer处理
        if (pid == -1) {
            // 心跳包
        }
    }
};

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