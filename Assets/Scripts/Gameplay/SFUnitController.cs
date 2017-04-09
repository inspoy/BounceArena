/**
 * Created on 2017/04/07 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFUnitController : MonoBehaviour
{
    public string uid;
    float m_curSpeedX;
    float m_curSpeedY;
    float m_curPosX;
    float m_curPosY;
    float m_curRotation;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_curPosX += m_curSpeedX * Time.deltaTime;
        m_curPosY += m_curSpeedY * Time.deltaTime;

        setPositionOfGo();
    }

    public void init(SFUnitConf conf)
    {
        uid = conf.uid;
        // 初始化坐标
        m_curPosX = conf.posX;
        m_curPosY = conf.posY;
        m_curRotation = conf.rotation;
        m_curSpeedX = conf.speedX;
        m_curSpeedY = conf.speedY;

        setPositionOfGo();
    }

    public void updateStatus(SFMsgDataUserSyncInfo info)
    {
        m_curPosX = info.posX;
        m_curPosY = info.posY;
        m_curRotation = info.rotation;
        m_curSpeedX = info.speedX;
        m_curSpeedY = info.speedY;

        setPositionOfGo();
    }

    void setPositionOfGo()
    {
        gameObject.transform.position = new Vector3(m_curPosX, 0, m_curPosY);
        gameObject.transform.rotation = Quaternion.Euler(0, m_curRotation, 0);
    }
}
