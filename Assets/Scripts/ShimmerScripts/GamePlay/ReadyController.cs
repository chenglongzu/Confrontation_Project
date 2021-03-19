using ShimmerSqlite;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyController : MonoBehaviour
{
    private UIButton[] cardButtons;
    private UISprite[] cardSprites;

    private UILabel usedPoint;

    private UILabel cartCount;
    void Start()
    {
        cardButtons = transform.Find("Background/CardCollection").GetComponentsInChildren<UIButton>();
        cardSprites = transform.Find("CheckCardPanel/Grid").GetComponentsInChildren<UISprite>();

        usedPoint = transform.Find("ResourcesPoint/UsedPoint").GetComponent<UILabel>();
        cartCount = transform.Find("CardCountLable/CardConut").GetComponent<UILabel>();

        //绑定事件，动态添加卡牌并存储到数据库
        CardButtonAddListen();
    }

    /// <summary>
    /// 动态查找卡牌并绑定监听事件
    /// </summary>
    private void CardButtonAddListen()
    {
        usedPoint.text = BattleManager.GetInstance().GetButtleCardResourcesPoint().ToString();
        cartCount.text = BattleManager.GetInstance().GetButtleCardLength().ToString();

        //点击卡牌开始重置
        UIEventListener.Get(transform.Find("ReChoose/ReChooseCard").gameObject).onClick = (value) => {
            BattleManager.GetInstance().ClearCardCollection();
            BattleManager.GetInstance().StoreMatchCardCardToDataBase();

            usedPoint.text = BattleManager.GetInstance().GetButtleCardResourcesPoint().ToString();
            cartCount.text = BattleManager.GetInstance().GetButtleCardLength().ToString();
        };

        //点击确定开始战斗
        UIEventListener.Get(transform.Find("OK").gameObject).onClick = (value) => {
            BattleManager.GetInstance().StoreMatchCardCardToDataBase();
            SceneManager.LoadScene("VS");
        };

        //点击查看卡组
        UIEventListener.Get(transform.Find("Player").gameObject).onClick = (value) => {
            transform.Find("CheckCardPanel").gameObject.SetActive(true);
            CheackChoosedCard();
        };

        UIButton OKButton = transform.Find("OK").gameObject.GetComponent<UIButton>();

        if (BattleManager.GetInstance().GetButtleCardLength()<=5)
        {
            //在选择五张卡牌一下的情况下无法开始游戏
            OKButton.state = UIButton.State.Disabled;

            OKButton.gameObject.GetComponent<BoxCollider>().enabled = false;
        }

        //动态查找卡片并添加事件绑定
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int cardIndex = int.Parse(cardButtons[i].gameObject.name.Substring(5, 1));

            if (!SqliteInitManager.GetInstance().CheackExitData("OwnCard", cardIndex))
            {
                cardButtons[i].gameObject.SetActive(false);
            }

            UIEventListener.Get(cardButtons[i].gameObject).onClick = (value) => {

                //资源点在最大限制以下
                if (!BattleManager.GetInstance().IfResourcesPointMaxed(DataManager.GetInstance().GetCardEntityById(cardIndex).resourcesPoint))
                {
                    BattleManager.GetInstance().AddCardToBattle(DataManager.GetInstance().GetCardEntityById(cardIndex));
                }

                usedPoint.text = BattleManager.GetInstance().GetButtleCardResourcesPoint().ToString();
                cartCount.text = BattleManager.GetInstance().GetButtleCardLength().ToString();

                //最少在选择五张卡牌的情况下 确定按钮可以正常点击
                if (BattleManager.GetInstance().GetButtleCardLength() >= 5)
                {
                    OKButton.state = UIButton.State.Normal;

                    OKButton.gameObject.GetComponent<BoxCollider>().enabled = true;
                }
            };
        }
    }

    /// <summary>
    ///  检查卡组界面
    /// </summary>
    public void CheackChoosedCard()
    {
        UIEventListener.Get(transform.Find("CheckCardPanel/ExitButton").gameObject).onClick = (value) => {
            transform.Find("CheckCardPanel").gameObject.SetActive(false);
        };

        for (int i = 0; i < cardSprites.Length; i++)
        {
            cardSprites[i].gameObject.SetActive(false);
        }

        List<CardEntity> AllButtleCard = BattleManager.GetInstance().GetAllButtleCard();

        for (int i = 0; i < AllButtleCard.Count; i++)
        {
            Debug.Log("当前可能超出边界的："+i);
            cardSprites[i].gameObject.SetActive(true);
            cardSprites[i].spriteName= "Card_"+ AllButtleCard[i].id;
        }

        BattleManager.GetInstance().StoreMatchCardCardToDataBase();
    }
}
