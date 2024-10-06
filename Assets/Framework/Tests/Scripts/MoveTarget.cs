// -------------------------
// 创建日期：2024/7/17 11:37:32
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Test
{
    public class MoveTarget : MonoBehaviour, ITypePoolObject
    {
        [Tooltip("移动速度")]
        public float speed = 1;
        [Tooltip("移动方位")]
        public Vector3 moveDirection = new Vector3(0, 0, 1);
        [Tooltip("消失时间（-1 负值不消失）")]
        public float vanishTime = 5;
        public bool isVanish = false;

        float _runTime;

        public Action<Collider> onTriggerEnterEvent;
        /// <summary>
        /// 消失事件
        /// </summary>
        public Action onVanishEvent;

        // Start is called before the first frame update
        protected virtual void Start()
        {

        }

        protected virtual void OnEnable()
        {
            isVanish = false;
            _runTime = 0;
        }

        protected virtual void OnDisable()
        {
            isVanish = true;
        }

        public virtual void Reset()
        {
            isVanish = false;
            _runTime = 0;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            onTriggerEnterEvent?.Invoke(other);
        }

        protected virtual void Update()
        {
            if (isVanish) return;

            _runTime += Time.deltaTime;
            if (vanishTime > 0 && _runTime > vanishTime)
            {
                isVanish = true;
                onVanishEvent?.Invoke();
                GetComponent<PoolRecordComponent>().Return();
                return;
            }

            transform.Translate(moveDirection * speed * Time.deltaTime);
        }

        void ITypePoolObject.Clear()
        {
            onTriggerEnterEvent = null;
            onVanishEvent = null;
        }
    }
}