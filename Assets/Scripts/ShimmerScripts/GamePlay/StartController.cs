using UnityEngine;
using UnityEngine.SceneManagement;
using ShimmerFramework;

public class StartController : MonoBehaviour
{
    private string buttonName;

    private UIButton[] levelButton;

    private UILabel introduceLable;

    void Start()
    {
        introduceLable = transform.Find("IntroduceInfo/IntroduceLabel").GetComponent<UILabel>();
        levelButton = transform.Find("MapCollection").GetComponentsInChildren<UIButton>();

        int levelIndex = PlayerPrefs.GetInt("LevelIndex",1);

        for (int i = 0; i < levelButton.Length; i++)
        {
            if (i<levelIndex)
            {
                levelButton[i].gameObject.SetActive(true);
            }else
            {
                levelButton[i].gameObject.SetActive(false);
            }
        }

        //选择卡牌
        UIEventListener.Get(transform.Find("ChooseCard").gameObject).onClick = (value) => {
            SceneManager.LoadSceneAsync("ChooseCard");
        };

        //开始战斗
        UIEventListener.Get(transform.Find("MapCharacter/CharacterToGame").gameObject).onClick = (value) => {
            SceneManager.LoadSceneAsync("VS");
        };

        //将数据库中的数据写入到内存当中
        BattleManager.GetInstance().GetCardDataFromDataBase();

        //从数据库中获取当前卡牌的长度
        transform.Find("ChooseCard/CardCount").GetComponent<UILabel>().text = BattleManager.GetInstance().GetButtleCardLength().ToString();

        ChangeIntroduceText(levelIndex);
    }

    private void Update()
    {
        if (UICamera.isOverUI)
        {
            //如果当前鼠标在关卡按钮上时
            if (UICamera.hoveredObject.name.Contains("Level"))
            {
                if (UICamera.hoveredObject.name == buttonName) return;

                buttonName = UICamera.hoveredObject.name;
                ChangeIntroduceText(int.Parse(buttonName.Substring(6, 1)));
            }
        }
    }

    /// <summary>
    /// 设置介绍文本
    /// </summary>
    /// <param name="id"></param>
    public void ChangeIntroduceText(int id)
    {
        Debug.Log(id - 1);
        introduceLable.text = DataManager.GetInstance().GetIntroduceContent(id-1);

        GameManager.GetInstance().SetLevelIndex(id);
    }
}
