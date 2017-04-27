/**
 * Created by inspoy on 2017/4/7.
 */

const commonConf = require("./../Conf/SFCommonConf");
const errCode = commonConf.errCode;

class Battle {
    constructor() {
        this.battleId = "";
        this.mapId = 0;
        this.users = {};
        this.walls = {};
        this.balls = {};
        this.runTime = 0;
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
                    uid:        this.users[uid].uid,
                    posX:       this.users[uid].posX,
                    posY:       this.users[uid].posY,
                    rotation:   this.users[uid].rotation,
                    speedX:     this.users[uid].speedX,
                    speedY:     this.users[uid].speedY,
                    life:       this.users[uid].life,
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
            topSpeed: 5,        // 最大速度
            mass: 1,            // 质量，决定碰撞效果
            accX: 0,
            accY: 0,
            accPower: 20,       // 加速度系数
            skillId: 0,         // 角色将要释放的技能ID
            size: 1,            // 角色尺寸，碰撞检测时使用，半径
            life: 50,
            maxLife: 50,
        };
        return 0;
    }

    /**
     * 添加一个火球到场景中
     * @param {string} ballId
     * @param {number} posX
     * @param {number} posY
     * @param {number} speedX
     * @param {number} speedY
     * @param {number} rotation
     */
    addBall(ballId, posX, posY, speedX, speedY, rotation) {
        if (this.balls.hasOwnProperty(ballId)){
            return -1;
        }
        this.balls[ballId] = {
            ballId: ballId,
            posX: posX,
            posY: posY,
            speedX: speedX,
            speedY: speedY,
            topSpeed: 10,
            accX: commonConf.ballAcc * Math.sin(rotation / 180 * Math.PI),
            accY: commonConf.ballAcc * Math.cos(rotation / 180 * Math.PI),
            size: 1,  // 球的尺寸，碰撞检测时使用
            power: 30, // 球威力，决定伤害和被炸飞角色的速度
            life: commonConf.ballLife,
            invincibleTime: commonConf.ballInvincibleTime,
            explode: false, // 是否爆炸了
        };
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
}

module.exports = {
    Battle: Battle,
    battleList: {},
    updateCost: 0
};
