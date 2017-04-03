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
        string m_infoMsg;
        SFNetworkManager m_mgr;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFSocketTestView;

            m_view.addEventListener(m_view.btnSend, SFEvent.EVENT_UI_CLICK, onSend);
            m_view.addEventListener(m_view.btnConnect, SFEvent.EVENT_UI_CLICK, onConnect);
            m_view.addEventListener(m_view.btnDisconnect, SFEvent.EVENT_UI_CLICK, onDisconnect);

            m_mgr = SFNetworkManager.getInstance();
            m_view.setUpdator(update);
            m_infoMsg = "";
        }

        public void onViewRemoved()
        {
        }

        void onSend(SFEvent e)
        {
            if (m_mgr.isReady())
            {
                string content = m_view.txtMsg.text;
                SFUtils.log("正在发送 " + content);
                SFRequestMsgUnitLogin req = new SFRequestMsgUnitLogin();
                req.uid = "abc";
                req.loginOrOut = 1;
                m_mgr.sendMessage(req);
            }
            else
            {
                m_infoMsg = "服务器未连接";
            }
        }

        void onConnect(SFEvent e)
        {
            if (m_mgr.isReady())
            {
                m_infoMsg = "已经连接过了";
                return;
            }
            m_infoMsg = "正在连接服务器";
            m_mgr.init();
            m_mgr.dispatcher.addEventListener(SFEvent.EVENT_NETWORK_READY, result =>
                {
                    SFSimpleEventData retCode = result.data as SFSimpleEventData;
                    if (retCode.intVal == 0)
                    {
                        m_infoMsg = "服务器连接成功";
                    }
                    else
                    {
                        m_infoMsg = "服务器连接失败";
                    }
                });
            m_mgr.dispatcher.addEventListener(SFEvent.EVENT_NETWORK_INTERRUPTED, onInterrupt);
            m_mgr.dispatcher.addEventListener(SFResponseMsgUnitLogin.pName, onRecvMsg);
        }

        void onDisconnect(SFEvent e)
        {
            if (!m_mgr.isReady())
            {
                m_infoMsg = "本来就没有连接";
                return;
            }
            m_mgr.uninit();
            m_infoMsg = "连接已断开";
        }

        void onInterrupt(SFEvent e)
        {
            m_infoMsg = "网络连接中断";
        }

        void onRecvMsg(SFEvent e)
        {
            m_infoMsg = "登陆成功";
        }

        void update(float dt)
        {
            m_view.lblInfo.text = m_infoMsg;
        }
    }
}
