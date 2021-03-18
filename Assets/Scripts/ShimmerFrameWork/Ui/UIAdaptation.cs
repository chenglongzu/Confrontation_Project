using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ShimmerFramework
{
    /// <summary>
    /// 挂载在canvas组件上 获取canvasSclaer组件
    /// </summary>
    public class UIAdaptation : MonoBehaviour
    {
        void Start()
        {
            //通过获取组件来判断当前屏幕的分辨率
            CanvasScaler canvasScaler = transform.GetComponent<CanvasScaler>();
            if ((float)Screen.width / (float)Screen.height > 1920f / 1080f)
            {
                //根据屏幕的高度来缩放画布
                canvasScaler.matchWidthOrHeight = 1;
            }
            else if ((float)Screen.width / (float)Screen.height < 1920f / 1080f)
            {
                //根据屏幕的宽度来缩放画布
                canvasScaler.matchWidthOrHeight = 0;
            }

            StartCoroutine(PosAdaptation());
        }

        /// <summary>
        ///协同程序 将UI物体传入修改每一个UI物体的位置
        /// </summary>
        /// <param name="UIWindow"></param>
        /// <returns></returns>
        IEnumerator PosAdaptation()
        {
            yield return null;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Transform Child = gameObject.transform.GetChild(i);
                Child.localPosition = new Vector2(Child.localPosition.x * (transform.GetComponent<RectTransform>().rect.width / 1920f), Child.localPosition.y * (transform.GetComponent<RectTransform>().rect.height / 1080f));
            }

        }

    }
}