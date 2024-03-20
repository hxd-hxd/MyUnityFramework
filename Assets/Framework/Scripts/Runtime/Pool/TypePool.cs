// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Framework
{
    /// <summary>
    /// 类型池
    /// </summary>
    public class TypePool
    {
        /// <summary>
        /// 公共池，不管理自己的对象池时使用
        /// </summary>
        public static TypePool root { get; } = new TypePool();

        Dictionary<Type, Queue<object>> pool;

        public TypePool()
        {
            pool = new Dictionary<Type, Queue<object>>();
        }

        public TypePool(int capacity)
        {
            pool = new Dictionary<Type, Queue<object>>(capacity);
        }

        public Dictionary<Type, Queue<object>> Pool => pool;

        public virtual int size => pool.Count;
        public virtual int sizeTotal
        {
            get
            {
                int sum = 0;
                foreach (var item in pool)
                {
                    sum += item.Value.Count;
                }
                return pool.Count;
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

        /// <summary>从对象池获取</summary>
        public virtual T Get<T>() where T : class
        {
            var target = typeof(T);
            T obj = (T)Get(target);
            return obj;
        }
        /// <summary>从对象池获取</summary>
        public virtual object Get(Type type)
        {
            object obj = null;

            var target = type;
            var has = pool.TryGetValue(target, out var tPool);

            if (has)
            {
                //tPool.TryDequeue(out var _obj);
                var _obj = tPool.Dequeue();
                obj = _obj;
            }
            else
            {
                tPool = new Queue<object>();
                pool[target] = tPool;
            }
            //var _o = obj;
            obj ??= CreateInstance(type);

            //Debug.Log($"目标是 {target} ,\r\n有池子 {has}，\t从池子里取出的 <color=yellow>{_o}</color> ，\t最终得到的 {obj}");

            return obj;
        }
        /// <summary>获取 <see cref="List{T}"/></summary>
        public List<T> GetList<T>() => Get<List<T>>();
        /// <summary>获取 <see cref="Dictionary{TKey, TValue}"/></summary>
        public Dictionary<TKey, TValue> GetDic<TKey, TValue>() => Get<Dictionary<TKey, TValue>>();

        /// <summary>返回对象池</summary>
        public virtual void Return<T>(T obj) where T : class
        {
            if (obj == null) return;

            if(obj is ITypePoolObject tpo) tpo.Clear();

            /* 
            这里避坑，
            要用实例 obj.GetType() 来获取类型，
            而不能用 typeof(T) 来获取类型。

            原因是：如果池中同时存有父子类，
                而 obj 传入的是子类，
                 T 却很有可能是父类，我这里遇到的情况是 T 必然是父类型，
                这时候获取到的类型就不一样，会把子类误存到父类的池子里，
                这样在下次取得时候，从父类池子里取到了一个子类对象，这必然是错误的。

            坑死人不偿命！！！
            */
            //var target = typeof(T);
            var target = obj.GetType();
            var has = pool.TryGetValue(target, out var tPool);

            if (!has)
            {
                tPool = new Queue<object>();
                pool[target] = tPool;
            }
            tPool.Enqueue(obj);
        }
        /// <summary>返回对象池</summary>
        public void Return<T>(List<T> v)
        {
            if (v == null) return;
            v.Clear();
            Return<List<T>>(v);
        }
        /// <summary>返回对象池</summary>
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

        public virtual void Clear()
        {
            pool.Clear();
        }
    }

}