using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButtton : MonoBehaviour
{
    private int index;
    private StartController startController;

    private void Start()
    {
        index = int.Parse(gameObject.name.Substring(6, 1));
        startController = GameObject.Find("UI Root").GetComponent<StartController>();
    }

    private void OnMouseEnter()
    {

        Debug.Log("当前鼠标指针进入啦");
    }
}
