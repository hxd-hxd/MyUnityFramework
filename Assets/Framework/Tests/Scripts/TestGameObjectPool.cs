// -------------------------
// 创建日期：2024/7/17 11:34:24
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Test
{
    public class TestGameObjectPool : MonoBehaviour
    {
        public List<GameObject> objects;

        public GameObjectPool pool = new GameObjectPool();

        // Start is called before the first frame update
        void Start()
        {
        
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var go = pool.Get(objects[0]);
                var c = go.GetComponent<MoveTarget>();
                go.transform.position = transform.position;
                go.transform.rotation = transform.rotation;
                go.SetActive(true);
                if (c)
                {
                    c.onVanishEvent = () =>
                    {
                        pool.Return(go, objects[0]);
                    };
                }
            }
        }
    }
}