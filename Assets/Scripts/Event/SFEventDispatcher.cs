/**
 * Created on 2017/3/19 by Inspoy.
 * All rights reserved.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SF
{

    public delegate void SFListenerSelector(SFEvent e);

    /// <summary>
    /// 事件派发器
    /// </summary>
    public class SFEventDispatcher
    {
        Dictionary<string, List<SFListenerSelector> > m_dictListeners;
        object m_target;

        public SFEventDispatcher(object target)
        {
            m_dictListeners = new Dictionary<string, List<SFListenerSelector>>();
            m_target = target;
        }

        /// <summary>
        /// 添加一个监听
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="sel">需要添加的监听</param>
        /// <returns>是否添加成功</returns>
        public bool addEventListener(string eventType, SFListenerSelector sel)
        {
            if (eventType != "" && sel != null) // 判断有效性
            {
                if (hasEventListener(eventType, sel))
                {
                    SFUtils.log(string.Format("重复监听！type={0}", eventType));
                }
                if (!m_dictListeners.ContainsKey(eventType))
                {
                    // 不存在的话就新建一个
                    List<SFListenerSelector> newSelectors = new List<SFListenerSelector>();
                    m_dictListeners[eventType] = newSelectors;
                }
                var selectors = m_dictListeners[eventType];
                selectors.Add(sel);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否已经添加了某事件的某监听
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="sel">需要检查的监听</param>
        /// <returns>是否已经添加</returns>
        public bool hasEventListener(string eventType, SFListenerSelector sel)
        {
            if (m_dictListeners.ContainsKey(eventType))
            {
                List<SFListenerSelector> selectors = m_dictListeners[eventType];
                SFListenerSelector target = selectors.Find(
                    delegate (SFListenerSelector src)
                    {
                        return sel.GetHashCode() == src.GetHashCode();
                    });
                if (target != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除指定监听
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="sel">要移除的监听</param>
        /// <returns>是否移除成功</returns>
        public bool removeEventListener(string eventType, SFListenerSelector sel)
        {
            if (hasEventListener(eventType, sel))
            {
                var selectors = m_dictListeners[eventType];
                selectors.Remove(sel);
            }
            return false;
        }

        /// <summary>
        /// 删除指定事件的所有监听
        /// </summary>
        /// <param name="eventType">不为空时删除指定事件的监听，为空时删除所有监听。默认为空</param>
        /// <returns>删除是否成功</returns>
        public bool removeAllEventListeners(string eventType = "")
        {
            if (eventType == "")
            {
                // 删除所有
                m_dictListeners.Clear();
                return true;
            }
            else
            {
                // 删除指定事件的
                if (m_dictListeners.ContainsKey(eventType))
                {
                    var selectors = m_dictListeners[eventType];
                    selectors.Clear();
                }
            }
            return false;
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="data">事件数据</param>
        /// <returns>通知的订阅者数量</returns>
        public int dispatchEvent(string eventType, ISFEventData data = null)
        {
            SFEvent e = new SFEvent(eventType, m_target);
            e.data = data;
            return dispatchEvent(e);
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="e">事件</param>
        /// <returns>通知的订阅者数量</returns>
        public int dispatchEvent(SFEvent e)
        {
            int count = 0;
            if (m_dictListeners.ContainsKey(e.eventType))
            {
                var selectors = m_dictListeners[e.eventType];
                foreach (var item in selectors)
                {
                    item(e);
                    count += 1;
                }
            }
            return count;
        }
    };

}