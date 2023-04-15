// -------------------------
// 创建日期：2023/4/7 18:21:33
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Framework;
using Framework.TextMeshProExpress;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

namespace Test
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TestDropdownPro))]
    class TestDropdownProInspector : Editor
    {
        int itemLocateValue = 0;
        int itemValue = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = EditorApplication.isPlaying;

            var my = (TestDropdownPro)target;

            GUILayout.Space(8);
            if (GUILayout.Button("随机添加一项", GUILayout.MinHeight(32)))
            {
                my.随机添加一项();
            }
            if (GUILayout.Button("最后一位移到第一位", GUILayout.MinHeight(32)))
            {
                my.最后一位移到第一位();
            }
            if (GUILayout.Button("移除第一位", GUILayout.MinHeight(32)))
            {
                my.移除第一位();
            }

            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            {
                itemLocateValue = EditorGUILayout.IntField(itemLocateValue);
                GUILayout.Space(8);
                if (GUILayout.Button("定位到目标"))
                {
                    my.定位元素(itemLocateValue);
                }
                GUILayout.Space(8);
                if (GUILayout.Button("定位到当前"))
                {
                    my.定位到当前元素();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                itemValue = EditorGUILayout.IntField(itemValue);
                GUILayout.Space(8);
                if (GUILayout.Button("选中"))
                {
                    my.选择元素(itemValue);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
        }
    }
#endif

    public class TestDropdownPro : MonoBehaviour
    {
        [Space]
        [Header("测试")]
        [Header("按键盘 P \t在随机位置添加一项")]
        [Header("按键盘 K \t将最后一位移到第一位")]
        [Header("按键盘 Shift + R \t移除第一位数据项")]
        public DropdownPro dropdownPro;
        public DropdownPro_Text dropdownProText;
        public DropdownPro_TMPText dropdownProTMP;

        [Space]
        public int createNum = 500;

        void Start()
        {
            Init(dropdownProText);

            Init(dropdownProTMP);

        }

        private void Update()
        {

            UpdataDP(dropdownProText);

            UpdataDP(dropdownProTMP);

        }

        public void Init<T>(DropdownProAbstract<T> dp) where T : MaskableGraphic
        {
            if (dp)
            {
                dp.SelectedEvent += (data) =>
                {
                    Log.Yellow($"选中：{data}");
                };

                for (int i = 0; i < createNum; i++)
                {
                    dp.AddData(i.ToString());
                }
            }
        }

        public void UpdataDP<T>(DropdownProAbstract<T> dp) where T : MaskableGraphic
        {
            if (dp)
            {
                if (Input.GetKeyDown(KeyCode.K))
                {
                    最后一位移到第一位(dp);
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    随机添加一项(dp);
                }

                if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.R))
                {
                    移除第一位(dp);
                }
            }
        }


        public void 随机添加一项<T>(DropdownProAbstract<T> dp) where T : MaskableGraphic
        {
            if (dp)
            {
                dp.AddOption(Random.Range(0, 1000000).ToString(), Random.Range(0, dp.Options.Count));
            }
        }
        public void 随机添加一项()
        {
            随机添加一项(dropdownProText);
            随机添加一项(dropdownProTMP);
        }


        public void 最后一位移到第一位<T>(DropdownProAbstract<T> dp) where T : MaskableGraphic
        {
            if (dp)
            {
                // 将最后一个挪到第一个

                //Options.MoveIndex(0, Options.Count - 1);
                //RefreshMenu();

                // 以上等价于下方

                dp.SetOptionPos(dp.Options.Count - 1, 0);
            }
        }
        public void 最后一位移到第一位()
        {
            最后一位移到第一位(dropdownProText);
            最后一位移到第一位(dropdownProTMP);
        }


        public void 移除第一位<T>(DropdownProAbstract<T> dp) where T : MaskableGraphic
        {
            if (dp)
            {
                dp.RemoveOption(0);
            }
        }
        public void 移除第一位()
        {
            移除第一位(dropdownProText);
            移除第一位(dropdownProTMP);
        }

        public void 选择元素<T>(DropdownProAbstract<T> dp, int itemIndex) where T : MaskableGraphic
        {
            if (dp)
            {
                dp.SelectOption(itemIndex);
            }
        }
        public void 选择元素(int itemIndex)
        {
            选择元素(dropdownProText, itemIndex);
            选择元素(dropdownProTMP, itemIndex);
        }

        public void 定位到当前元素<T>(DropdownProAbstract<T> dp) where T : MaskableGraphic
        {
            if (dp)
            {
                dp.LocateCurrent();
            }
        }
        public void 定位到当前元素()
        {
            定位到当前元素(dropdownProText);
            定位到当前元素(dropdownProTMP);
        }

        public void 定位元素<T>(DropdownProAbstract<T> dp, int itemIndex) where T : MaskableGraphic
        {
            if (dp)
            {
                dp.LocateTarget(itemIndex);
            }
        }
        public void 定位元素(int itemIndex)
        {
            定位元素(dropdownProText, itemIndex);
            定位元素(dropdownProTMP, itemIndex);
        }
    }
}