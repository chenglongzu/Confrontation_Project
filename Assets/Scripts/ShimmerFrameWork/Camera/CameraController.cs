using UnityEngine;
using System.Collections;

namespace ShimmerFramework
{
    public class CameraController : SingletonMono<CameraController>
    {
        private bool isOver;

        private Vector3 orginPos;

        public Camera mainCamera;

        void Start()
        {
           
            mainCamera = gameObject.AddComponent<Camera>();

            EventManager.GetInstance().AddAction("LightShakeCamera", LightShakeCamera);
            EventManager.GetInstance().AddAction("NormalShakeCamera", NormalShakeCamera);
            EventManager.GetInstance().AddAction("HardShakeCamera", HardShakeCamera);
        }

        void Update()
        {
            orginPos = transform.position;
        }

        #region 相机抖动效果
        private void NormalShakeCamera()
        {
            if (isOver)
            {
                isOver = false;

                StartCoroutine(CameraShake(0.05f, 0.5f));
            }
        }

        private void LightShakeCamera()
        {
            if (isOver)
            {
                isOver = false;
                StartCoroutine(CameraShake(0.02f, 0.2f));
            }
        }

        private void HardShakeCamera()
        {
            if (isOver)
            {
                isOver = false;

                StartCoroutine(CameraShake(0.1f, 0.7f));
            }
        }

        /// <summary>
        /// 相机抖动协程
        /// </summary>
        /// <param name="shakeTime"></param>
        /// <param name="shakeIntensity"></param>
        /// <returns></returns>
        private IEnumerator CameraShake(float shakeTime, float shakeIntensity)
        {
            while (shakeTime > 0)
            {
                shakeTime -= Time.deltaTime;
                yield return new WaitForSeconds(0.1f);
                transform.position = orginPos + new Vector3(Random.Range(-shakeIntensity, shakeIntensity), /*Random.Range(-shakeIntensity, shakeIntensity)*/0, 0);
            }
            isOver = true;
        }
        #endregion
    }
}
