/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";
const errCode = {
    duplicatedLogin: 1001, // 重复登陆

    success: 0
};

module.exports = {
    serverPort: 19621,

    // 日志
    enableLog_SocketHandler: true,
    enableLog_GameServer: true,
    logLevel: 1,

    errCode: errCode,

    __more__: 0
};