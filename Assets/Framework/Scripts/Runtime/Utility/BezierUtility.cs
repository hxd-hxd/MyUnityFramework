// -------------------------
// 创建日期：2024/8/12 14:56:19
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class BezierUtility
    {

        /// <summary>
        /// 计算曲线位置
        /// </summary>
        /// <param name="start"></param>
        /// <param name="mid"></param>
        /// <param name="end"></param>
        /// <param name="t">曲线进度比率 0 到 1</param>
        /// <returns></returns>
        public static Vector3 Bezier(Vector3 start, Vector3 mid, Vector3 end, float t)
        {
            Vector3 p0p1 = (1 - t) * start + t * mid;
            Vector3 p1p2 = (1 - t) * mid + t * end;
            Vector3 result = (1 - t) * p0p1 + t * p1p2;
            return result;
        }
        /// <summary>
        /// 获取贝塞尔曲线路径，计算的路径点存入 paths
        /// </summary>
        /// <param name="start"></param>
        /// <param name="mid"></param>
        /// <param name="end"></param>
        /// <param name="count"></param>
        /// <param name="paths"></param>
        public static bool GetBezierPath(Transform start, Transform mid, Transform end, int count, List<Vector3> paths)
        {
            if (!start || !end) return false;

            GetBezierPath(start.position, mid ? mid.position : Vector3.zero, end.position, count, paths);
            return true;
        }
        /// <summary>
        /// 获取贝塞尔曲线路径，计算的路径点存入 paths
        /// </summary>
        /// <param name="start"></param>
        /// <param name="mid"></param>
        /// <param name="end"></param>
        /// <param name="count"></param>
        /// <param name="paths"></param>
        public static void GetBezierPath(Vector3 start, Vector3 mid, Vector3 end, int count, List<Vector3> paths)
        {
            int num = count < 1 ? 1 : count;
            float u = 1f / num;
            //paths.Add(p0);
            for (float i = 0; i < 1; i += u)
            {
                var b = BezierUtility.Bezier(start, mid, end, i);
                paths.Add(b);
            }
            paths.Add(end);
        }

        /// <summary>
        /// 三个控制点的贝塞尔曲线
        /// </summary>
        /// <param name="handles"></param>
        /// <param name="vertexCount"></param>
        /// <returns>返回贝塞尔曲线路径点表</returns>
        public static List<Vector3> BezierCurveWithThree(Transform[] handles, int vertexCount)
        {
            List<Vector3> pointList = new List<Vector3>();
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                Vector3 tangentLineVertex1 = Vector3.Lerp(handles[0].position, handles[1].position, ratio);
                Vector3 tangentLineVertex2 = Vector3.Lerp(handles[1].position, handles[2].position, ratio);
                Vector3 bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                pointList.Add(bezierPoint);
            }
            pointList.Add(handles[2].position);

            return pointList;
        }

        /// <summary>
        /// 超过三个控制点的贝塞尔曲线
        /// </summary>
        /// <param name="handlesPositions"></param>
        /// <param name="vertexCount"></param>
        public static List<Vector3> BezierCurveWithUnlimitPoints(Transform[] handlesPositions, int vertexCount)
        {
            List<Vector3> pointList = new List<Vector3>();
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                pointList.Add(UnlimitBezierCurve(handlesPositions, ratio));
            }
            pointList.Add(handlesPositions[handlesPositions.Length - 1].position);

            return pointList;
        }
        public static Vector3 UnlimitBezierCurve(Transform[] trans, float t)
        {
            Vector3[] temp = new Vector3[trans.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = trans[i].position;
            }
            int n = temp.Length - 1;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n - i; j++)
                {
                    temp[j] = Vector3.Lerp(temp[j], temp[j + 1], t);
                }
            }
            return temp[0];
        }
    }
}