/**
 * Created by inspoy on 2017/4/7.
 */

"use strict";

const events = require("events");
const userData = require("./../Data/SFUserData");
const battleData = require("./../Data/SFBattleData");
const utils = require("./../Conf/SFUtils");
const logInfo = utils.logInfo;
const dispatcher = utils.eventDispatcher;
let m_pusher = null;

/**
 * 处理请求
 * @param {object} req
 */
const onRequest = function (req) {
    if (req.pid == 1) {
        // 用户登陆登出
        if (req["loginOrOut"] == 1) {
            userData.onlineUserList[req.uid] = {};
            logInfo("用户登陆：" + req.uid);
        }
        else {
            delete userData.onlineUserList[req.uid];
            logInfo("用户登出：" + req.uid);
        }
        process.send({type: "onUserLogin", data: req.uid});
        process.on("onBattleStart", function () {
            m_pusher([req.uid], '{"pid":1,"retCode":0}');
        });
    }
};

/**
 * 创建一场战斗
 * @returns {string} battleId
 */
const createBattle = function () {
    const battle = new battleData.Battle();
    battle.battleId = utils.getRandomString("battle_", 5);
    logInfo("创建了一场新的战斗: " + battle.battleId);
    return battle.battleId;
};

/**
 * 固定时间更新，处理逻辑
 */
const onUpdate = function () {
    // dt = 40ms
    const battleList = battleData.battleList;
    for (let i = 0; i < battleList.length; ++i) {
        const battle = battleList[i];

    }
};

/**
 * 用户加入时更新数据并推送给他当前场上的信息
 */
const onUserLogin = function (data) {
    const uid = data.uid;
    const battleId = data.battleId;
    logInfo(`用户"${uid}"加入了战场${battleId}`);
    const resp = {
        pid: 2,
        mapId: 0,
        users: [],
        posX: 0,
        posY: 0,
        rotation: 0
    };
    m_pusher([uid], JSON.stringify(resp));
    dispatcher.emit("onBattleStart", uid);
};

const onUserLogout = function (data) {
    const uid = data.uid;
    const battleId = data.battleId;
    logInfo(`用户"${uid}"离开了战场${battleId}`);
    dispatcher.emit("onBattleStart", uid);
};

dispatcher.addListener("onUserLogin", onUserLogin);
dispatcher.addListener("onUserLogout", onUserLogout);
setInterval(onUpdate, 40);

module.exports = {
    createBattle: createBattle,
    onRequest: onRequest,
    setPusher: function (pusher) {
        m_pusher = pusher;
    }
};
