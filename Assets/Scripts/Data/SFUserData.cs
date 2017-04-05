/**
 * Created on 2017/04/05 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    public class SFUserData
    {
        // 私有构造函数
        private SFUserData()
        {
        }

        private static SFUserData sm_instance;

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <value>The instance.</value>
        public static SFUserData instance { get { return getInstance(); } }

        public static SFUserData getInstance()
        {
            if (null == sm_instance)
            {
                sm_instance = new SFUserData();
            }
            return sm_instance;
        }

        public SFEventDispatcher dispatcher;

        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public string serverIp = "127.0.0.1";

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int serverPort = 19621;

        /// <summary>
        /// 用户UID
        /// </summary>
        public string uid;
    }
}