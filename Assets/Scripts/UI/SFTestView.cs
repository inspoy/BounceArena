/**
 * Created on 2017/03/23 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF;

public class SFTestView : SFBaseView
{
    public Text lblTitle { get { return m_lblTitle; } }
    public Button btnOk { get { return m_btnOk; } }

    private Text m_lblTitle;
    private Button m_btnOk;

    private SFTestPresenter m_presenter;

    void Start()
{
#if UNITY_EDITOR
        var time1 = DateTime.Now;
#endif
        GameObject lblTitleGO = SFUtils.findChildWithParent(gameObject, "lblTitle");
        if (lblTitleGO != null)
        {
            m_lblTitle = lblTitleGO.GetComponent<Text>();
        }

        GameObject btnOkGO = SFUtils.findChildWithParent(gameObject, "btnOk");
        if (btnOkGO != null)
        {
            m_btnOk = btnOkGO.GetComponent<Button>();
        }

        m_presenter = new SFTestPresenter();
        m_presenter.initWithView(this);

#if UNITY_EDITOR
        var time2 = DateTime.Now;
        var diff = time2.Subtract(time1);
        SFUtils.log(string.Format("View created: vwTest, cost {0}ms", diff.TotalMilliseconds));
#else
        SFUtils.log("View created: vwTest");
#endif
    }
}
