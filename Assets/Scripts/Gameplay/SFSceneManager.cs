using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF;

public class SFSceneManager : MonoBehaviour
{
    static public GameObject uiRoot = null;

    static public GameObject addView(GameObject prefab, Transform trans = null, int sibIdx = -1)
    {
        if (prefab == null)
        {
            SFUtils.logWarning("prefab为空");
            return null;
        }
        Transform parent = trans;
        if (parent == null)
        {
            parent = SFSceneManager.uiRoot.transform;
        }
        var GO = GameObject.Instantiate(prefab, parent) as GameObject;
        if (sibIdx <= 0)
        {
            GO.transform.SetSiblingIndex(sibIdx);
        }
        return GO;
    }

    static public GameObject addView(string viewName, Transform trans = null, int sibIdx = -1)
    {
        var prefab = Resources.Load("Prefabs/Views/" + viewName) as GameObject;
        if (prefab == null)
        {
            SFUtils.logWarning(string.Format("找不到view:{0}", viewName));
            return null;
        }
        return SFSceneManager.addView(prefab, trans, sibIdx);
    }

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
