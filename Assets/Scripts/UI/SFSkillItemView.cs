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

public class SFSkillItemView : SFBaseView
{
    public Text lblSkillName { get { return m_lblSkillName; } }

    private Text m_lblSkillName;


    void Start()
    {
#if UNITY_EDITOR
        var time1 = DateTime.Now;
#endif
        GameObject lblSkillNameGO = SFUtils.findChildWithParent(gameObject, "lblSkillName");
        if (lblSkillNameGO != null)
        {
            m_lblSkillName = lblSkillNameGO.GetComponent<Text>();
        }

        m_presenter = new SFSkillItemPresenter() as ISFBasePresenter;

        m_presenter.initWithView(this);

#if UNITY_EDITOR
        var time2 = DateTime.Now;
        var diff = time2.Subtract(time1);
        SFUtils.log(string.Format("View created: vwSkillItem, cost {0}ms", diff.TotalMilliseconds));
#else
        SFUtils.log("View created: vwSkillItem");
#endif
    }
}
