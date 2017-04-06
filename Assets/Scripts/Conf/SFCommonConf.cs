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
        public double heatbeatInterval = 2.0;
    }
}
