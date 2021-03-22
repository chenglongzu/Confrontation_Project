using ShimmerSqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShimmerFramework;

public class DataManager : SingletonMono<DataManager>
{
    [SerializeField]
    public int maxResourcesPoint;
    [SerializeField]
    public int maxLifeValue;

    [SerializeField]
    private CardEntity[] cardEntities;

    [SerializeField]
    private string[] levelIntroduce;

    private List<CardEntity> cardEntitiesList;
    private Dictionary<int,CardEntity> cardEntitiesDic;

    //当前解锁的所有关卡
    public int LevelIndex;
    protected override void Awake()
    {
        base.Awake();

        cardEntitiesList = new List<CardEntity>();
        cardEntitiesDic = new Dictionary<int, CardEntity>();

        for (int i = 0; i < cardEntities.Length; i++)
        {
            cardEntitiesList.Add(cardEntities[i]);
            cardEntitiesDic.Add(cardEntities[i].id, cardEntities[i]);
        }

        //如果没获取到这个字段的话则为1
        LevelIndex = PlayerPrefs.GetInt("LevelIndex",1);
    }

    /// <summary>
    /// 通过序号获取从内存中获取一个卡牌实体
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public CardEntity GetCardEntityByIndex(int index)
    {
        if (index >= cardEntitiesList.Count) return null;

        return cardEntitiesList[index];
    }

    /// <summary>
    /// 通过实体的ID值获取一个卡牌的实体
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CardEntity GetCardEntityById(int id)
    {
        CardEntity cardEntity = null; ;

        if (cardEntitiesDic.TryGetValue(id,out cardEntity))
        {
            return cardEntity;
        }

        return null;
    }

    /// <summary>
    /// 获取关卡的介绍
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string GetIntroduceContent(int index)
    {
        return levelIntroduce[index];
    }    
}
