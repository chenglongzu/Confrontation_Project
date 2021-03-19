using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShimmerSqlite;
using ShimmerFramework;
using System;

public class BattleManager : SingletonMono<BattleManager>
{
    //牌库资源
    private List<CardEntity> playerCardsList;
    private Dictionary<int, List<CardEntity>> playerCradsDic;

    //手牌资源
    private List<CardEntity> playerRoundCardList;
    private Dictionary<int, List<CardEntity>> playerRoundCardDic;

    //玩家和AI
    private PlayerBase userPlayer;
    private PlayerBase aiPlayer;
    void Start()
    {
        playerCardsList = new List<CardEntity>();
        playerCradsDic = new Dictionary<int, List<CardEntity>>();

        playerRoundCardList = new List<CardEntity>();
        playerRoundCardDic = new Dictionary<int, List<CardEntity>>();
    }


    #region CardToBattle战斗
    /// <summary>
    /// 开始一个回合的战斗
    /// </summary>
    /// <param name="firstTime"></param>
    /// <param name="SecondTime"></param>
    public void StartRound(Action firstTime,Action SecondTime)
    {
        StartCoroutine(CountDownNumber(3,firstTime,SecondTime));
    }

    private IEnumerator CountDownNumber(float time, Action firstTime, Action SecondTime)
    {
        bool firstTimeExcute=false;
        bool secondTimeExcute=false;

        while (true)
        {
            yield return null;

            time -= Time.deltaTime;

            if (time <= 2)
            {
                if (!firstTimeExcute)
                {
                    firstTime();
                    firstTimeExcute = true;
                }
            }

            if (time <= 1)
            {
                if (!secondTimeExcute)
                {
                    SecondTime();
                    secondTimeExcute = true;
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
    /// 获取随机五张手牌并存储起来 游戏开始
    /// </summary>
    public void GetCardToBattle()
    {
        playerRoundCardList=GetRandomCard(playerCardsList);

        for (int i = 0; i < playerRoundCardList.Count; i++)
        {
            if (playerRoundCardDic.ContainsKey(playerRoundCardList[i].id))
            {
                playerRoundCardDic[playerRoundCardList[i].id].Add(playerRoundCardList[i]);
            }else
            {
                playerRoundCardDic.Add(playerRoundCardList[i].id, new List<CardEntity>() { playerRoundCardList[i] });
            }
        }

        userPlayer = new PlayerBase();
        aiPlayer = new PlayerBase();
    }

    /// <summary>
    /// 获得随机五张手牌
    /// </summary>
    /// <param name="playerCards"></param>
    /// <returns></returns>
    private List<CardEntity> GetRandomCard(List<CardEntity> playerCards)
    {
        List<CardEntity> newPlayerCardList = playerCards;

        for (int i = 0; i < newPlayerCardList.Count; i++)
        {
            int ran = UnityEngine.Random.Range(0, newPlayerCardList.Count);
            CardEntity temp;

            temp = newPlayerCardList[i];
            newPlayerCardList[i] = playerCards[ran];
            newPlayerCardList[ran] = temp;
        }

        List<CardEntity> returnPlayerCardList = new List<CardEntity>();

        for (int i = 0; i < 5; i++)
        {
            newPlayerCardList.Remove(newPlayerCardList[i]);
            returnPlayerCardList.Add(newPlayerCardList[i]);
        }

        return returnPlayerCardList;
    }
    #endregion

    #region CardReadyToBattle 牌库
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

    public void ClearCardCollection()
    {
        playerCardsList.Clear();
        playerCradsDic.Clear();
    }

    #endregion


}
