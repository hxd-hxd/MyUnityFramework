// -------------------------
// 创建日期：2023/4/7 17:22:04
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Framework
{
    /// <summary>
    /// 
    /// </summary>
    public static class TextUtility
    {
        /// <summary>
        /// 设置文本内容
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textContent"></param>
        public static void SetText(object text, string textContent)
        {
            if (text == null)
            {
                return;
            }

            if (text is Text _text)
            {
                _text.text = textContent;
            }
            else if (text is TMP_Text _texttmp)
            {
                _texttmp.text = textContent;
            }
            else
            {
                Log.Error($"不受支持的类型 {text.GetType()}");
            }
        }

        /// <summary>
        /// 获取文本内容
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textContent"></param>
        public static string GetText(object text)
        {
            if (text == null)
            {
                return null;
            }

            if (text is Text _text)
            {
                return _text.text;
            }
            else if (text is TMP_Text _texttmp)
            {
                return _texttmp.text;
            }
            else
            {
                Log.Error($"不受支持的类型 {text.GetType()}");
            }

            return null;
        }
    }
}