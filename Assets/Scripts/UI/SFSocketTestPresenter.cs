/**
 * Created on 2017/03/31 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF;

namespace SF
{
    public class SFSocketTestPresenter : ISFBasePresenter
    {
        SFSocketTestView m_view;
        SFTcpClient m_client;
        string m_infoMsg;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFSocketTestView;

            m_view.addEventListener(m_view.btnSend, SFEvent.EVENT_UI_CLICK, onSend);
            m_view.addEventListener(m_view.btnConnect, SFEvent.EVENT_UI_CLICK, onConnect);
            m_view.addEventListener(m_view.btnDisconnect, SFEvent.EVENT_UI_CLICK, onDisconnect);

            m_client = new SFTcpClient();
            m_view.setUpdator(update);
            m_infoMsg = "";
        }

        public void onViewRemoved()
        {
            if (m_client.isReady)
            {
                onDisconnect(null);
            }
        }

        void onSend(SFEvent e)
        {
            if (m_client.isReady)
            {
                string content = m_view.txtMsg.text;
                SFUtils.log("正在发送 " + content);
                m_client.sendData(content);
            }
            else
            {
                m_infoMsg = "服务器未连接";
            }
        }

        void onConnect(SFEvent e)
        {
            if (m_client.isReady)
            {
                m_infoMsg = "已经连接过了";
                return;
            }
            m_client.init("127.0.0.1", 12345, onRecvMsg, result =>
                {
                    if (result == 0)
                    {
                        m_infoMsg = "服务器连接成功";
                    }
                    else
                    {
                        m_infoMsg = "服务器连接失败";
                    }
                });
            m_infoMsg = "正在连接服务器";
        }

        void onDisconnect(SFEvent e)
        {
            if (!m_client.isReady)
            {
                m_infoMsg = "本来就没有连接";
                return;
            }
            m_client.close();
            m_infoMsg = "连接已断开";
        }

        void onRecvMsg(string msg)
        {
            SFUtils.log("Recv msg: " + msg);
            m_infoMsg = msg;
        }

        void update(float dt)
        {
            m_view.lblInfo.text = m_infoMsg;
        }
    }
}
