// -------------------------
// 创建日期：2022/10/31 22:34:33
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Networking.UnityWebRequest;

namespace Framework
{
    /// <summary>
    /// 点击事件穿透，使 其当前层和下层都可接收到事件
    /// </summary>
    public class EventTriggerTrans : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            
        }

        //监听按下
        public void OnPointerDown(PointerEventData eventData)
        {
            //PassEvent(eventData, ExecuteEvents.pointerDownHandler);
        }

        //监听抬起
        public void OnPointerUp(PointerEventData eventData)
        {
            //PassEvent(eventData, ExecuteEvents.pointerUpHandler);
        }

        //监听点击
        public void OnPointerClick(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.submitHandler);
            PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        }


        //把事件透下去
        public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
            where T : IEventSystemHandler
        {
            results.Clear();
            EventSystem.current.RaycastAll(data, results);
            GameObject current = data.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                GameObject go = results[i].gameObject;
                if (current != go)
                {
                    //data.pointerEnter = go;
                    //data.pointerClick = go;
                    //data.pointerCurrentRaycast = results[i];

                    try
                    {
                        ExecuteEvents.Execute(go, data, function);
                    }
                    catch (Exception e)
                    {
                        Log.Error("事件穿透引发的异常");
                        Log.Error(e);
                        //throw;
                    }
                    break;
                    //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
                }
            }
        }
    }
}