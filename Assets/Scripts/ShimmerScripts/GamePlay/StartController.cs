using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ShimmerSqlite;

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
            SceneManager.LoadSceneAsync("ChooseCard");
        };

        UIEventListener.Get(transform.Find("MapCharacter/CharacterToGame").gameObject).onClick = (value) => {
            SceneManager.LoadSceneAsync("VS");
        };

        //将数据库中的数据写入到内存当中
        BattleManager.GetInstance().GetCardDataFromDataBase();

        transform.Find("ChooseCard/CardCount").GetComponent<UILabel>().text = BattleManager.GetInstance().GetButtleCardLength().ToString();
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
            }
        }
    }

    public void ChangeIntroduceText(int id)
    {
        introduceLable.text = DataManager.GetInstance().GetIntroduceContent(id);
    }
}
