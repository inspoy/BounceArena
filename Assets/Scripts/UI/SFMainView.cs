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

public class SFMainView : SFBaseView
{
    public Text lblUid { get { return m_lblUid; } }
    public Button btnLogout { get { return m_btnLogout; } }
    public Image imgPos { get { return m_imgPos; } }

    private Text m_lblUid;
    private Button m_btnLogout;
    private Image m_imgPos;


    void Start()
    {
#if UNITY_EDITOR
        var time1 = DateTime.Now;
#endif
        GameObject lblUidGO = SFUtils.findChildWithParent(gameObject, "lblUid");
        if (lblUidGO != null)
        {
            m_lblUid = lblUidGO.GetComponent<Text>();
        }

        GameObject btnLogoutGO = SFUtils.findChildWithParent(gameObject, "btnLogout");
        if (btnLogoutGO != null)
        {
            m_btnLogout = btnLogoutGO.GetComponent<Button>();
        }

        GameObject imgPosGO = SFUtils.findChildWithParent(gameObject, "imgPos");
        if (imgPosGO != null)
        {
            m_imgPos = imgPosGO.GetComponent<Image>();
        }

        m_presenter = new SFMainPresenter() as ISFBasePresenter;

        m_presenter.initWithView(this);

#if UNITY_EDITOR
        var time2 = DateTime.Now;
        var diff = time2.Subtract(time1);
        SFUtils.log(string.Format("View created: vwMain, cost {0}ms", diff.TotalMilliseconds));
#else
        SFUtils.log("View created: vwMain");
#endif
    }

    public SFMainPresenter getPresenter()
    {
        return m_presenter as SFMainPresenter;
    }
}
