/**
 * Created on 2017/03/31 by inspoy
 * All rights reserved.
 */

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

        SFTcpClient m_client;
        Queue<string> m_sendQueue;
        Queue<string> m_recvQueue;

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
        /// <returns>The instance.</returns>
        public static SFNetworkManager getInstance()
        {
            if (null == sm_instance)
            {
                sm_instance = new SFNetworkManager();
            }
            return sm_instance;
        }

        public void init()
        {
            dispatcher = new SFEventDispatcher(this);
            m_client = new SFTcpClient();
            m_client.init("127.0.0.1", 19621, onRecvMsg, ret =>
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
            m_client.dispatcher.addEventListener(SFEvent.EVENT_NETWORK_INTERRUPTED, e =>
                {
                    dispatcher.dispatchEvent(e);
                });
            m_sendQueue = new Queue<string>();
            m_recvQueue = new Queue<string>();
            SFUtils.log("正在连接GameServer...");
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
        }

        /// <summary>
        /// 往服务器发送信息
        /// </summary>
        /// <param name="req">请求信息</param>
        public void SendMessage(SFBaseRequestMessage req)
        {
            string data = JsonUtility.ToJson(req);
            m_sendQueue.Enqueue(data);
        }

        void onRecvMsg(string msg)
        {
            m_recvQueue.Enqueue(msg);
        }

        void update(float dt)
        {
            while (m_sendQueue.Count > 0)
            {
                string data = m_recvQueue.Dequeue();
                m_client.sendData(data);
                SFUtils.log("Sending message[]: ", 0, data);
            }
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
                    SFUtils.log("收到信息:协议号={0}\ndata={1}", 0, obj.pid, data);
                }
                else
                {
                    SFUtils.logWarning("不能解析的信息格式");
                }
            }
        }

        void handleProtocol(int pid, string jsonData)
        {
            // 这个方法应该由主线程调用
            try
            {
                string pName = string.Format("socket_{0}", pid);
                SFBaseResponseMessage obj = null;
                if (pid == 0)
                {
                } // __start__
                else if (pid == 1)
                {
                    obj = JsonUtility.FromJson<SFResponseMsgUnitLogin>(jsonData);
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
    }
}
