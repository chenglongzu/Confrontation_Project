using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShimmerFramework
{
    /// <summary>
    /// Josn解析管理器
    /// 里氏转换原则
    /// </summary>
    public class JsonManager : BaseManager<JsonManager>
    {
        //目前无法通过异步的方式解析并传出数据
#if Addressable
#else
        public Dictionary<T, Z> ReadJson<T, Z>(string jsonPath) where Z : ContentBase
        {
            string jsonContent = ResourcesManager.GetInstance().LoadAsset<TextAsset>(jsonPath).text;
            string newJsonContent = "{ \"contentCollection\": " + jsonContent + "}";

            //将json数据转化为ContentCollection<V>类型的数据结构
            ContentCollection<Z> contentDic = JsonUtility.FromJson<ContentCollection<Z>>(newJsonContent);

            Dictionary<T, Z> jsonInfo = new Dictionary<T, Z>();

            for (int i = 0; i < contentDic.contentCollection.Count; i++)
            {
                jsonInfo.Add((T)contentDic.contentCollection[i].id, contentDic.contentCollection[i]);
            }

            return jsonInfo;
        }

        public List<T> ReadJson<T>(string jsonPath) where T : ContentBase
        {
            string jsonContent = ResourcesManager.GetInstance().LoadAsset<Text>(jsonPath).text;
            string newJsonContent = "{ \"contentCollection\": " + jsonContent + "}";

            ContentCollection<T> contentDic = JsonUtility.FromJson<ContentCollection<T>>(newJsonContent);

            List<T> jsonInfo = new List<T>();

            for (int i = 0; i < contentDic.contentCollection.Count; i++)
            {
                jsonInfo.Add(contentDic.contentCollection[i] as T);
            }

            return jsonInfo;
        }


#endif
    }
        //存储数据类型的数据结构
    public class ContentCollection<V> where V : ContentBase
    {
        public List<V> contentCollection;
    }

    //基本的解析数据类型
    [SerializeField]
    public class ContentBase
    {
        public object id = "";
    }
}