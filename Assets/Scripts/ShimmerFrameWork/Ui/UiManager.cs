using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ShimmerFramework
{
    public enum Panel_Type
    {
        Bot,
        Mid,
        Top,
        System
    }

    public class UiManager : BaseManager<UiManager>
    {

        private Dictionary<string, GameObject> panelDic = new Dictionary<string, GameObject>();

        private Transform bot;
        private Transform mid;
        private Transform top;
        private Transform system;

        public void ShowPanel<T>(string panelName, Panel_Type panelType, UnityAction<T> callBack = null) where T : BasePanel
        {
            Transform mainPanel = GameObject.FindGameObjectWithTag("MainPanel").transform;

            bot = mainPanel.Find("Bot");
            mid = mainPanel.Find("Mid");
            top = mainPanel.Find("Top");
            system = mainPanel.Find("System");

            if (panelDic.ContainsKey(panelName))
            {
                callBack(panelDic[panelName] as T);
            }
            ResourcesManager.GetInstance().LoadAssetAsync<GameObject>(
#if Addressable
                panelName, 
#else
                "Ui/" + panelName,
#endif
            (uiPanel) =>
            {
                Transform father = bot;

                switch (panelType)
                {
                    case Panel_Type.Mid:
                        father = mid;
                        break;
                    case Panel_Type.Top:
                        father = top;
                        break;
                    case Panel_Type.System:
                        father = system;
                        break;
                }

                uiPanel.transform.parent = father;
                uiPanel.transform.localPosition = Vector3.zero;
                uiPanel.transform.localScale = Vector3.one;

                (uiPanel.transform as RectTransform).offsetMax = Vector2.zero;
                (uiPanel.transform as RectTransform).offsetMin = Vector2.zero;

                T panelController = uiPanel as T;

                if (callBack != null)
                {
                    callBack(panelController);
                }

                panelDic.Add(panelName, uiPanel);
            }
            );
        }
        public void ShowPanel(string panelName, Panel_Type panelType)
        {
            Transform mainPanel = GameObject.FindGameObjectWithTag("MainPanel").transform;

            bot = mainPanel.Find("Bot");
            mid = mainPanel.Find("Mid");
            top = mainPanel.Find("Top");
            system = mainPanel.Find("System");


            ResourcesManager.GetInstance().LoadAssetAsync<GameObject>(
#if Addressable
                panelName, 
#else
                "Ui/" + panelName,
#endif
            (uiPanel) =>
            {

                Transform father = bot;

                switch (panelType)
                {
                    case Panel_Type.Mid:
                        father = mid;
                        break;
                    case Panel_Type.Top:
                        father = top;
                        break;
                    case Panel_Type.System:
                        father = system;
                        break;
                }

                uiPanel.GetComponent<Transform>().parent = father;
                uiPanel.transform.localPosition = Vector3.zero;
                uiPanel.transform.localScale = Vector3.one;

                (uiPanel.transform as RectTransform).offsetMax = Vector2.zero;
                (uiPanel.transform as RectTransform).offsetMin = Vector2.zero;

                panelDic.Add(panelName, uiPanel);
            }
            );
        }

        public void ShowPanelSmooth(string panelName, Panel_Type panelType, Vector2 localPos, Vector2 targePos, float tweenTime)
        {
            Transform mainPanel = GameObject.FindGameObjectWithTag("MainPanel").transform;

            bot = mainPanel.Find("Bot");
            mid = mainPanel.Find("Mid");
            top = mainPanel.Find("Top");
            system = mainPanel.Find("System");

            ResourcesManager.GetInstance().LoadAssetAsync<GameObject>(
#if Addressable
                panelName, 
#else
                "Ui/" + panelName,
#endif
            (uiPanel) =>
            {
                Transform father = bot;

                switch (panelType)
                {
                    case Panel_Type.Mid:
                        father = mid;
                        break;
                    case Panel_Type.Top:
                        father = top;
                        break;
                    case Panel_Type.System:
                        father = system;
                        break;
                }

                uiPanel.transform.parent = father;
                uiPanel.transform.localPosition = Vector3.zero;
                uiPanel.transform.localScale = Vector3.one;

                (uiPanel.transform as RectTransform).offsetMax = Vector2.zero;
                (uiPanel.transform as RectTransform).offsetMin = Vector2.zero;

                GameObject activeUi = uiPanel as GameObject;

                activeUi.transform.localScale = new Vector3(0, 0, 0);
                activeUi.transform.localPosition = localPos;
                activeUi.transform.DOLocalMove(targePos, tweenTime);
                activeUi.transform.DOScale(1, tweenTime);

                panelDic.Add(panelName, uiPanel);
            }
            );
        }

        public void HidePanel(string panelName)
        {

            if (panelDic.ContainsKey(panelName))
            {
                GameObject.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }
        }

        public void HidePanel<T>(string panelName, float tweenTime, UnityAction<T> callBack = null) where T : Object
        {
            if (panelDic.ContainsKey(panelName))
            {
                MonoManager.GetInstance().StartCoroutine(ReallyDestory(panelName, tweenTime));
                callBack(panelDic[panelName] as T);
            }
        }

        public void SmoothHidePanel(string panelName, Vector2 localPos, float tweenTime)
        {
            if (panelDic.ContainsKey(panelName))
            {
                panelDic[panelName].transform.DOScale(0, tweenTime);
                panelDic[panelName].transform.DOLocalMove(localPos, tweenTime);

                MonoManager.GetInstance().StartCoroutine(ReallyDestory(panelName, tweenTime));
            }
        }

        private IEnumerator ReallyDestory(string panelName, float tweenTime)
        {
            yield return new WaitForSeconds(tweenTime);
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);

        }

        /// <summary>
        /// 为组件添加监听事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventTriggerType"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void AddCustomEventTrigger(Component obj, EventTriggerType eventTriggerType, UnityAction<BaseEventData> callback)
        {
            //添加EventTrigger组件
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = obj.gameObject.AddComponent<EventTrigger>();
            }

            //获取事件列表
            List<EventTrigger.Entry> entries = trigger.triggers;
            if (entries == null)
            {
                entries = new List<EventTrigger.Entry>();
            }


            //获取对应事件
            EventTrigger.Entry entry = new EventTrigger.Entry();

            bool isExist = false;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].eventID == eventTriggerType)
                {
                    entry = entries[i];
                    isExist = true;
                }
            }

            entry.eventID = eventTriggerType;
            entry.callback.AddListener(callback);



            if (!isExist)
            {
                trigger.triggers.Add(entry);
            }
        }
    }
}