using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SF
{
    #region 基础类型
    /// <summary>
    /// 基础请求协议类型，不可以作为事件数据来传递
    /// </summary>
    public class SFBaseRequestMessage
    {
        /// <summary>
        /// 协议id
        /// </summary>
        public int pid;
        /// <summary>
        /// 用户id
        /// </summary>
        public string uid;

        public SFBaseRequestMessage()
        {
            uid = SFUserData.instance.uid;
        }
    };

    /// <summary>
    /// 基础响应协议类型，同时也可以作为事件数据来传递
    /// </summary>
    public class SFBaseResponseMessage : ISFEventData
    {
        /// <summary>
        /// 协议id
        /// </summary>
        public int pid;
        /// <summary>
        /// 协议返回错误码
        /// </summary>
        public int retCode;
    };
    #endregion

    #region 0-心跳包
    /// <summary>
    /// [Req]心跳包
    /// </summary>
    [Serializable]
    public class SFRequestMsgSocketHeartbeat : SFBaseRequestMessage
    {
        public SFRequestMsgSocketHeartbeat()
        {
            pid = 0;
        }
    };

    /// <summary>
    /// [Resp]心跳包
    /// </summary>
    [Serializable]
    public class SFResponseMsgSocketHeartbeat : SFBaseResponseMessage
    {
        public const string pName = "socket_0";
        public SFResponseMsgSocketHeartbeat()
        {
            pid = 0;
        }
    };
    #endregion

    #region 1-用户登陆登出
    /// <summary>
    /// [Req]用户登陆登出
    /// </summary>
    [Serializable]
    public class SFRequestMsgUnitLogin : SFBaseRequestMessage
    {
        public SFRequestMsgUnitLogin()
        {
            pid = 1;
        }

        /// <summary>
        /// 1=login 2=logout
        /// </summary>
        public int loginOrOut;
    };

    /// <summary>
    /// [Resp]用户登陆登出
    /// </summary>
    [Serializable]
    public class SFResponseMsgUnitLogin : SFBaseResponseMessage
    {
        public const string pName = "socket_1";
        public SFResponseMsgUnitLogin()
        {
            pid = 1;
        }
    };
    #endregion

    #region 2-登陆时推送场上其他玩家的信息
    /// <summary>
    /// [Resp][Notify]登陆时推送场上其他玩家的信息
    /// </summary>
    [Serializable]
    public class SFResponseMsgNotifyRemoteUsers : SFBaseResponseMessage
    {
        public const string pName = "socket_2";
        public SFResponseMsgNotifyRemoteUsers()
        {
            pid = 2;
        }

        /// <summary>
        /// 地图ID
        /// </summary>
        public int mapId;

        /// <summary>
        /// 加入战斗时地图已经运行的时间
        /// </summary>
        public int runTime;

        /// <summary>
        /// 其他玩家的信息
        /// </summary>
        public List<SFMsgDataRemoteUserInfo> users;

        /// <summary>
        /// 自己的初始位置X
        /// </summary>
        public float posX;

        /// <summary>
        /// 自己的初始位置Y
        /// </summary>
        public float posY;

        /// <summary>
        /// 自己的初始位置朝向
        /// </summary>
        public float rotation;
    };
    #endregion

    #region 3-发送自己的状态
    /// <summary>
    /// [Req]发送自己的状态
    /// </summary>
    [Serializable]
    public class SFRequestMsgUnitSync : SFBaseRequestMessage
    {
        public SFRequestMsgUnitSync()
        {
            pid = 3;
        }
        public float moveX;
        public float moveY;
        public float rotation;
        public int skillId;
    };

    /// <summary>
    /// [Resp]发送自己的状态
    /// </summary>
    [Serializable]
    public class SFResponseMsgUnitSync : SFBaseResponseMessage
    {
        public const string pName = "socket_3";
        public SFResponseMsgUnitSync()
        {
            pid = 3;
        }
    };
    #endregion

    #region 4-推送其他角色的状态
    /// <summary>
    /// [Resp][Notify]推送其他角色的状态
    /// </summary>
    [Serializable]
    public class SFResponseMsgNotifyUnitStatus : SFBaseResponseMessage
    {
        public const string pName = "socket_4";
        public SFResponseMsgNotifyUnitStatus()
        {
            pid = 4;
        }

        /// <summary>
        /// 单位毫秒
        /// </summary>
        public int runTime;
        public List<SFMsgDataUserSyncInfo> infos;
    };
    #endregion
}
// Last Update: 2017/04/11
