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

public class SFPlayView : SFBaseView
{
    public Button btnHost { get { return m_btnHost; } }
    public Button btnJoin { get { return m_btnJoin; } }
    public Text lblInfo { get { return m_lblInfo; } }

    private Button m_btnHost;
    private Button m_btnJoin;
    private Text m_lblInfo;


    void Start()
    {
#if UNITY_EDITOR
        var time1 = DateTime.Now;
#endif
        GameObject btnHostGO = SFUtils.findChildWithParent(gameObject, "btnHost");
        if (btnHostGO != null)
        {
            m_btnHost = btnHostGO.GetComponent<Button>();
        }

        GameObject btnJoinGO = SFUtils.findChildWithParent(gameObject, "btnJoin");
        if (btnJoinGO != null)
        {
            m_btnJoin = btnJoinGO.GetComponent<Button>();
        }

        GameObject lblInfoGO = SFUtils.findChildWithParent(gameObject, "lblInfo");
        if (lblInfoGO != null)
        {
            m_lblInfo = lblInfoGO.GetComponent<Text>();
        }

        m_presenter = new SFPlayPresenter() as ISFBasePresenter;

        m_presenter.initWithView(this);

#if UNITY_EDITOR
        var time2 = DateTime.Now;
        var diff = time2.Subtract(time1);
        SFUtils.log(string.Format("View created: vwPlay, cost {0}ms", diff.TotalMilliseconds));
#else
        SFUtils.log("View created: vwPlay");
#endif
    }

    public SFPlayPresenter getPresenter()
    {
        return m_presenter as SFPlayPresenter;
    }
}
