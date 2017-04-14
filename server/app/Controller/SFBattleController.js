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
        userItem.accX = req["moveX"] * userItem.accPower / userItem.mass;
        userItem.accY = req["moveY"] * userItem.accPower / userItem.mass;
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
                utils.traverse(battle.balls, function (ballItem) {
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
                    ballItem.life -= dt;
                    ballItem.invincibleTime -= dt;
                });

                // 计算碰撞
                const colliders = {};
                Object.assign(colliders, battle.users, battle.walls, battle.balls);
                utils.traverse(colliders, function (item1) {
                    let id1 = "";
                    let type1 = "";
                    if (item1.hasOwnProperty("uid")) {
                        id1 = item1.uid;
                        type1 = "user";
                    }
                    else if (item1.hasOwnProperty("ballId")) {
                        id1 = item1.ballId;
                        type1 = "ball";
                    }
                    else if (item1.hasOwnProperty("wallId")) {
                        id1 = item1.wallId;
                        type1 = "wall";
                    }
                    if (id1 == "") {
                        return;
                    }
                    utils.traverse(colliders, function (item2) {
                        let id2 = "";
                        let type2 = "";
                        if (item2.hasOwnProperty("uid")) {
                            id2 = item2.uid;
                            type2 = "user";
                        }
                        else if (item2.hasOwnProperty("ballId")) {
                            id2 = item2.ballId;
                            type2 = "ball";
                        }
                        else if (item2.hasOwnProperty("wallId")) {
                            id2 = item2.wallId;
                            type2 = "wall";
                        }
                        if (id2 == "") {
                            return;
                        }
                        if (type1 == type2 && id1 >= id2) {
                            return;
                        }
                        checkCollision(battle, item1, id1, type1, item2, id2, type2, dt);
                    });
                });

                // 推送数据
                let users = [];
                let infos = [];
                let ballsInfo = [];
                utils.traverse(battle.balls, function (ballItem) {
                    ballsInfo.push({
                        ballId: ballItem.ballId,
                        posX: ballItem.posX,
                        posY: ballItem.posY,
                        speedX: ballItem.speedX,
                        speedY: ballItem.speedY,
                        explode: ballItem.explode
                    });
                });
                utils.traverse(battle.users, function (userItem) {
                    users.push(userItem.uid);
                    infos.push({
                        uid: userItem.uid,
                        posX: userItem.posX,
                        posY: userItem.posY,
                        rotation: userItem.rotation,
                        speedX: userItem.speedX,
                        speedY: userItem.speedY,
                        life: userItem.life,
                        skillId: userItem.skillId,
                        skillData: userItem.skillData
                    });
                });

                const resp = {
                    pid: 4,
                    retCode: 0,
                    runTime: battle.runTime,
                    infos: infos,
                    balls: ballsInfo
                };
                if (users.length > 0) {
                    m_pusher(users, JSON.stringify(resp));
                }

                // 清理状态数据
                // 清理技能释放状态
                utils.traverse(battle.users, function (userItem) {
                    if (userItem.skillId != 0) {
                        userItem.skillId = 0;
                        userItem.skillData = "";
                    }
                });
                // 清理爆炸了的火球
                utils.traverse(battle.balls, function (ballItem) {
                    if (ballItem.explode == true) {
                        delete battle.balls[ballItem.ballId];
                    }
                });
                // 清理被击败的角色
                utils.traverse(battle.users, function (userItem) {
                    if (userItem.life <= 0)
                    {
                        delete battle.users[userItem.uid];
                    }
                });
            })(battle);
        }
    }
    const endTime = new Date();
    const costTime = endTime - startTime;
    if (costTime > 40) {
        logInfo(`BattleController.update()耗时过多: ${costTime}ms`, -1);
    }
    if (costTime > 1) {
        logInfo("costTime of update(): " + costTime, 3);
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
            const users = [];
            utils.traverse(battle.users, function (userItem) {
                users.push(userItem.uid);
            });
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
                // 通知其他玩家有新玩家加入
                const resp2 = {
                    pid: 5,
                    retCode: 0,
                    inOrOut: true,
                    uid: uid,
                    posX: posX,
                    posY: posY,
                    rotation: 0
                };
                m_pusher(users, JSON.stringify(resp2));
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
    const battle = battleData.battleList[battleId];
    battle.removeUnit(uid);
    const users = [];
    utils.traverse(battle.users, function (userItem) {
        users.push(userItem.uid);
    });
    logInfo("通知" + users + "，用户" + uid + "离开");
    // 通知其他玩家有玩家离开
    const resp2 = {
        pid: 5,
        retCode: 0,
        inOrOut: false,
        uid: uid,
        posX: 0,
        posY: 0,
        rotation: 0
    };
    m_pusher(users, JSON.stringify(resp2));
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

/**
 * 进行碰撞检测和处理
 * @param {object} battle
 * @param {object} item1
 * @param {string} id1
 * @param {string} type1
 * @param {object} item2
 * @param {string} id2
 * @param {string} type2
 * @param {number} dt
 */
const checkCollision = function (battle, item1, id1, type1, item2, id2, type2, dt) {
    if (type1 == "user") {
        if (type2 == "user") {
            // 角色和角色
            const distance = utils.getDistance1(item1.posX, item1.posY, item2.posX, item2.posY);
            if (distance < item1.size + item2.size) {
                onUserEnterUser(item1, item2, dt);
            }
        }
        else if (type2 == "wall") {
            // 角色和墙
            const NX = item2.normalX;
            const NY = item2.normalY;
            const threshold = item2.width / 2 + item1.size;
            const PX = item1.posX - item2.posX;
            const PY = item1.posY - item2.posY;
            const PNDot = utils.vector2Dot(PX, PY, NX, NY);
            const PPX = PNDot * NX;
            const PPY = PNDot * NY;
            if (utils.vector2Dot(PPX, PPY, NX, NY) > 0 &&
                PPX * PPX + PPY * PPY < threshold * threshold) {
                // 小于厚度
                onUserEnterWall(item1, item2);
            }
        }
    }
    else if (type1 == "ball" && item1.explode == false) {
        if (type2 == "user") {
            // 球和角色
            const distance = utils.getDistance1(item1.posX, item1.posY, item2.posX, item2.posY);
            if (distance < item1.size + item2.size) {
                onBallEnterUser(item1, item2);
            }
        }
        else if (type2 == "wall") {
            // 球和墙
            // 方案：如果这一帧有碰撞，则修正速度，再演算一帧，确保下一帧不会碰撞，不行的话再改
            const NX = item2.normalX;
            const NY = item2.normalY;
            const threshold = item2.width / 2 + item1.size;
            const PX = item1.posX - item2.posX;
            const PY = item1.posY - item2.posY;
            const PNDot = utils.vector2Dot(PX, PY, NX, NY);
            const PPX = PNDot * NX;
            const PPY = PNDot * NY;
            if (utils.vector2Dot(PPX, PPY, NX, NY) > 0 &&
                PPX * PPX + PPY * PPY < threshold * threshold) {
                onBallEnterWall(item1, item2, dt);
            }
        }
        else if (type2 == "ball") {
            // 球和球
            const distance = utils.getDistance1(item1.posX, item1.posY, item2.posX, item2.posY);
            if (distance < item1.size + item2.size) {
                onBallEnterBall(item1, item2);
            }
        }
    }
    // 墙因为是静态的所以不会主动和其他物体发生碰撞
};

/**
 * 当角色碰撞到角色
 * @param user1
 * @param user2
 * @param dt
 */
const onUserEnterUser = function (user1, user2, dt) {
    // 碰撞到其他角色 - 交换速度，根据动量和能量守恒
    // v1'=((m1-m2)v1+2m2v2)/(m1+m2) v2'=((m2-m1)v2+2m1v1)/(m1+m2)
    const m1 = user1.mass;
    const m2 = user2.mass;
    const v1X = user1.speedX;
    const v1Y = user1.speedY;
    const v2X = user2.speedX;
    const v2Y = user2.speedY;
    user1.speedX = ((m1-m2)*v1X+2*m2*v2X)/(m1+m2);
    user1.speedY = ((m1-m2)*v1Y+2*m2*v2Y)/(m1+m2);
    user2.speedX = ((m2-m1)*v2X+2*m1*v1X)/(m1+m2);
    user2.speedY = ((m2-m1)*v2Y+2*m1*v1Y)/(m1+m2);

    user1.posX += user1.speedX * dt;
    user1.posY += user1.speedY * dt;
    user2.posX += user2.speedX * dt;
    user2.posY += user2.speedY * dt;

    // SFUnit unit1 = this;
    // SFUnit unit2 = col.gameObject.GetComponent<SFUnit>();
    // m_nextSpeed = (
    //         (unit1.getMass() - unit2.getMass()) * unit1.getCurSpeed()
    //         + 2 * unit2.getMass() * unit2.getCurSpeed()
    //     ) / (unit1.getMass() + unit2.getMass());
};

/**
 * 当角色碰撞到墙
 * @param user
 * @param wall
 */
const onUserEnterWall = function (user, wall) {
    // 墙壁法线方向的速度归零 R'=I-(IN)N
    const NX = wall.normalX;
    const NY = wall.normalY;
    const IX = user.speedX;
    const IY = user.speedY;
    const INDot = utils.vector2Dot(IX, IY, NX, NY);
    const RX = IX - INDot * NX;
    const RY = IY - INDot * NY;
    user.speedX = RX;
    user.speedY = RY;
    // 修正边界
    const threshold = wall.width / 2 + user.size;
    const PX = user.posX - wall.posX;
    const PY = user.posY - wall.posY;
    const PNDot = utils.vector2Dot(PX, PY, NX, NY);
    const PPX = PNDot * NX;
    const PPY = PNDot * NY;
    if (utils.vector2Dot(PPX, PPY, NX, NY) > 0 &&
        PPX * PPX + PPY * PPY < threshold * threshold) {
        // 修正坐标
        const OAX = threshold * NX;
        const OAY = threshold * NY;
        const OBX = PX + OAX - PPX;
        const OBY = PY + OAY - PPY;
        user.posX = wall.posX + OBX;
        user.posY = wall.posY + OBY;
    }

    /*
     Vector3 N = wall.getNormal();
     Vector3 I = m_curSpeed;
     Vector3 R = I - Vector3.Dot(I, N) * N;
     float threhold = wall.transform.localScale.z / 2 + m_bodySize;
     // R是反弹方向
     m_curSpeed = R;
     // 判断边界
     // transform.position 在 wall 的法线方向上的投影应当大于等于 wall.transform.localScale.z/2
     // P'=(PN)N
     Vector3 P = transform.position - wall.transform.position;
     Vector3 pp = Vector3.Dot(P, N) * N;
     // pp是N上的投影，OP'
     if (Vector3.Dot(pp, N) < 0 || pp.sqrMagnitude < threhold)
     {
     // unit在wall里面，应该修正坐标
     Vector3 OA = threhold * N;
     Vector3 OB = P + OA - pp;
     transform.position = wall.transform.position + OB;
     }
     */
};

/**
 * 当球碰撞到墙
 * @param ball
 * @param wall
 * @param dt
 */
const onBallEnterWall = function (ball, wall, dt) {
    if (ball.life < 0) {
        ball.explode = true;
        return;
    }
    const NX = wall.normalX;
    const NY = wall.normalY;
    const IX = ball.speedX;
    const IY = ball.speedY;
    ball.speedX = IX - 2 * utils.vector2Dot(IX, IY, NX, NY) * NX;
    ball.speedY = IY - 2 * utils.vector2Dot(IX, IY, NX, NY) * NY;
    ball.accX = 0;
    ball.accY = 0;
    ball.posX += ball.speedX * dt;
    ball.posY += ball.speedY * dt;
    /*
     // R = I - 2(I*N)N
     SFWall wall = col.gameObject.GetComponent<SFWall>();
     Vector3 N = wall.getNormal();
     Vector3 I = m_curSpeed;
     Vector3 R = I - 2 * Vector3.Dot(I, N) * N;
     m_curSpeed = R;
     // 撞墙也去掉加速度
     m_curAcc = Vector3.zero;
     */
};

/**
 * 当球碰撞到角色
 * @param ball
 * @param user
 */
const onBallEnterUser = function (ball, user) {
    if (ball.invincibleTime > 0) {
        return;
    }
    ball.explode = true;
    user.life -= ball.power;
    const PX = user.posX - ball.posX;
    const PY = user.posY - ball.posY;
    const dis = Math.sqrt(PX * PX + PY * PY);
    const NX = PX / dis;
    const NY = PY / dis;
    user.speedX = NX * ball.power / user.mass;
    user.speedY = NY * ball.power / user.mass;
};

/**
 * 当球碰撞到球
 * @param ball1
 * @param ball2
 */
const onBallEnterBall = function (ball1, ball2) {
    if (ball1.invincibleTime > 0 || ball2.invincibleTime > 0) {
        return;
    }
    ball1.explode = true;
    ball2.explode = true;
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
