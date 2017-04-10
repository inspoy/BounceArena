/**
 * Created by inspoy on 2017/4/7.
 */

"use strict";

const events = require("events");
const userData = require("./../Data/SFUserData");
const battleData = require("./../Data/SFBattleData");
const utils = require("./../Conf/SFUtils");
const errCode = require("./../Conf/SFCommonConf").errCode;
const logInfo = utils.logInfo;
const dispatcher = utils.eventDispatcher;
let m_pusher = null;

/**
 * 处理请求
 * @param {object} req
 */
const onRequest = function (req) {
    if (req.pid == 3) {
        // 用户上传操作信息
        let retCode = 0;
        do {
            if (!userData.onlineUserList.hasOwnProperty(req.uid)) {
                retCode = errCode.userNotJoin;
                break;
            }
            const battleId = userData.onlineUserList[req.uid].battleId;
            if (!battleData.battleList.hasOwnProperty(battleId)) {
                retCode = errCode.battleIdNotExist;
                break;
            }
            const battle = battleData.battleList[battleId];
            if (!battle.users.hasOwnProperty(req.uid)) {
                retCode = errCode.userNotJoin;
                break;
            }
            const userItem = battle.users[req.uid];
            userItem.accX = req["moveX"] * userItem.accPower;
            userItem.accY = req["moveY"] * userItem.accPower;
            userItem.rotation = req.rotation;
        } while (false);
        const resp = {pid: 2, retCode: retCode};
        m_pusher([req.uid], JSON.stringify(resp));
    } // end of if(pid == 3)
};

/**
 * 创建一场战斗
 * @returns {string} battleId
 */
const createBattle = function () {
    const battle = new battleData.Battle();
    battle.battleId = utils.getRandomString("battle_", 5);
    battleData.battleList[battle.battleId] = battle;
    logInfo("创建了一场新的战斗: " + battle.battleId);
    return battle.battleId;
};

/**
 * 固定时间更新，处理逻辑
 */
const onUpdate = function () {
    // dt = 40ms
    const dt = 0.04;
    const battleList = battleData.battleList;
    for (const battleId in battleList) {
        if (battleList.hasOwnProperty(battleId)) {
            /**
             * battle为SFBattleData.Battle的一个实例
             * @see battleData.Battle
             */
            const battle = battleList[battleId];
            // 更新坐标
            for (const uid in battle.users) {
                if (battle.users.hasOwnProperty(uid)) {
                    const userItem = battle.users[uid];
                    userItem.speedX += userItem.accX * dt;
                    userItem.speedY += userItem.accY * dt;
                    // 限制最高速度
                    const k = calcSpeedLimit(userItem.speedX, userItem.speedY, userItem.topSpeed);
                    userItem.speedX *= k;
                    userItem.speedY *= k;
                    userItem.posX += userItem.speedX * dt;
                    userItem.posY += userItem.speedY * dt;
                    if (userItem.accX <= 0 && userItem.accY <= 0) {
                        // 如果当前没有移动，速度均匀衰减
                        userItem.speedX *= 0.9;
                        userItem.speedY *= 0.9;
                    }
                }
            }

            // TODO: 更新碰撞

            // TODO: 更新火球...

            // 推送数据给所有用户
            let users = [];
            let infos = [];
            for (const uid in battle.users) {
                if (battle.users.hasOwnProperty(uid)) {
                    const userItem = battle.users[uid];
                    users.push(uid);
                    infos.push({
                        uid: userItem.uid,
                        posX: userItem.posX,
                        posY: userItem.posY,
                        rotation: userItem.rotation,
                        speedX: userItem.speedX,
                        speedY: userItem.speedY,
                        skillId: 0
                    });
                }
            }
            const resp = {
                pid: 4,
                retCode: 0,
                reqId: battle.getReqId(),
                infos: infos
            };
            if (users.length > 0) {
                m_pusher(users, JSON.stringify(resp));
            }
        }
    }
};

/**
 * 用户加入时更新数据并推送给他当前场上的信息
 */
const onUserLogin = function (data) {
    const uid = data.uid;
    const battleId = data.battleId;
    let ok = false;
    let retCode = 0;
    if (battleData.battleList.hasOwnProperty(battleId)) {
        logInfo(`用户"${uid}"加入了战场${battleId}`);
        if (userData.onlineUserList.hasOwnProperty(uid)) {
            userData.onlineUserList[uid].battleId = battleId;
            const battle = battleData.battleList[battleId];
            const posX = Math.random() * 20 - 10;
            const posY = Math.random() * 20 - 10;
            retCode = battle.addUnit(uid, posX, posY);
            if (retCode == 0) {
                const resp = {
                    pid: 2,
                    retCode: retCode,
                    mapId: battle.mapId,
                    users: battle.getUserList(uid),
                    posX: posX,
                    posY: posY,
                    rotation: 0
                };
                m_pusher([uid], JSON.stringify(resp));
                ok = true;
            }
            else {
                logInfo("加入战场失败");
                // retCode = userAlreadyJoin;
            }
        }
        else {
            logInfo("用户未登录:" + uid);
            retCode = errCode.userNotLogin;
        }
    }
    else {
        logInfo("不存在的battleId:" + battleId);
        retCode = errCode.battleIdNotExist;
    }
    if (!ok) {
        const resp = {
            pid: 2,
            retCode: retCode,
            mapId: 0,
            users: [],
            posX: 0,
            posY: 0,
            rotation: 0
        };
        m_pusher([uid], JSON.stringify(resp));
    }
    dispatcher.emit("onBattleStart", uid);
};

const onUserLogout = function (data) {
    const uid = data.uid;
    const battleId = data.battleId;
    logInfo(`用户"${uid}"离开了战场${battleId}`);
    battleData.battleList[battleId].removeUnit(uid);
    dispatcher.emit("onBattleStart", uid);
};

/**
 * 为了控制速度上限，计算修正值
 * @param {number} speedX
 * @param {number} speedY
 * @param {number} topSpeed
 * @returns {number} 修正值，乘以修正前的速度即可获得修正后的速度
 */
const calcSpeedLimit = function (speedX, speedY, topSpeed) {
    const curSpeed = speedX * speedX + speedY * speedY; // squared speed
    if (curSpeed < topSpeed * topSpeed) {
        return 1;
    }
    else {
        return topSpeed * topSpeed / curSpeed;
    }
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
