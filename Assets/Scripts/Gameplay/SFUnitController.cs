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
    int m_curLife;
    int m_maxLife;

    // 转向加速度
    const int ROTATE_ACC = 10;

    // 位置修正加速度
    const int MOVE_ACC = 20;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 本地模拟的运动
        m_curPosX += m_curSpeedX * Time.deltaTime;
        m_curPosY += m_curSpeedY * Time.deltaTime;

        setPositionOfGo();

        if (uid == SFUserData.instance.uid)
        {
            var data = new SFUnitLifeChange();
            data.uid = uid;
            data.curLife = m_curLife;
            data.maxLife = m_maxLife;
            SFUserData.instance.dispatcher.dispatchEvent(SFEvent.EVENT_HERO_LIFE_CHANGE, data);
        }
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
        m_curLife = conf.life;
        m_maxLife = conf.maxLife;

        transform.position = new Vector3(m_curPosX, 0, m_curPosY);
        transform.rotation = Quaternion.Euler(0, m_curRotation, 0);
    }

    public void updateStatus(SFMsgDataUserSyncInfo info)
    {
        m_curPosX = info.posX;
        m_curPosY = info.posY;
        m_curRotation = info.rotation;
        m_curSpeedX = info.speedX;
        m_curSpeedY = info.speedY;
        m_curLife = info.life;
        m_maxLife = info.maxLife;

        if (info.skillId > 0)
        {
            this.skill(info.skillId, info.skillData);
        }
    }

    void setPositionOfGo()
    {
        // 转向缓动
        float realRot = gameObject.transform.rotation.eulerAngles.y;
        float diff = m_curRotation - realRot;
        if (diff > 180)
        {
            diff -= 360;
        }
        if (diff < -180)
        {
            diff += 360;
        }
        realRot += diff * Time.deltaTime * ROTATE_ACC;
        if (realRot > 360)
        {
            realRot -= 360;
        }
        if (realRot < 0)
        {
            realRot += 360;
        }
        transform.rotation = Quaternion.Euler(0, realRot, 0);

        // 位置如果差距不大则不改变，较大差距快速缓动
        float distance = Vector3.Distance(gameObject.transform.position, new Vector3(m_curPosX, 0, m_curPosY));
        if (distance > SFCommonConf.instance.syncPosThreshold)
        {
            Vector3 realPos = gameObject.transform.position;
            Vector3 posDiff = new Vector3(m_curPosX - realPos.x, 0, m_curPosY - realPos.z);
            realPos += posDiff * Time.deltaTime * MOVE_ACC;
            transform.position = realPos;
        }
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="skillId">技能ID</param>
    /// <param name="skillData">技能描述信息，详见代码</param>
    public void skill(int skillId, string skillData)
    {
        if (skillId == (int)ESkill.FireBall)
        {
            // 释放火球
            // skillData表示火球ID，形如"ball_FGihX"的随机字符串
            if (SFBallManager.current != null)
            {
                SFBallConf conf = new SFBallConf();
                conf.ballId = skillData;
                conf.posX = transform.position.x;
                conf.posY = transform.position.z;
                SFBallManager.current.addBall(conf);
            }
        }
    }

    public void destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
