using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TestInterface : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var testClass1Child1 = new TestClass1Child1();
            if (testClass1Child1 is ITestInterface iti1)
            {
                iti1.TestMethod();
            }

            var testClass1Child2 = new TestClass1Child2();
            if (testClass1Child2 is ITestInterface iti2)
            {
                iti2.TestMethod();
            }
        }

        public interface ITestInterface
        {
            void TestMethod();
        }

        public class TestClass1 : ITestInterface
        {
            void ITestInterface.TestMethod()
            {
                Debug.Log($"{nameof(TestClass1)} 显示实现");
            }
        }

        public class TestClass1Child1 : TestClass1
        {
            // 会继承父类的显示实现
        }

        public class TestClass1Child2 : TestClass1, ITestInterface
        {
            // 会覆盖父类的显示实现
            void ITestInterface.TestMethod()
            {
                Debug.Log($"{nameof(TestClass1Child2)} 显示实现");
            }
        }
    }
}