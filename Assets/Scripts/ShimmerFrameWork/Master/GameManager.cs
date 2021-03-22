using UnityEngine;

namespace ShimmerFramework
{
    public class GameManager : SingletonMono<GameManager>
    {
        //当前关卡
        public int CurrrtenLevel { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            ApplicationInit();

            MonoController.GetInstance();

        }

        private void Start()
        {
            //添加按键检测监听
            MonoManager.GetInstance().AddUpdateAction(InputManager.GetInstance().MyUpdate);

            //添加AI卡片到牌库
            BattleManager.GetInstance().AddAICardToBattle();

            CurrrtenLevel = PlayerPrefs.GetInt("LevelIndex", 1);
        }

        /// <summary>
        /// 增加关卡的序号
        /// </summary>
        public void AddLevelIndex()
        {
            if (CurrrtenLevel<10)
            {
                CurrrtenLevel++;
            }

            if (PlayerPrefs.GetInt("LevelIndex",1)< CurrrtenLevel)
            {
                PlayerPrefs.SetInt("LevelIndex", CurrrtenLevel);

                SqliteInitManager.GetInstance().RefashOwnCards();
            }
        }

        /// <summary>
        /// 设置关卡序号
        /// </summary>
        /// <param name="index"></param>
        public void SetLevelIndex(int index)
        {
            CurrrtenLevel = index;
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