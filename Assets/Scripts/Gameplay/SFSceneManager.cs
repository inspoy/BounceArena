using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFSceneManager : MonoBehaviour
{
    static public GameObject uiRoot = null;

    // Use this for initialization
    void Awake()
    {
        var uiRootGO = GameObject.Find("UIRoot");
        if (uiRootGO == null)
        {
            SFUtils.logWarning("当前场景没有找到UIRoot节点");
        }
        uiRoot = uiRootGO;
    }

    void OnDestroy()
    {
        uiRoot = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        // 需要Update的都放在这里
    }
}
