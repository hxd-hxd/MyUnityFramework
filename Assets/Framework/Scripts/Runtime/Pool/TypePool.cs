// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace Framework
{
    /// <summary>
    /// 类型池
    /// </summary>
    [Serializable]
    public class TypePool
    {
        /// <summary>
        /// 公共池，不管理自己的对象池时使用
        /// </summary>
        public static TypePool root { get; } = new TypePool();

        protected Dictionary<Type, List<object>> _pool;

        public TypePool()
        {
            _pool = new Dictionary<Type, List<object>>();
        }

        public TypePool(int capacity)
        {
            _pool = new Dictionary<Type, List<object>>(capacity);
        }

        public Dictionary<Type, List<object>> Pool => _pool;

        /// <summary>
        /// 池子数量，每个类型对应一个池子
        /// </summary>
        public virtual int poolCount => _pool.Count;
        /// <summary>
        /// 池子里的对象数量
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
                return _pool.Count;
            }
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual T CreateInstance<T>() => Activator.CreateInstance<T>();
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual object CreateInstance(Type type) => Activator.CreateInstance(type);
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected virtual object CreateInstance(Type type, params object[] args) => Activator.CreateInstance(type, args);

        /// <summary>从对象池获取</summary>
        public virtual T Get<T>(params object[] ctorArgs)
        {
            var target = typeof(T);
            T obj = (T)Get(target, ctorArgs);
            return obj;
        }
        /// <summary>从对象池获取</summary>
        public virtual object Get(Type type, params object[] ctorArgs)
        {
            object obj = null;

            var target = type;
            var has = _pool.TryGetValue(target, out var tPool);

            if (has && tPool.Count > 0)
            {
                //tPool.TryDequeue(out obj);
                //obj = tPool.Dequeue();
                //obj = tPool.Last();
                obj = Fetch(tPool);
            }
            else
            {
                tPool = CreatePool();
                _pool[target] = tPool;
            }

            if (ctorArgs == null || ctorArgs.Length < 1)
                obj ??= CreateInstance(type);
            else
                obj ??= CreateInstance(type, ctorArgs);

            //Debug.Log($"目标是 {target} ,\r\n有池子 {has}，\t从池子里取出的 <color=yellow>{_o}</color> ，\t最终得到的 {obj}");

            return obj;
        }
        /// <summary>获取 <see cref="List{T}"/></summary>
        public List<T> GetList<T>() => Get<List<T>>();
        /// <summary>获取 <typeparamref name="T"/>[]</summary>
        public T[] GetArray<T>(int length)
        {
            object obj = null;

            var target = typeof(T[]);
            var has = _pool.TryGetValue(target, out var tPool);

            if (has && tPool.Count > 0)
            {
                int index = 0;
                for (var i = 0; i < tPool.Count; i++)
                {
                    if ((tPool[i] as T[]).Length == length)
                    {
                        index = i;
                        break;
                    }
                }
                obj = Fetch(tPool, index);
            }
            else
            {
                tPool = CreatePool();
                _pool[target] = tPool;
            }

            if (obj == null)
            {
                obj = Array.CreateInstance(typeof(T), length);
            }

            return obj as T[];
        }
        /// <summary>获取 <see cref="Dictionary{TKey, TValue}"/></summary>
        public Dictionary<TKey, TValue> GetDic<TKey, TValue>() => Get<Dictionary<TKey, TValue>>();

        /// <summary>返回对象池</summary>
        public virtual void Return<T>(T obj) where T : class
        {
            if (obj == null) return;

            /* 
            这里避坑，
            要用实例 obj.GetType() 来获取类型，
            而不能用 typeof(T) 来获取类型。

            原因是：如果池中同时存有父子类，
                而 obj 传入的是子类，
                 T 却很有可能是父类，我这里遇到的情况是 T 必然是父类型，
                这时候获取到的类型就不一样，会把子类误存到父类的池子里，
                这样在下次取得时候，从父类池子里取到了一个子类对象，这必然是错误的。
            */
            //var target = typeof(T);
            var target = obj.GetType();
            var has = _pool.TryGetValue(target, out var tPool);

            if (!has)
            {
                tPool = CreatePool();
                _pool[target] = tPool;
            }
            //tPool.Enqueue(obj);
            tPool.Add(obj);

            CleanupObject(obj);
        }

        // 以下基本容器类型
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(List<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<List<T>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(ArrayList v)
        {
            if (v == null) return;
            v.Clear();
            Return<ArrayList>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<TKey, TValue>(Dictionary<TKey, TValue> v)
        {
            if (v == null) return;
            v.Clear();
            Return<Dictionary<TKey, TValue>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(StringBuilder v)
        {
            if (v == null) return;
            v.Clear();
            Return<StringBuilder>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会将元素置为默认值</remarks>
        public void Return<T>(T[] v)
        {
            if (v == null) return;
            for (var i = 0; i < v.Length; i++) v[i] = default;
            Return<T[]>(v);
        }
        /// <summary>返回对象池，建议使用 <see cref="Return{T}(T[])"/></summary>
        /// <remarks>会将元素置为默认值</remarks>
        public void Return(Array v)
        {
            if (v == null) return;
            var t = v.GetType();
            var et = t.GetElementType();
            bool isValueType = et.IsValueType;
            for (var i = 0; i < v.Length; i++)
            {
                if (isValueType)
                {
                    v.SetValue(CreateInstance(et), i);
                }
                else
                {
                    v.SetValue(default, i);
                }
            }
            Return<Array>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(Queue<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<Queue<T>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(Queue v)
        {
            if (v == null) return;
            v.Clear();
            Return<Queue>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(Stack<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<Stack<T>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(Stack v)
        {
            if (v == null) return;
            v.Clear();
            Return<Stack>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(HashSet<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<HashSet<T>>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(Hashtable v)
        {
            if (v == null) return;
            v.Clear();
            Return<Hashtable>(v);
        }

        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(IList v)
        {
            if (v == null) return;
            v.Clear();
            Return<IList>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return(IDictionary v)
        {
            if (v == null) return;
            v.Clear();
            Return<IDictionary>(v);
        }
        /// <summary>返回对象池</summary>
        /// <remarks>会清空</remarks>
        public void Return<T>(ICollection<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<ICollection<T>>(v);
        }


        /// <summary>取出最后一个元素，避免中间的元素挪动影响性能，取出的元素会被移除</summary>
        protected object Fetch(List<object> tPool)
        {
            if (tPool.Count <= 0) return null;
            return Fetch(tPool, tPool.Count - 1);
        }
        /// <summary>取出指定索引处元素，取出的元素会被移除</summary>
        protected object Fetch(List<object> tPool, int index)
        {
            if (tPool.Count <= 0) return null;
            var _obj = tPool[index];
            tPool.RemoveAt(index);
            return _obj;
        }
        protected List<object> CreatePool()
        {
            return new List<object>();
        }

        /// <summary>
        /// 清理 <see cref="ITypePoolObject.Clear()"/>
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void CleanupObject(object obj)
        {
            if (obj is ITypePoolObject tpo) tpo.Clear();
        }

        /// <summary>清除对象池</summary>
        public virtual void Clear()
        {
            _pool.Clear();
        }
    }

}