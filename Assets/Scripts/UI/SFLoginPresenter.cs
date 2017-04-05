/**
 * Created on 2017/04/05 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SF;

namespace SF
{
    public class SFLoginPresenter : ISFBasePresenter
    {
        SFLoginView m_view;
        string m_infoMsg;
        bool m_willReset;
        bool m_willSwitch;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFLoginView;

            m_view.addEventListener(m_view.btnLogin, SFEvent.EVENT_UI_CLICK, onLogin);

            m_view.setUpdator(update);
            m_infoMsg = "";
            m_willReset = false;
            m_willSwitch = false;
        }

        public void onViewRemoved()
        {
        }

        void onLogin(SFEvent e)
        {
            string username = m_view.txtUsername.text;
            if (username == "")
            {
                m_infoMsg = "用户名不能为空";
                return;
            }
            SFUserData.instance.uid = username;
            m_view.txtUsername.interactable = false;
            m_view.btnLogin.interactable = false;
            m_infoMsg = "正在连接服务器...";
            SFNetworkManager.instance.init();
            SFNetworkManager.instance.dispatcher.addEventListener(SFEvent.EVENT_NETWORK_READY, onConnectResult);
            SFNetworkManager.instance.dispatcher.addEventListener(SFEvent.EVENT_NETWORK_INTERRUPTED, result =>
                {
                    m_infoMsg = "网络连接中断";
                    m_willReset = true;
                });
            SFNetworkManager.instance.dispatcher.addEventListener(SFResponseMsgUnitLogin.pName, onLoginResult);
        }

        void onConnectResult(SFEvent e)
        {
            var retCode = e.data as SFSimpleEventData;
            if (retCode.intVal == 0)
            {
                m_infoMsg = "连接成功，正在登陆...";
                doLogin();
            }
            else
            {
                m_infoMsg = "无法连接到服务器";
                m_willReset = true;
            }
        }

        void resetConnection()
        {
            m_view.txtUsername.interactable = true;
            m_view.btnLogin.interactable = true;
            SFNetworkManager.instance.uninit();
        }

        void update(float dt)
        {
            m_view.lblInfo.text = m_infoMsg;
            if (m_willReset)
            {
                resetConnection();
                m_willReset = false;
            }
            if (m_willSwitch)
            {
                m_view.StartCoroutine(loadSceneGame());
                m_willSwitch = false;
            }
        }

        void doLogin()
        {
            SFRequestMsgUnitLogin req = new SFRequestMsgUnitLogin();
            req.loginOrOut = 1;
            SFNetworkManager.instance.sendMessage(req);
        }

        void onLoginResult(SFEvent e)
        {
            var data = e.data as SFResponseMsgUnitLogin;
            if (data.retCode == 0)
            {
                m_infoMsg = "登陆成功，正在加载游戏...";
                m_willSwitch = true;
            }
            else
            {
                m_infoMsg = string.Format("登陆失败，错误码：{0}", data.retCode);
                m_willReset = true;
            }
        }

        IEnumerator loadSceneGame()
        {
            var op = SceneManager.LoadSceneAsync("SceneGame");
            yield return op;
        }
    }
}
