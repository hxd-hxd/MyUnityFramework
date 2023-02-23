// -------------------------
// 创建日期：2022/10/27 11:47:04
// -------------------------


using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface IResourcesManager
    {
        T GetObject<T>(string path) where T : Object;

        Object GetObject(string path);
    }
}