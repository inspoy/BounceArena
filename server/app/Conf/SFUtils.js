/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";

const events = require("events");

/**
 *
 * @param {string} prefix 前缀
 * @param {number} len
 * @returns {string} 生成的随机字符串
 */
exports.getRandomString = function (prefix = "", len = 16) {
    let ret = "";
    for (let i = 0; i < len; ++i) {
        let randVal = Math.random() * 52;
        let charCode = 0;
        if (randVal > 26) {
            charCode = randVal + 71;
        }
        else {
            charCode = randVal + 65;
        }
        ret += String.fromCharCode(charCode);
    }
    return prefix + ret;
};

/**
 * 用指定的方法遍历对象
 * @param {object} obj
 * @param {function} callback 返回true表示中断不再继续遍历
 */
exports.traverse = function(obj, callback) {
    if (obj && typeof(obj) == "object") {
        for (const key in obj) {
            if (obj.hasOwnProperty(key)) {
                const item = obj[key];
                if (callback(item)) {
                    return;
                }
            }
        }
    }
};

/**
 * 写日志
 * @param msg 日志内容
 * @param level 等级，数字越大优先级越低，默认0
 */
exports.logInfo = function (msg, level = 0) {
    if (msg) {
        const msgObj = {
            type: "LOG",
            level: level,
            data: msg.toString()
        };
        process.send(msgObj);
    }
};

exports.eventDispatcher = new events.EventEmitter();

Date.prototype.Format = function (fmt) {
    const o = {
        "M+": this.getMonth() + 1, //月份
        "d+": this.getDate(), //日
        "h+": this.getHours(), //小时
        "m+": this.getMinutes(), //分
        "s+": this.getSeconds(), //秒
        "S": this.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (const k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(
            RegExp.$1,
            (RegExp.$1.length == 1) ?
                (o[k]) :
                (("00" + o[k]).substr(("" + o[k]).length))
        );
    }
    return fmt;
};