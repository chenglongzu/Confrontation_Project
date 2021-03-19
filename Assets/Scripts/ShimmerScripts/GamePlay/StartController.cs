using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    private string buttonName;

    private UIButton[] levelButton;

    private UILabel introduceLable;
    void Start()
    {
        introduceLable = transform.Find("IntroduceInfo/IntroduceLabel").GetComponent<UILabel>();
        levelButton = transform.Find("MapCollection").GetComponentsInChildren<UIButton>();

        UIEventListener.Get(transform.Find("ChooseCard").gameObject).onClick = (value) => {
            SceneManager.LoadScene("ChooseCard");
        };
    }

    private void Update()
    {
        if (UICamera.isOverUI)
        {
            if (UICamera.hoveredObject.name.Contains("Level"))
            {
                if (UICamera.hoveredObject.name == buttonName) return;

                buttonName = UICamera.hoveredObject.name;
                ChangeIntroduceText(int.Parse(buttonName.Substring(6, 1)));

                Debug.Log(UICamera.hoveredObject.name);
            }
        }

    }

    public void ChangeIntroduceText(int id)
    {
        introduceLable.text = DataManager.GetInstance().GetIntroduceContent(id);
    }
}
