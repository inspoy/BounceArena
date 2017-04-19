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
    public class SFPlayPresenter : ISFBasePresenter
    {
        SFPlayView m_view;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFPlayView;

            m_view.addEventListener(m_view.btnHost, SFEvent.EVENT_UI_CLICK, onHost);
            m_view.addEventListener(m_view.btnJoin, SFEvent.EVENT_UI_CLICK, onJoin);
        }

        public void onViewRemoved()
        {
        }

        void onHost(SFEvent e)
        {
        }

        void onJoin(SFEvent e)
        {
        }
    }
}
