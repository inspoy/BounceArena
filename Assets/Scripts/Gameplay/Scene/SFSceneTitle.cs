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
        SFSceneManager.addView("vwLogin");
    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.Rotate(Vector3.up, Time.deltaTime * 30);
    }
}
