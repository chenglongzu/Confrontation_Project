using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSController : MonoBehaviour
{
    private UISprite[] playerHealthSprite;
    private UISprite[] aiHealthSprite;

    void Start()
    {
        //添加AI卡片到牌库
        BattleManager.GetInstance().AddAICardToBattle();

        //获取牌库中随机的五张卡牌
        BattleManager.GetInstance().PlayRandomGetCardToBattle();
        BattleManager.GetInstance().AiRandomGetCardToBattle();

        playerHealthSprite = transform.Find("BittleGround/PlayerCardGroup/PlayerHealth").GetComponentsInChildren<UISprite>();
        aiHealthSprite = transform.Find("BittleGround/AICardGroup/AIHealth").GetComponentsInChildren<UISprite>();
    }

}
