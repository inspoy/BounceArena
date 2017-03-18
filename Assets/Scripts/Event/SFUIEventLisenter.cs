using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SF;

public class SFUIEventListener : EventTrigger
{
    public SFEventDispatcher dispatcher = null;

    /// <summary>
    /// 根据UI控件获取它的事件派发器，没有的话会自动创建
    /// </summary>
    /// <param name="go">指定的UI控件</param>
    /// <returns>它的事件派发器</returns>
    public static SFEventDispatcher getDispatcherWithGo(GameObject go)
    {
        var listener = go.GetComponent<SFUIEventListener>();
        if (listener == null)
        {
            listener = go.AddComponent<SFUIEventListener>();
        }
        if (listener.dispatcher == null)
        {
            listener.dispatcher = new SFEventDispatcher(go);
        }

        return listener.dispatcher;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (dispatcher != null)
        {
            dispatcher.dispatchEvent(SFEvent.EVENT_UI_CLICK);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (dispatcher != null)
        {
            dispatcher.dispatchEvent(SFEvent.EVENT_UI_DOWN);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (dispatcher != null)
        {
            dispatcher.dispatchEvent(SFEvent.EVENT_UI_UP);
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (dispatcher != null)
        {
            dispatcher.dispatchEvent(SFEvent.EVENT_UI_ENTER);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (dispatcher != null)
        {
            dispatcher.dispatchEvent(SFEvent.EVENT_UI_EXIT);
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (dispatcher != null)
        {
            dispatcher.dispatchEvent(SFEvent.EVENT_UI_SELECT);
        }
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (dispatcher != null)
        {
            dispatcher.dispatchEvent(SFEvent.EVENT_UI_UPDATE_SELECTED);
        }
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        if (dispatcher != null)
        {
            dispatcher.dispatchEvent(SFEvent.EVENT_UI_SUBMIT);
        }
    }
}
