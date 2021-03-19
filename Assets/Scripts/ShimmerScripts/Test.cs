using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private List<int> tempList;
    void Start()
    {
        tempList = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

        GetRandomCard(tempList);
    }

    private void GetRandomCard(List<int> tempList)
    {
        for (int i = 0; i < tempList.Count; i++)
        {
            int temp = 0;
            int ran = Random.Range(0, tempList.Count);
            temp = tempList[i];
            tempList[i] = tempList[ran];
            tempList[ran] = temp;
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            Debug.Log(tempList[i]);
        }
    }

}
