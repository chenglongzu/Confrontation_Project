using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSController : MonoBehaviour
{
    [SerializeField]
    private Color32[] timeColor;

    private float aiAttackTime;

    //两位玩家 AI和角色
    private PlayerBase aiPlayer;
    private PlayerBase userPlayer;

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
    //玩家和AI的手牌实体类List
    private List<CardEntity> playerRoundCardList;
    private Dictionary<int, Queue<CardEntity>> playerRoundCardDic;

    private List<CardEntity> aiRoundCardList;
    private Dictionary<int, Queue<CardEntity>> aiRoundCardDic;
    #endregion

    #region 游戏占位符
    private bool isStart=true;  //当前点击空格键是开始游戏还是暂停游戏
    private bool isFirst=true;       //是否为第一次点击空格键

    private bool isOnRound;     //是否在回合中

    private bool isCanBePause=true; //是否可以暂停

    private bool isPlayerSuppled;   //玩家是否被补充卡牌
    private bool isAiSuppled;       //AI是否被补充卡牌
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
        aiPlayer = BattleManager.GetInstance().aiPlayer;
        userPlayer = BattleManager.GetInstance().userPlayer;
    }

    /// <summary>
    /// 初始化卡牌实体数据
    /// </summary>
    private void InitCardData()
    {
        //添加AI卡片到牌库
        BattleManager.GetInstance().AddAICardToBattle();

        //获取牌库中随机的五张卡牌
        BattleManager.GetInstance().PlayRandomGetCardToBattle();
        BattleManager.GetInstance().AiRandomGetCardToBattle();

    }

    /// <summary>
    /// 获取场景中物体的索引
    /// </summary>
    private void GetSceneIndexs()
    {
        //倒计时的Lable
        countDownLable = transform.Find("CountDown").GetComponent<UILabel>();

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
        //从BattleManager单例中获取玩家和AI的手牌
        playerRoundCardList = BattleManager.GetInstance().GetPlayerRoundCardList();
        aiRoundCardList = BattleManager.GetInstance().GetAiRoundCardList();

        playerRoundCardDic = BattleManager.GetInstance().GetPlayerRoundCardDic();
        aiRoundCardDic = BattleManager.GetInstance().GetAiRoundCardDic();

        for (int i = 0; i < playerRoundCardList.Count; i++)
        {
            if (!playerRoundCardDic.ContainsKey(playerRoundCardList[i].id))
            {
                playerRoundCardDic.Add(playerRoundCardList[i].id, new Queue<CardEntity>() { });
            }
            playerRoundCardDic[playerRoundCardList[i].id].Enqueue(playerRoundCardList[i]);
        }


        for (int i = 0; i < aiRoundCardList.Count; i++)
        {
            if (!aiRoundCardDic.ContainsKey(aiRoundCardList[i].id))
            {
                aiRoundCardDic.Add(aiRoundCardList[i].id, new Queue<CardEntity>() { });

            }
            aiRoundCardDic[aiRoundCardList[i].id].Enqueue(aiRoundCardList[i]);
        }

        RefashSceneCardData();

        //设置两个玩家的初始生命值
        aiPlayer.SetLifeValue(DataManager.GetInstance().maxLifeValue);
        userPlayer.SetLifeValue(DataManager.GetInstance().maxLifeValue);
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
                playerCardSprite[i].spriteName = "Card_" + playerRoundCardList[i].id;
                playerCardSprite[i].gameObject.name = "Card_" + playerRoundCardList[i].id;
            }
        }

        //刷新AI的卡牌数据
        for (int i = 0; i < aiCardSprite.Length; i++)
        {
            Debug.Log("AICount:"+aiRoundCardList.Count);
            if (i < aiRoundCardList.Count)
            {
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
    }

    /// <summary>
    /// 刷新UI数值
    /// </summary>
    private void RefashUIValue()
    {
        transform.Find("BittleGround/PlayerCardGroup/Label").GetComponent<UILabel>().text = BattleManager.GetInstance().GetButtleCardLength().ToString();
        transform.Find("BittleGround/AICardGroup/Label").GetComponent<UILabel>().text = BattleManager.GetInstance().GetAiButtleCardLength().ToString();

        //添加玩家和AI的生命值的体现
        int aiLife = BattleManager.GetInstance().aiPlayer.lifeValue;
        int userLife = BattleManager.GetInstance().userPlayer.lifeValue;

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

                AddTweenPostionEffect(aiCardCollection, aiCardCollectionOriPos, aiCardCollectionTargetPos, 1);

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
            aiPlayer.AddDiedActionListener(()=> { 
            
            });
            //用户玩家的血量被扣完后的监听
            userPlayer.AddDiedActionListener(() => {

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
                        isOnRound = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 真正的出牌逻辑
    /// </summary>
    private void PlayerPlayCard()
    {
        if (userPlayer.isAttacked) return;

        CardEntity cardEntity = null;

        int index = Convert.ToInt32(UICamera.hoveredObject.name.Substring(5, 1));

        if (playerRoundCardDic.ContainsKey(index))
        {
            cardEntity = playerRoundCardDic[index].Dequeue();
        }
        //获得当前点击卡牌的实体
        RemoveUserCardEntity(cardEntity);

        //设置玩家出牌
        userPlayer.SetCurrtenCard(cardEntity);

        currtenPlayerCard = Instantiate<GameObject>(playerTempUsedCard, UICamera.hoveredObject.transform.position, Quaternion.identity, transform);

        currtenPlayerCard.GetComponent<UISprite>().spriteName = "Card_" + cardEntity.id;

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
    /// 开启一个回合
    /// </summary>
    public void StartRound()
    {
        isOnRound = true;

        //设置玩家角色每次的初始化
        aiPlayer.StartRound();
        userPlayer.StartRound();

        aiAttackTime = UnityEngine.Random.Range(0f, 3f);

        //调用开启一个回合
        BattleManager.GetInstance().StartRound(FirstDectectCard, SecondDectectCard, OverTime, (value) => {
            //当传递出来的数据小于0就不用再更新了
            if (value < 0) return;
            //回合实时更新，传递进来数据为三秒倒计时
            countDownLable.text = value.ToString("#0.00") + "s";
            countDownLable.SetColorNoAlpha(timeColor[0]);

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
        Debug.Log("第一个回合的监测");
        //更换字体颜色
        countDownLable.SetColorNoAlpha(timeColor[1]);
    }

    /// <summary>
    /// 回调函数，在倒计时一秒的时候
    /// </summary>
    private void SecondDectectCard()
    {
        //开始检测哪一方未出牌
        countDownLable.SetColorNoAlpha(timeColor[2]);

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
        }
        else if (!userPlayer.isAttacked && aiPlayer.isAttacked)
        {
            //AI出牌了
            aiPlayer.isCardUseful = true;
            userPlayer.isCardUseful = false;
        }
    }

    /// <summary>
    /// 一个回合的结束
    /// </summary>
    private void OverTime()
    {
        isOnRound = false;

        if (currtenPlayerCard == null) return;

        //强制松牌
        AddTweenPostionEffect(currtenPlayerCard, playerTargetPos,0.5f);

        //最终结算发起进攻
        aiPlayer.Attack(userPlayer);
        userPlayer.Attack(aiPlayer);

        RefashUIValue();

        countDownLable.text = "End";

        StartCoroutine(DelayToStartRound());
    }

    private IEnumerator DelayToStartRound()
    {
        yield return new WaitForSeconds(1);
       
        Destroy(currtenAiCard);
        Destroy(currtenPlayerCard);

        //保证当前还有手牌
        if (aiRoundCardList.Count>0&&playerRoundCardList.Count>0)
        {
            StartRound();
        }
        else if(aiRoundCardList.Count<=0 && playerRoundCardList.Count <= 0)
        {
            //玩家和AI的手牌都打光了
            //分别补充AI和玩家的卡牌
        }
        else if (aiRoundCardList.Count >= 0 && playerRoundCardList.Count <= 0)
        {
            //玩家的手牌打光了
            //补充玩家的卡牌
        }
        else if(aiRoundCardList.Count <= 0 && playerRoundCardList.Count >= 0)
        {
            //AI的手牌打光了
            //补充AI的卡牌
        }
    }

}
