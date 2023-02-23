// -------------------------
// 创建日期：2022/10/27 10:12:12
// -------------------------

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Framework
{
    /// <summary>
    /// Resources 资源管理器
    /// </summary>
    public class ResourcesManager
    {
        static Dictionary<string, Object> m_Resources = new Dictionary<string, Object>();

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T Load<T>(string path) where T : UnityEngine.Object
        {
            //Object obj = GetObject(path);
            //T newObj = null;
            //if (obj != null) newObj = (T)obj;

            Object obj = null;
            T newObj = null;
            if (!m_Resources.TryGetValue(path, out obj))
            {
                obj = Resources.Load<T>(path);
                m_Resources.Add(path, obj);
            }
            if (obj != null) newObj = (T)obj;

            return newObj;
        }
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Object Load(string path)
        {
            Object obj = null;
            Object newObj = null;

            if (!m_Resources.TryGetValue(path, out obj))
            {
                obj = Resources.Load(path);
                m_Resources.Add(path, obj);
            }

            //if (obj) newObj = Object.Instantiate(obj);
            newObj = obj;
            return newObj;
        }
    }
}