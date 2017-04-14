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
    public class SFSkillItemPresenter : ISFBasePresenter
    {
        SFSkillItemView m_view;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFSkillItemView;

        }

        public void onViewRemoved()
        {
            m_view.GetHashCode();
        }
    }
}
