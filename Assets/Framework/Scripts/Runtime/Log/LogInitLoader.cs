using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.LogSystem
{
    /// <summary>
    /// log 初始加载
    /// </summary>
    public class LogInitLoader
    {
#if !LOG_SYSTEM_AUTO_LOADER_DISABLE
        [RuntimeInitializeOnLoadMethod]
#endif
        public static void Load()
        {
#if UNITY_2020_1_OR_NEWER
            var logUIs = GameObject.FindObjectsOfType<LogSystemUI>(true);
#else
            //var logUI = GameObject.FindObjectOfType<LogSystemUI>();
            var logUIs = Resources.FindObjectsOfTypeAll<LogSystemUI>();
#endif
            if (logUIs != null && logUIs.Length > 0)
            {
                foreach (var logUI in logUIs)
                    GameObject.DontDestroyOnLoad(logUI);
            }
            else
            {

                string resPath = "LogUI/LogSystemUI";
                var request = Resources.LoadAsync<GameObject>(resPath);
                request.completed += (ao) =>
                {
                    var logUITemplate = (GameObject)request.asset;
                    if (logUITemplate != null)
                    {
                        var ui = GameObject.Instantiate(logUITemplate);
                        ui.name = $"[{logUITemplate.name}]";
                        GameObject.DontDestroyOnLoad(ui);
                    }
                    else
                    {
                        Debug.LogError($"日志资源不存在：{resPath}");
                    }
                };
            }
        }
    }
}