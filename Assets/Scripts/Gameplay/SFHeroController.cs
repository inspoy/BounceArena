/**
 * Created on 2017/04/07 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFHeroController : MonoBehaviour
{
    public SFUnitController m_hero;

    float m_lastMoveX;
    float m_lastMoveY;
    float m_lastRotation;
    float m_screenWidth;
    float m_screenHeight;
    int m_skillId;

    // Use this for initialization
    void Start()
    {
        m_lastMoveX = 0;
        m_lastMoveY = 0;
        m_lastRotation = 0;
        m_screenWidth = Screen.width;
        m_screenHeight = Screen.height;
    }

    // Update is called every specific interval
    void FixedUpdate()
    {
        if (m_hero == null)
        {
            return;
        }
        bool needSync = false;
        float curX = Input.GetAxis("Horizontal");
        float curY = Input.GetAxis("Vertical");
        float curRot = getCurRotation();
        if (curX != m_lastMoveX || curY != m_lastMoveY || curRot != m_lastRotation)
        {
            m_lastMoveX = curX;
            m_lastMoveY = curY;
            m_lastRotation = curRot;
            needSync = true;
        }
        m_skillId = 0;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SFUtils.log("按下了空格键");
            m_skillId += SFUserData.instance.skillConf.space;
        }
        if (Input.GetMouseButtonDown(0))
        {
            SFUtils.log("按下了鼠标左键");
            m_skillId += SFUserData.instance.skillConf.left;
        }
        if (Input.GetMouseButtonDown(1))
        {
            SFUtils.log("按下了鼠标右键");
            m_skillId += SFUserData.instance.skillConf.right;
        }
        if (m_skillId > 0)
        {
            needSync = true;
        }
        if (needSync)
        {
            syncData();
        }
    }

    void syncData()
    {
        SFRequestMsgUnitSync req = new SFRequestMsgUnitSync();
        req.posX = m_lastMoveX;
        req.posY = m_lastMoveY;
        req.rotation = m_lastRotation;
        req.skillId = m_skillId;
        SFNetworkManager.instance.sendMessage(req);
    }

    public void setHero(SFUnitController hero)
    {
        m_hero = hero;
    }

    float getCurRotation()
    {
        float posX = Input.mousePosition.x;
        float posY = Input.mousePosition.y;
        posX = Mathf.Clamp(posX, 0, m_screenWidth);
        posY = Mathf.Clamp(posY, 0, m_screenHeight);
        posX -= m_screenWidth / 2;
        posY -= m_screenHeight / 2;
        if (Mathf.Abs(posX) < m_screenWidth / 10 &&
            Mathf.Abs(posY) < m_screenHeight / 10)
        {
            return m_lastRotation;
        }
        return Mathf.Atan2(posX, posY) * Mathf.Rad2Deg;
    }
}
