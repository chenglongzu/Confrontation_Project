using UnityEngine;

namespace ShimmerFramework
{
    public class DontDestoryOnLoad : MonoBehaviour
    {
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}