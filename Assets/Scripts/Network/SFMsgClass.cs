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
    #region 1-用户登陆登出
    /// <summary>
    /// [Req]用户登陆登出
    /// </summary>
    [Serializable]
    public class SFRequestMsgUnitLogin : SFBaseRequestMessage
    {
        public const string pName = "socket_1";
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
}
// Last Update: 2017/03/01