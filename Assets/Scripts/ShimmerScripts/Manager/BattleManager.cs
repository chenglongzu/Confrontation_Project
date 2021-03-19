using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShimmerSqlite;
using ShimmerFramework;
using System;

public class BattleManager : SingletonMono<BattleManager>
{
    //玩家牌库资源
    private List<CardEntity> playerCardsList;
    private Dictionary<int, List<CardEntity>> playerCradsDic;

    //玩家手牌资源
    private List<CardEntity> playerRoundCardList;
    private Dictionary<int, List<CardEntity>> playerRoundCardDic;

    //AI牌库资源
    private List<CardEntity> aiCardList;
    private Dictionary<int, List<CardEntity>> aiCardDic;

    //AI手牌资源
    private List<CardEntity> aiRoundCardList;
    private Dictionary<int, List<CardEntity>> aiRoundCardDic;

    //玩家和AI
    private PlayerBase userPlayer;
    private PlayerBase aiPlayer;

    protected override void Awake()
    {
        base.Awake();

        playerCardsList = new List<CardEntity>();
        playerCradsDic = new Dictionary<int, List<CardEntity>>();

        aiCardList = new List<CardEntity>();
        aiCardDic = new Dictionary<int, List<CardEntity>>();

        playerRoundCardList = new List<CardEntity>();
        playerRoundCardDic = new Dictionary<int, List<CardEntity>>();

        aiRoundCardList = new List<CardEntity>();
        aiRoundCardDic = new Dictionary<int, List<CardEntity>>();
    }

    private void Start()
    {
    }

    #region CardToBattle战斗
    /// <summary>
    /// 开始一个回合的战斗
    /// </summary>
    /// <param name="firstTime"></param>
    /// <param name="SecondTime"></param>
    public void StartRound(Action dectectTime)
    {
        StartCoroutine(CountDownNumber(3, dectectTime));
    }

    private IEnumerator CountDownNumber(float time, Action dectectTime)
    {
        bool firstTimeExcute=false;

        while (true)
        {
            yield return null;

            time -= Time.deltaTime;

            if (time <= 1)
            {
                if (!firstTimeExcute)
                {
                    dectectTime();
                    firstTimeExcute = true;
                }
            }

            if (time <= 0)
            {
                userPlayer.isPlayed = false;
                aiPlayer.isPlayed = false;

                StopCoroutine("CountDownNumber");
            }
        }
    }

    public void ToCard(CardEntity playerCard, CardEntity aiCard)
    {

    }

    public void PlayerToCard()
    {

    }

    public void AiToCard()
    {

    }

    #endregion

    #region GetRoundCard 手牌
    /// <summary>
    /// 玩家获取随机五张手牌并存储起来 游戏开始
    /// </summary>
    public void PlayRandomGetCardToBattle()
    {
        playerRoundCardList=GetRandomCard(playerCardsList,playerCradsDic);

        for (int i = 0; i < playerRoundCardList.Count; i++)
        {
            if (playerRoundCardDic.ContainsKey(playerRoundCardList[i].id))
            {
                playerRoundCardDic[playerRoundCardList[i].id].Add(playerRoundCardList[i]);
            }else
            {
                playerRoundCardDic.Add(playerRoundCardList[i].id, new List<CardEntity>() { playerRoundCardList[i] });
            }

            Debug.Log(string.Format("玩家手牌分别为：id：{0}，name：{1}", playerRoundCardList[i].id, playerRoundCardList[i].name));
        }

        aiPlayer = new PlayerBase();
    }

    /// <summary>
    /// AI获取随机五张手牌并存储起来 游戏开始
    /// </summary>
    public void AiRandomGetCardToBattle()
    {
        aiRoundCardList = GetRandomCard(aiCardList, aiCardDic);

        for (int i = 0; i < aiRoundCardList.Count; i++)
        {
            if (aiRoundCardDic.ContainsKey(aiRoundCardList[i].id))
            {
                aiRoundCardDic[aiRoundCardList[i].id].Add(aiRoundCardList[i]);
            }
            else
            {
                aiRoundCardDic.Add(aiRoundCardList[i].id, new List<CardEntity>() { aiRoundCardList[i] });
            }

            Debug.Log(string.Format("AI手牌分别为：id：{0}，name：{1}", playerRoundCardList[i].id, playerRoundCardList[i].name));
        }

        aiPlayer = new PlayerBase();
    }

    /// <summary>
    /// 获得随机五张手牌
    /// </summary>
    /// <param name="playerCards"></param>
    /// <returns></returns>
    private List<CardEntity> GetRandomCard(List<CardEntity> cardsList, Dictionary<int, List<CardEntity>>cardsDic)
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

        List<CardEntity> returnPlayerCardList = new List<CardEntity>();

        for (int i = 0; i < 5; i++)
        {
            CardEntity tempCardEntity = newPlayerCardList[i];

            cardsDic[tempCardEntity.id].RemoveAt(0);

            returnPlayerCardList.Add(newPlayerCardList[i]);
        }

        cardsList.Clear();
        foreach (var item in cardsDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                cardsList.Add(item.Value[i]);
            }
        }

        Debug.Log("当前牌库数量：" + cardsList.Count);

        return returnPlayerCardList;
    }
    #endregion

    #region CardReadyToBattle 牌库

    #region AI
    /// <summary>
    /// 向AI牌库添加卡片
    /// </summary>
    public void AddAICardToBattle()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 1; j < 5; j++)
            {
                CardEntity cardEntity = DataManager.GetInstance().GetCardEntityById(j);

                aiCardList.Add(cardEntity);

                if (aiCardDic.ContainsKey(cardEntity.id))
                {
                    aiCardDic[cardEntity.id].Add(cardEntity);
                }else
                {
                    aiCardDic.Add(cardEntity.id,new List<CardEntity>() { cardEntity });
                }
            }
        }
    }
    #endregion

    #region 玩家

    /// <summary>
    /// 向牌库添加卡牌
    /// </summary>
    /// <param name="entity"></param>
    public void AddCardToBattle(CardEntity entity)
    {
        playerCardsList.Add(entity);

        if (playerCradsDic.ContainsKey(entity.id))
        {
            playerCradsDic[entity.id].Add(entity);
        }else
        {
            playerCradsDic.Add(entity.id, new List<CardEntity>() { entity });
        }
    }

    /// <summary>
    /// 从牌库中移除卡牌
    /// </summary>
    /// <param name="entity"></param>
    public void RemoveCardToBattle(CardEntity entity)
    {
        if (playerCardsList.Contains(entity))
        {
            playerCardsList.Remove(entity);
        }

        if (playerCradsDic.ContainsKey(entity.id))
        {
            playerCradsDic[entity.id].Remove(entity);
        }
    }

    /// <summary>
    /// 获取所有的牌库数据
    /// </summary>
    /// <returns></returns>
    public List<CardEntity> GetAllButtleCard()
    {
        return playerCardsList;
    }

    /// <summary>
    /// 获取战斗卡牌总的资源点
    /// </summary>
    /// <returns></returns>
    public int GetButtleCardResourcesPoint()
    {
        int allRecourcesPoint = 0;

        for (int i = 0; i < playerCardsList.Count; i++)
        {
            allRecourcesPoint += playerCardsList[i].resourcesPoint;
        }

        return allRecourcesPoint;
    }

    /// <summary>
    /// 如果加上当前资源点数会不会超出最大的点数
    /// </summary>
    /// <returns></returns>
    public bool IfResourcesPointMaxed(int resourcePoint)
    {
        int currtenResourcesPoint = GetButtleCardResourcesPoint();

        if ((currtenResourcesPoint += resourcePoint) >DataManager.GetInstance().maxResourcesPoint)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获得牌库卡牌的数量
    /// </summary>
    /// <returns></returns>
    public int GetButtleCardLength()
    {
        return playerCardsList.Count;
    }

    /// <summary>
    /// 将组装的卡牌存储进入数据库
    /// </summary>
    public void StoreMatchCardCardToDataBase()
    {
        SqlManager.GetInstance().DeleTableAllData("MatchCard");

        for (int i = 0; i < playerCardsList.Count; i++)
        {
            SqlManager.GetInstance().Insert("MatchCard", playerCardsList[i]);
        }
    }

    /// <summary>
    /// 从数据库将数据写入到内存中
    /// </summary>
    public void GetCardDataFromDataBase()
    {
        //首先将内存中的数据清空
        ClearCardCollection();

        //调用数据库中的卡组数据
        List<Dictionary<string, object>> matchCardList = SqlManager.GetInstance().GetTableData("MatchCard");

        //将数据库中卡组数据写入到内存当中
        for (int i = 0; i < matchCardList.Count; i++)
        {
            int id = 0;
            string name = string.Empty;
            string fileName = string.Empty;
            int resourcesPoint = 0;
            int hurtValue = 0;
            string character = string.Empty;

            foreach (var item in matchCardList[i])
            {
                if (item.Key == "id")
                {
                    id = Convert.ToInt32(item.Value);
                }
                if (item.Key == "name")
                {
                    name = Convert.ToString(item.Value);
                }
                if (item.Key == "fileName")
                {
                    fileName = Convert.ToString(item.Value);
                }
                if (item.Key == "resourcesPoint")
                {
                    resourcesPoint = Convert.ToInt32(item.Value);
                }
                if (item.Key == "hurtValue")
                {
                    hurtValue = Convert.ToInt32(item.Value);
                }
                if (item.Key == "character")
                {
                    character = Convert.ToString(item.Value);
                }
            }

            CardEntity tempCardEntity = new CardEntity(id, name, fileName, resourcesPoint, hurtValue, character);

            AddCardToBattle(tempCardEntity);
            Debug.Log("当前玩家牌库内容为："+tempCardEntity.ToString());
        }

    }
    public void ClearCardCollection()
    {
        playerCardsList.Clear();
        playerCradsDic.Clear();
    }

    #endregion
    #endregion


}
