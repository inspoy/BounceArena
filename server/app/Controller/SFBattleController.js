/**
 * Created by inspoy on 2017/4/7.
 */

"use strict";

const events = require("events");
const commonConf = require("./../Conf/SFCommonConf");
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
        onUserSyncInfo(req);
    }
};

/**
 * 创建一场战斗
 * @returns {string} battleId
 */
const createBattle = function () {
    const battle = new battleData.Battle();
    battle.battleId = utils.getRandomString("battle_", 5);
    battle.walls = [
        {
            posX: 0,
            posY: -15,
            length: 32,
            normalX: 0,
            normalY: 1
        },
        {
            posX: -15,
            posY: 0,
            length: 32,
            normalX: 1,
            normalY: 0
        },
        {
            posX: 0,
            posY: 15,
            length: 32,
            normalX: 0,
            normalY: -1
        },
        {
            posX: 15,
            posY: 0,
            length: 32,
            normalX: -1,
            normalY: 0
        }
    ]; // TODO: 临时先弄4面墙，之后再改成配置
    battleData.battleList[battle.battleId] = battle;
    logInfo("创建了一场新的战斗: " + battle.battleId);
    return battle.battleId;
};

/**
 * 用户同步操作信息
 * @param req
 */
const onUserSyncInfo = function (req) {
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
        userItem.rotation = req["rotation"];
        userItem.skillId = req["skillId"];
    } while (false);
    const resp = {pid: 3, retCode: retCode};
    m_pusher([req.uid], JSON.stringify(resp));
};

/**
 * 固定时间更新，处理逻辑
 */
const onUpdate = function () {
    // dt = 50ms
    const dt = 0.05;
    const startTime = new Date();
    const battleList = battleData.battleList;
    for (const battleId in battleList) {
        if (battleList.hasOwnProperty(battleId)) {
            /**
             * battle为SFBattleData.Battle的一个实例
             * @see battleData.Battle
             */
            const battle = battleList[battleId];
            (function (battle) {
                battle.runTime += 40;
                // 更新坐标
                utils.traverse(battle.users, function (userItem) {
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
                        if (Math.abs(userItem.speedX) < 0.01) {
                            userItem.speedX = 0;
                        }
                        if (Math.abs(userItem.speedY) < 0.01) {
                            userItem.speedY = 0;
                        }
                    }
                });

                // 更新角色碰撞
                utils.traverse(battle.users, function (userItem) {
                    utils.traverse(battle.walls, function (wallItem) {
                        // todo
                    });
                });

                // 更新释放技能
                utils.traverse(battle.users, function (userItem) {
                    if (userItem.skillId != 0) {
                        logInfo(`角色${userItem.uid}释放了技能${userItem.skillId}`);
                        if (userItem.skillId & 1 > 0) {
                            // 火球
                            userItem.skillData = utils.getRandomString("ball_", 5);
                            battle.addBall(
                                userItem.skillData,
                                userItem.posX,
                                userItem.posY,
                                userItem.speedX,
                                userItem.speedY,
                                userItem.rotation
                            );
                        }
                    }
                });

                // 更新火球移动
                utils.traverse(battle.balls, function (ballItem, ballId) {
                    ballItem.speedX += ballItem.accX * dt;
                    ballItem.speedY += ballItem.accY * dt;
                    const k = calcSpeedLimit(ballItem.speedX, ballItem.speedY, ballItem.topSpeed);
                    if (k < 1) {
                        // 已经达到速度上限，去掉加速度
                        ballItem.accX = 0;
                        ballItem.accY = 0;
                    }
                    ballItem.posX += ballItem.speedX * dt;
                    ballItem.posY += ballItem.speedY * dt;
                });

                // TODO: 更新火球碰撞

                // TODO: 推送事件，如火球爆炸等

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
                            skillId: userItem.skillId,
                            skillData: userItem.skillData
                        });
                    }
                }

                // 清理技能数据
                utils.traverse(battle.users, function (userItem) {
                    if (userItem.skillId != 0) {
                        userItem.skillId = 0;
                        userItem.skillData = "";
                    }
                });

                const resp = {
                    pid: 4,
                    retCode: 0,
                    runTime: battle.runTime,
                    infos: infos
                };
                if (users.length > 0) {
                    m_pusher(users, JSON.stringify(resp));
                }
            })(battle);
        }
    }
    const endTime = new Date();
    const costTime = endTime - startTime;
    if (costTime > 40) {
        logInfo(`BattleController.update()耗时过多: ${costTime}ms`, -1);
    }
    if (costTime > 1) {
        logInfo("costTime of update(): " + costTime, 3)
    }
    battleData.updateCost = costTime;
};

/**
 * 用户加入时更新数据并推送给他当前场上的信息
 */
const onUserLogin = function (data) {
    const uid = data.uid;
    const battleId = data.battleId;
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
                    runTime: battle.runTime,
                    users: battle.getUserList(uid),
                    posX: posX,
                    posY: posY,
                    rotation: 0
                };
                m_pusher([uid], JSON.stringify(resp));
            }
            else {
                logInfo("加入战场失败", commonConf.logLevel_warning);
                // retCode = userAlreadyJoin;
            }
        }
        else {
            logInfo("用户未登录:" + uid, commonConf.logLevel_warning);
            retCode = errCode.userNotLogin;
        }
    }
    else {
        logInfo("不存在的battleId:" + battleId, commonConf.logLevel_warning);
        retCode = errCode.battleIdNotExist;
    }
    if (retCode != 0) {
        const resp = {
            pid: 2,
            retCode: retCode,
            mapId: 0,
            runTime: 0,
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
    const curSpeed = speedX * speedX + speedY * speedY; // squared
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
