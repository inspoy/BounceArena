/**
 * Created on 2017/03/23 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace SF
{
    public enum ESocketState
    {
        eST_OK = 0,
        eST_Error = 1
    }

    public delegate void SFClientCallback(string data);
    public delegate void SFSocketStateCallback(int state);

    public class SFTcpClient
    {
        /// <summary>
        /// socket连接是否准备就绪
        /// </summary>
        public bool isReady { get { return m_isReady && m_socket != null && m_socket.Connected; } }

        /// <summary>
        /// 统计一共发送了多少字节的数据
        /// </summary>
        public long totalSendLength { get { return m_totalSend; } }

        /// <summary>
        /// 统计一共接收了多少字节的数据
        /// </summary>
        public long totalRecvLength { get { return m_totalRecv; } }

        public SFEventDispatcher dispatcher;

        SFClientCallback m_callback;
        SFSocketStateCallback m_stateCallback;
        IPEndPoint m_ipend;
        Socket m_socket;
        bool m_isReady;
        long m_totalSend;
        long m_totalRecv;
        long m_startTime;

        public SFTcpClient()
        {
            dispatcher = new SFEventDispatcher(this);
        }

        /// <summary>
        /// 初始化TCP客户端
        /// </summary>
        /// <param name="ip">目标主机的IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="callback">处理数据的回调函数</param>
        public void init(string ip, int port, SFClientCallback callback, SFSocketStateCallback stateCallback)
        {
            m_ipend = new IPEndPoint(IPAddress.Parse(ip), port);
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_callback = callback;
            m_stateCallback = stateCallback;
            m_isReady = false;
            m_totalSend = 0;
            m_totalRecv = 0;
            m_startTime = SFUtils.getTimeStampNow();
            m_socket.BeginConnect(m_ipend, result =>
                {
                    try
                    {
                        m_socket.EndConnect(result);
                        m_isReady = true;
                        m_stateCallback((int)ESocketState.eST_OK);
                        socketRecv();
                    }
                    catch (Exception)
                    {
                        m_stateCallback((int)ESocketState.eST_Error);
                    }
                }, null);
        }

        /// <summary>
        /// 关闭TCP客户端
        /// </summary>
        public void uninit()
        {
            if (m_socket != null)
            {
                if (m_socket.Connected)
                {
                    m_socket.Disconnect(false);
                }
                m_socket.Close();
                m_socket = null;
            }
            m_isReady = false;
            long endTime = SFUtils.getTimeStampNow();
            float totalTime = endTime - m_startTime;
            if (totalTime < 1)
            {
                totalTime = 1;
            }
            SFUtils.log("共发送{0:F2} KB, 共接收{0:F2} KB", 0, totalSendLength / 1024.0, totalRecvLength / 1024.0);
            SFUtils.log("平均流量：{0:F2} KB/sec", 0, 1.0 * (totalSendLength + totalRecvLength) / totalTime / 1024.0);
            SFUtils.log("连接已关闭");
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">要发送的消息</param>
        public void sendData(string msg)
        {
            if (!isReady || msg == "")
            {
                return;
            }
            try
            {
                // 编码
                byte[] data = Encoding.UTF8.GetBytes(msg);
                m_socket.BeginSend(data, 0, data.Length, SocketFlags.None, result =>
                {
                    // 发送完成
                    m_socket.EndSend(result);
                }, null);
                m_totalSend += data.Length;
            }
            catch (Exception e)
            {
                SFUtils.logWarning("Socket消息发送失败: " + e.Message);
            }
        }

        void socketRecv()
        {
            if (m_socket.Connected && m_isReady)
            {
                byte[] data = new byte[1024]; // 以1024字节为单位接收数据
                m_socket.BeginReceive(data, 0, data.Length, SocketFlags.None, result =>
                {
                    try
                    {
                        int length = m_socket.EndReceive(result);
                        m_totalRecv += length;
                        // 解码并执行回调
                        if (length > 0)
                        {
                            onRecvMsg(Encoding.UTF8.GetString(data));
                            socketRecv();
                        }
                        else
                        {
                            throw new Exception("Socket recv 0 byte");
                        }
                    }
                    catch (Exception e)
                    {
                        uninit();
                        SFUtils.logWarning("网络连接中断：" + e.Message);
                        dispatcher.dispatchEvent(SFEvent.EVENT_NETWORK_INTERRUPTED);
                    }
                }, null);
            }
        }

        void onRecvMsg(string data)
        {
            // 分包操作
            m_callback(data);
        }
    }
}
