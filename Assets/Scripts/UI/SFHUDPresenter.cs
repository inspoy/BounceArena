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
        float m_runTime;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFHUDView;

            m_view.addEventListener(m_view.btnMenu, SFEvent.EVENT_UI_CLICK, onMenu);
            SFNetworkManager.instance.dispatcher.addEventListener(this, SFEvent.EVENT_NETWORK_PING, onPing);
            SFUserData.instance.dispatcher.addEventListener(this, SFEvent.EVENT_HERO_LIFE_CHANGE, onLifeChange);

            m_view.scrLeftPlayers.SetFillType(1);
            m_view.scrSkills.SetFillType(2);
            m_view.setUpdator(onUpdate);
            m_runTime = 0;
            onPing(null);
        }

        public void onViewRemoved()
        {
            SFNetworkManager.instance.dispatcher.removeAllEventListenersWithTarget(this);
            SFUserData.instance.dispatcher.removeAllEventListenersWithTarget(this);
        }

        void onUpdate(float dt)
        {
            m_runTime += dt;
            m_view.lblTime.text = string.Format("{0:D}:{1:F2}", (int)m_runTime / 60, m_runTime % 60);
        }

        void onMenu(SFEvent e)
        {
            m_view.scrSkills.AddItem(SFSceneManager.getView("vwSkillItem"));
        }

        void onPing(SFEvent e)
        {
            m_view.lblPing.text = string.Format("Ping: {0:F2}ms", SFNetworkManager.instance.ping);
        }

        void onLifeChange(SFEvent e)
        {
            var data = e.data as SFUnitLifeChange;
            // TODO
        }
    }
}
