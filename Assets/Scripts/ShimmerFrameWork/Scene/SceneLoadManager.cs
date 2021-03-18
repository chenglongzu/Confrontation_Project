using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace ShimmerFramework
{
    public class SceneLoadManager : BaseManager<SceneLoadManager>
    {
#if Addressable

        #region Adressable 加载场景
        public void LoadScene(string scneneName, UnityAction unityAction)
        {
            SceneManager.LoadScene(scneneName);
            unityAction();
        }

        public void LoadSceneAsync(string scneneName, UnityAction unityAction, UnityAction<float> process = null)
        {
            MonoManager.GetInstance().StartCoroutine(ReallyLoadSceneAsync(scneneName, unityAction, process));
        }
        private IEnumerator ReallyLoadSceneAsync(string scneneName, UnityAction unityAction, UnityAction<float> process)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(scneneName);
            while (!ao.isDone)
            {
                if (process != null)
                {
                    process(ao.progress);
                }

                yield return ao.progress;
            }

            if (ao.isDone)
            {
                unityAction();
            }

        }

        #endregion

#else

        #region 通过BuildingSetting加载场景
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsync(string scneneName, UnityAction unityAction, UnityAction<float> process = null)
    {
        MonoManager.GetInstance().StartCoroutine(ReallyLoadSceneAsync(scneneName, unityAction, process));
    }
    private IEnumerator ReallyLoadSceneAsync(string scneneName, UnityAction unityAction, UnityAction<float> process)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(scneneName);
        while (!ao.isDone)
        {
            if (process != null)
            {
                process(ao.progress);
            }

            yield return ao.progress;
        }

        if (ao.isDone)
        {
            unityAction();
        }

    }
        #endregion

#endif
    }
}
