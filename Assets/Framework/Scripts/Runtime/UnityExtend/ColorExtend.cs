// -------------------------
// 创建日期：2023/4/7 18:31:12
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Extend
{
    public static class ColorExtend
    {
        public static Color ToHtmlStringRGB(this Color color)
        {
            ColorUtility.ToHtmlStringRGB(color);
            return color;
        }
        public static Color ToHtmlStringRGBA(this Color color)
        {
            ColorUtility.ToHtmlStringRGBA(color);
            return color;
        }
        public static Color ParseHtmlString(this Color self, string color)
        {
            ColorUtility.TryParseHtmlString(color, out self); 
            return self;
        }
        public static Color ParseHtmlString(this string color)
        {
            ColorUtility.TryParseHtmlString(color, out Color _c);
            return _c;
        }
    }
}