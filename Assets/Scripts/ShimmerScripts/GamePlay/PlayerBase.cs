using System;
using UnityEngine;


public class PlayerBase : MonoBehaviour
{
    public int lifeValue;

    public bool isAttacked { get; protected set; }     //是否选择了卡牌

    public bool isCardUsed;     //当前卡牌是否有用

    public CardEntity currtenCard;

    /// <summary>
    /// 开始一个新的回合，重置是否出牌占位符
    /// </summary>
    public void StartRound()
    {
        isAttacked = false;
    }

    /// <summary>
    /// 出牌
    /// </summary>
    /// <param name="currtenCard"></param>
    public void SetCurrtenCard(CardEntity currtenCard)
    {
        isAttacked = true;
        this.currtenCard = currtenCard;
    }

    /// <summary>
    /// 我方卡牌进攻 在最终结算时调用
    /// </summary>
    /// <param name="enemy">敌方角色对象</param>
    /// <param name="index">当前卡片的序号</param>
    public void Attack(PlayerBase enemy)
    {
        enemy.BeAttacked(currtenCard, this);

        switch (currtenCard.id)
        {
            case 1:
                //无特殊情况 正常发动伤害  （已完成逻辑）
                break;
            case 2:
                //无特殊情况 正常发动伤害  （已完成逻辑）
                break;
            case 3:
                //抽1张牌，格挡对手本次造成的伤害  （抽牌逻辑未完成）
                break;
            case 4:
                //抽3张牌   （抽牌逻辑未完成）
                break;
            case 5:
                ExtraHurtValue(2);
                //造成3点伤害，对自身造成2点伤害  （已完成逻辑）
                break;
            case 6:
                //若本次未受到伤害，恢复满生命    （已完成逻辑）
                break;
            case 7:
                //造成1点不可被格挡的伤害  （已完成逻辑，在每次格挡卡片中调用）
                break;
            case 8:
                //格挡对手本次造成的伤害，并返还这次伤害，该返还伤害减1   （已完成逻辑）
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 被敌方卡牌攻击
    /// </summary>
    /// <param name="emeryCard">敌方卡片</param>
    /// <param name="index">敌方卡牌的序号</param>
    /// <param name="callBack">回调函数传出是否造成伤害</param>
    public void BeAttacked(CardEntity emeryCard, PlayerBase enemy, Action callBack=null)
    {
        //我方卡牌是否生效
        if (isCardUsed)
        {
            //我方卡牌和对方卡牌可能产生的一系列反应
            switch (currtenCard.id)
            {
                case 1:
                    //无特殊情况 正常受到伤害  （已完成逻辑）
                    break;
                case 2:
                    //无特殊情况 正常受到伤害  （已完成逻辑）
                    break;
                case 3:
                    if (emeryCard.id != 7)
                    {
                        this.lifeValue -= 0;
                    }
                    else
                    {
                        this.lifeValue -= emeryCard.hurtValue;
                    }
                    return;
                //抽1张牌，格挡对手本次造成的伤害 （抽牌逻辑未完成）
                case 4:
                    //抽3张牌 （抽牌逻辑未完成）
                    break;
                case 5:
                    //造成3点伤害，对自身造成2点伤害  （已完成逻辑）
                    break;
                case 6:
                    if (emeryCard.hurtValue == 0)
                    {
                        lifeValue = DataManager.GetInstance().maxLifeValue;
                    }
                    //若本次未受到伤害，恢复满生命    （已完成逻辑）
                    break;
                case 7:
                    //造成1点不可被格挡的伤害  （已完成逻辑，在每次格挡卡片中调用）
                    break;
                case 8:
                    if (emeryCard.id != 7)
                    {
                        this.lifeValue -= 0;
                    }
                    else
                    {
                        this.lifeValue -= emeryCard.hurtValue;
                        return;
                    }

                    enemy.ExtraHurtValue(emeryCard.hurtValue - 1);
                    return;
                //格挡对手本次造成的伤害，并返还这次伤害，该返还伤害减1   (已完成逻辑)
                default:
                    break;
            }
        }

        //判断敌方卡牌对我方的影响
        switch (emeryCard.id)
        {
            case 1:
                this.lifeValue -= emeryCard.hurtValue;  //（已完成逻辑正常收到伤害）
                break;
            case 2:
                this.lifeValue -= emeryCard.hurtValue;  //（已完成逻辑正常受到伤害）
                break;
            case 3:
                //抽1张牌，格挡对手本次造成的伤害 伤害为0 （抽牌逻辑未完成）
                break;
            case 4:
                //抽3张牌 伤害为0 （抽牌逻辑未完成）
                break;
            case 5:
                this.lifeValue -= emeryCard.hurtValue;  //造成3点伤害，对自身造成2点伤害（已完成逻辑，另外在对手逻辑中）
                break;
            case 6:
                //若本次未受到伤害，恢复满生命    （已完成逻辑，在持有卡片逻辑中）
                break;
            case 7:
                this.lifeValue -= emeryCard.hurtValue;
                //造成1点不可被格挡的伤害  （已完成逻辑，在每次格挡卡片中调用）
                break;
            case 8:
                //格挡对手本次造成的伤害，并返还这次伤害，该返还伤害减1   （已完成逻辑，在持有卡片中）
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 造成的额外伤害
    /// </summary>
    /// <param name="hurtValue"></param>
    public void ExtraHurtValue(int hurtValue)
    {
        this.lifeValue -= hurtValue;
    }
}
