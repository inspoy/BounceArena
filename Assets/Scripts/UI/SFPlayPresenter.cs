/**
 * Created on 2017/04/18 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SF;

namespace SF
{
    public class SFPlayPresenter : ISFBasePresenter
    {
        SFPlayView m_view;
        bool m_willSwitch;
        string m_info;

        public void initWithView(SFBaseView view)
        {
            m_view = view as SFPlayView;

            m_view.addEventListener(m_view.btnJoin, SFEvent.EVENT_UI_CLICK, onJoin);
            SFNetworkManager.instance.dispatcher.addEventListener(this, SFResponseMsgNotifyRemoteUsers.pName, onRemoteUsers);
            SFNetworkManager.instance.dispatcher.addEventListener(this, SFResponseMsgJoinRoom.pName, onJoinResult);

            m_view.setUpdator(update);
            m_willSwitch = false;
            m_info = "";
        }

        public void onViewRemoved()
        {
            if (SFNetworkManager.instance.dispatcher != null)
            {
                SFNetworkManager.instance.dispatcher.removeAllEventListenersWithTarget(this);
            }
        }

        void update(float dt)
        {
            m_view.lblInfo.text = m_info;
            if (m_willSwitch)
            {
                m_view.StartCoroutine(loadSceneGame());
                m_willSwitch = false;
            }
        }

        void onJoin(SFEvent e)
        {
            SFRequestMsgJoinRoom req = new SFRequestMsgJoinRoom();
            SFNetworkManager.instance.sendMessage(req);
            m_view.btnJoin.interactable = false;
            m_info = "正在搜索房间，请稍后...";
        }

        void onJoinResult(SFEvent e)
        {
            var data = e.data as SFResponseMsgJoinRoom;
            if (data.retCode == 0)
            {
                m_willSwitch = true;
                m_info = "已找到房间，正在切换场景...";
            }
        }

        IEnumerator loadSceneGame()
        {
            var op = SceneManager.LoadSceneAsync("SceneGame");
            yield return op;
        }

        void onRemoteUsers(SFEvent e)
        {
            var data = e.data as SFResponseMsgNotifyRemoteUsers;
            if (data.retCode == 0)
            {
                SFBattleData.instance.enterBattle_mapId = data.mapId;
                SFBattleData.instance.enterBattle_remoteUsers = data.users;
                SFBattleData.instance.enterBattle_posX = data.posX;
                SFBattleData.instance.enterBattle_posY = data.posY;
                SFBattleData.instance.enterBattle_rotation = data.rotation;
                SFBattleData.instance.enterBattle_maxLife = data.maxLife;
                SFBattleData.instance.enterBattle_initRunTime = data.runTime;

                SFUtils.log("玩家初始坐标:({0}, {1}),rot={2}", data.posX, data.posY, data.rotation);
            }
        }
    }
}
