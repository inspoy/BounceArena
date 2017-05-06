/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";

const userData = require("./../Data/SFUserData");
const battleData = require("./../Data/SFBattleData");
const utils = require("./../Conf/SFUtils");
const commonConf = require("./../Conf/SFCommonConf");
const logInfo = utils.logInfo;
const dispatcher = utils.eventDispatcher;
let m_pusher = null;

/**
 * 处理请求
 * @param {object} req
 */
const onRequest = function (req) {
    if (req.pid == 1) {
        onUserLogin(req);
    }
    else if (req.pid == 6) {
        onJoinRoom(req);
    }
};

/**
 * 用户登陆登出
 * @param req
 */
const onUserLogin = function (req) {
    if (req["loginOrOut"] == 1) {
        userData.onlineUserList[req.uid] = new userData.OnlineUser(req.uid);
        logInfo("用户登陆：" + req.uid);
        m_pusher([req.uid], '{"pid":1,"retCode":0}');
    }
    else {
        let battleId = "";
        if (userData.onlineUserList.hasOwnProperty(req.uid)) {
            battleId = userData.onlineUserList[req.uid].battleId;
        }
        delete userData.onlineUserList[req.uid];
        logInfo("用户登出：" + req.uid);
        if (battleId != "") {
            dispatcher.emit("onUserLogout", {uid: req.uid, battleId: battleId});
        }
        else {
            onLogoutResult(req.uid);
        }
    }
};

/**
 * 加入房间
 * @param req
 */
const onJoinRoom = function (req) {
    if (!userData.onlineUserList.hasOwnProperty(req.uid)) {
        // 用户未登录
        m_pusher([req.uid], JSON.stringify({
            pid: 6,
            retCode: commonConf.errCode.userNotLogin,
            roomId: ""
        }));
        return;
    }
    // 先寻找可用的房间
    let found = false;
    let battleId = "";
    utils.traverse(battleData.battleList, function (battleItem) {
        if (Object.keys(battleItem.users).length == 0 < commonConf.roomUserLimit) {
            found = true;
            battleId = battleItem.battleId;
        }
    });
    if (!found) {
        // 没有找到可用的房间，创建一个新的
        battleId = createBattle();
    }
    dispatcher.emit("onUserLogin", {uid: req.uid, battleId: battleId});
};

const onBattleStart = function (uid) {
    m_pusher([uid], JSON.stringify({
        pid: 6,
        retCode: 0,
        roomId: userData.onlineUserList[uid].battleId
    }));
};

/**
 * 创建一场战斗
 * @returns {string} battleId
 */
const createBattle = function () {
    const battle = new battleData.Battle();
    battle.battleId = utils.getRandomString("battle_", 5);
    battle.walls = {
        wall1: {
            wallId: "wall1",
            posX: 0,
            posY: -16,
            length: 32,
            width: 1,
            normalX: 0,
            normalY: 1
        },
        wall2: {
            wallId: "wall2",
            posX: -16,
            posY: 0,
            length: 32,
            width: 1,
            normalX: 1,
            normalY: 0
        },
        wall3: {
            wallId: "wall3",
            posX: 0,
            posY: 16,
            length: 32,
            width: 1,
            normalX: 0,
            normalY: -1
        },
        wall4: {
            wallId: "wall4",
            posX: 16,
            posY: 0,
            length: 32,
            width: 1,
            normalX: -1,
            normalY: 0
        }
    }; // TODO: 临时先弄4面墙，之后再改成配置
    battleData.battleList[battle.battleId] = battle;
    logInfo("创建了一场新的战斗: " + battle.battleId);
    return battle.battleId;
};

const onLogoutResult = function (uid) {
    m_pusher([uid], '{"pid":1,"retCode":0}');
};

dispatcher.addListener("onLogoutResult", onLogoutResult);
dispatcher.addListener("onBattleStart", onBattleStart);

module.exports = {
    onRequest: onRequest,
    setPusher: function (pusher) {
        m_pusher = pusher;
    }
};
