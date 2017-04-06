/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";

const userData = require("./../Data/SFUserData");
const logInfo = require("./../Conf/SFUtils").logInfo;
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
        m_pusher([req.uid], '{"pid":1,"retCode":0}');
    }
};

module.exports = {
    onRequest: onRequest,
    setPusher: function (pusher) {
        m_pusher = pusher;
    }
};
