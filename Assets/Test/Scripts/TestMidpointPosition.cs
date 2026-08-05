using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    // 测试本地坐标的转换
    [ExecuteAlways]
    public class TestMidpointPosition : MonoBehaviour
    {
        public MidpointMode midpointMode = MidpointMode.Pairwise;
        public List<Transform> targets;
        public Vector3 midPos;

        void Start()
        {

        }

        void Update()
        {
            midPos = Vector3.zero;

            switch (midpointMode)
            {
                case MidpointMode.Average:
                    // 方案1：计算所有目标的平均位置
                    foreach (var target in targets)
                    {
                        if (target == null) continue;
                        midPos += target.position;
                    }
                    midPos /= targets.Count;
                    break;
                case MidpointMode.Bounds:
                    {
                        // 方案2：使用Bounds计算包围盒的中心点
                        // 此方法最准确，能完美区分计算边缘和内部
                        Vector3 bPos = Vector3.zero;
                        int firstValidIndex = targets.FindIndex(0, targets.Count, target => target != null);
                        if (firstValidIndex != -1) bPos = targets[firstValidIndex].position;
                        Bounds bounds = new Bounds(bPos, Vector3.zero);
                        foreach (var target in targets)
                        {
                            if (target == null) continue;
                            var renderer = target.GetComponent<Renderer>();
                            if (renderer)
                            {
                                bounds.Encapsulate(renderer.bounds);
                            }
                            else
                            {
                                //var tBounds = new Bounds(target.position, target.localScale);
                                bounds.Encapsulate(target.position);
                            }
                        }
                        midPos = bounds.center;
                    }
                    break;
                case MidpointMode.Pairwise:
                    {
                        // 方案3：两两计算中心点，得出的结果再与后面的计算中心点
                        // 此方法的结果偏向于靠近最后一个目标
                        int firstValidIndex = targets.FindIndex(0, targets.Count, target => target != null);
                        if (firstValidIndex != -1) midPos = targets[firstValidIndex].position;
                        for (int i = firstValidIndex + 1; i < targets.Count; i++)
                        {
                            var target = targets[i];
                            if (target == null) continue;
                            //midPos = (target.position - midPos) / 2 + midPos;
                            midPos = (target.position + midPos) / 2;
                        }
                    }
                    break;
                default:
                    break;
            }

            transform.position = midPos;
        }

        /// <summary>中点计算方式</summary>
        public enum MidpointMode
        {
            /// <summary>所有求均值</summary>
            Average,
            /// <summary>使用包围盒计算</summary>
            Bounds,
            /// <summary>两两计算</summary>
            Pairwise
        }
    }
}