using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hpnumber : MonoBehaviour
{
    public int maxNumber = 5;

    public UISprite[] HPnumber;

    private void Awake()
    {
        maxNumber = HPnumber.Length;

    }
    private void Update()
    {
        UpdateShow();

    }

    void UpdateShow()
    {
        int i = maxNumber;


        HPnumber[i].gameObject.SetActive(false);
    }
    
        
    }

















