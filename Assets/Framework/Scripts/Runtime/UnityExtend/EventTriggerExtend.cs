// -------------------------
// 创建日期：2022/11/2 14:22:25
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using static UnityEngine.EventSystems.EventTrigger;

namespace Framework
{
    /// <summary>
    /// 事件触发器的扩展
    /// </summary>
    public static class EventTriggerExtend
    {

        /// <summary>
        /// 注册点击事件
        /// </summary>
        /// <param name="action"></param>
        public static void AddPointerClick(this EventTrigger eventTrigger, UnityAction<BaseEventData> action)
        {
            eventTrigger.AddPointerEvent(EventTriggerType.PointerClick, action);// 添加点击后的事件回调
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventTriggerType"></param>
        /// <param name="action"></param>
        public static void AddPointerEvent(this EventTrigger eventTrigger, EventTriggerType eventTriggerType, UnityAction<BaseEventData> action)
        {
            Entry ete = eventTrigger.AddOrGetEventType(eventTriggerType);

            ete.callback.AddListener(action);// 添加点击后的事件回调
        }

        /// <summary>
        /// 获取触发实例，没有就添加
        /// </summary>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public static Entry AddOrGetEventType(this EventTrigger eventTrigger, EventTriggerType triggerType)
        {
            Entry ete = eventTrigger.GetEventType(triggerType);

            if (ete == null)// 没有就添加一个
            {
                ete = new Entry() { eventID = EventTriggerType.PointerClick };
                eventTrigger.triggers.Add(ete);
            }

            return ete;
        }

        /// <summary>
        /// 获取触发实例
        /// </summary>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public static Entry GetEventType(this EventTrigger eventTrigger, EventTriggerType triggerType)
        {
            Entry ete = null;
            ete = eventTrigger.triggers.Find((value) =>
            {
                return value.eventID == triggerType;// 查找点击事件类型
            });

            return ete;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear(this EventTrigger eventTrigger)
        {
            eventTrigger.triggers.Clear();// 添加点击后的事件回调
        }
        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear(this EventTrigger eventTrigger, EventTriggerType triggerType)
        {
            Entry ete = eventTrigger.GetEventType(triggerType);
            if(ete != null) eventTrigger.triggers.Remove(ete);
        }
        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear(this EventTrigger eventTrigger, EventTriggerType triggerType, UnityAction<BaseEventData> action)
        {
            Entry ete = eventTrigger.GetEventType(triggerType);
            if (ete != null)
            {
                ete.callback.RemoveListener(action);
            }
        }
        /// <summary>
        /// 清除
        /// </summary>
        public static void ClearEvent(this EventTrigger eventTrigger, EventTriggerType triggerType)
        {
            Entry ete = eventTrigger.GetEventType(triggerType);
            if (ete != null)
            {
                ete.callback.RemoveAllListeners();
            }
        }
    }
}