// -------------------------
// 创建日期：2025/12/7 22:01:34
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TestUpdateDeltaTime : MonoBehaviour
    {
        public int targetFrameRate = 60;

        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = targetFrameRate;
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log($"当前帧率：{Application.targetFrameRate}，单帧时间：{Time.deltaTime}，不缩放的单帧时间：{Time.unscaledDeltaTime}");

            Application.targetFrameRate = targetFrameRate;

        }
    }
}