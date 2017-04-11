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
 * @param {function} callback 返回true表示break，返回false或不返回表示continue
 */
exports.traverse = function (obj, callback) {
    if (obj && typeof(obj) == "object") {
        for (const key in obj) {
            if (obj.hasOwnProperty(key)) {
                const item = obj[key];
                if (callback(item, key)) {
                    break;
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

/**
 * 全局事件派发器
 * @type {events.EventEmitter}
 */
exports.eventDispatcher = new events.EventEmitter();

/**
 * 给Date添加Format的方法，可以给日期进行格式化
 * @param fmt 示例：(new Date()).Format("yy-MM-dd hh:mm:ss.SSS")=>"17-4-10 17:42:48.233"
 * @returns {*}
 * @constructor
 */
Date.prototype.Format = function (fmt) {
    const ms = this.getMilliseconds();
    const o = {
        "M+": this.getMonth() + 1, //月份
        "d+": this.getDate(), //日
        "h+": this.getHours(), //小时
        "m+": this.getMinutes(), //分
        "s+": this.getSeconds(), //秒
        "S": ms > 100 ? ms : (ms > 10 ? "0" + ms : "00" + ms) //毫秒
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
