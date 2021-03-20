using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSController : MonoBehaviour
{
    [SerializeField]
    private Color32[] timeColor;

    [SerializeField]
    private Vector3 targetPos;

    //两个卡片集的动画位置
    [SerializeField]
    private Vector3 userCardCollectionTargetPos;
    [SerializeField]
    private Vector3 userCardCollectionOriPos;

    [SerializeField]
    private Vector3 aiCardCollectionTargetPos;
    [SerializeField]
    private Vector3 aiCardCollectionOriPos;

    //两位玩家 AI和角色
    private PlayerBase aiPlayer;
    private PlayerBase userPlayer;

    //倒计时的Lable索引
    private UILabel countDownLable;

    //玩家和AI的生命值图标索引
    private UISprite[] playerHealthSprite;
    private UISprite[] aiHealthSprite;

    //玩家卡牌的图片
    private UISprite[] playerCardSprite;
    //AI卡牌的图片
    private UISprite[] aiCardSprite;

    //玩家和AI的手牌实体类List
    private List<CardEntity> playerRoundCardList;
    private Dictionary<int, Queue<CardEntity>> playerRoundCardDic;

    private List<CardEntity> aiRoundCardList;
    private Dictionary<int, Queue<CardEntity>> aiRoundCardDic;

    //玩家和AI卡牌的动画脚本
    private GameObject userCardCollection;
    private GameObject aiCardCollection;

    private bool isStart=true;  //当前点击空格键是开始游戏还是暂停游戏
    private bool isFirst=true;       //是否为第一次点击空格键

    private bool isOnRound;     //是否在回合中

    private bool isCanBePause=true; //是否可以暂停

    private GameObject currtenCard;     //当前出的牌
    private GameObject tempUsedCard;
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
        tempUsedCard = Resources.Load<GameObject>("UserCard_Temp");
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
        aiPlayer.lifeValue = DataManager.GetInstance().maxLifeValue;
        userPlayer.lifeValue = DataManager.GetInstance().maxLifeValue;
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
    /// 移除手牌数据结构中的实体数据，当前方法只移除List数据结构中的数据
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
        Debug.Log("3目前手牌长度为：" + playerRoundCardList.Count);
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
            if (i >= aiRoundCardList.Count)
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
                //游戏开始
                TweenPosition userTweenPos = userCardCollection.AddComponent<TweenPosition>();
                userTweenPos.to = userCardCollectionOriPos;
                userTweenPos.from = userCardCollectionTargetPos;

                userTweenPos.onFinished.Add(new EventDelegate(() => {
                    //第一次调用卡牌出现时才开启回合
                    if (isFirst)
                    {
                        StartRound();

                        isFirst = false;
                    }
                    Destroy(userTweenPos);
                }));

                TweenPosition aiTweenPos = aiCardCollection.AddComponent<TweenPosition>();
                aiTweenPos.to = aiCardCollectionOriPos;
                aiTweenPos.from = aiCardCollectionTargetPos;

                aiTweenPos.onFinished.Add(new EventDelegate(() => {Destroy(aiTweenPos); }));


                Time.timeScale = 1;

                isStart = false;
            }
            else
            {
                //游戏暂停
                //游戏开始
                TweenPosition userTweenPos = userCardCollection.AddComponent<TweenPosition>();
                userTweenPos.from = userCardCollectionOriPos;
                userTweenPos.to = userCardCollectionTargetPos;

                userTweenPos.onFinished.Add(new EventDelegate(() => { Destroy(userTweenPos); }));

                TweenPosition aiTweenPos = aiCardCollection.AddComponent<TweenPosition>();
                aiTweenPos.from = aiCardCollectionOriPos;
                aiTweenPos.to = aiCardCollectionTargetPos;

                aiTweenPos.onFinished.Add(new EventDelegate(() => { Destroy(aiTweenPos); }));

                Time.timeScale = 0;

                isStart = true;
            }
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
                if (UICamera.hoveredObject.name.Contains("Card_"))
                {
                    //按下出牌
                    if (Input.GetMouseButtonDown(0))
                    {
                        CardEntity cardEntity = null;

                        int index = System.Convert.ToInt32(UICamera.hoveredObject.name.Substring(5, 1));

                        if (playerRoundCardDic.ContainsKey(index))
                        {
                            cardEntity = playerRoundCardDic[index].Dequeue();
                        }
                        //获得当前点击卡牌的实体

                        Debug.Log("1目前手牌长度为：" + playerRoundCardList.Count);

                        RemoveUserCardEntity(cardEntity);

                        Debug.Log("2目前手牌长度为："+playerRoundCardList.Count);

                        //设置玩家出牌
                        userPlayer.SetCurrtenCard(cardEntity);

                        currtenCard = Instantiate<GameObject>(tempUsedCard, UICamera.hoveredObject.transform.position, Quaternion.identity, transform);

                        currtenCard.GetComponent<UISprite>().spriteName = "Card_" + cardEntity.id;

                        RefashSceneCardData();
                    }

                    //按下不松手跟随鼠标移动
                    if (Input.GetMouseButton(0))
                    {
                        currtenCard.transform.position = GameObject.Find("Camera").GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                    }

                    //鼠标抬起牌打出卡牌
                    if (Input.GetMouseButtonUp(0))
                    {
                        TweenPosition cardTweenPostion = currtenCard.GetComponent<TweenPosition>();

                        cardTweenPostion.from = currtenCard.transform.localPosition;
                        cardTweenPostion.to = targetPos;

                        cardTweenPostion.enabled = true;

                        cardTweenPostion.onFinished.Add(new EventDelegate(() => { /*cardTweenPostion.gameObject.SetActive(false);*/ }));

                        //回合结束，避免出第二次牌
                        isOnRound = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 开启一个回合
    /// </summary>
    public void StartRound()
    {
        isOnRound = true;

        //调用开启一个回合
        BattleManager.GetInstance().StartRound(FirstDectectCard, SecondDectectCard, OverTime, (value) => {
            countDownLable.text = value.ToString("#0.00") + "s";
            countDownLable.color = timeColor[0];

            //设置玩家角色每次的初始化
            aiPlayer.StartRound();
            userPlayer.StartRound();
        });
    }

    /// <summary>
    /// 回调函数，在倒计时两秒的时候
    /// </summary>
    private void FirstDectectCard()
    {
        //更换字体颜色
        countDownLable.color = timeColor[1];
    }

    /// <summary>
    /// 回调函数，在倒计时一秒的时候
    /// </summary>
    private void SecondDectectCard()
    {
        //开始检测哪一方未出牌
        countDownLable.color = timeColor[2];

        //两方都在两秒之内出牌了
        if (userPlayer.isAttacked && aiPlayer.isAttacked) return;

        if (!userPlayer.isAttacked&&!aiPlayer.isAttacked)
        {
            //两方都没出牌

        }
        else if(userPlayer.isAttacked && !aiPlayer.isAttacked)
        {
            //玩家出牌了

        }
        else if (!userPlayer.isAttacked && aiPlayer.isAttacked)
        {
            //AI没有出牌

        }
    }

    /// <summary>
    /// 一个回合的结束
    /// </summary>
    private void OverTime()
    {
        isOnRound = false;

        if (currtenCard == null) return;

        //强制松牌
        TweenPosition cardTweenPostion = currtenCard.GetComponent<TweenPosition>();

        cardTweenPostion.enabled = true;

        cardTweenPostion.from = currtenCard.transform.localPosition;
        cardTweenPostion.to = targetPos;

        cardTweenPostion.onFinished.Add(new EventDelegate(() => {
            //卡牌归位后的回调函数
            /*cardTweenPostion.gameObject.SetActive(false);*/ 
        }));

    }

    
}
