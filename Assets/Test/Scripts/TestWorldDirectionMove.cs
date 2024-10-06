// -------------------------
// 创建日期：2024/9/27 14:25:01
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(TestWorldDirectionMove))]
    class TestWorldDirectionMoveInspector : Editor
    {
        TestWorldDirectionMove my => (TestWorldDirectionMove)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Undo.RecordObject(target, "");
            //Undo.RecordObject(this, "");

            if (my)
            {
                if (GUILayout.Button("确定方向"))
                {
                    my.TargetDirectionMove();
                }
            }

        }
    }
#endif


    [ExecuteAlways]
    public class TestWorldDirectionMove : MonoBehaviour
    {
        [Header("用于确定方向的目标")]
        public Transform target;
        [Header("根据目标计算出的方向")]
        public Vector3 direction;
        [Header("移动速度")]
        public float speed = 1;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(direction * Time.deltaTime * speed);
        }

        /// <summary>
        /// 朝目标方向移动
        /// </summary>
        public void TargetDirectionMove()
        {
            if (target != null)
            {
                direction = target.position - transform.position;
                direction = direction.normalized;
            }
        }
    }
}