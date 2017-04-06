/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";
const child_process = require("child_process");
const commconConfig = require("./Conf/SFCommonConf");
let socketHandler = null;
let gameServer = null;

const main = function () {
    try {
        socketHandler = child_process.fork(__dirname + "/SFSocketHandler.js");
        socketHandler.on("message", function (msg) {
            if (msg.type == "LOG") {
                if (commconConfig.enableLog_SocketHandler) {
                    if (msg.level <= commconConfig.logLevel)
                    console.log("[SocketHandler] - " + msg.data);
                }
            }
            else if (msg.type == "REQ") {
                // 转发消息
                if (gameServer != null) {
                    gameServer.send(msg);
                }
            }
        });
        socketHandler.on('error', function (err) {
            console.log('[SocketHandler]ERROR: ' + err);
        });
        socketHandler.on('exit', function (code, signal) {
            console.log('[SocketHandler]EXITED:');
            console.log('code=' + code + ' signal=' + signal);
        });
    }
    catch (e) {
        console.log("启动SocketHandler失败:");
        console.log(e);
    }
    //
    // try {
    //     gameServer = child_process.fork('./SFGameServer.js');
    //     gameServer.on('message', function (msg) {
    //         if (msg.type == "LOG") {
    //             if (commconConfig.enableLog_GameServer) {
    //                 console.log('[ Game Server ] - ' + msg.data);
    //             }
    //         }
    //         else if (msg.type == "RESP") {
    //             // 转发消息
    //             if (socketHandler != null) {
    //                 socketHandler.send(msg);
    //             }
    //         }
    //     });
    //     gameServer.on('error', function (err) {
    //         console.log('[ Game Server ]ERROR: ' + err);
    //     });
    //     gameServer.on('exit', function (code, signal) {
    //         console.log('[ Game Server ]EXITED:');
    //         console.log('code=' + code + ' signal=' + signal);
    //     });
    // }
    // catch (e) {
    //     console.log('启动GameServer失败');
    //     console.log(e);
    // }

    process.on("SIGINT", onExit);
};

const onExit = function () {
    console.log("\n[MAIN] - Received SIGINT, prepare to exit...");
    process.exit(0);
};

main();