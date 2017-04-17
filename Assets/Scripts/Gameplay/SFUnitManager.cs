/**
 * Created on 2017/04/07 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFUnitManager : MonoBehaviour
{
    public GameObject unitPrefab;
    Dictionary<string, SFUnitController> m_controllers;
    SFHeroController m_heroController;
    int m_runTime;

    // Use this for initialization
    void Start()
    {
        m_heroController = GetComponent<SFHeroController>();
        m_controllers = new Dictionary<string, SFUnitController>();
        m_runTime = 0;

        SFNetworkManager.instance.dispatcher.addEventListener(this, SFResponseMsgNotifyUnitStatus.pName, onNotifyUnitStatus);
        SFNetworkManager.instance.dispatcher.addEventListener(this, SFResponseMsgNotifyNewUserJoin.pName, onNotifyUnitJoin);
    }

    // Update is called once per frame
    void Update()
    {
        m_runTime += (int)(Time.deltaTime * 1000);
    }

    /// <summary>
    /// 根据登陆信息初始化自己以及场上的其他角色
    /// </summary>
    public void initUnits()
    {
        m_runTime = SFBattleData.instance.enterBattle_initRunTime;
        SFUtils.log("初始化角色...");
        // 自己
        SFUnitConf heroConf = new SFUnitConf();
        heroConf.uid = SFUserData.instance.uid;
        heroConf.posX = SFBattleData.instance.enterBattle_posX;
        heroConf.posY = SFBattleData.instance.enterBattle_posY;
        heroConf.rotation = SFBattleData.instance.enterBattle_rotation;
        heroConf.speedX = 0;
        heroConf.speedY = 0;
        heroConf.life = SFBattleData.instance.enterBattle_maxLife;
        heroConf.maxLife = SFBattleData.instance.enterBattle_maxLife;
        m_heroController.setHero(addUnit(heroConf));

        // 其他角色
        var users = SFBattleData.instance.enterBattle_remoteUsers;
        foreach (var item in users)
        {
            SFUnitConf conf = new SFUnitConf();
            conf.uid = item.uid;
            conf.posX = item.posX;
            conf.posY = item.posY;
            conf.rotation = item.rotation;
            conf.speedX = item.speedX;
            conf.speedY = item.speedY;
            conf.life = item.life;
            conf.maxLife = item.maxLife;
            addUnit(conf);
        }
        SFUtils.log("初始化角色完成");
    }

    /// <summary>
    /// 根据指定配置信息添加角色到场景
    /// </summary>
    /// <returns>角色controller</returns>
    /// <param name="conf">配置信息</param>
    public SFUnitController addUnit(SFUnitConf conf)
    {
        if (m_controllers.ContainsKey(conf.uid))
        {
            SFUtils.logWarning("已经存在的UID: {0}", conf.uid);
            return null;
        }
        if (unitPrefab != null)
        {
            var controllerGO = GameObject.Instantiate(unitPrefab, gameObject.transform.parent);
            var controller = controllerGO.GetComponent<SFUnitController>();
            controller.init(conf);
            m_controllers.Add(conf.uid, controller);
            var eventData = new SFUnitAddRemove();
            eventData.uid = conf.uid;
            eventData.addOrRemove = true;
            eventData.curLife = conf.life;
            eventData.maxLife = conf.maxLife;
            SFBattleData.instance.dispatcher.dispatchEvent(SFEvent.EVENT_UNIT_ADD_REMOVE, eventData);
            return controller;
        }
        return null;
    }

    /// <summary>
    /// 根据指定uid移除角色
    /// </summary>
    /// <returns>成功移除返回<c>true</c>, 否则返回<c>false</c></returns>
    /// <param name="uid">角色uid</param>
    public bool removeUnit(string uid)
    {
        if (m_controllers.ContainsKey(uid))
        {
            var unit = m_controllers[uid];
            unit.destroy();
            m_controllers.Remove(uid);
            var eventData = new SFUnitAddRemove();
            eventData.uid = uid;
            eventData.addOrRemove = false;
            SFBattleData.instance.dispatcher.dispatchEvent(SFEvent.EVENT_UNIT_ADD_REMOVE, eventData);
            return true;
        }
        return false;
    }

    void onNotifyUnitStatus(SFEvent e)
    {
        var data = e.data as SFResponseMsgNotifyUnitStatus;
//        if (data.runTime < m_runTime - SFCommonConf.instance.maxDiscardLag)
//        {
//            // 这个信息延迟超过100ms了，抛弃掉
//            SFUtils.logWarning("消息延迟了{0}ms, 被抛弃({1} - {2})", data.runTime - m_runTime, data.runTime, m_runTime);
//            return;
//        }
        var infos = data.infos;
        foreach (var item in infos)
        {
            foreach (var controller in m_controllers)
            {
                if (controller.Key == item.uid)
                {
                    controller.Value.updateStatus(item);
                    break;
                }
            }
        }

        var balls = data.balls;
        if (SFBallManager.current != null)
        {
            SFBallManager.current.updateBall(balls);
        }
    }

    void onNotifyUnitJoin(SFEvent e)
    {
        var data = e.data as SFResponseMsgNotifyNewUserJoin;
        if (data.inOrOut)
        {
            SFUnitConf conf = new SFUnitConf();
            conf.uid = data.uid;
            conf.posX = data.posX;
            conf.posY = data.posY;
            conf.rotation = data.rotaion;
            conf.life = data.life;
            conf.maxLife = data.maxLife;
            addUnit(conf);
        }
        else
        {
            removeUnit(data.uid);
        }
    }
}
