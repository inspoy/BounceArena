/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";

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
            dispatcher.emit("onUserLogin", {uid: req.uid, battleId: battleData.battleId});
        }
        else {
            delete userData.onlineUserList[req.uid];
            logInfo("用户登出：" + req.uid);
            dispatcher.emit("onUserLogout", {uid: req.uid, battleId: battleData.battleId});
        }
    }
};

const onBattleStart = function (uid) {
    m_pusher([uid], '{"pid":1,"retCode":0}');
};

dispatcher.addListener("onBattleStart", onBattleStart);

module.exports = {
    onRequest: onRequest,
    setPusher: function (pusher) {
        m_pusher = pusher;
    }
};
