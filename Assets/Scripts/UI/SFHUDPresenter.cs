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
            SFBattleData.instance.dispatcher.addEventListener(this, SFEvent.EVENT_UNIT_LIFE_CHANGE, onLifeChange);
            SFBattleData.instance.dispatcher.addEventListener(this, SFEvent.EVENT_UNIT_ADD_REMOVE, onUnitAddRemove);

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
            int min = (int)m_runTime / 60;
            int sec = (int)m_runTime % 60;
            float ms = m_runTime - (int)m_runTime;
            m_view.lblTime.text = string.Format("{0:D}:{1:D2}.{2:D2}", min, sec, (int)(ms * 100));
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
            if (data.uid == SFUserData.instance.uid)
            {
                m_view.proLife.setProgress(1.0f * data.curLife / data.maxLife);
                SFUtils.log("当前血量{0}/{1}", data.curLife, data.maxLife);
            }
        }

        void onUnitAddRemove(SFEvent e)
        {
            var data = e.data as SFUnitAddRemove;
            if (data.addOrRemove == true)
            {
                // 添加角色
                var newGO = m_view.scrLeftPlayers.AddItem(SFSceneManager.getView("vwUnitStatusItem"));
                if (newGO != null)
                {
                    var item = newGO.GetComponent<SFUnitStatusItemView>().getPresenter();
                    item.init(data);
                }
            }
            else
            {
                // 移除角色
            }
        }
    }
}
