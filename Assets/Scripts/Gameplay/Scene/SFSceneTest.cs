/**
 * Created on 2017/03/20 by Inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFSceneTest : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        float a = 123.456f;
        SFUtils.log(string.Format("{0:D}:{1:F2}", (int)a / 60, a % 60));
    }
}
