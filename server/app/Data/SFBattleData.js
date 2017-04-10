/**
 * Created by inspoy on 2017/4/7.
 */

const errCode = require("./../Conf/SFCommonConf").errCode;

class Battle {
    constructor() {
        this.battleId = "";
        this.mapId = 0;
        this.users = {};
        this.walls = [];
        this.balls = [];
        this.reqId = 0;
    }

    /**
     * 获取当前角色信息
     * @returns {Array}
     */
    getUserList(myUid = "") {
        let ret = [];
        for (const uid in this.users) {
            if (myUid != uid && this.users.hasOwnProperty(uid)) {
                ret.push({
                    uid: this.users[uid].uid,
                    posX: this.users[uid].posX,
                    posY: this.users[uid].posY,
                    rotation: this.users[uid].rotation,
                    speedX: this.users[uid].speedX,
                    speedY: this.users[uid].speedY
                });
            }
        }
        return ret;
    }

    /**
     * 根据指定uid和初始位置坐标添加一个角色到战场
     * 返回0表示添加成功，否则失败
     * @param {string} uid
     * @param {number} posX
     * @param {number} posY
     * @returns {number}
     */
    addUnit(uid, posX, posY) {
        if (this.users.hasOwnProperty(uid)) {
            // 已经存在的uid
            return errCode.userAlreadyJoin;
        }
        this.users[uid] = {
            uid: uid,
            posX: posX,
            posY: posY,
            rotation: 0,
            speedX: 0,
            speedY: 0,
            topSpeed: 5,
            accX: 0,
            accY: 0,
            accPower: 10
        };
        return 0;
    }

    /**
     * 移除指定的uid
     * @param uid
     */
    removeUnit(uid) {
        if (!this.users.hasOwnProperty(uid)) {
            // 不存在的uid
            return errCode.userNotJoin;
        }
        delete this.users[uid];
        return 0;
    }

    /**
     * 获取一个递增的序列ID
     * @returns {number}
     */
    getReqId() {
        this.reqId += 1;
        return this.reqId;
    }
}

module.exports = {
    battleId: "", // TODO:临时的战斗ID，之后要改成动态创建，这个属性要删掉
    Battle: Battle,
    battleList: {}
};
