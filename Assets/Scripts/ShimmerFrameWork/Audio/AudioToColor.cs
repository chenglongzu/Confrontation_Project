using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ShimmerFramework
{
    /// <summary>
    /// 声音数据可视化 挂载在数据条物体上
    /// </summary>
    public class AudioToColor : MonoBehaviour
    {
        public AudioSource audioSource;
        public float minSize = 10;
        public float maxSize = 200;
        public float lerpSpeed = 10.0f; // 用于控制变化速度
        private List<RectTransform> barList = new List<RectTransform>();

        // 注意，采样数组的大小必须为2的次方
        // 另外，采样数据的个数，一定要 >= 条形UI的数量
        private float[] sampleData = new float[64];


        private void Start()
        {
            // 查找所有的子物体，就是我们要用来显示音频数据的条形UI
            RectTransform[] childs = GetComponentsInChildren<RectTransform>();
            for (int i = 1; i < childs.Length; ++i)
            {
                barList.Add(childs[i]);
            }
        }

        /// <summary>
        /// 进行标准化数据，将数据映射为 0 到 1 之间的数值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private float[] NormalizeData(float[] input)
        {
            float[] output = new float[input.Length];
            float max = 0;
            float min = 0;
            for (int i = 0; i < input.Length; i++)
            {
                max = Mathf.Max(max, input[i]);
                min = Mathf.Min(min, input[i]);
            }

            float len = max - min;

            for (int i = 0; i < input.Length; i++)
            {
                if (len <= 0)
                {
                    output[i] = 0;
                }
                else
                {
                    output[i] = (input[i] - min) / len;
                }
            }

            return output;
        }

        void Update()
        {
            float[] normalizedData = null;

            // 获取原始采样数据
            audioSource.GetOutputData(sampleData, 0);

            // 进行标准化处理
            normalizedData = NormalizeData(sampleData);

            for (int i = 0; i < barList.Count; ++i)
            {
                float newHeight = minSize + (maxSize - minSize) * normalizedData[i];
                float currHeight = Mathf.Lerp(barList[i].sizeDelta.y, newHeight, Time.deltaTime * lerpSpeed) * 0.8f;

                barList[i].GetComponent<Image>().color = HSVtoRGB(normalizedData[i], 1, 1, 1);

                barList[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currHeight);
            }
        }

        /// <summary>
        /// 颜色数据转换
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="value"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static Color HSVtoRGB(float hue, float saturation, float value, float alpha)
        {
            while (hue > 1f)
            {
                hue -= 1f;
            }
            while (hue < 0f)
            {
                hue += 1f;
            }
            while (saturation > 1f)
            {
                saturation -= 1f;
            }
            while (saturation < 0f)
            {
                saturation += 1f;
            }
            while (value > 1f)
            {
                value -= 1f;
            }
            while (value < 0f)
            {
                value += 1f;
            }
            if (hue > 0.999f)
            {
                hue = 0.999f;
            }
            if (hue < 0.001f)
            {
                hue = 0.001f;
            }
            if (saturation > 0.999f)
            {
                saturation = 0.999f;
            }
            if (saturation < 0.001f)
            {
                return new Color(value * 255f, value * 255f, value * 255f);

            }
            if (value > 0.999f)
            {
                value = 0.999f;
            }
            if (value < 0.001f)
            {
                value = 0.001f;
            }

            float h6 = hue * 6f;
            if (h6 == 6f)
            {
                h6 = 0f;
            }
            int ihue = (int)(h6);
            float p = value * (1f - saturation);
            float q = value * (1f - (saturation * (h6 - (float)ihue)));
            float t = value * (1f - (saturation * (1f - (h6 - (float)ihue))));
            switch (ihue)
            {
                case 0:
                    return new Color(value, t, p, alpha);
                case 1:
                    return new Color(q, value, p, alpha);
                case 2:
                    return new Color(p, value, t, alpha);
                case 3:
                    return new Color(p, q, value, alpha);
                case 4:
                    return new Color(t, p, value, alpha);
                default:
                    return new Color(value, p, q, alpha);
            }
        }

    }
}
