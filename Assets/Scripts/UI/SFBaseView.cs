using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFBaseView : MonoBehaviour
{
    public delegate void SFViewUdpate(float dt);

    protected ISFBasePresenter m_presenter;
    bool m_isViewRemoved = false;
    SFViewUdpate m_updator = null;
    SFViewUdpate m_fixedUpdator = null;

    public bool isViewRemoved{ get { return m_isViewRemoved; } }

    /// <summary>
    /// 添加UI事件监听
    /// </summary>
    /// <param name="widget">UI组件</param>
    /// <param name="eventType">事件类型</param>
    /// <param name="sel">回调函数</param>
    public void addEventListener(Component widget, string eventType, SFListenerSelector sel)
    {
        var dispatcher = SFUIEventListener.getDispatcherWithGo(widget.gameObject);
        dispatcher.addEventListener(eventType, sel);
    }

    /// <summary>
    /// 移除UI View
    /// </summary>
    /// <param name="immediately">If set to <c>true</c> immediately.</param>
    public void removeView(bool immediately = false)
    {
        m_presenter.onViewRemoved();
        m_isViewRemoved = true;
        if (immediately)
        {
            GameObject.DestroyImmediate(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    /// <summary>
    /// 设置Update函数
    /// </summary>
    /// <param name="updator">update函数</param>
    /// <param name="isFixed"><c>true</c>表示使用FixedUpdate</param>
    public void setUpdator(SFViewUdpate updator, bool isFixed = false)
    {
        if (isFixed)
        {
            m_fixedUpdator = updator;
        }
        else
        {
            m_updator = updator;
        }
    }

    void Update()
    {
        if (m_updator != null)
        {
            m_updator(Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (m_fixedUpdator != null)
        {
            m_fixedUpdator(Time.fixedDeltaTime);
        }
    }

    void OnDestroy()
    {
        if (!m_isViewRemoved)
        {
            // 意外的情况，没有清理presenter的话补调用一下
            m_presenter.onViewRemoved();
        }
    }
}
