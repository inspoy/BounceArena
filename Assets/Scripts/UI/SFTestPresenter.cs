/**
 * Created on 2017/03/24 by inspoy
 * All rights reserved.
 */

/**
 * Created on 2017/03/24 by inspoy
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
    public class SFTestPresenter : ISFBasePresenter
    {
        SFTestView m_view;
        public void initWithView(SFBaseView view)
        {
            m_view = view as SFTestView;

            m_view.addEventListener(m_view.btnOk, SFEvent.EVENT_UI_CLICK, onOk);
            m_view.addEventListener(m_view.btnClose, SFEvent.EVENT_UI_CLICK, onClose);
        }

        public void onViewRemoved()
        {
            SFUtils.log("SFTestView被关闭了");
        }


        void onOk(SFEvent e)
        {
            m_view.lblTitle.text = "Button Clicked!";
            m_view.btnOk.SetText("修改了button的Text");
        }

        void onClose(SFEvent e)
        {
            m_view.removeView();
        }
    }
}
