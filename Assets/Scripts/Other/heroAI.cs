using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroAI : MonoBehaviour
{
    public int maxhp = 5;
    public int minhp = 0;

    private UISprite hp;
    private UILabel hpLabel;
    private int hpCount = 5;

    void Awake()
    {
        hpLabel = this.transform.Find("hp").GetComponent<UILabel>();

    }
    public void TakeDamage(int damage)//吸收伤害的值
    {
        hpCount -= damage;
        hpLabel.text = hpCount + "";
        if (hpCount <= minhp)//游戏结束逻辑
        {

        }

    }
    public void PlusHP(int hp)
    {
        hpCount += hp;
        if (hpCount >= maxhp)
        {
            hpCount = maxhp;

        }

        hpLabel.text = hpCount + "";
    }
}
