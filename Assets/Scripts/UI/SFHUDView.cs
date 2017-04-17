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

public class SFHUDView : SFBaseView
{
    public Text lblPing { get { return m_lblPing; } }
    public Button btnMenu { get { return m_btnMenu; } }
    public ScrollRect scrLeftPlayers { get { return m_scrLeftPlayers; } }
    public Text lblTime { get { return m_lblTime; } }
    public ScrollRect scrSkills { get { return m_scrSkills; } }
    public SFProgressBar proLife { get { return m_proLife; } }

    private Text m_lblPing;
    private Button m_btnMenu;
    private ScrollRect m_scrLeftPlayers;
    private Text m_lblTime;
    private ScrollRect m_scrSkills;
    private SFProgressBar m_proLife;


    void Start()
    {
#if UNITY_EDITOR
        var time1 = DateTime.Now;
#endif
        GameObject lblPingGO = SFUtils.findChildWithParent(gameObject, "lblPing");
        if (lblPingGO != null)
        {
            m_lblPing = lblPingGO.GetComponent<Text>();
        }

        GameObject btnMenuGO = SFUtils.findChildWithParent(gameObject, "btnMenu");
        if (btnMenuGO != null)
        {
            m_btnMenu = btnMenuGO.GetComponent<Button>();
        }

        GameObject scrLeftPlayersGO = SFUtils.findChildWithParent(gameObject, "scrLeftPlayers");
        if (scrLeftPlayersGO != null)
        {
            m_scrLeftPlayers = scrLeftPlayersGO.GetComponent<ScrollRect>();
        }

        GameObject lblTimeGO = SFUtils.findChildWithParent(gameObject, "lblTime");
        if (lblTimeGO != null)
        {
            m_lblTime = lblTimeGO.GetComponent<Text>();
        }

        GameObject scrSkillsGO = SFUtils.findChildWithParent(gameObject, "scrSkills");
        if (scrSkillsGO != null)
        {
            m_scrSkills = scrSkillsGO.GetComponent<ScrollRect>();
        }

        GameObject proLifeGO = SFUtils.findChildWithParent(gameObject, "proLife");
        if (proLifeGO != null)
        {
            m_proLife = proLifeGO.GetComponent<SFProgressBar>();
        }

        m_presenter = new SFHUDPresenter() as ISFBasePresenter;

        m_presenter.initWithView(this);

#if UNITY_EDITOR
        var time2 = DateTime.Now;
        var diff = time2.Subtract(time1);
        SFUtils.log(string.Format("View created: vwHUD, cost {0}ms", diff.TotalMilliseconds));
#else
        SFUtils.log("View created: vwHUD");
#endif
    }

    public SFHUDPresenter getPresenter()
    {
        return m_presenter as SFHUDPresenter;
    }
}
