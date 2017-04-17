/**
 * Created on 2017/3/19 by Inspoy.
 * All rights reserved.
 */

using UnityEngine;
using System.Collections;

namespace SF
{
    /// <summary>
    /// 简单的事件数据，可以存储1个int，1个float，1个string以及一个object
    /// </summary>
    public class SFSimpleEventData : ISFEventData
    {
        public SFSimpleEventData(int _val)
        {
            intVal = _val;
        }

        public SFSimpleEventData(float _val)
        {
            floatVal = _val;
        }

        public SFSimpleEventData(string _val)
        {
            strVal = _val;
        }

        public object objVal = null;
        public int intVal = 0;
        public float floatVal = 0;
        public string strVal = "";
    };

    /// <summary>
    /// 用于记录角色操作的事件数据
    /// </summary>
    public class SFUnitOperationStateData : ISFEventData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SF.SFUnitOperationState"/> class.
        /// </summary>
        public SFUnitOperationStateData()
        {
            moveX = 0;
            moveY = 0;
            rotation = 0;
        }

        /// <summary>
        /// 范围在-1~1之间
        /// </summary>
        public float moveX;

        /// <summary>
        /// 范围在-1~1之间
        /// </summary>
        public float moveY;

        /// <summary>
        /// 朝向，即transform.rotation.eulerAngles.y。0为朝上
        /// </summary>
        public float rotation;
    };

    /// <summary>
    /// 用于同步角色位置和速度信息的事件数据
    /// </summary>
    public class SFUnitStateSyncData : ISFEventData
    {
        /// <summary>
        /// 当前的位置X坐标
        /// </summary>
        public float posX;

        /// <summary>
        /// 当前的位置Y坐标
        /// </summary>
        public float posY;

        /// <summary>
        /// 当前的朝向，0-360
        /// </summary>
        public float rotation;

        /// <summary>
        /// 当前X方向的的速度
        /// </summary>
        public float speedX;

        /// <summary>
        /// 当前Y方向的速度
        /// </summary>
        public float speedY;
    };

    /// <summary>
    /// 用于同步血量给UI
    /// </summary>
    public class SFUnitLifeChange : ISFEventData
    {
        public string uid;
        public int curLife;
        public int maxLife;
    };

    public class SFUnitAddRemove : ISFEventData
    {
        public string uid;
        public bool addOrRemove;
        public int curLife;
        public int maxLife;
    }
}