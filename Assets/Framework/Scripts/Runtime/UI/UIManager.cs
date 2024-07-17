using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 用于管理 <see cref="IUI"/> 和 <see cref="BaseUI"/>
    /// </summary>
    public static class UIManager
    {
        static Dictionary<Type, IUI> uis = new Dictionary<Type, IUI>(5);

        public static bool IsRegister<T>(T ui) where T : class, IUI
        {
            if (ui == null)
            {
                Debug.LogError("空 ui ");
                return false;
            }
            Type type = ui.GetType();
            if (uis.TryGetValue(type, out var bui))
            {
                // 判断实例
                //if (bui != ui)
                //if (!object.ReferenceEquals(bui, ui))
                if (!object.Equals(bui, ui))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static bool Register<T>(T ui) where T : class, IUI
        {
            if (ui == null)
            {
                Debug.LogError("不能注册空 ui");
                return false;
            }
            Type type = ui.GetType();
            if (uis.TryGetValue(type, out var bui))
            {
                //if (bui == ui)
                if (object.Equals(bui, ui))
                {
                    Debug.LogWarning($"已经注册过 ui \"{ui.name}\"");
                    return false;
                }
                else
                {
                    Debug.LogWarning($"已经注册过 \"{type.Name}\" 的同类型 ui，将使用 \"{ui.name}\" 替换 \"{bui?.name}\"");
                }
            }

            uis[type] = ui;
            return true;
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static bool Unregister<T>(T ui) where T : class, IUI
        {
            if (ui == null)
            {
                Debug.LogError($"不能注销空 ui 实例 \"{typeof(T)}\"");
                return false;
            }
            Type type = ui.GetType();
            if (uis.TryGetValue(type, out var bui))
            {
                //if (bui == ui)
                if (object.Equals(bui, ui))
                {
                    uis.Remove(type);
                }
                else
                {
                    Debug.LogWarning($"不能注销 ui 实例 \"{ui}\"，和已注册的 \"{bui}\" 不是同一个 \"{typeof(T)}\" 实例");
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Unregister<T>() where T : class, IUI
        {
            Type type = typeof(T);
            if (uis.ContainsKey(type))
            {
                uis.Remove(type);
            }
            else { return false; }

            return true;
        }

        public static T GetUI<T>() where T : class, IUI
        {
            Type type = typeof(T);
            T ui = default;
            if (uis.TryGetValue(type, out var bui))
            {
                if (bui != null)
                {
                    ui = (T)bui;
                }
                else
                {
                    Debug.LogWarning($"要获取的 ui \"{type.Name}\" 为空，可能已经被销毁");
                }
            }
            else
            {
                Debug.LogWarning($"要获取的 ui \"{type.Name}\" 不存在，可能未注册到管理器");
            }
            return ui;
        }
        /// <summary>
        /// 期望获取指定类型的 ui
        /// <para>如果未从已注册列表中找到，则会尝试从已加载的 Unity 对象中找，找到后会将其注册到管理器</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ExpectGetUI<T>() where T : class, IUI
        {
            Type type = typeof(T);
            T ui = GetUI<T>();
            if (ui == null)
            {
                //var m = ui as MonoBehaviour;
                //if (m == null)
                //{
                //    ui = GameObject.FindObjectOfType(type, true) as T;// 尝试从已加载的 Unity 对象中找
                //    if (ui != null)
                //    {
                //        Register(ui);
                //    }
                //}

#if UNITY_2020_1_OR_NEWER
                ui = GameObject.FindObjectOfType(type, true) as T;// 尝试从已加载的 Unity 对象中找
#else
                //ui = GameObject.FindObjectOfType(type) as T;// 尝试从已加载的 Unity 对象中找
                var uis = Resources.FindObjectsOfTypeAll(type);
                //ui = (uis.Length > 0 ? uis[0] : null) as T;
                foreach (var item in uis)
                {
                    if (item is MonoBehaviour cui)
                    {
                        if(cui.gameObject.scene.path != "")// 排除预制体
                        {
                            ui = item as T;
                            break;
                        }
                    }
                }
                if(ui == null) ui = (uis.Length > 0 ? uis[0] : null) as T;
#endif
                if (ui != null)
                {
                    Register(ui);
                }
            }
            return ui;
        }
    }
}