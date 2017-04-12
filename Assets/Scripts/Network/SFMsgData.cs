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

    /// <summary>
    /// 角色状态同步
    /// </summary>
    [Serializable]
    public struct SFMsgDataUserSyncInfo
    {
        public string uid;
        public float posX;
        public float posY;
        public float rotation;
        public float speedX;
        public float speedY;
        public int skillId;
        public string skillData;
    };

    /// <summary>
    /// 火球状态同步
    /// </summary>
    [Serializable]
    public struct SFMsgDataBallSyncInfo
    {
        public string ballId;
        public float posX;
        public float posY;
        public float speedX;
        public float speedY;
        public bool explode;
    };

}
