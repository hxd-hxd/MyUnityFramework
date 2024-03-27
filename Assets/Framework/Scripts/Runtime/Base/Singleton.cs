using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class Singleton<T> where T : new()
    {
        static T inst;
        public static T Instance
        {
            get
            {
                if (inst == null)
                {
                    inst = new T();
                }
                return inst;
            }
            protected set { inst = value; }
        }

    }
}