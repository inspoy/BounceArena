/**
 * Created on 2017/04/05 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFSceneTitle : MonoBehaviour
{
    public GameObject cube;

    // Use this for initialization
    void Start()
    {
        if (SFNetworkManager.instance != null && SFNetworkManager.instance.isReady())
        {
            SFSceneManager.addView("vwMain");
        }
        else
        {
            SFSceneManager.addView("vwLogin");
        }
    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.Rotate(Vector3.up, Time.deltaTime * 30);
    }
}
