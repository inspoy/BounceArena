/**
 * Created on 2017/03/24 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SF;

public static class SFUIExtension
{
    /// <summary>
    /// 扩展UI.Button，增加SetText的方法
    /// </summary>
    /// <param name="btn">Button.</param>
    /// <param name="str">String.</param>
    static public void SetText(this Button btn, string str)
    {
        var com = SFButtonExtension.Get(btn.gameObject);
        if (com != null)
        {
            com.SetText(str);
        }
    }
}

public class SFButtonExtension : MonoBehaviour
{
    /// <summary>
    /// 按钮子节点中包含的Text组件
    /// </summary>
    public Text text;

    private bool m_inited = false;

    void Start()
    {
        if (text == null)
        {
            text = GetComponentInChildren<Text>();
        }
        m_inited = true;
    }

    public void SetText(string str)
    {
        text.text = str;
    }

    static public SFButtonExtension Get(GameObject go)
    {
        if (go == null)
        {
            return null;
        }
        var com = go.GetComponent<SFButtonExtension>();
        if (com == null)
        {
            com = go.AddComponent<SFButtonExtension>();
        }
        if (!com.m_inited)
        {
            com.Start();
        }
        return com;
    }
}