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

public class SFUnitStatusItemView : SFBaseView
{
    public SFProgressBar proLife { get { return m_proLife; } }
    public Text lblUserName { get { return m_lblUserName; } }

    private SFProgressBar m_proLife;
    private Text m_lblUserName;


    void Start()
    {
#if UNITY_EDITOR
        var time1 = DateTime.Now;
#endif
        GameObject proLifeGO = SFUtils.findChildWithParent(gameObject, "proLife");
        if (proLifeGO != null)
        {
            m_proLife = proLifeGO.GetComponent<SFProgressBar>();
        }

        GameObject lblUserNameGO = SFUtils.findChildWithParent(gameObject, "lblUserName");
        if (lblUserNameGO != null)
        {
            m_lblUserName = lblUserNameGO.GetComponent<Text>();
        }

        m_presenter = new SFUnitStatusItemPresenter() as ISFBasePresenter;

        m_presenter.initWithView(this);

#if UNITY_EDITOR
        var time2 = DateTime.Now;
        var diff = time2.Subtract(time1);
        SFUtils.log(string.Format("View created: vwUnitStatusItem, cost {0}ms", diff.TotalMilliseconds));
#else
        SFUtils.log("View created: vwUnitStatusItem");
#endif
    }

    public SFUnitStatusItemPresenter getPresenter()
    {
        return m_presenter as SFUnitStatusItemPresenter;
    }
}
