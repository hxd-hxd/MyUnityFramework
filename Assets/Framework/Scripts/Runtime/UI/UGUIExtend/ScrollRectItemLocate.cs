// -------------------------
// 创建日期：2023/4/11 15:09:47
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// 根据 <see cref="ScrollRect"/> 组件的 项进行定位
    /// </summary>
    public class ScrollRectItemLocate
    {
        /// <summary>
        /// 定位
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="item"></param>
        public static void Locate(ScrollRect sr, RectTransform item)
        {
            var rt = sr.GetComponent<RectTransform>();
            
            float srWidthHalf = rt.rect.width / 2;
            float srHeightHalf = rt.rect.height / 2;

            // item 相对 ScrollRect 的位置
            Vector3 itemRelativeSRPos = item.position - rt.position;


        }
    }
}