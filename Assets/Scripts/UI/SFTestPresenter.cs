using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    public class SFTestPresenter
    {
        SFTestView m_view;
        public void initWithView(SFTestView view)
        {
            m_view = view;
            m_view.addEventListener(m_view.btnOk, SFEvent.EVENT_UI_CLICK, onButtonClicked);
        }

        void onButtonClicked(SFEvent e)
        {
            m_view.lblTitle.text = "Button Clicked!";
        }
    }
}