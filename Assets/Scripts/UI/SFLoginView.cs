/**
 * Created on 2017/04/10 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF;

public class SFLoginView : SFBaseView
{
    public InputField txtUsername { get { return m_txtUsername; } }
    public Button btnLogin { get { return m_btnLogin; } }
    public Text lblInfo { get { return m_lblInfo; } }

    private InputField m_txtUsername;
    private Button m_btnLogin;
    private Text m_lblInfo;


    void Start()
    {
#if UNITY_EDITOR
        var time1 = DateTime.Now;
#endif
        GameObject txtUsernameGO = SFUtils.findChildWithParent(gameObject, "txtUsername");
        if (txtUsernameGO != null)
        {
            m_txtUsername = txtUsernameGO.GetComponent<InputField>();
        }

        GameObject btnLoginGO = SFUtils.findChildWithParent(gameObject, "btnLogin");
        if (btnLoginGO != null)
        {
            m_btnLogin = btnLoginGO.GetComponent<Button>();
        }

        GameObject lblInfoGO = SFUtils.findChildWithParent(gameObject, "lblInfo");
        if (lblInfoGO != null)
        {
            m_lblInfo = lblInfoGO.GetComponent<Text>();
        }

        m_presenter = new SFLoginPresenter() as ISFBasePresenter;

        m_presenter.initWithView(this);

#if UNITY_EDITOR
        var time2 = DateTime.Now;
        var diff = time2.Subtract(time1);
        SFUtils.log(string.Format("View created: vwLogin, cost {0}ms", diff.TotalMilliseconds));
#else
        SFUtils.log("View created: vwLogin");
#endif
    }
}
