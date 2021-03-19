using UnityEngine;

namespace ShimmerFramework
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            ApplicationInit();

            MonoController.GetInstance();
        }

        private void Start()
        {
            //添加按键检测监听
            MonoManager.GetInstance().AddUpdateAction(InputManager.GetInstance().MyUpdate);

            DontDestroyOnLoad(gameObject);

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private void ApplicationInit()
        {
            Screen.SetResolution(960, 540, false);
            Screen.fullScreen = false;
        }



    }
}