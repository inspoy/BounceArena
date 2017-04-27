/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";
const errCode = {
    duplicatedLogin: 1001,      // 重复登陆
    battleIdNotExist: 2001,     // 不存在的battleId
    userNotLogin: 2002,         // 用户未登录
    userAlreadyJoin: 2003,      // 用户已经加入了战场
    userNotJoin: 2004,          // 用户未加入战场
    joinNotAvailable: 2005,     // 没有合适的房间

    success: 0
};

module.exports = {
    serverPort: 19621,

    // 日志
    enableLog_SocketHandler: true,
    enableLog_GameServer: true,
    logLevel_warning: -1,
    logLevel_error: -2,
    logLevel: 1, // 允许最高的日志等级

    errCode: errCode,

    // 游戏相关配置
    updateDt: 40, // 40ms更新一次游戏逻辑
    ballAcc: 30, // 火球发射后的加速度
    ballLife: 5, // 火球寿命
    ballInvincibleTime: 0.5, // 无敌时间
    roomUserLimit: 6, // 每个房间能容纳的最大玩家数量

    __more__: 0
};