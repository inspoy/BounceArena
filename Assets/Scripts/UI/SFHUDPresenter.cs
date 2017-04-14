/**
 * Created on 2017/04/14 by inspoy
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
    public class SFHUDPresenter : ISFBasePresenter
    {
        SFHUDView m_view;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFHUDView;

            m_view.addEventListener(m_view.btnMenu, SFEvent.EVENT_UI_CLICK, onMenu);

            m_view.scrSkills.SetFillType(2);
        }

        public void onViewRemoved()
        {
        }

        void onMenu(SFEvent e)
        {
            m_view.scrSkills.AddItem(SFSceneManager.getView("vwSkillItem"));
        }
    }
}
