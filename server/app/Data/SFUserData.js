/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";

class OnlineUser {
    constructor(uid) {
        // uid
        this.uid = uid;
        // 登陆时间，以毫秒为单位的unix时间戳
        this.loginTime = new Date().getTime();
        // 加入的战场id
        this.battleId = "";
    }
}

module.exports = {
    OnlineUser: OnlineUser,
    onlineUserList: {}
};