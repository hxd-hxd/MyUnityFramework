// -------------------------
// 创建日期：2023/4/3 11:27:20
// -------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Extend
{
    public static class ListExtend
    {
        /// <summary>
        /// 检查索引是否有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IndexValid<T>(this List<T> list, int index)
        {
            return list != null && list.Count > 0 && index >= 0 && index < list.Count;
        }

        /// <summary>
        /// 移动元素到指定的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="newIndex"></param>
        /// <param name="item"></param>
        public static void Move<T>(this List<T> list, int newIndex, T item)
        {
            if (!IndexValid(list, newIndex) || newIndex == list.IndexOf(item)) return;
            list.Remove(item);
            list.Insert(newIndex, item);
        }

        /// <summary>
        /// 移动指定位置的元素到新位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="newIndex"></param>
        /// <param name="oldIndex"></param>
        public static void MoveIndex<T>(this List<T> list, int newIndex, int oldIndex)
        {
            if (newIndex == oldIndex) return;
            if (!IndexValid(list, oldIndex) || !IndexValid(list, newIndex)) return;

            var item = list[oldIndex];
            Move(list, newIndex, oldIndex, item);
        }
        static void Move<T>(this List<T> list, int newIndex, int oldIndex, T item)
        {
            if (newIndex == oldIndex) return;
            if (!IndexValid(list, newIndex) || !IndexValid(list, newIndex)) return;

            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }


        /// <summary>
        /// 尝试获取对应索引的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryIndex<T>(this List<T> list, int index, out T value)
        {
            value = default;
            if (!IndexValid(list, index))
            {
                return false;
            }

            value = list[index];

            return true;
        }

        /// <summary>
        /// 尝试获取对应索引的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns>对应索引的元素，非法索引则返回元素类型的默认值</returns>
        public static T TryIndex<T>(this List<T> list, int index)
        {
            T value = default;
            //if (list == null || list.Count <= 0 || index < 0 || list.Count <= index)
            if (!IndexValid(list, index))
            {
                return value;
            }

            value = list[index];

            return value;
        }

        /// <summary>
        /// 尝试移除元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool TryRemove<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                return false;
            }

            return list.Remove(item);
        }
        /// <summary>
        /// 尝试移除对应索引的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TryRemoveAt<T>(this List<T> list, int index)
        {
            //if (index >= 0 && index < list.Count)
            if (IndexValid(list, index))
            {
                list.RemoveAt(index);
                return true;
            }
            return false;
        }
    }
}