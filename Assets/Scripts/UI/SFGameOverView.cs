/**
 * Created on 2017/04/27 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF;

public class SFGameOverView : SFBaseView
{
    public Button btnOk { get { return m_btnOk; } }

    private Button m_btnOk;


    void Start()
    {
#if UNITY_EDITOR
        var time1 = DateTime.Now;
#endif
        GameObject btnOkGO = SFUtils.findChildWithParent(gameObject, "btnOk");
        if (btnOkGO != null)
        {
            m_btnOk = btnOkGO.GetComponent<Button>();
        }

        m_presenter = new SFGameOverPresenter() as ISFBasePresenter;

        m_presenter.initWithView(this);

#if UNITY_EDITOR
        var time2 = DateTime.Now;
        var diff = time2.Subtract(time1);
        SFUtils.log(string.Format("View created: vwGameOver, cost {0}ms", diff.TotalMilliseconds));
#else
        SFUtils.log("View created: vwGameOver");
#endif
    }

    public SFGameOverPresenter getPresenter()
    {
        return m_presenter as SFGameOverPresenter;
    }
}
