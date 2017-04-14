/**
 * Created on 2017/04/07 by inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFProgressBar : MonoBehaviour
{
    public Slider slider;

    /// <summary>
    /// 设置进度条的值
    /// </summary>
    /// <param name="val">范围从0-1</param>
    public void setProgress(float val)
    {
        val = Mathf.Clamp01(val);
        if (slider != null)
        {
            slider.value = val;
        }
    }

    /// <summary>
    /// 返回进度条当前的值
    /// </summary>
    /// <returns>The progress.</returns>
    public float getProgress()
    {
        if (slider != null)
        {
            return slider.value;
        }
        return 0;
    }
}