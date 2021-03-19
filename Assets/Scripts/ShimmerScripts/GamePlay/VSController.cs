using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VSController : MonoBehaviour
{
    void Start()
    {
        //添加AI卡片到牌库
        BattleManager.GetInstance().AddAICardToBattle();

        //获取牌库中随机的五张卡牌
        BattleManager.GetInstance().PlayRandomGetCardToBattle();
        BattleManager.GetInstance().AiRandomGetCardToBattle();

    }

    void Update()
    {
        
    }
}
