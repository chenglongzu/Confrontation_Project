using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    public GameObject cardPrefefab;
    public Transform fromcard;
    public Transform tocard;
    public string[]cardnames;

    public float transformTime = 2f;
    public int transformSpeed = 20;
    private bool isTransforming = false;



    private float timer = 0;

    private UISprite nowGenerateCard;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RandomGenerateCard();

        }
        if(isTransforming)
        {
            timer += Time.deltaTime;
            int index = (int)(timer / (1f / transformSpeed));
            index %= cardnames.Length;

            nowGenerateCard.spriteName  = cardnames[index];



            if (timer > transformTime)//变换结束
            {
                isTransforming = false;


            }
        }
    }

    public void RandomGenerateCard()
    {
        GameObject go = NGUITools.AddChild(this.gameObject, cardPrefefab);
        go.transform.position = fromcard.position;
        iTween.MoveTo(go, tocard.position, 1f);
        isTransforming = true;







    }






}
