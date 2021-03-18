using UnityEngine;

namespace ShimmerFramework
{
    /// <summary>
    /// 脚本继承后由外部调用GetInstance方法自动创建物体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T GetInstance()
        {
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();

                DontDestroyOnLoad(obj);
                instance = obj.AddComponent<T>();
            }
            return instance;
        }

    }
}
