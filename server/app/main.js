/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";

const child_process = require("child_process");
const colors = require("colors");
const commonConfig = require("./Conf/SFCommonConf");
const utils = require("./Conf/SFUtils");
let socketHandler = null;
let gameServer = null;

const logType_SocketHandler = 1;
const logType_GameServer = 2;

let isSocketHandlerRunning = false;
let isGameServerRunning = false;

const log = function (type, str, level) {
    if (level <= commonConfig.logLevel) {
        const timeNow = new Date();
        const timeStr = timeNow.Format("yy-MM-dd hh:mm:ss.S - ");
        let typeStr = type == logType_SocketHandler ? "[SocketHandler]".cyan : "[ Game Server ]".blue;
        let typeStr2 = "";
        if (level == commonConfig.logLevel_warning) {
            typeStr2 = "[WARNING]".yellow;
        }
        else if (level == commonConfig.logLevel_error) {
            typeStr2 = "[ERROR]".red;
        }
        else {
            typeStr2 = "[INFO]".green;
        }
        console.log(timeStr.white + typeStr + typeStr2 + " - " + str.white);
    }
};

const main = function () {
    try {
        socketHandler = child_process.fork(__dirname + "/SFSocketHandler.js");
        socketHandler.on("message", function (msg) {
            if (msg.type == "LOG") {
                log(logType_SocketHandler, msg.data, msg.level);
            }
            else if (msg.type == "REQ") {
                // 转发消息
                if (gameServer != null) {
                    gameServer.send(msg);
                }
            }
        });
        socketHandler.on('error', function (err) {
            log(logType_SocketHandler, "Process Error:\n".red + err, -2);
        });
        socketHandler.on('exit', function (code, signal) {
            log(logType_SocketHandler, "Process Exit:\n" + ("code=" + code + " signal=" + signal), 0);
            isSocketHandlerRunning = false;
            onExit();
        });
        isSocketHandlerRunning = true;
    }
    catch (e) {
        log(logType_SocketHandler, "启动失败: " + e, -2);
    }

    try {
        gameServer = child_process.fork('./SFGameServer.js');
        gameServer.on('message', function (msg) {
            if (msg.type == "LOG") {
                log(logType_GameServer, msg.data, msg.level);
            }
            else if (msg.type == "RESP") {
                // 转发消息
                if (socketHandler != null) {
                    socketHandler.send(msg);
                }
            }
        });
        gameServer.on('error', function (err) {
            log(logType_GameServer, "processError:\n".red + err, -2);
        });
        gameServer.on('exit', function (code, signal) {
            log(logType_GameServer, "Process Exit:\n" + ("code=" + code + " signal=" + signal), 0);
            isGameServerRunning = false;
            onExit();
        });
        isGameServerRunning = true;
    }
    catch (e) {
        log(logType_GameServer, "启动失败: " + e, -2);
    }

    process.on("SIGINT", function () {
        console.log("\n[MAIN] - 收到信号 SIGINT, 等待退出...".magenta);
    });
};

const onExit = function () {
    if (!isSocketHandlerRunning && !isGameServerRunning) {
        console.log("[MAIN] - 子进程均安全退出，准备关闭主进程".magenta);
        process.exit(0);
    }
};

main();