/**
 * Created by inspoy on 2017/4/6.
 */

"use strict";
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
