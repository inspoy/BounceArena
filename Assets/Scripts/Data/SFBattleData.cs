/**
 * Created on 2017/04/05 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    public class SFBattleData
    {
        // 私有构造函数
        private SFBattleData()
        {
            dispatcher = new SFEventDispatcher(this);
        }

        private static SFBattleData sm_instance;

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <value>The instance.</value>
        public static SFBattleData instance { get { return getInstance(); } }

        static SFBattleData getInstance()
        {
            if (null == sm_instance)
            {
                sm_instance = new SFBattleData();
            }
            return sm_instance;
        }

        public SFEventDispatcher dispatcher;

        // 协议2返回数据
        public int enterBattle_mapId;
        public List<SFMsgDataRemoteUserInfo> enterBattle_remoteUsers;
        public float enterBattle_posX;
        public float enterBattle_posY;
        public float enterBattle_rotation;
    }
}
