/**
 * Created on 2017/04/06 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    public class SFCommonConf
    {
        // 私有构造函数
        private SFCommonConf()
        {
        }

        private static SFCommonConf sm_instance;

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <value>The instance.</value>
        public static SFCommonConf instance { get { return getInstance(); } }

        public static SFCommonConf getInstance()
        {
            if (null == sm_instance)
            {
                sm_instance = new SFCommonConf();
            }
            return sm_instance;
        }

        /// <summary>
        /// 心跳包发送间隔
        /// </summary>
        public double heatbeatInterval = 5.0;

        /// <summary>
        /// 服务器IP地址
        /// </summary>
//        public string serverIp = "120.25.198.51";
        public string serverIp = "127.0.0.1";

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int serverPort = 19621;

        /// <summary>
        /// 超过这个延迟的同步数据将会被忽略
        /// </summary>
        public int maxDiscardLag = 100;

        /// <summary>
        /// 实际值与参考值的差小于这个阈值就不做位置修正了
        /// </summary>
        public float syncPosThrehold = 0.1f;
    }
}
