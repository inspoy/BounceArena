/**
 * Created on 2017/04/17 by inspoy
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
    public class SFUnitStatusItemPresenter : ISFBasePresenter
    {
        SFUnitStatusItemView m_view;
        string m_uid;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFUnitStatusItemView;

            SFBattleData.instance.dispatcher.addEventListener(SFEvent.EVENT_UNIT_LIFE_CHANGE, onLifeChange);
        }

        public void onViewRemoved()
        {
            SFBattleData.instance.dispatcher.removeAllEventListenersWithTarget(this);
        }

        public void init(SFUnitAddRemove info)
        {
            m_uid = info.uid;
            m_view.proLife.setProgress(1.0f);
        }

        void onLifeChange(SFEvent e)
        {
            var data = e.data as SFUnitLifeChange;
            if (data.uid == m_uid)
            {
                m_view.proLife.setProgress(1.0f * data.curLife / data.maxLife);
            }
        }
    }
}
