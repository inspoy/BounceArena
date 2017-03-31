/**
 * Created on 2017/03/31 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF;

public class SFSocketTestView : SFBaseView
{
    public InputField txtMsg { get { return m_txtMsg; } }
    public Button btnSend { get { return m_btnSend; } }
    public Button btnConnect { get { return m_btnConnect; } }
    public Button btnDisconnect { get { return m_btnDisconnect; } }
    public Text lblInfo { get { return m_lblInfo; } }

    private InputField m_txtMsg;
    private Button m_btnSend;
    private Button m_btnConnect;
    private Button m_btnDisconnect;
    private Text m_lblInfo;


    void Start()
{
#if UNITY_EDITOR
        var time1 = DateTime.Now;
#endif
        GameObject txtMsgGO = SFUtils.findChildWithParent(gameObject, "txtMsg");
        if (txtMsgGO != null)
        {
            m_txtMsg = txtMsgGO.GetComponent<InputField>();
        }

        GameObject btnSendGO = SFUtils.findChildWithParent(gameObject, "btnSend");
        if (btnSendGO != null)
        {
            m_btnSend = btnSendGO.GetComponent<Button>();
        }

        GameObject btnConnectGO = SFUtils.findChildWithParent(gameObject, "btnConnect");
        if (btnConnectGO != null)
        {
            m_btnConnect = btnConnectGO.GetComponent<Button>();
        }

        GameObject btnDisconnectGO = SFUtils.findChildWithParent(gameObject, "btnDisconnect");
        if (btnDisconnectGO != null)
        {
            m_btnDisconnect = btnDisconnectGO.GetComponent<Button>();
        }

        GameObject lblInfoGO = SFUtils.findChildWithParent(gameObject, "lblInfo");
        if (lblInfoGO != null)
        {
            m_lblInfo = lblInfoGO.GetComponent<Text>();
        }

        m_presenter = new SFSocketTestPresenter() as ISFBasePresenter;
        m_presenter.initWithView(this);

#if UNITY_EDITOR
        var time2 = DateTime.Now;
        var diff = time2.Subtract(time1);
        SFUtils.log(string.Format("View created: vwSocketTest, cost {0}ms", diff.TotalMilliseconds));
#else
        SFUtils.log("View created: vwSocketTest");
#endif
    }
}
