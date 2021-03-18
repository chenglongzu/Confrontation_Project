using UnityEngine;

namespace ShimmerFramework
{
    /// <summary>
    /// 单例模式 继承类便于获取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T GetInstance()
        {
            return instance;
        }

        protected virtual void Awake()
        {
            instance = this as T;
        }

    }
}
