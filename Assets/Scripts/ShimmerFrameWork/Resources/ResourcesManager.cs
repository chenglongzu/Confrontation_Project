using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if Addressable
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace ShimmerFramework
{
    public class ResourcesManager : BaseManager<ResourcesManager>
    {
#if Addressable
        #region Addressable 异步加载资源
        /// <summary>
        /// Adressable 加载管理资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        public void LoadAssetAsync<T>(string assetName)
        {
            AsyncOperationHandle<T> operateHandler = Addressables.LoadAssetAsync<T>(assetName);

            operateHandler.Completed += (obj) =>
            {            
                if (obj.Result != null)
                {
                    if (obj.Result is GameObject)
                    {
                        GameObject.Instantiate<GameObject>(obj.Result as GameObject);
                        return;
                    }
                }
                else
                {
                    Debug.LogError("Null reference exception");
                }

            };
    }

    public void LoadAssetAsync(string assetName, UnityAction<object> callBack)
    {
        AsyncOperationHandle<object> operateHandler = Addressables.LoadAssetAsync<object>(assetName);

        operateHandler.Completed += (obj) =>
        {
            if (obj.Result != null)
            {
                if (obj.Result is GameObject)
                {
                    callBack(GameObject.Instantiate<GameObject>(obj.Result as GameObject));
                    return;
                }

                callBack(obj.Result);
            }
            else
            {
                Debug.LogError("Null reference exception");
            }
        };
    }

    public void LoadSpriteAssetAsync(string assetName, UnityAction<Sprite> callBack)
    {
        AsyncOperationHandle<Sprite> operateHandler = Addressables.LoadAssetAsync<Sprite>(assetName);

        operateHandler.Completed += (obj) =>
        {
            if (obj.Result != null)
            {
                callBack(obj.Result);
            }
            else
            {
                Debug.LogError("Null reference exception");
            }

        };
    }

    public void LoadAssetAsync<T>(string assetName, UnityAction<T>callBack)where T:Object
    {
        AsyncOperationHandle<T> operateHandler = Addressables.LoadAssetAsync<T>(assetName);

        operateHandler.Completed += (obj) =>
        {
            if (obj.Result != null)
            {
                if (obj.Result is GameObject)
                {
                    callBack(GameObject.Instantiate<GameObject>(obj.Result as GameObject) as T);
                    return;
                }

                callBack(obj.Result);
            }
            else
            {
                Debug.LogError("Null reference exception");
            }
        };
    }

    public void LoadAssetAsync<T>(string assetName, Vector3 postion, UnityAction<T> callBack) where T : Object
    {
        AsyncOperationHandle<T> operateHandler = Addressables.LoadAssetAsync<T>(assetName);

        operateHandler.Completed += (obj) =>
        {
            if (obj.Result != null)
            {
                if (obj.Result is GameObject)
                {
                    callBack(GameObject.Instantiate<GameObject>(obj.Result as GameObject, postion, Quaternion.identity) as T);
                    return;
                }

                callBack(obj.Result);
            }
            else
            {
                Debug.LogError("Null reference exception");
            }
        };
    }

    public void LoadAssetAsync<T>(string assetName, Transform parent, UnityAction<T> callBack) where T : Object
    {
        AsyncOperationHandle<T> operateHandler = Addressables.LoadAssetAsync<T>(assetName);

        operateHandler.Completed += (obj) =>
        {
            if (obj.Result!=null)
            {
                if (obj.Result is GameObject)
                {
                    if ((obj.Result == null))
                    {
                        Debug.Log(assetName);
                    }

                    callBack(GameObject.Instantiate<GameObject>(obj.Result as GameObject, parent) as T);
                    return;
                }

                callBack(obj.Result);

            }else
            {
                Debug.LogError("Null reference exception");
            }
        };
    }

    public void LoadAssetAsync<T>(string assetName, Vector3 postion, Vector3 scale, UnityAction<T> callBack) where T : Object
    {
        AsyncOperationHandle<T> operateHandler = Addressables.LoadAssetAsync<T>(assetName);

        operateHandler.Completed += (obj) =>
        {
            if (obj.Result != null)
            {
                if (obj.Result is GameObject)
                {
                    callBack((GameObject.Instantiate<GameObject>(obj.Result as GameObject, postion, Quaternion.identity).transform.localScale = scale) as T);
                    return;
                }

                callBack(obj.Result);
            }
            else
            {
                Debug.LogError("Null reference exception");
            }
        };
    }

    public void LoadAssetAsync<T>(string assetName, Vector3 postion,Quaternion rotation, UnityAction<T> callBack) where T : Object
    {
        AsyncOperationHandle<T> operateHandler = Addressables.LoadAssetAsync<T>(assetName);

        operateHandler.Completed += (obj) =>
        {
            if (obj.Result != null)
            {
                if (obj.Result is GameObject)
                {
                    callBack(GameObject.Instantiate<GameObject>(obj.Result as GameObject, postion, rotation) as T);
                    return;
                }

                callBack(obj.Result);
            }
            else
            {
                Debug.LogError("Null reference exception");
            }
        };
    }

    public void LoadAssetAsync<T>(string assetName,Vector3 postion,Quaternion rotation,Transform parent, UnityAction<T> callBack) where T : Object
    {
        AsyncOperationHandle<T> operateHandler = Addressables.LoadAssetAsync<T>(assetName);

        operateHandler.Completed += (obj) =>
        {
            if (obj.Result != null)
            {
                if (obj.Result is GameObject)
                {
                    callBack(GameObject.Instantiate<GameObject>(obj.Result as GameObject, postion, rotation, parent) as T);
                    return;
                }

                callBack(obj.Result);
            }
            else
            {
                Debug.LogError("Null reference exception");
            }

        };
    }
#endregion

#else

#region Resources 同步加载资源
        public T LoadAsset<T>(string AssetPath) where T : Object
        {
            T ass = null;
            ass = Resources.Load<T>(AssetPath) as T;
            if (ass is GameObject)
            {
                return GameObject.Instantiate<T>(ass);
            }
            return ass;
        }

        public T LoadAsset<T>(string AssetPath, Vector3 position) where T : Object
        {
            T ass = null;
            ass = Resources.Load<T>(AssetPath) as T;
            if (ass is GameObject)
            {
                GameObject obj = GameObject.Instantiate<T>(ass) as GameObject;
                obj.transform.position = position;

                return obj as T;
            }
            return ass;
        }

        public T LoadAsset<T>(string AssetPath, Transform parent) where T : Object
        {
            T ass = null;
            ass = Resources.Load<T>(AssetPath) as T;
            if (ass is GameObject)
            {
                GameObject obj = GameObject.Instantiate<T>(ass, parent) as GameObject;

                return obj as T;
            }
            return ass;
        }

        public T LoadAsset<T>(string AssetPath, Transform parent, Vector3 position) where T : Object
        {
            T ass = null;
            ass = Resources.Load<T>(AssetPath) as T;
            if (ass is GameObject)
            {
                GameObject obj = GameObject.Instantiate<T>(ass) as GameObject;
                obj.transform.parent = parent;
                obj.transform.position = position;

                return obj as T;
            }
            return ass;
        }

        public T LoadAsset<T>(string AssetPath, Vector3 position, Quaternion quaternion, Transform parent = null) where T : Object
        {
            T ass = null;
            ass = Resources.Load<T>(AssetPath) as T;
            if (ass is GameObject)
            {
                GameObject obj = GameObject.Instantiate<T>(ass) as GameObject;
                obj.transform.parent = parent;
                obj.transform.position = position;
                obj.transform.rotation = quaternion;
                return obj as T;
            }
            return ass;
        }

#endregion

#region Resources异步加载资源
        public void LoadAssetAsync<T>(string AssetName, UnityAction<T> callback = null) where T : Object
        {
            MonoManager.GetInstance().StartCoroutine(ReallyLoadAsync1(AssetName, callback));
        }

        private IEnumerator ReallyLoadAsync1<T>(string AssetName, UnityAction<T> callback) where T : Object
        {
            ResourceRequest Asset = Resources.LoadAsync<T>(AssetName);
            while (!Asset.isDone)
            {
                yield return Asset.progress;
            }

            if (Asset.isDone)
            {
                if (callback != null)
                {
                    if (Asset.asset is GameObject)
                    {
                        callback(GameObject.Instantiate(Asset.asset) as T);
                    }
                    else
                    {
                        callback(Asset.asset as T);
                    }

                }
                else
                {
                    GameObject.Instantiate(Asset.asset);
                }

            }

            yield return Asset;

        }
#endregion

#endif
    }
}
