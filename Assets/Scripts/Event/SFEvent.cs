/**
 * Created on 2017/3/19 by Inspoy.
 * All rights reserved.
 */

using UnityEngine;
using System.Collections;

namespace SF
{

    /// <summary>
    /// 事件
    /// </summary>
    public class SFEvent
    {
        public SFEvent(string _eventType, object _target)
        {
            eventType = _eventType;
            target = _target;
        }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string eventType;
        /// <summary>
        /// 事件包含的自定义数据
        /// </summary>
        public ISFEventData data;
        /// <summary>
        /// 事件发送者
        /// </summary>
        public object target;

        // 以下是事件枚举
        public const string EVENT_UI_CLICK = "EVENT_CLICK";
        public const string EVENT_UI_DOWN = "EVENT_UI_DOWN";
        public const string EVENT_UI_UP = "EVENT_UI_UP";
        public const string EVENT_UI_ENTER = "EVENT_UI_ENTER";
        public const string EVENT_UI_EXIT = "EVENT_UI_EXIT";
        public const string EVENT_UI_SELECT = "EVENT_UI_SELECT";
        public const string EVENT_UI_UPDATE_SELECTED = "EVENT_UI_UPDATE_SELECTED";
        public const string EVENT_UI_SUBMIT = "EVENT_UI_SUBMIT";

        public const string EVENT_TEST = "EVENT_TEST";
        public const string EVENT_HERO_OPERATION = "EVENT_HERO_OPERATION";                  // 主角操作
        public const string EVENT_HERO_SKILL = "EVENT_HERO_SKILL";                          // 主角释放技能
        public const string EVENT_UNIT_SYNC = "EVENT_UNIT_SYNC";                            // 同步角色状态
        public const string EVENT_NETWORK_READY = "EVENT_NETWORK_READY";                    // 网络连接就绪
        public const string EVENT_NETWORK_INTERRUPTED = "EVENT_NETWORK_INTERRUPTED";        // 网络连接中断
    };

    /// <summary>
    /// 事件数据
    /// </summary>
    public interface ISFEventData
    {
    };

}