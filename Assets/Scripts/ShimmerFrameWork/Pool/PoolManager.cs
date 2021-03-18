using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ShimmerFramework
{
    //返还物体的如果循环使用方法切记放enable方法中
    /// <summary>
    /// 对象池
    /// </summary>
    public class PoolManager : BaseManager<PoolManager>
    {

        GameObject ObjePool = null;
        GameObject ObjectController = null;

        private Dictionary<string, Queue<GameObject>> ObjectPool = new Dictionary<string, Queue<GameObject>>();


        #region 从对象池中取出物品

#if Addressable

        public void GetObj(string objName, UnityAction<GameObject> callback)
        {
            if (ObjectController == null)
            {
                ObjectController = new GameObject("ObjectController");
            }

            if (ObjectPool.ContainsKey(objName) && ObjectPool[objName].Count > 0)
            {
                GameObject temp = ObjectPool[objName].Dequeue();
                temp.SetActive(true);

                callback(temp);
            }
            else
            {
                
                ResourcesManager.GetInstance().LoadAssetAsync<GameObject>(objName,(obj)=> {

                    obj.transform.parent = ObjectController.transform;

                    callback(obj);
                });

            }
        }

        public void GetObj(string objName, Vector3 position, UnityAction<GameObject> callback)
        {
            if (ObjectController == null)
            {
                ObjectController = new GameObject("ObjectController");
            }

            if (ObjectPool.ContainsKey(objName) && ObjectPool[objName].Count > 0)
            {

                GameObject temp = ObjectPool[objName].Dequeue();
                temp.transform.parent = GameObject.Find("ObjectController").transform;
                temp.transform.position = position;
                temp.SetActive(true);

                callback(temp);
            }
            else
            {
                ResourcesManager.GetInstance().LoadAssetAsync<GameObject>(objName, position,(temp)=> {
                    temp.transform.parent = ObjectController.transform;
                    callback(temp);
                });

            }
        }

        public void GetObj(string objName,Vector3 position, Quaternion quaternion,UnityAction<GameObject> callback)
        {
            if (ObjectController == null)
            {
                ObjectController = new GameObject("ObjectController");
            }

            if (ObjectPool.ContainsKey(objName) && ObjectPool[objName].Count > 0)
            {
                GameObject temp = ObjectPool[objName].Dequeue();
                if (temp != null)
                {
                    temp.transform.parent = GameObject.Find("ObjectController").transform;
                    temp.transform.position = position;
                    temp.transform.rotation = quaternion;
                    temp.SetActive(true);

                    callback(temp);
                }else
                {
                    Debug.LogError("PoolManager Warnning");
                }
            }
            else
            {
                 ResourcesManager.GetInstance().LoadAssetAsync<GameObject>(objName, position,(obj)=> {
                     GameObject temp = obj;
                     temp.transform.parent = ObjectController.transform;
                 });

            }
        }

#else

        public GameObject GetObj(string objName)
        {
            if (ObjectController == null)
            {
                ObjectController = new GameObject("ObjectController");
            }

            if (ObjectPool.ContainsKey(objName) && ObjectPool[objName].Count > 0)
            {
                GameObject temp = ObjectPool[objName].Dequeue();
                temp.SetActive(true);
                return temp;
            }
            else
            {
                GameObject temp = ResourcesManager.GetInstance().LoadAsset<GameObject>(objName);

                temp.transform.parent = ObjectController.transform;

                return temp;
            }
        }

        public GameObject GetObj(string objName, string path)
        {
            if (ObjectController == null)
            {
                ObjectController = new GameObject("ObjectController");
            }

            if (ObjectPool.ContainsKey(objName) && ObjectPool[objName].Count > 0)
            {
                GameObject temp = ObjectPool[objName].Dequeue();
                temp.SetActive(true);
                return temp;
            }
            else
            {
                GameObject temp = ResourcesManager.GetInstance().LoadAsset<GameObject>(path);

                temp.transform.parent = ObjectController.transform;

                return temp;
            }
        }

        public GameObject GetObj(string objName, string path, Vector3 position)
        {
            if (ObjectController == null)
            {
                ObjectController = new GameObject("ObjectController");
            }

            if (ObjectPool.ContainsKey(objName) && ObjectPool[objName].Count > 0)
            {

                GameObject temp = ObjectPool[objName].Dequeue();
                temp.transform.parent = GameObject.Find("ObjectController").transform;
                temp.transform.position = position;
                temp.SetActive(true);
                return temp;
            }
            else
            {
                GameObject temp = ResourcesManager.GetInstance().LoadAsset<GameObject>(path, position);

                temp.transform.parent = ObjectController.transform;
                return temp;
            }
        }

        public GameObject GetObj(string objName, string path, Vector3 position, Quaternion quaternion)
        {
            if (ObjectController == null)
            {
                ObjectController = new GameObject("ObjectController");
            }

            if (ObjectPool.ContainsKey(objName) && ObjectPool[objName].Count > 0)
            {
                GameObject temp = ObjectPool[objName].Dequeue();
                if (temp != null)
                {
                    temp.transform.parent = GameObject.Find("ObjectController").transform;
                    temp.transform.position = position;
                    temp.transform.rotation = quaternion;
                    temp.SetActive(true);
                }
                return temp;

            }
            else
            {
                GameObject temp = ResourcesManager.GetInstance().LoadAsset<GameObject>(path, position);

                temp.transform.parent = ObjectController.transform;
                return temp;
            }
        }
#endif

        #endregion
        #region 往对象池中返还物品 切记返还的方法放在onenable方法中
        public void ReturnObj(string objName, GameObject obj)
        {
            if (ObjePool == null)
            {
                ObjePool = new GameObject("ObjePool");
            }

            obj.transform.parent = ObjePool.transform;


            obj.SetActive(false);

            if (ObjectPool.ContainsKey(objName))
            {
                ObjectPool[objName].Enqueue(obj);
            }
            else
            {
                Queue<GameObject> objTemp = new Queue<GameObject>();
                objTemp.Enqueue(obj);
                ObjectPool.Add(objName, objTemp);
            }
        }
        public void ReturnObj(string objName, GameObject obj, float delayTime, CanBeReturn canBeReturn)
        {
            MonoManager.GetInstance().StartCoroutine(DelayTime(objName, obj, delayTime, canBeReturn));
        }

        public void ReturnObj(string objName, GameObject obj, float delayTime)
        {
            MonoManager.GetInstance().StartCoroutine(DelayTime(objName, obj, delayTime));
        }

        private IEnumerator DelayTime(string objName, GameObject obj, float time)
        {
            yield return new WaitForSeconds(time);

            if (ObjePool == null)
            {
                ObjePool = new GameObject("ObjePool");
            }
            if (obj != null)
            {
                obj.transform.parent = ObjePool.transform;

                obj.SetActive(false);
            }


            if (ObjectPool.ContainsKey(objName))
            {
                ObjectPool[objName].Enqueue(obj);
            }
            else
            {
                Queue<GameObject> objTemp = new Queue<GameObject>();
                objTemp.Enqueue(obj);
                ObjectPool.Add(objName, objTemp);
            }
        }
        private IEnumerator DelayTime(string objName, GameObject obj, float time, CanBeReturn canBeReturn)
        {
            yield return new WaitForSeconds(time);

            if (canBeReturn.canBeReturn)
            {

                if (ObjePool == null)
                {
                    ObjePool = new GameObject("ObjePool");
                }
                if (obj != null)
                {
                    obj.transform.parent = ObjePool.transform;

                    obj.SetActive(false);
                }


                if (ObjectPool.ContainsKey(objName))
                {
                    ObjectPool[objName].Enqueue(obj);

                }
                else
                {
                    Queue<GameObject> objTemp = new Queue<GameObject>();
                    objTemp.Enqueue(obj);
                    ObjectPool.Add(objName, objTemp);

                }
            };
        }
        #endregion


        public void ClearPool()
        {
            ObjectPool.Clear();
        }
    }

    public class CanBeReturn
    {
        public bool canBeReturn;

        public CanBeReturn(bool canBeReturn)
        {
            this.canBeReturn = canBeReturn;
        }
    }
}