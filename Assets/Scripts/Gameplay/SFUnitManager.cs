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

    // Use this for initialization
    void Start()
    {
        m_heroController = GetComponent<SFHeroController>();
        m_controllers = new Dictionary<string, SFUnitController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 根据登陆信息初始化自己以及场上的其他角色
    /// </summary>
    public void initUnits()
    {
        SFUtils.log("初始化角色...");
        // 自己
        SFUnitConf heroConf;
        heroConf.uid = SFUserData.instance.uid;
        heroConf.posX = SFBattleData.instance.enterBattle_posX;
        heroConf.posY = SFBattleData.instance.enterBattle_posY;
        heroConf.rotation = SFBattleData.instance.enterBattle_rotation;
        heroConf.speedX = 0;
        heroConf.speedY = 0;
        m_heroController.setHero(addUnit(heroConf));

        // 其他角色
        var users = SFBattleData.instance.enterBattle_remoteUsers;
        foreach (var item in users)
        {
            SFUnitConf conf;
            conf.uid = item.uid;
            conf.posX = item.posX;
            conf.posY = item.posY;
            conf.rotation = item.rotation;
            conf.speedX = item.speedX;
            conf.speedY = item.speedY;
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
        return false;
    }
}
