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

    success: 0
};

module.exports = {
    serverPort: 19621,

    // 日志
    enableLog_SocketHandler: true,
    enableLog_GameServer: true,
    logLevel: 10, // 允许最高的日志等级

    errCode: errCode,

    __more__: 0
};