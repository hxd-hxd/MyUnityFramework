// -------------------------
// 创建日期：2024/7/17 14:05:47
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 碰撞器事件
    /// </summary>
    [ExecuteAlways]
    public class ColliderEvent : MonoBehaviour
    {
        //public Collider _collider;

        public UnityEvent<Collider> onTriggerEnterEvent, onTriggerExitEvent, onTriggerStayEvent;
        public UnityEvent<Collider2D> onTriggerEnter2DEvent, onTriggerExit2DEvent, onTriggerStay2DEvent;

        public UnityEvent<Collision> onCollisionEnterEvent, onCollisionExitEvent, onCollisionStayEvent;
        public UnityEvent<Collision2D> onCollisionEnter2DEvent, onCollisionExit2DEvent, onCollisionStay2DEvent;

        protected virtual void Start()
        {
            //if (!_collider)
            //{
            //    _collider = GetComponent<Collider>();
            //}
        }

        //protected virtual void 
        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnterEvent?.Invoke(other);
        }

        // 如果另一个碰撞器停止接触触发器，则调用 OnTriggerExit
        private void OnTriggerExit(Collider other)
        {
            onTriggerExitEvent?.Invoke(other);
        }

        // 对于触动触发器的所有“另一个碰撞器”，OnTriggerStay 将在每一帧被调用一次
        private void OnTriggerStay(Collider other)
        {
            onTriggerStayEvent?.Invoke(other);
        }

        // 如果其他每个碰撞器 2D 接触触发器，OnTriggerStay2D 将在每一帧被调用一次(仅限 2D 物理)
        private void OnTriggerStay2D(Collider2D other)
        {
            onTriggerStay2DEvent?.Invoke(other);
        }

        // 如果另一个碰撞器 2D 停止接触触发器，则调用 OnTriggerExit2D (仅限 2D 物理)
        private void OnTriggerExit2D(Collider2D other)
        {
            onTriggerExit2DEvent?.Invoke(other);
        }

        // 如果另一个碰撞器 2D 进入了触发器，则调用 OnTriggerEnter2D (仅限 2D 物理)
        private void OnTriggerEnter2D(Collider2D other)
        {
            onTriggerEnter2DEvent?.Invoke(other);
        }

        // 当此碰撞器/刚体开始接触另一个刚体/碰撞器时，调用 OnCollisionEnter
        private void OnCollisionEnter(Collision collision)
        {
            onCollisionEnterEvent?.Invoke(collision);
        }

        // 当此碰撞器/刚体停止接触另一刚体/碰撞器时调用 OnCollisionExit
        private void OnCollisionExit(Collision collision)
        {
            onCollisionExitEvent?.Invoke(collision);
        }

        // 每当此碰撞器/刚体接触到刚体/碰撞器时，OnCollisionStay 将在每一帧被调用一次
        private void OnCollisionStay(Collision collision)
        {
            onCollisionStayEvent?.Invoke(collision);
        }

        // 当此碰撞器 2D/刚体 2D 开始接触另一刚体 2D/碰撞器 2D 时调用 OnCollisionEnter2D (仅限 2D 物理)
        private void OnCollisionEnter2D(Collision2D collision)
        {
            onCollisionEnter2DEvent?.Invoke(collision);
        }

        // 当此碰撞器 2D/刚体 2D 停止接触另一刚体 2D/碰撞器 2D 时调用 OnCollisionExit2D (仅限 2D 物理)
        private void OnCollisionExit2D(Collision2D collision)
        {
            onCollisionExit2DEvent?.Invoke(collision);
        }

        // 每当碰撞器 2D/刚体 2D 接触到刚体 2D/碰撞器 2D 时，OnCollisionStay2D 将在每一帧被调用一次(仅限 2D 物理)
        private void OnCollisionStay2D(Collision2D collision)
        {
            onCollisionStay2DEvent?.Invoke(collision);
        }


    }
}