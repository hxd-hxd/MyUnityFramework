// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Framework
{
    /// <summary>
    /// <see cref="GameObject"/> 池
    /// </summary>
    [Serializable]
    public class GameObjectPool
    {
        static GameObjectPool _root;
        static GameObject _GOManager;
        /// <summary>
        /// 公共池，不管理自己的对象池时使用
        /// </summary>
        //public static GameObjectPool root { get; } = new GameObjectPool();
        public static GameObjectPool root
        {
            get
            {
                if (_root == null) _root = new GameObjectPool();
                return _root;
            }
        }
        protected static GameObject GOManager
        {
            get
            {
                InitStatic();
                return _GOManager;
            }
            set { _GOManager = value; }
        }

        protected static void InitStatic()
        {
            if (_GOManager == null)
            {
                try
                {
                    if (Application.isPlaying)
                    {
                        _GOManager = new GameObject("<GameObjectPool>");
                        _GOManager.SetActive(false);
                        GameObject.DontDestroyOnLoad(_GOManager);
                    }
                }
                catch (Exception)
                {
                }
            }

        }

        [SerializeField]
        GameObject _template;
        Dictionary<GameObject, Queue<GameObject>> _pool;
        List<Coroutine> _preCreateInstanceCoroutines = new List<Coroutine>();
        ///// <summary>
        ///// 实例化事件
        ///// </summary>
        //public event Func<GameObject> CreateInstanceEvent;

        public GameObjectPool()
        {
            _pool = new Dictionary<GameObject, Queue<GameObject>>(1);
        }
        public GameObjectPool(int capacity)
        {
            _pool = new Dictionary<GameObject, Queue<GameObject>>(capacity);
        }
        public GameObjectPool(GameObject template)
        {
            _pool = new Dictionary<GameObject, Queue<GameObject>>(1);
            this._template = template;
        }
        public GameObjectPool(int capacity, GameObject template)
        {
            _pool = new Dictionary<GameObject, Queue<GameObject>>(capacity);
            this._template = template;
        }

        /// <summary>
        /// 模板池
        /// </summary>
        public Dictionary<GameObject, Queue<GameObject>> Pool => _pool;
        /// <summary>
        /// 默认模板
        /// </summary>
        public GameObject Template { get => _template; set => _template = value; }

        /// <summary>
        /// 池子的数量
        /// </summary>
        public virtual int poolCount => _pool.Count;
        /// <summary>
        /// 池子里包含的对象数量
        /// </summary>
        public virtual int itemSize
        {
            get
            {
                int sum = 0;
                foreach (var item in _pool)
                {
                    sum += item.Value.Count;
                }
                return sum;
            }
        }
        /// <summary>
        /// 预创建的异步协程列表
        /// </summary>
        public List<Coroutine> preCreateInstanceCoroutines { get => _preCreateInstanceCoroutines; set => _preCreateInstanceCoroutines = value; }
        /// <summary>
        /// 预创建实例的协程数量
        /// </summary>
        public int preCreateInstanceCoroutineNum => _preCreateInstanceCoroutines.Count;

        //protected void Init(string name)
        //{
        //    var goRoot = new GameObject($"<>");
        //}

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual GameObject CreateInstance(GameObject value)
        {
            if (value == null)
            {
                Debug.LogError("[GameObjectPool]：要实例化的目标模板是空对象");
                return null;
            }

            //GameObject resault = CreateInstanceEvent?.Invoke();
            //if(!resault) resault = GameObject.Instantiate(value);
            //return resault;
            return GameObject.Instantiate(value);
        }

        /// <summary>
        /// 预先为默认模板创建指定数量的实例
        /// </summary>
        public virtual void PreCreateInstance(int num) => PreCreateInstance(_template, num);
        /// <summary>
        /// 预先为对应模板创建指定数量的实例
        /// </summary>
        /// <param name="template"></param>
        /// <param name="num"></param>
        public virtual void PreCreateInstance(GameObject template, int num)
        {
            for (int i = 1; i <= num; i++)
            {
                var go = CreateInstance(template);
                Return(go, template);
            }
        }
        /// <summary>
        /// 预先为对应模板创建指定数量的实例，该操作是异步的
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public virtual Coroutine PreCreateInstanceAsync(int num) => PreCreateInstanceAsync(_template, num);
        /// <summary>
        /// 预先为对应模板创建指定数量的实例，该操作是异步的
        /// </summary>
        /// <param name="template"></param>
        /// <param name="num"></param>
        public virtual Coroutine PreCreateInstanceAsync(GameObject template, int num)
        {
            var c = Coroutines.Instance.BeginCoroutine(_PreCreateInstanceCoroutine(template, num));
            _preCreateInstanceCoroutines.Add(c);
            return c;
        }
        protected IEnumerator _PreCreateInstanceCoroutine(GameObject template, int num)
        {
            for (int i = 1; i <= num; i++)
            {
                var go = CreateInstance(template);
                Return(go, template);
                yield return null;
            }
            yield break;
        }
        /// <summary>
        /// 取消所有预创建
        /// </summary>
        public virtual void CancelPreCreateInstance()
        {
            foreach (var item in _preCreateInstanceCoroutines)
            {
                if (item != null)
                    Coroutines.Instance.StopCoroutine(item);
            }
            _preCreateInstanceCoroutines.Clear();
        }

        /// <summary>从对象池获取，从默认模板 <see cref="Template"/> 对应的池子里取</summary>
        public virtual GameObject Get()
        {
            return Get(_template);
        }
        /// <summary>从对象池获取</summary>
        public virtual GameObject Get(GameObject template)
        {
            if (template == null) return null;

            GameObject obj = null;

            var pool = this._pool;
            var target = template;
            bool has = pool.TryGetValue(target, out var tPool);

            if (has && tPool.Count > 0)
            {
                //tPool.TryDequeue(out obj);
                obj = tPool.Dequeue();
            }
            else
            {
                tPool = GetNewPoolQueue();
                pool[target] = tPool;
            }
            if (!obj) obj = CreateInstance(target);

            return obj;
        }

        /// <summary>返回对象池，默认返回到 <see cref="Template"/> 对应的池子，如果不确定请使用 <see cref="Return(GameObject, GameObject)"/> 已指定返回到哪个池子</summary>
        /// <remarks>和 <see cref="TypePool.Return{T}(T)"/> 一样，会执行 <see cref="ITypePoolObject.Clear"/> 的清理操作，清理操作会在其他操作之后进行。</remarks>
        public virtual void Return(GameObject obj) => Return(obj, _template);
        /// <summary>返回对象池</summary>
        /// <remarks>和 <see cref="TypePool.Return{T}(T)"/> 一样，会执行 <see cref="ITypePoolObject.Clear"/> 的清理操作，清理操作会在其他操作之后进行。</remarks>
        public virtual void Return(GameObject obj, GameObject template)
        {
            if (obj == null) return;

            var target = template;
            bool has = _pool.TryGetValue(target, out var tPool);

            if (!has)
            {
                tPool = GetNewPoolQueue();
                _pool[target] = tPool;
            }
            tPool.Enqueue(obj);
            obj.transform.SetParent(GOManager.transform);

            // 清除操作
            var tpos = obj.GetComponents<ITypePoolObject>();
            if (tpos != null)
            {
                foreach (var tpo in tpos)
                {
                    tpo.Clear();
                }
            }
        }

        public virtual void Clear(GameObject template)
        {
            var target = template;
            bool has = _pool.TryGetValue(target, out var tPool);

            if (has)
            {
                //tPool = new Queue<GameObject>();
                while (tPool.Count > 0)
                {
                    var _go = tPool.Dequeue();
                    GameObject.Destroy(_go);
                }
            }
        }
        public virtual void Clear()
        {
            foreach (var t in _pool)
            {
                foreach (var _go in t.Value)
                {
                    if (_go)
                        GameObject.Destroy(_go);
                }
            }
            _pool.Clear();
        }

        protected static Queue<GameObject> GetNewPoolQueue()
        {
            return new Queue<GameObject>(1);
        }
    }
}