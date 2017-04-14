/**
 * Created on 2017/04/11 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFBallController : MonoBehaviour
{
    public ParticleSystem fireFX;
    public ParticleSystem explodeFX;
    public GameObject body;

    string m_ballId;
    float m_curSpeedX;
    float m_curSpeedY;
    float m_curPosX;
    float m_curPosY;

    // 位置修正加速度
    const int MOVE_ACC = 20;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_curPosX += m_curSpeedX * Time.deltaTime;
        m_curPosY += m_curSpeedY * Time.deltaTime;

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

    public void init(SFBallConf conf)
    {
        m_curPosX = conf.posX;
        m_curPosY = conf.posY;
        m_curSpeedX = 0;
        m_curSpeedY = 0;
        transform.position = new Vector3(conf.posX, 0, conf.posY);
        m_ballId = conf.ballId;
    }

    public void onSync(SFMsgDataBallSyncInfo info)
    {
        if (info.ballId != m_ballId)
        {
            return;
        }
        m_curPosX = info.posX;
        m_curPosY = info.posY;
        m_curSpeedX = info.speedX;
        m_curSpeedY = info.speedY;
    }

    public void destroy(bool explode)
    {
        m_curSpeedX = 0;
        m_curSpeedY = 0;
        body.SetActive(false);
        fireFX.Stop();
        explodeFX.Play();
        GameObject.Destroy(gameObject, 1.5f);
    }
}
