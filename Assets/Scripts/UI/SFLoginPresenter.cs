/**
 * Created on 2017/04/05 by inspoy
 * All rights reserved.
 */

/**
 * Created on 2017/04/05 by inspoy
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
    public class SFLoginPresenter : ISFBasePresenter
    {
        SFLoginView m_view;
        string m_infoMsg;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFLoginView;

            m_view.addEventListener(m_view.btnLogin, SFEvent.EVENT_UI_CLICK, onLogin);

            m_view.setUpdator(update);
            m_infoMsg = "";
        }

        public void onViewRemoved()
        {
        }

        void onLogin(SFEvent e)
        {
            m_view.btnLogin.interactable = false;
        }

        void update(float dt)
        {
            m_view.lblInfo.text = m_infoMsg;
        }
    }
}
