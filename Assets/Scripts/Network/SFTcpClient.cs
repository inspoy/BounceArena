/**
 * Created on 2017/03/23 by inspoy
 * All rights reserved.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace SF
{
    public delegate void SFClientCallback(string data);

    public class SFTcpClient
    {
        public SFTcpClient()
        {
        }

        public bool init(string ip, int port, SFClientCallback callback)
        {
            return true;
        }
    }
}
