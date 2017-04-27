
/**
 * Created on 2017/04/07 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SF;

public class SFBattleController : MonoBehaviour
{
    public GameObject unitContainer;
    public SFUnitManager unitMgr;
    private bool m_willReturn = false;
    public static SFBattleController currentBattle = null;
    // Use this for initialization
    void Start()
    {
        if (unitContainer)
        {
            unitMgr = unitContainer.GetComponent<SFUnitManager>();
        }
        SFBattleController.currentBattle = this;

        // 加载场景
        SFUtils.log("mapId:{0}", SFBattleData.instance.enterBattle_mapId);
        string mapName = string.Format("map_{0}", SFBattleData.instance.enterBattle_mapId);
        var mapPrefab = Resources.Load("Prefabs/Maps/" + mapName) as GameObject;
        if (mapPrefab == null)
        {
            SFUtils.log("场景加载失败：Prefabs/Maps/" + mapName);
        }
        else
        {
            GameObject.Instantiate(mapPrefab, unitContainer.transform.parent);
        }

        // 加载HUD
        SFSceneManager.addView("vwHUD");

        // 加载角色
        if (unitMgr)
        {
            unitMgr.initUnits();
        }

        SFBattleData.instance.isGameOver = false;

        // 网络断开
        SFNetworkManager.instance.dispatcher.addEventListener(SFEvent.EVENT_NETWORK_INTERRUPTED, onNetworkInterrupted);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_willReturn)
        {
            m_willReturn = false;
            SceneManager.LoadScene("SceneTitle");
        }
    }

    void onNetworkInterrupted(SFEvent e)
    {
        // 网络连接断开，回到标题页面
        SFUtils.log("[Scene] - 网络连接断开，正在返回登陆界面...");
        m_willReturn = true;
    }
}
