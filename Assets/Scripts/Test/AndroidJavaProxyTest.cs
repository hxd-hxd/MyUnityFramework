// -------------------------
// 创建日期：2023/3/6 18:22:45
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class AndroidJavaProxyTest : AndroidJavaProxy
    {
        public AndroidJavaProxyTest(string javaInterface) : base(javaInterface)
        {

        }

        public AndroidJavaProxyTest(AndroidJavaClass javaInterface) : base(javaInterface)
        {

        }


    }
}