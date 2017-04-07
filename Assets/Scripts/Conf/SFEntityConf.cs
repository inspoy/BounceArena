/**
 * Created on 2017/04/07 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{
    /// <summary>
    /// 角色配置，用于添加一个角色到场景中
    /// </summary>
    public struct SFUnitConf
    {
        public string uid;
        public float posX;
        public float posY;
        public float rotation;
        public float speedX;
        public float speedY;
    }

    public enum ESkill
    {
        None = 0,
        FireBall = 1,
        Flash = 2,
        Shield = 4
    }
}
