/**
 * Created on 2017/03/31 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    public class SFNetworkManager
    {
        /// <summary>
        /// 时间派发器
        /// </summary>
        public SFEventDispatcher dispatcher;

        /// <summary>
        /// TCP客户端
        /// </summary>
        /// <value>The client.</value>
        public SFTcpClient client { get { return m_client; } }

        public double ping{ get { return m_ping; } }

        SFTcpClient m_client;
        Queue<SFBaseRequestMessage> m_sendQueue;
        Queue<string> m_recvQueue;
        double m_ping;
        DateTime m_heartbeatStartTime;
        double m_heartbeatTimer;

        private SFNetworkManager()
        {
        }

        ~SFNetworkManager()
        {
            if (m_client != null)
            {
                m_client.uninit();
            }
        }

        private static SFNetworkManager sm_instance = null;

        /// <summary>
        /// 获取唯一实例
        /// </summary>
        /// <value>The instance.</value>
        public static SFNetworkManager instance { get { return getInstance(); } }

        private static SFNetworkManager getInstance()
        {
            if (null == sm_instance)
            {
                sm_instance = new SFNetworkManager();
            }
            return sm_instance;
        }

        public void init()
        {
            SFUtils.log("正在连接GameServer...");
            m_sendQueue = new Queue<SFBaseRequestMessage>();
            m_recvQueue = new Queue<string>();
            dispatcher = new SFEventDispatcher(this);
            m_client = new SFTcpClient();
            m_ping = -1;
            m_heartbeatTimer = 0;
            m_client.init(SFCommonConf.instance.serverIp, SFCommonConf.instance.serverPort, onRecvMsg, ret =>
                {
                    if (ret == 0)
                    {
                        SFUtils.log("连接GameServer成功");
                    }
                    else
                    {
                        SFUtils.logWarning("连接GameServer失败");
                    }
                    dispatcher.dispatchEvent(SFEvent.EVENT_NETWORK_READY, new SFSimpleEventData(ret));
                });
            m_client.dispatcher.addEventListener(this, SFEvent.EVENT_NETWORK_INTERRUPTED, e =>
                {
                    dispatcher.dispatchEvent(e);
                });
            dispatcher.addEventListener(this, SFResponseMsgSocketHeartbeat.pName, onHeartbeat);
        }

        public void uninit()
        {
            if (m_client != null)
            {
                m_client.uninit();
                m_client = null;
            }
            if (m_sendQueue != null)
            {
                m_sendQueue = null;
            }
            if (m_recvQueue != null)
            {
                m_recvQueue = null;
            }
            if (dispatcher != null)
            {
                dispatcher.removeAllEventListeners();
            }
        }

        /// <summary>
        /// 服务器是否就绪
        /// </summary>
        /// <returns><c>true</c> if game server is ready; otherwise, <c>false</c>.</returns>
        public bool isReady()
        {
            return m_client != null && m_client.isReady;
        }

        /// <summary>
        /// 往服务器发送信息
        /// </summary>
        /// <param name="req">请求信息</param>
        public void sendMessage(SFBaseRequestMessage req)
        {
            m_sendQueue.Enqueue(req);
        }

        void onRecvMsg(string msg)
        {
            lock (m_recvQueue)
            {
                m_recvQueue.Enqueue(msg);
            }
        }

        public void update(float dt)
        {
            // 发送队列
            while (m_sendQueue.Count > 0)
            {
                SFBaseRequestMessage req = m_sendQueue.Dequeue();
                string data = JsonUtility.ToJson(req);
                m_client.sendData(data);
                if (req.pid != 0 && req.pid != 3)
                {
                    SFUtils.log("Sending message[{0}]: {1}", data.Length, data);
                }
            }

            // 接收队列
            while (m_recvQueue.Count > 0)
            {
                string data = m_recvQueue.Dequeue();
                SFBaseResponseMessage obj = null;
                try
                {
                    obj = JsonUtility.FromJson<SFBaseResponseMessage>(data);
                }
                catch
                {
                }
                if (obj != null)
                {
                    handleProtocol(obj.pid, data);
                    if (obj.pid != 0 && obj.pid != 3 && obj.pid != 4)
                    {
                        SFUtils.log("收到信息:协议号={0}\ndata={1}", obj.pid, data);
                    }
                }
                else
                {
                    SFUtils.logWarning("不能解析的信息格式:\n" + data);
                }
            }

            // 心跳包
            m_heartbeatTimer += dt;
            if (m_heartbeatTimer > SFCommonConf.instance.heatbeatInterval)
            {
                m_heartbeatTimer -= SFCommonConf.instance.heatbeatInterval;
                sendHeartbeat();
            }
        }

        void handleProtocol(int pid, string jsonData)
        {
            // 这个方法应该由主线程调用
            try
            {
                string pName = string.Format("socket_{0}", pid);
                SFBaseResponseMessage obj = null;
                if (pid == int.MaxValue)
                {
                } // __start__
                else if (pid == 0)
                {
                    obj = JsonUtility.FromJson<SFResponseMsgSocketHeartbeat>(jsonData);
                }
                else if (pid == 1)
                {
                    obj = JsonUtility.FromJson<SFResponseMsgUnitLogin>(jsonData);
                }
                else if (pid == 2)
                {
                    obj = JsonUtility.FromJson<SFResponseMsgNotifyRemoteUsers>(jsonData);
                }
                else if (pid == 3)
                {
                    obj = JsonUtility.FromJson<SFResponseMsgUnitSync>(jsonData);
                }
                else if (pid == 4)
                {
                    obj = JsonUtility.FromJson<SFResponseMsgNotifyUnitStatus>(jsonData);
                }
                else // __end__
                {
                    SFUtils.logWarning("不能识别的协议号: {0}", 0, pid);
                }
                if (obj != null)
                {
                    dispatcher.dispatchEvent(pName, obj);
                }
            }
            catch
            {
                SFUtils.logWarning("解析协议失败: {0}\ndata: {1}", 0, pid, jsonData);
            }
        }

        void sendHeartbeat()
        {
            m_heartbeatStartTime = DateTime.Now;
            SFRequestMsgSocketHeartbeat req = new SFRequestMsgSocketHeartbeat();
            sendMessage(req);
        }

        void onHeartbeat(SFEvent e)
        {
            var now = DateTime.Now;
            var diff = now.Subtract(m_heartbeatStartTime);
            m_ping = diff.TotalMilliseconds;
            SFUtils.log("ping: {0:F2}", m_ping);
            dispatcher.dispatchEvent(SFEvent.EVENT_NETWORK_PING);
        }
    }
}
