using ShimmerFramework;
using UnityEngine;

public class ModelInitManager : MonoBehaviour
{
    void Start()
    {
        //初始化数据管理器
        GameModelManager.GetInstance().Init();

    }
}
