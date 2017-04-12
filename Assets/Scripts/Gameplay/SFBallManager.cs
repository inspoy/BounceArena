/**
 * Created on 2017/04/11 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFBallManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject ballContainer = null;
    public static SFBallManager current = null;
    Dictionary<string, SFBallController> m_balls;

    // Use this for initialization
    void Start()
    {
        ballContainer = this.gameObject;
        current = this;
        m_balls = new Dictionary<string, SFBallController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 增加一个火球到场景
    /// </summary>
    public void addBall(SFBallConf conf)
    {
        var ballGO = GameObject.Instantiate(ballPrefab);
        var ballComponent = ballGO.GetComponent<SFBallController>();
        ballComponent.init(conf);
        m_balls.Add(conf.ballId, ballComponent);
    }

    /// <summary>
    /// 根据服务端的消息更新火球位置信息
    /// </summary>
    public void updateBall(List<SFMsgDataBallSyncInfo> balls)
    {
        foreach (var item in balls)
        {
            // TODO: 火球同步
        }
    }

    /// <summary>
    /// 移除一个火球
    /// </summary>
    /// <param name="ballId">火球ID</param>
    /// <param name="explode">是否播放爆炸动画, 默认为<c>true</c></param>
    public void removeBall(string ballId, bool explode = true)
    {
        
    }
}
