using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hero1 : MonoBehaviour
{
    public int maxhp = 5;
    public int minhp = 0;
    private UISprite hp;
    private UILabel hpLabel;
    private int hpCount = 5;


   
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            TakeDamage(Random.Range(1, 5));

        }
        if (Input.GetKey(KeyCode.R))
        {
          PlusHP(Random.Range(1, 5));

        }
    }

    public void TakeDamage(int damage)//吸收伤害的值
    {
        hpCount -= damage;
        hpLabel.text = hpCount + "";
        if(hpCount <= minhp)//游戏结束逻辑
        {

        }

    }
    public void PlusHP(int hp)
    {
        hpCount += hp;
        if(hpCount >= maxhp)
        {
            hpCount = maxhp;

        }

        hpLabel.text = hpCount + "";
    }








}
