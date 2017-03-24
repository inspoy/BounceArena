/**
 * Created on 2017/03/20 by Inspoy
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
    public class SFTestPresenter
    {
        SFTestView m_view;
        public void initWithView(SFTestView view)
        {
            m_view = view;

            m_view.addEventListener(m_view.btnOk, SFEvent.EVENT_UI_CLICK, onOk);
        }

        void onOk(SFEvent e)
        {
            m_view.lblTitle.text = "Button Clicked!";
            m_view.btnOk.SetText("修改了button的Text");
        }
    }
}
