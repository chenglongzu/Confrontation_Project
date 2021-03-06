using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ShimmerFramework;

public class VSController : MonoBehaviour
{
    private float aiAttackTime;

    //两位玩家 AI和角色
    private PlayerBase aiPlayer;
    private PlayerBase userPlayer;

    private string currtenColorText;

    private int aiLife;
    private int userLife;

    #region 场景动画索引
    //两个卡片集的动画位置
    [SerializeField]
    private Vector3 userCardCollectionTargetPos;
    [SerializeField]
    private Vector3 userCardCollectionOriPos;

    [SerializeField]
    private Vector3 aiCardCollectionTargetPos;
    [SerializeField]
    private Vector3 aiCardCollectionOriPos;

    [SerializeField]
    private Vector3 playerTargetPos;

    [SerializeField]
    private Vector3 aiTargetPos;
    #endregion

    #region 场景物体索引
    //倒计时的Lable索引
    private UILabel countDownLable;

    private UILabel levelIndexLable;

    //玩家和AI的生命值图标索引
    private UISprite[] playerHealthSprite;
    private UISprite[] aiHealthSprite;

    //玩家卡牌的图片
    private UISprite[] playerCardSprite;
    //AI卡牌的图片
    private UISprite[] aiCardSprite;

    //玩家和AI卡牌的动画脚本
    private GameObject userCardCollection;
    private GameObject aiCardCollection;

    #endregion

    #region 实体类数据结构
    //玩家牌库资源
    private List<CardEntity> playerCardsList;
    private Dictionary<int, Queue<CardEntity>> playerCardsDic;

    //AI牌库资源
    private List<CardEntity> aiCardList;
    private Dictionary<int, Queue<CardEntity>> aiCardDic;

    //玩家手牌资源
    private List<CardEntity> playerRoundCardList;
    private Dictionary<int, Queue<CardEntity>> playerRoundCardDic;

    //AI手牌资源
    private List<CardEntity> aiRoundCardList;
    private Dictionary<int, Queue<CardEntity>> aiRoundCardDic;

    #endregion

    #region 游戏占位符
    private bool isStart=true;  //当前点击空格键是开始游戏还是暂停游戏
    private bool isFirst=true;       //是否为第一次点击空格键

    private bool isOver;        //游戏是否结束

    private bool isOnRound;     //是否在回合中

    private bool isCanBePause=true; //是否可以暂停

    private bool isPlayerSuppled;   //玩家是否被补充卡牌
    private bool isAiSuppled;       //AI是否被补充卡牌

    private bool isPlayerCardActive=true;    //玩家的卡牌是否生效
    private bool isAiCardActive=true;        //AI的卡牌是否生效(默认都是生效的)

    private bool isFirstHandCard;       //是否第一次出牌
    #endregion

    #region 实例化物体和索引
    private GameObject playerTempUsedCard;
    private GameObject aiTempUsedCard;

    private GameObject currtenPlayerCard;     //当前出的牌
    private GameObject currtenAiCard;     //当前出的牌
    #endregion

    void Start()
    {
        InitCardData();

        InitPlayerValue();

        GetSceneIndexs();

        InitSceneData();

        AddUIEventListen();
    }

    /// <summary>
    /// 初始化两位玩家的数据
    /// </summary>
    private void InitPlayerValue()
    {
        aiLife = DataManager.GetInstance().maxLifeValue;
        userLife = DataManager.GetInstance().maxLifeValue;
    }

    /// <summary>
    /// 初始化卡牌实体数据
    /// </summary>
    private void InitCardData()
    {
        //初始化当前回合的牌库，不影响本来的牌库
        playerCardsList = BattleManager.GetInstance().ClonePlayerCard();
        aiCardList = BattleManager.GetInstance().CloneAICard();

        playerCardsDic = new Dictionary<int, Queue<CardEntity>>();
        aiCardDic = new Dictionary<int, Queue<CardEntity>>();

        for (int i = 0; i < playerCardsList.Count; i++)
        {
            if (!playerCardsDic.ContainsKey(playerCardsList[i].id))
            {
                playerCardsDic.Add(playerCardsList[i].id,new Queue<CardEntity>());
            }
            playerCardsDic[playerCardsList[i].id].Enqueue(playerCardsList[i]);
        }

        for (int i = 0; i < aiCardList.Count; i++)
        {
            if (!aiCardDic.ContainsKey(aiCardList[i].id))
            {
                aiCardDic.Add(aiCardList[i].id, new Queue<CardEntity>());
            }
            aiCardDic[aiCardList[i].id].Enqueue(aiCardList[i]);
        }


        playerRoundCardList = new List<CardEntity>();
        playerRoundCardDic = new Dictionary<int, Queue<CardEntity>>();

        aiRoundCardList = new List<CardEntity>();
        aiRoundCardDic = new Dictionary<int, Queue<CardEntity>>();

        //获取牌库中随机的五张卡牌
        PlayRandomGetCardToBattle(5);
        AiRandomGetCardToBattle(5);
    }

    /// <summary>
    /// 获取场景中物体的索引
    /// </summary>
    private void GetSceneIndexs()
    {
        //倒计时的Lable
        countDownLable = transform.Find("CountDown").GetComponent<UILabel>();

        levelIndexLable = transform.Find("LevelIndex").GetComponent<UILabel>();

        //玩家和AI卡牌动画脚本
        userCardCollection = transform.Find("CardCollection/UserCardCollection").gameObject;
        aiCardCollection = transform.Find("CardCollection/AICardCollection").gameObject;

        //玩家和AI血量的图标
        playerHealthSprite = transform.Find("BittleGround/PlayerCardGroup/PlayerHealth").GetComponentsInChildren<UISprite>();
        aiHealthSprite = transform.Find("BittleGround/AICardGroup/AIHealth").GetComponentsInChildren<UISprite>();

        //获取玩家和AI手牌图片的索引
        playerCardSprite = transform.Find("CardCollection/UserCardCollection").GetComponentsInChildren<UISprite>();
        aiCardSprite = transform.Find("CardCollection/AICardCollection").GetComponentsInChildren<UISprite>();

        //加载资源索引
        playerTempUsedCard = Resources.Load<GameObject>("UserCard_Temp");
        aiTempUsedCard = Resources.Load<GameObject>("AICard_Temp");
    }

    /// <summary>
    /// 初始化场景数据
    /// </summary>
    private void InitSceneData()
    {        
        RefashSceneCardData();

        Debug.Log("aiPlayer:"+ aiPlayer);

        aiPlayer = new PlayerBase();
        userPlayer = new PlayerBase();

        //设置两个玩家的初始生命值
        aiPlayer.SetLifeValue(DataManager.GetInstance().maxLifeValue);
        userPlayer.SetLifeValue(DataManager.GetInstance().maxLifeValue);

        //设置当前关卡的数值
        levelIndexLable.text = "Level "+GameManager.GetInstance().CurrrtenLevel.ToString();

        transform.Find("OverPanel").gameObject.SetActive(false);
    }

    /// <summary>
    /// 移除AI数据结构中的数据结构
    /// </summary>
    /// <param name="cardEntity"></param>
    private void RemoveAICardEntity(CardEntity cardEntity)
    {
        if (aiRoundCardList.Contains(cardEntity))
        {
            aiRoundCardList.Remove(cardEntity);
        }

        if (aiRoundCardDic.ContainsKey(cardEntity.id))
        {
            aiRoundCardDic[cardEntity.id].Dequeue();
        }
    }

    /// <summary>
    /// 移除玩家手牌数据结构中的实体数据，当前方法只移除List数据结构中的数据
    /// </summary>
    /// <param name="cardEntity"></param>
    private void RemoveUserCardEntity(CardEntity cardEntity)
    {
        if (playerRoundCardList.Contains(cardEntity))
        {
            playerRoundCardList.Remove(cardEntity);
        }else
        {
            for (int i = 0; i < playerRoundCardList.Count; i++)
            {
                if (playerRoundCardList[i].id==cardEntity.id)
                {
                    playerRoundCardList.Remove(cardEntity);
                    RefashSceneCardData();
                    Debug.LogError("没有初步获取数据");

                    return;
                }
            }
            Debug.LogError("没有从数据结构中获取数据");
        }
    }

    /// <summary>
    /// 刷新场景中的卡牌数据
    /// </summary>
    private void RefashSceneCardData()
    {
        //为玩家手牌图片附上当前玩家的手牌数据 调用则刷新
        for (int i = 0; i < playerCardSprite.Length; i++)
        {
            if (i >= playerRoundCardList.Count)
            {
                playerCardSprite[i].gameObject.SetActive(false);
            }
            else
            {
                playerCardSprite[i].gameObject.SetActive(true);

                playerCardSprite[i].spriteName = "Card_" + playerRoundCardList[i].id;
                playerCardSprite[i].gameObject.name = "Card_" + playerRoundCardList[i].id;
            }
        }

        //刷新AI的卡牌数据
        for (int i = 0; i < aiCardSprite.Length; i++)
        {
            if (i < aiRoundCardList.Count)
            {
                aiCardSprite[i].gameObject.SetActive(true);

                aiCardSprite[i].gameObject.name = "AICard_" + aiRoundCardList[i].id;
            }else
            {
                aiCardSprite[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 添加UI事件的监听
    /// </summary>
    private void AddUIEventListen()
    {
        RefashUIValue();

        //UIEventListener.Get(transform.Find("OverPanel/Restart").gameObject).onClick+=(value)=> {
        //    Time.timeScale = 1;
        //    SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        //};

        //UIEventListener.Get(transform.Find("OverPanel/MainMeanu").gameObject).onClick += (value) => {
        //    Time.timeScale = 1;
        //    SceneManager.LoadSceneAsync("Start");
        //};

    }

    /// <summary>
    /// 刷新UI数值
    /// </summary>
    private void RefashUIValue()
    {
        transform.Find("BittleGround/PlayerCardGroup/Label").GetComponent<UILabel>().text = playerCardsList.Count.ToString();
        transform.Find("BittleGround/AICardGroup/Label").GetComponent<UILabel>().text = aiCardList.Count.ToString();

        //添加玩家和AI的生命值的体现
        Debug.Log(string.Format("这里看到的数据AI:{0},User:{1}:", aiLife, userLife));

        for (int i = 0; i < aiHealthSprite.Length; i++)
        {
            if (i<aiLife)
            {
                aiHealthSprite[i].gameObject.SetActive(true);
            }else
            {
                aiHealthSprite[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < playerHealthSprite.Length; i++)
        {
            if (i < userLife)
            {
                playerHealthSprite[i].gameObject.SetActive(true);
            }else
            {
                playerHealthSprite[i].gameObject.SetActive(false);
            }
        }

    }

    private void Update()
    {
        PlayerPlayAHand();

        StartAndPauseGame();
    }

    /// <summary>
    /// 游戏开始和暂停相关逻辑
    /// </summary>
    private void StartAndPauseGame()
    {
        if (isOver) return;
        //不可以被暂停的话 则后面的逻辑不再执行
        if (!isCanBePause) return;

        //控制游戏的开始和暂停
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isStart)
            {
                AddTweenPostionEffect(userCardCollection, userCardCollectionOriPos, userCardCollectionTargetPos, 1, () => {
                    //第一次调用卡牌出现时才开启回合
                    GameStart();
                });

                AddTweenPostionEffect(aiCardCollection, aiCardCollectionOriPos, aiCardCollectionTargetPos, 1);

                Time.timeScale = 1;

                isStart = false;
            }
            else
            {
                //游戏暂停
                //游戏开始
                AddTweenPostionEffect(userCardCollection, userCardCollectionOriPos, userCardCollectionTargetPos,1);

                AddTweenPostionEffect(aiCardCollection, aiCardCollectionTargetPos, aiCardCollectionOriPos, 1);

                Time.timeScale = 0;

                isStart = true;
            }
        }
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    private void GameStart()
    {
        if (isFirst)
        {
            //AI玩家的血量扣完之后的监听
            aiPlayer.AddActionListener((blood)=> {
                //GameOver  判断玩家死亡 玩家血量扣完
                Debug.Log("AI的剩余血量" + blood);
                aiLife = blood;
                if (blood<=0)
                {
                    AiGameOver("AI血量扣完");
                }
                RefashUIValue();
            },
            (value)=> {
                //Ai添加手牌
                if (!AiRandomGetCardToBattle(value))
                {
                    //AI游戏失败
                    AiGameOver("AI牌库数量不足");
                }

                RefashSceneCardData();
            });

            //用户玩家的血量被扣完后的监听
            userPlayer.AddActionListener((blood) => {
                //GameOver  判断AI死亡 AI血量扣完
                Debug.Log("玩家的剩余血量"+blood);
                userLife = blood;
                if (blood<=0)
                {
                    PlayerGameOver("玩家血量扣完");
                }
                RefashUIValue();
            },
            (value)=> {
                //玩家添加手牌
                if (!PlayRandomGetCardToBattle(value))
                {
                    //玩家游戏失败
                    PlayerGameOver("玩家牌库数量不足");
                }

                RefashSceneCardData();
            });

            StartRound();

            isFirst = false;
        }
    }

    /// <summary>
    /// 玩家出牌相关操作
    /// </summary>
    public void PlayerPlayAHand()
    {
        //鼠标没有任何操作时则不再执行
        if (!(Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))) return;

        //出牌相关操作
        if (isOnRound)
        {
            if (UICamera.isOverUI)
            {
                if (UICamera.hoveredObject == null) return;
                if (UICamera.hoveredObject.name.Contains("Card_"))
                {
                    Debug.Log(UICamera.hoveredObject.name);
                    //按下出牌
                    if (Input.GetMouseButtonDown(0))
                    {
                        PlayerPlayCard();

                        RefashSceneCardData();
                    }

                    //按下不松手跟随鼠标移动
                    if (Input.GetMouseButton(0))
                    {
                        PlayerPlayCard();

                        currtenPlayerCard.transform.position = GameObject.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                    }

                    //鼠标抬起牌打出卡牌
                    if (Input.GetMouseButtonUp(0))
                    {
                        PlayerPlayCard();

                        AddTweenPostionEffect(currtenPlayerCard, playerTargetPos, 0.5f);
                        //回合结束，避免出第二次牌
                    }
                }
            }
        }

        if (isFirstHandCard)
        {
            //按下不松手跟随鼠标移动
            if (Input.GetMouseButton(0) && currtenPlayerCard != null)
            {
                currtenPlayerCard.transform.position = GameObject.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            }

            //鼠标抬起牌打出卡牌
            if (Input.GetMouseButtonUp(0) && currtenPlayerCard != null)
            {
                AddTweenPostionEffect(currtenPlayerCard, playerTargetPos, 0.5f);
            }
        }

    }

    /// <summary>
    /// 真正的出牌逻辑
    /// </summary>
    private void PlayerPlayCard()
    {
        if (userPlayer.isAttacked) return;
        isFirstHandCard = true;
        isOnRound = false;

        CardEntity cardEntity = null;

        int index = Convert.ToInt32(UICamera.hoveredObject.name.Substring(5, 1));

        if (playerRoundCardDic.ContainsKey(index))
        {
            cardEntity = playerRoundCardDic[index].Dequeue();
        }
        //获得当前点击卡牌的实体
        RemoveUserCardEntity(cardEntity);

        Debug.Log("玩家出牌："+cardEntity.name);

        //设置玩家出牌
        userPlayer.SetCurrtenCard(cardEntity);

        currtenPlayerCard = Instantiate<GameObject>(playerTempUsedCard, UICamera.hoveredObject.transform.position, Quaternion.identity, transform);

        currtenPlayerCard.GetComponent<UISprite>().spriteName = "Card_" + cardEntity.id;

        if (!isPlayerCardActive)
        {
            currtenPlayerCard.GetComponent<UISprite>().color = new Color32(130,130,130,255);
        }
    }

    /// <summary>
    /// 动态给UI实现动画效果
    /// </summary>
    /// <param name="tweenObj"></param>
    /// <param name="targetPos"></param>
    /// <param name="duringTime"></param>
    /// <param name="action"></param>
    private void AddTweenPostionEffect(GameObject tweenObj,Vector3 targetPos,float duringTime, Action action=null)
    {
        TweenPosition cardTweenPostion = tweenObj.AddComponent<TweenPosition>();

        cardTweenPostion.from = tweenObj.transform.localPosition;
        cardTweenPostion.to = targetPos;
        cardTweenPostion.duration = duringTime;

        cardTweenPostion.onFinished.Add(new EventDelegate(() => {
            if (action!=null)
            {
                action();
            }

            Destroy(cardTweenPostion);
        }));

    }
    private void AddTweenPostionEffect(GameObject tweenObj,Vector3 oriPos, Vector3 targetPos, float duringTime, Action action = null)
    {
        TweenPosition cardTweenPostion = tweenObj.AddComponent<TweenPosition>();

        cardTweenPostion.from = oriPos;
        cardTweenPostion.to = targetPos;
        cardTweenPostion.duration = duringTime;

        cardTweenPostion.onFinished.Add(new EventDelegate(() => {
            if (action != null)
            {
                action();
            }

            Destroy(cardTweenPostion);
        }));

    }

    /// <summary>
    /// 开启一个回合 包含AI出牌的逻辑
    /// </summary>
    public void StartRound()
    {
        isOnRound = true;

        //设置玩家角色每次的初始化
        aiPlayer.StartRound();
        userPlayer.StartRound();

        aiAttackTime = UnityEngine.Random.Range(0f, 3f);
        
        currtenColorText = "[FFE800]";

        //调用开启一个回合
        BattleManager.GetInstance().StartRound(FirstDectectCard, SecondDectectCard, OverTime, (value) => {
            //当传递出来的数据小于0就不用再更新了
            if (value < 0) return;

            //回合实时更新，传递进来数据为三秒倒计时
            countDownLable.text =currtenColorText + value.ToString("#0.00") + "s";


            //AI出牌逻辑
            if (!aiPlayer.isAttacked)
            {
                if (value <= aiAttackTime)
                {
                    CardEntity cardEntity = aiRoundCardList[UnityEngine.Random.Range(0,aiRoundCardList.Count)];
                    RemoveAICardEntity(cardEntity);
                    
                    //出牌
                    aiPlayer.SetCurrtenCard(cardEntity);

                    GameObject tempGObj=null;
                    for (int i = 0; i < aiCardSprite.Length; i++)
                    {
                        if (aiCardSprite[i].gameObject.name.Substring(7,1)== cardEntity.id.ToString())
                        {
                            tempGObj = aiCardSprite[i].gameObject;
                        }
                    }

                    currtenAiCard= Instantiate<GameObject>(aiTempUsedCard, tempGObj.transform.position, Quaternion.identity, transform);
                    RefashSceneCardData();

                    if (!isAiCardActive)
                    {
                        currtenAiCard.GetComponent<UISprite>().color = new Color32(130, 130, 130,255);
                    }

                    AddTweenPostionEffect(currtenAiCard, aiTargetPos,0.5f,()=> {
                        currtenAiCard.GetComponent<UISprite>().spriteName = "Card_" + cardEntity.id;
                    });

                    TweenScale aiTweenScale = currtenAiCard.AddComponent<TweenScale>();
                    aiTweenScale.to = new Vector3(0.6f,0.6f,0.6f);
                    aiTweenScale.duration = 0.5f;
                }
            }
        });
    }

    /// <summary>
    /// 回调函数，在倒计时两秒的时候
    /// </summary>
    private void FirstDectectCard()
    {
        //设置倒计时文本的颜色
        currtenColorText = "[08FF00]";
    }

    /// <summary>
    /// 回调函数，在倒计时一秒的时候
    /// </summary>
    private void SecondDectectCard()
    {

        //开始检测哪一方未出牌
        currtenColorText = "[FF3F00]";

        //两方都在两秒之内出牌了
        if (userPlayer.isAttacked && aiPlayer.isAttacked) return;

        if (userPlayer.isAttacked && aiPlayer.isAttacked)
        {
            //两方都出牌了
            aiPlayer.isCardUseful = true;
            userPlayer.isCardUseful = true;
        }
        else if (!userPlayer.isAttacked&&!aiPlayer.isAttacked)
        {
            //两方都没出牌
            aiPlayer.isCardUseful = true;
            userPlayer.isCardUseful = true;
        }
        else if(userPlayer.isAttacked && !aiPlayer.isAttacked)
        {
            //玩家出牌了
            aiPlayer.isCardUseful = false;
            userPlayer.isCardUseful = true;

            //设置AI的卡牌失效
            isAiCardActive = false;
        }
        else if (!userPlayer.isAttacked && aiPlayer.isAttacked)
        {
            //AI出牌了
            aiPlayer.isCardUseful = true;
            userPlayer.isCardUseful = false;

            //设置玩家的卡牌失效
            isPlayerCardActive = false;
        }
    }

    /// <summary>
    /// 一个回合的结束
    /// </summary>
    private void OverTime()
    {
        isOnRound = false;
        isFirstHandCard = false;

        isPlayerCardActive = true;
        isAiCardActive = true;

        //最终结算发起进攻
        aiPlayer.Attack(userPlayer);
        userPlayer.Attack(aiPlayer);

        RefashUIValue();

        countDownLable.text = "End";

        StartCoroutine(DelayToStartRound());

        if (currtenPlayerCard == null) return;

        //强制松牌
        AddTweenPostionEffect(currtenPlayerCard, playerTargetPos, 0.5f);

    }

    /// <summary>
    /// 开启下一回合 并判断是否合乎逻辑
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayToStartRound()
    {
        yield return new WaitForSeconds(1);
       
        Destroy(currtenAiCard);
        Destroy(currtenPlayerCard);

        if (isOver) yield break;
        //首先保证玩家和AI的生命值都大于0
        if (aiLife > 0 && userLife > 0)
        {
            //保证当前还有手牌
            if (aiRoundCardList.Count > 0 && playerRoundCardList.Count > 0)
            {
                StartRound();
            }
            else
            {
                //AI和玩家的手牌都没了
                if (aiRoundCardList.Count <= 0 && playerRoundCardList.Count <= 0)
                {
                    if (!isPlayerSuppled)
                    {
                        if (!PlayRandomGetCardToBattle(2))
                        {
                            //玩家牌库不足 判断玩家失败
                            PlayerGameOver("玩家牌库不足");
                        }

                        RefashSceneCardData();

                        isPlayerSuppled = true;
                    }

                    if (!isAiSuppled)
                    {
                        if (!AiRandomGetCardToBattle(2))
                        {
                            //AI牌库不足 判断AI失败
                            AiGameOver("AI牌库不足");
                        }

                        RefashSceneCardData();

                        isAiSuppled = true;
                    }

                    //两方都经过了补充 判定平局
                    if (isAiSuppled && isPlayerSuppled)
                    {
                        DrawGame("两方牌库的卡牌都不足");
                    }
                    //玩家和AI的手牌都打光了
                    //分别补充AI和玩家的卡牌
                }
                //玩家的手牌没了 AI的手牌还在
                else if (aiRoundCardList.Count >= 0 && playerRoundCardList.Count <= 0)
                {
                    if (!isPlayerSuppled)
                    {
                        if (!PlayRandomGetCardToBattle(2))
                        {
                            //玩家牌库不足 判断玩家失败
                            PlayerGameOver("玩家牌库不足");
                            yield break;
                        }

                        RefashSceneCardData();

                        isPlayerSuppled = true;
                    }
                    else
                    {
                        PlayerGameOver("玩家牌库不足");
                    }

                    //玩家的手牌打光了
                    //补充玩家的卡牌
                }
                //AI的手牌没了 玩家的手牌还在
                else if (aiRoundCardList.Count <= 0 && playerRoundCardList.Count >= 0)
                {
                    if (!isAiSuppled)
                    {
                        if (!AiRandomGetCardToBattle(2))
                        {
                            //AI牌库不足 判断AI失败
                            AiGameOver("AI牌库不足");
                            yield break;
                        }

                        RefashSceneCardData();

                        isAiSuppled = true;
                    }
                    else
                    {
                        AiGameOver("AI牌库不足");
                    }

                    //AI的手牌打光了
                    //补充AI的卡牌
                }

                //经过一次补充后如果还是符合条件则开启一个回合
                if (aiRoundCardList.Count > 0 && playerRoundCardList.Count > 0)
                {
                    StartRound();
                }
            }

        }
        //两个玩家的生命值同时都小于0
        else if (aiLife <= 0 && userLife <= 0)
        {
            DrawGame("两个玩家的生命值同时都小于0");
        }
        //AI的生命值小于0
        else if (aiLife <= 0 && userLife > 0)
        {
            AiGameOver("AI的生命值小于0");
        }
        //玩家的生命值小于0
        else if (aiLife > 0 && userLife <= 0)
        {
            PlayerGameOver("玩家的生命值小于0");
        }
    }

    /// <summary>
    /// AI游戏结束
    /// </summary>
    private void AiGameOver(string Reason)
    {
        Debug.Log("游戏胜利"+ Reason);
        SetOverPanelIndex(1, Reason);
    }

    /// <summary>
    /// 玩家游戏结束
    /// </summary>
    private void PlayerGameOver(string Reason)
    {
        Debug.Log("游戏结束"+ Reason);
        SetOverPanelIndex(0, Reason);
    }

    /// <summary>
    /// 平局
    /// </summary>
    private void DrawGame(string Reason)
    {
        Debug.Log("平局"+ Reason);
        SetOverPanelIndex(2, Reason);
    }

    /// <summary>
    /// 设置游戏结束的界面
    /// </summary>
    /// <param name="index"></param>
    /// <param name="info"></param>
    private void SetOverPanelIndex(int index,string info)
    {
        StartCoroutine(DelayToSetOver(index,info));
    }

    private IEnumerator DelayToSetOver(int index, string info)
    {
        yield return new WaitForSeconds(1);

        //获取当前关卡的序号
        int levelIndex = GameManager.GetInstance().CurrrtenLevel;

        if (index==1)
        {
            levelIndex++;
        }

        transform.Find("OverPanel").gameObject.SetActive(true);

        transform.Find("OverPanel/Info").GetComponent<UILabel>().text = info;

        if (levelIndex < 5)
        {
            UILabel countDownLable = transform.Find("OverPanel/CountDownLabel").GetComponent<UILabel>();

            transform.Find("OverPanel/Option").gameObject.SetActive(false);

            CountDown(3f,
                (value) => {
                    countDownLable.text = value.ToString("#0");
                },
            () => {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            });
        }
        else
        {
            if (index>=10)
            {
                transform.Find("OverPanel/Option/Option").gameObject.SetActive(false);
            }

            //选项面板激活 倒计时面板取消激活
            transform.Find("OverPanel/Option").gameObject.SetActive(true);
            transform.Find("OverPanel/CountDownLabel").gameObject.SetActive(false);
            transform.Find("OverPanel/Next").gameObject.SetActive(false);

            UIEventListener.Get(transform.Find("OverPanel/Option/Return").gameObject).onClick += (value) => {
                SceneManager.LoadSceneAsync("Start");
            };

            UIEventListener.Get(transform.Find("OverPanel/Option/Option").gameObject).onClick+=(value)=>{
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            };
        }

        switch (index)
        {
            case 0:
                //失败
                transform.Find("OverPanel/Head").GetComponent<UILabel>().text = "失败";
                transform.Find("OverPanel/Next").GetComponent<UILabel>().text = "再来本关";
                transform.Find("OverPanel/Option/Option").GetComponent<UILabel>().text = "再来本关";
                break;
            case 1:
                //成功
                transform.Find("OverPanel/Head").GetComponent<UILabel>().text = "成功";
                transform.Find("OverPanel/Next").GetComponent<UILabel>().text = "下一关";
                transform.Find("OverPanel/Option/Option").GetComponent<UILabel>().text = "下一关";
                GameManager.GetInstance().AddLevelIndex();
                break;
            case 2:
                //平局
                transform.Find("OverPanel/Head").GetComponent<UILabel>().text = "平局";
                transform.Find("OverPanel/Next").GetComponent<UILabel>().text = "再来本关";
                transform.Find("OverPanel/Option/Option").GetComponent<UILabel>().text = "再来本关";
                break;
            default:
                break;
        }

        isOver = true;

    }

    private void CountDown(float time,Action<float> callback,Action over)
    {
        StartCoroutine(CountDownIEnumerator(time, callback, over));
    }

    private IEnumerator CountDownIEnumerator(float time, Action<float> callback, Action over)
    {
        while (time>0)
        {
            yield return null;
            time -= Time.deltaTime;
            callback(time);
        }

        over();
    }

    #region GetRoundCard 手牌
    /// <summary>
    /// 玩家获取随机手牌并存储起来 游戏开始
    /// </summary>
    public bool PlayRandomGetCardToBattle(int value)
    {
        if (value > playerCardsList.Count) return false;

        List<CardEntity> tempList = GetDataFromList(value, playerCardsList, playerCardsDic);

        for (int i = 0; i < tempList.Count; i++)
        {
            playerRoundCardList.Add(tempList[i]);
        }

        for (int i = 0; i < playerRoundCardList.Count; i++)
        {
            if (playerRoundCardDic.ContainsKey(playerRoundCardList[i].id))
            {
                playerRoundCardDic[playerRoundCardList[i].id].Enqueue(playerRoundCardList[i]);
            }
            else
            {
                playerRoundCardDic.Add(playerRoundCardList[i].id, new Queue<CardEntity>());
                playerRoundCardDic[playerRoundCardList[i].id].Enqueue(playerRoundCardList[i]);
            }

            Debug.Log(string.Format("玩家手牌分别为：id：{0}，name：{1}", playerRoundCardList[i].id, playerRoundCardList[i].name));
        }
        return true;
    }

    /// <summary>
    /// AI获取随机手牌并存储起来 游戏开始
    /// </summary>
    public bool AiRandomGetCardToBattle(int value)
    {
        Debug.Log("AI牌库资源：" + aiCardList.Count);

        if (value > aiCardList.Count) return false;

        List<CardEntity> tempList = GetDataFromList(value, aiCardList, aiCardDic);
        for (int i = 0; i < tempList.Count; i++)
        {
            aiRoundCardList.Add(tempList[i]);
        }

        for (int i = 0; i < aiRoundCardList.Count; i++)
        {
            if (aiRoundCardDic.ContainsKey(aiRoundCardList[i].id))
            {
                aiRoundCardDic[aiRoundCardList[i].id].Enqueue(aiRoundCardList[i]);
            }
            else
            {
                aiRoundCardDic.Add(aiRoundCardList[i].id, new Queue<CardEntity>());
                aiRoundCardDic[aiRoundCardList[i].id].Enqueue(aiRoundCardList[i]);
            }

            Debug.Log(string.Format("AI手牌分别为：id：{0}，name：{1}", aiRoundCardList[i].id, aiRoundCardList[i].name));
        }
        return true;
    }

    /// <summary>
    /// 从数据结构中获取一系列数据
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cardsList"></param>
    /// <param name="cardsDic"></param>
    /// <returns></returns>
    public List<CardEntity> GetDataFromList(int value, List<CardEntity> cardsList, Dictionary<int, Queue<CardEntity>> cardsDic)
    {
        Debug.Log(string.Format("List长度：{0}，Dic长度：{1}", cardsList.Count, cardsDic.Count));

        List<CardEntity> tempPlayerCardList = GetRandomList(cardsList);

        List<CardEntity> returnPlayerCardList = new List<CardEntity>();

        for (int i = 0; i < value; i++)
        {
            CardEntity tempCardEntity = tempPlayerCardList[0];

            tempPlayerCardList.RemoveAt(0);

            if (cardsDic.ContainsKey(tempCardEntity.id))
            {
                cardsDic[tempCardEntity.id].Dequeue();
            }

            returnPlayerCardList.Add(tempCardEntity);
        }

        Debug.Log("当前牌库数量List：" + tempPlayerCardList.Count);
        return returnPlayerCardList;
    }

    /// <summary>
    /// 将传递进来的List顺序打乱
    /// </summary>
    /// <param name="playerCards"></param>
    /// <returns></returns>
    private List<CardEntity> GetRandomList(List<CardEntity> cardsList)
    {
        List<CardEntity> newPlayerCardList = cardsList;

        for (int i = 0; i < newPlayerCardList.Count; i++)
        {
            int ran = UnityEngine.Random.Range(0, newPlayerCardList.Count);

            CardEntity temp;

            temp = newPlayerCardList[i];
            newPlayerCardList[i] = cardsList[ran];
            newPlayerCardList[ran] = temp;
        }

        return newPlayerCardList;
    }
    #endregion

}
