using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyController : MonoBehaviour
{
    private UIButton[] cardButtons;

    private UILabel usedPoint;

    private UILabel cartCount;
    void Start()
    {
        cardButtons = transform.Find("Background/CardCollection").GetComponentsInChildren<UIButton>();

        usedPoint = transform.Find("ResourcesPoint/UsedPoint").GetComponent<UILabel>();
        cartCount = transform.Find("CardCountLable/CardConut").GetComponent<UILabel>();


        UIEventListener.Get(transform.Find("ReChoose").gameObject).onClick = (value) => {
            BattleManager.GetInstance().ClearCardCollection();

            usedPoint.text = BattleManager.GetInstance().GetButtleCardResourcesPoint().ToString();
            cartCount.text = BattleManager.GetInstance().GetButtleCardLength().ToString();
        };

        UIEventListener.Get(transform.Find("OK").gameObject).onClick = (value) => {
            SceneManager.LoadScene("VS");
        };

        for (int i = 0; i < cardButtons.Length; i++)
        {        
            UIEventListener.Get(cardButtons[i].gameObject).onClick = (value) => {
                int cardIndex= int.Parse(value.name.Substring(5, 1));

                //资源点在最大限制以下 且最多抽五张牌
                if (!BattleManager.GetInstance().IfResourcesPointMaxed(DataManager.GetInstance().GetCardEntityById(cardIndex).resourcesPoint))
                {
                    BattleManager.GetInstance().AddCardToBattle(DataManager.GetInstance().GetCardEntityById(cardIndex));
                }

                usedPoint.text = BattleManager.GetInstance().GetButtleCardResourcesPoint().ToString();
                cartCount.text = BattleManager.GetInstance().GetButtleCardLength().ToString();

            };
        }
    }
}
