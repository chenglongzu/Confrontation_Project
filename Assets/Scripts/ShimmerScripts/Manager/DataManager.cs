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
    }

    public CardEntity GetCardEntityByIndex(int index)
    {
        if (index >= cardEntitiesList.Count) return null;

        return cardEntitiesList[index];
    }

    public CardEntity GetCardEntityById(int id)
    {
        CardEntity cardEntity = null; ;
        if (cardEntitiesDic.TryGetValue(id,out cardEntity))
        {
            return cardEntity;
        }
        return null;
    }

    public string GetIntroduceContent(int index)
    {
        return levelIntroduce[index];
    }    
}
