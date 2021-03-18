using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShimmerFramework
{
    public class TipManager : BaseManager<TipManager>
    {
        //展示一个提示
        public void ShowOneTip(string head, string content, UnityAction actionTwo, UnityAction actionOne = null)
        {
#if Addressable
            ResourcesManager.GetInstance().LoadAssetAsync<GameObject>("Ui/Tip/Tip",(obj)=> {
                GameObject Tip = obj;

                Tip.transform.Find("Background/TipImage/Head").GetComponent<Text>().text = head;
                Tip.transform.Find("Background/TipImage/Content").GetComponent<Text>().text = content;

                if (actionOne != null)
                {
                    Tip.transform.Find("Background/TipImage/Button_1").GetComponent<Button>().onClick.AddListener(actionOne);
                }
                else
                {
                    Tip.transform.Find("Background/TipImage/Button_1").GetComponent<Button>().interactable = false;
                }


                Tip.transform.Find("Background/TipImage/Button_2").GetComponent<Button>().onClick.AddListener(actionTwo);

            });

#else
            GameObject Tip = ResourcesManager.GetInstance().LoadAsset<GameObject>("Ui/Tip/Tip");

            Tip.transform.Find("Background/TipImage/Head").GetComponent<Text>().text = head;
            Tip.transform.Find("Background/TipImage/Content").GetComponent<Text>().text = content;

            if (actionOne != null)
            {
                Tip.transform.Find("Background/TipImage/Button_1").GetComponent<Button>().onClick.AddListener(actionOne);
            }
            else
            {
                Tip.transform.Find("Background/TipImage/Button_1").GetComponent<Button>().interactable = false;
            }


            Tip.transform.Find("Background/TipImage/Button_2").GetComponent<Button>().onClick.AddListener(actionTwo);

#endif

        }
    }
}