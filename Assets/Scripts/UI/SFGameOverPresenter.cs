/**
 * Created on 2017/04/27 by inspoy
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
    public class SFGameOverPresenter : ISFBasePresenter
    {
        SFGameOverView m_view;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFGameOverView;

            m_view.addEventListener(m_view.btnOk, SFEvent.EVENT_UI_CLICK, onOk);
        }

        public void onViewRemoved()
        {
        }

        void onOk(SFEvent e)
        {
            SceneManager.LoadScene("SceneTitle");
        }
    }
}
