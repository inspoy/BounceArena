/**
 * Created on 2017/3/19 by Inspoy.
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace SF
{
    class SFUtils
    {
        /// <summary>
        /// 获得当前时间的Unix时间戳
        /// </summary>
        /// <returns>The time stamp now.</returns>
        static public long getTimeStampNow()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        const int LOG_LEVEL_WARNING = -1;
        const int LOG_LEVEL_ERROR = -2;
        const int LOG_LEVEL_INFO = 0;

        static FileInfo sm_logFileInfo = null;

        /// <summary>
        /// 输出日志，同时输出到Unity控制台和外部log文件
        /// 日志等级默认为LOG_LEVEL_INFO(0)
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="paras">参数列表</param>
        static public void log(string msg, params object[] paras)
        {
            logLevel(LOG_LEVEL_INFO, msg, paras);
        }

        /// <summary>
        /// 输出日志，同时输出到Unity控制台和外部log文件
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <param name="msg">日志信息</param>
        /// <param name="paras">参数列表</param>
        static public void logLevel(int level, string msg, params object[] paras)
        {
            string logStr = msg;
            if (paras.Length > 0)
            {
                logStr = string.Format(msg, paras);
            }
            if (level == LOG_LEVEL_WARNING)
            {
                Debug.LogWarning(logStr);
                logStr = "[WARNING] - " + logStr;
            }
            else if (level == LOG_LEVEL_ERROR)
            {
                Debug.LogError(logStr);
                logStr = "  [ERROR] - " + logStr;
            }
            else
            {
                Debug.Log(logStr);
                logStr = " [INFO-" + level.ToString() + "] - " + logStr;
            }
            SFUtils.logToFile(logStr);
        }

        /// <summary>
        /// 新建Log文件，应当在游戏启动时调用
        /// </summary>
        static public void clearLogFile()
        {
            // 新建一个以当前时间命名的日志文件
            DateTime dt = DateTime.Now;
            string path = Application.persistentDataPath + "/GameLog" + dt.ToString("yyyyMMddHHmmss") + ".txt";
            sm_logFileInfo = new FileInfo(path);
            var sw = sm_logFileInfo.CreateText();
            sw.WriteLine("[BounceArena] - " + dt.ToString("G"));
            sw.Close();
            Debug.Log("Log file created at " + path);
        }

        static void logToFile(string msg)
        {
            // 输出到log文件
            DateTime dt = DateTime.Now;
            string output = dt.ToString("HH:mm:ss.ff ") + msg;
            if (sm_logFileInfo != null && sm_logFileInfo.Exists)
            {
                var sw = sm_logFileInfo.AppendText();
                sw.WriteLine(output);
                sw.Close();
            }
        }

        /// <summary>
        /// 输出警告信息日志
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="paras">参数列表</param>
        static public void logWarning(string msg, params object[] paras)
        {
            logLevel(LOG_LEVEL_WARNING, msg, paras);
        }

        /// <summary>
        /// 输出错误信息日志
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="paras">参数列表</param>
        static public void logError(string msg, params object[] paras)
        {
            logLevel(LOG_LEVEL_ERROR, msg, paras);
        }

        /// <summary>
        /// 根据condition断言
        /// </summary>
        /// <param name="condition">应当为<c>true</c>，否则输出错误信息</param>
        /// <param name="msg">Message.</param>
        static public void assert(bool condition, string msg = "")
        {
            if (!condition)
            {
                logError(msg);
            }
            Debug.Assert(condition, msg);
        }

        /// <summary>
        /// 错误码map
        /// </summary>
        static Dictionary<int, string> msgMap;

        static public string getMsgByErrorCode(int code)
        {
            if (msgMap == null)
            {
                msgMap = new Dictionary<int, string>();
                msgMap[SFErrorCode.duplicatedLogin] = "重复的UID";
                msgMap[SFErrorCode.battleIdNotExist] = "战场不存在";
                msgMap[SFErrorCode.userNotLogin] = "用户未登录";
                msgMap[SFErrorCode.userNotJoin] = "用户未加入战斗";
                msgMap[SFErrorCode.userAlreadyJoin] = "用户已经加入了战斗";
            }
            if (msgMap.ContainsKey(code))
            {
                return msgMap[code];
            }
            return "错误码[" + code.ToString() + "]";
        }

        /// <summary>
        /// 查找某GO下的子物体，注意这个方法会消耗相当多的时间，最好不要在update里调用
        /// 如果有多个同名的子物体，只会返回第一个
        /// </summary>
        /// <returns>子物体</returns>
        /// <param name="parent">父物体</param>
        /// <param name="childName">子物体的name</param>
        static public GameObject findChildWithParent(GameObject parent, string childName)
        {
            Transform parentTrans = parent.transform;
            foreach (Transform trans in parentTrans.GetComponentInChildren<Transform>())
            {
                if (trans.name == childName)
                {
                    return trans.gameObject;
                }
                else
                {
                    var child = SFUtils.findChildWithParent(trans.gameObject, childName);
                    if (child != null)
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取字符串的MD5值
        /// </summary>
        /// <returns>The Md5.</returns>
        /// <param name="src">Source String.</param>
        static public string getMD5(string src)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(src);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }
    };
}
