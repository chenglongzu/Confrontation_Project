using UnityEngine;

namespace ShimmerFramework
{
    public class UiInitManager : MonoBehaviour
    {
        void Start()
        {
#if Addressable
            ResourcesManager.GetInstance().LoadAssetAsync<GameObject>("Ui/Master/MainUiCanvas");
            ResourcesManager.GetInstance().LoadAssetAsync<GameObject>("Ui/Master/EventSystem",(obj)=> { 
                obj.AddComponent<DontDestoryOnLoad>();
            });

#else
            ResourcesManager.GetInstance().LoadAsset<GameObject>("Ui/Master/MainUiCanvas");
            ResourcesManager.GetInstance().LoadAsset<GameObject>("Ui/Master/EventSystem").AddComponent<DontDestoryOnLoad>();

#endif
        }
    }
}