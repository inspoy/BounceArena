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
        }

        public void onViewRemoved()
        {
        }

        public void onLogout(SFEvent e)
        {
            SFSceneManager.addView("vwTest", m_view.imgPos.transform);
        }
    }
}
