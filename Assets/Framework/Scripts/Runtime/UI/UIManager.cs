using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// ���ڹ��� <see cref="IUI"/> �� <see cref="BaseUI"/>
    /// </summary>
    public static class UIManager
    {
        static Dictionary<Type, IUI> uis = new Dictionary<Type, IUI>(5);

        public static bool IsRegister<T>(T ui) where T : class, IUI
        {
            if (ui == null)
            {
                //Debug.LogError("�� ui ");
                return false;
            }
            Type type = ui.GetType();
            if (uis.TryGetValue(type, out var bui))
            {
                // �ж�ʵ��
                if (bui is UnityEngine.Object uui)
                {
                    return uui;
                }
                else
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
        /// ע��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static bool Register<T>(T ui) where T : class, IUI
        {
            if (ui == null)
            {
                Debug.LogError("����ע��� ui");
                return false;
            }
            Type type = ui.GetType();
            if (uis.TryGetValue(type, out var bui))
            {
                //if (bui == ui)
                if (object.Equals(bui, ui))
                {
                    Debug.LogWarning($"�Ѿ�ע��� ui \"{ui.name}\"");
                    return false;
                }
                else
                {
                    Debug.LogWarning($"�Ѿ�ע��� \"{type.Name}\" ��ͬ���� ui����ʹ�� \"{ui.name}\" �滻 \"{bui?.name}\"");
                }
            }

            uis[type] = ui;
            return true;
        }
        /// <summary>
        /// ע��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static bool Unregister<T>(T ui) where T : class, IUI
        {
            if (ui == null)
            {
                Debug.LogError($"����ע���� ui ʵ�� \"{typeof(T)}\"");
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
                    Debug.LogWarning($"����ע�� ui ʵ�� \"{ui}\"������ע��� \"{bui}\" ����ͬһ�� \"{typeof(T)}\" ʵ��");
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// ע��
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
                if (bui is UnityEngine.Object uui)
                {
                    if (uui != null)
                    {
                        ui = (T)bui;
                    }
                    else
                    {
                        Debug.LogWarning($"Ҫ��ȡ�� ui \"{type.Name}\" Ϊ�գ������Ѿ�������");
                    }
                }
                else
                if (bui != null)
                {
                    ui = (T)bui;
                }
                else
                {
                    Debug.LogWarning($"Ҫ��ȡ�� ui \"{type.Name}\" Ϊ�գ������Ѿ�������");
                }
            }
            else
            {
                Debug.LogWarning($"Ҫ��ȡ�� ui \"{type.Name}\" �����ڣ�����δע�ᵽ������");
            }
            return ui;
        }
        /// <summary>
        /// ������ȡָ�����͵� ui
        /// <para>���δ����ע���б����ҵ�����᳢�Դ��Ѽ��ص� Unity �������ң��ҵ���Ὣ��ע�ᵽ������</para>
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
                //    ui = GameObject.FindObjectOfType(type, true) as T;// ���Դ��Ѽ��ص� Unity ��������
                //    if (ui != null)
                //    {
                //        Register(ui);
                //    }
                //}

#if UNITY_2020_1_OR_NEWER
                ui = GameObject.FindObjectOfType(type, true) as T;// ���Դ��Ѽ��ص� Unity ��������
#else
                //ui = GameObject.FindObjectOfType(type) as T;// ���Դ��Ѽ��ص� Unity ��������
                var uis = Resources.FindObjectsOfTypeAll(type);
                //ui = (uis.Length > 0 ? uis[0] : null) as T;
                foreach (var item in uis)
                {
                    if (item is MonoBehaviour cui)
                    {
                        if(cui.gameObject.scene.path != "")// �ų�Ԥ����
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