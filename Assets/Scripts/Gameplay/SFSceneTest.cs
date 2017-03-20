/**
 * Created on 2017/03/20 by Inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFSceneTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameObject.Instantiate(Resources.Load("Prefabs/Views/vwTest"), SFSceneManager.uiRoot.transform);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
