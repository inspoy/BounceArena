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
        public struct SFUserSkillConf
        {
            public int space;
            public int left;
            public int right;
        }
        
        // 私有构造函数
        private SFUserData()
        {
            dispatcher      = new SFEventDispatcher(this);
            skillConf       = new SFUserSkillConf();
            skillConf.space = (int)ESkill.FireBall;
            skillConf.left  = (int)ESkill.Flash;
            skillConf.right = (int)ESkill.Shield;
        }

        private static SFUserData sm_instance;

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <value>The instance.</value>
        public static SFUserData instance { get { return getInstance(); } }

        static SFUserData getInstance()
        {
            if (null == sm_instance)
            {
                sm_instance = new SFUserData();
            }
            return sm_instance;
        }

        public SFEventDispatcher dispatcher;

        /// <summary>
        /// 用户UID
        /// </summary>
        public string uid;

        public SFUserSkillConf skillConf;
    }
}