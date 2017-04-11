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
    string m_ballId;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init(SFBallConf conf)
    {
        transform.position = new Vector3(conf.posX, 0, conf.posY);
        m_ballId = conf.ballId;
    }
}
