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
        static void Load()
        {
            var logUI = GameObject.FindObjectOfType<LogSystemUI>(true);
            if (logUI)
            {
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