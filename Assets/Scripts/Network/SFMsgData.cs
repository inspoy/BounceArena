using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    /// <summary>
    /// 角色登陆时推送的初始状态信息
    /// </summary>
    [Serializable]
    public struct SFMsgDataRemoteUserInfo
    {
        public string uid;
        public float posX;
        public float posY;
        public float rotation;
        public float speedX;
        public float speedY;
    };

}
