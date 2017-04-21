/**
 * Created on 2017/04/18 by inspoy
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
    public class SFMainPresenter : ISFBasePresenter
    {
        SFMainView m_view;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFMainView;

            m_view.addEventListener(m_view.btnLogout, SFEvent.EVENT_UI_CLICK, onLogout);
            SFNetworkManager.instance.dispatcher.addEventListener(this, SFResponseMsgUnitLogin.pName, onLogoutResult);

            SFSceneManager.addView("vwPlay", m_view.imgPos.transform);
            m_view.lblUid.text = SFUserData.instance.uid;
        }

        public void onViewRemoved()
        {
            SFNetworkManager.instance.dispatcher.removeAllEventListenersWithTarget(this);
        }

        public void onLogout(SFEvent e)
        {
            SFRequestMsgUnitLogin req = new SFRequestMsgUnitLogin();
            req.loginOrOut = 2;
            SFNetworkManager.instance.sendMessage(req);
            m_view.btnLogout.interactable = false;
        }

        void onLogoutResult(SFEvent e)
        {
            var data = e.data as SFResponseMsgUnitLogin;
            if (data.retCode == 0)
            {
                SFNetworkManager.instance.uninit();
                m_view.removeView();
                SFSceneManager.addView("vwLogin");
            }
            else
            {
                SFUtils.logWarning("登出失败");
            }
        }
    }
}
