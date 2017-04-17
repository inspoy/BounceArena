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
            com.setText(str);
        }
    }

    /// <summary>
    /// 设置填充类型
    /// </summary>
    /// <param name="scr">ScrollRect.</param>
    /// <param name="type">1-Vertical 2-Horizonal</param>
    static public void SetFillType(this ScrollRect scr, int type)
    {
        var com = SFScrollRectExtension.Get(scr.gameObject);
        if (com != null)
        {
            if (type == 1 || type == 2)
            {
                com.fillType = type;
            }
        }
    }

    /// <summary>
    /// 扩展ScrollRect，增加AddItem的方法
    /// </summary>
    /// <param name="scr">ScrollRect</param>
    /// <param name="item">要加的Item</param>
    static public GameObject AddItem(this ScrollRect scr, GameObject item)
    {
        var com = SFScrollRectExtension.Get(scr.gameObject);
        if (com != null)
        {
            return com.addItem(item);
        }
        return null;
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

    public void setText(string str)
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

public class SFScrollRectExtension : MonoBehaviour
{
    public ScrollRect sr;
    /// <summary>
    /// 1-Vertical 2-Horizonal
    /// </summary>
    public int fillType = 1;

    private bool m_inited = false;
    private float m_curLength;
    private int m_curItemCount;
    private List<GameObject> m_items;

    void Start()
    {
        if (sr == null)
        {
            sr = GetComponent<ScrollRect>();
        }
        m_inited = true;
        m_curLength = 0;
        m_curItemCount = 0;
        m_items = new List<GameObject>();
    }

    public GameObject addItem(GameObject item)
    {
        if (item == null)
        {
            return null;
        }
        var trans = item.GetComponent<RectTransform>();
        if (trans == null)
        {
            return null;
        }
        m_items.Add(item);
        m_curItemCount += 1;
        Vector3 newPos;
        if (fillType == 1)
        {
            // 纵向
            m_curLength += trans.sizeDelta.y;
            newPos = new Vector3(0, m_curLength - trans.sizeDelta.y / 2);
        }
        else
        {
            // 横向
            m_curLength += trans.sizeDelta.x;
            newPos = new Vector3(m_curLength - trans.sizeDelta.x / 2, -trans.sizeDelta.y / 2);
        }
        var newOne = GameObject.Instantiate(item, sr.content.transform, false);
        newOne.transform.localPosition = newPos;
        return newOne;
    }

    static public SFScrollRectExtension Get(GameObject go)
    {
        if (go == null)
        {
            return null;
        }
        var com = go.GetComponent<SFScrollRectExtension>();
        if (com == null)
        {
            com = go.AddComponent<SFScrollRectExtension>();
        }
        if (!com.m_inited)
        {
            com.Start();
        }
        return com;
    }
}
