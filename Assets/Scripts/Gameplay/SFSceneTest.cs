/**
 * Created on 2017/03/20 by Inspoy
 * All rights reserved.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SF;

public class SFSceneTest : MonoBehaviour
{
    SFTcpClient m_client;

    // Use this for initialization
    void Start()
    {
        SFSceneManager.addView("vwTest");
        m_client = new SFTcpClient();
        m_client.init("127.0.0.1", 12345, onRecvMsg, result =>
        {
            if (result == 0)
            {
                SFUtils.log("服务器连接成功");
                m_client.sendData("Test数据");
            }
            else
            {
                SFUtils.log("服务器连接失败");
            }
        });
        SFUtils.log("正在连接服务器");
    }

    void onRecvMsg(string data)
    {
        SFUtils.log("RecvData: " + data);
    }

    void OnDestroy()
    {
        m_client.close();
        test("共发送{0} Bytes", m_client.totalSendLength);
        test("共接收{0} Bytes", m_client.totalRecvLength);
        test("连接已关闭");
    }

    void test(string msg, params object[] paras)
    {
        if (paras.Length == 0)
        {
            SFUtils.log(msg);
        }
        else
        {
            SFUtils.log(string.Format(msg, paras));
        }
    }
}
