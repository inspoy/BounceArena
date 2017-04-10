/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";

const SFUserController = require("./Controller/SFUserController");
const SFBattleController = require("./Controller/SFBattleController");
const battleData = require("./Data/SFBattleData");
const utils = require("./Conf/SFUtils");
const logInfo = utils.logInfo;
const controllerMap = {};

/**
 * 收到客户端请求
 * @param {string} jsonString
 */
const onRequest = function (jsonString) {
    try {
        const reqObj = JSON.parse(jsonString);
        const pid = reqObj["pid"];
        const uid = reqObj["uid"];
        processProtocol(pid, uid, reqObj);
    }
    catch (e) {
        logInfo("解析请求失败:" + jsonString +
            "\n错误信息:" + e);
    }
};

/**
 * 根据指明的pid和uid处理请求
 * @param {number} pid
 * @param {string} uid
 * @param {object} reqObj
 */
const processProtocol = function (pid, uid, reqObj) {
    if (!pid || !uid || !reqObj) {
        throw "无效的参数";
    }

    const controller = getController(pid);
    if (controller && typeof(controller.onRequest) == "function") {
        controller.onRequest(reqObj);
    }
};

/**
 * 获得pid对应的Controller
 * @param {number} pid
 * @returns {object} the controller
 */
const getController = function (pid) {
    if (controllerMap.hasOwnProperty(pid)) {
        return controllerMap[pid];
    }
    else {
        return controllerMap[0];
    }
};

/**
 * 推送消息给客户端
 * @param {Array} users
 * @param {string} data
 */
const pushMessage = function (users, data) {
    if (users && users.length > 0) {
        logInfo(`将发送给${users.length}个用户: ${data}` ,3);
        const obj = {
            user_list: users,
            response_data: data
        };
        process.send({type: "RESP", data: JSON.stringify(obj)});
    }
};

/**
 * 初始化Controller列表
 */
const initControllers = function () {
    controllerMap[0] = {
        onRequest: function (req) {
            logInfo("不能识别的请求：" + req.pid + "from" + req.uid);
        }
    };
    controllerMap[1] = SFUserController;
    controllerMap[3] = SFBattleController;

    utils.traverse(controllerMap, function (item) {
        if (item && typeof(item.setPusher) == "function") {
            item.setPusher(pushMessage);
        }
    });
};

/**
 * 程序入口
 */
const main = function () {
    initControllers();

    // TODO: 先临时创建一场战斗
    battleData.battleId = SFBattleController.createBattle();

    process.on("SIGINT", function () {
        logInfo("GameServer即将退出");
        process.exit(0);
    });

    process.on("message", function (data) {
        if (data.type == "REQ") {
            onRequest(data.data);
        }
    });

    logInfo("GameServer Started")
};

main();