using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShimmerFramework
{
    public class BasePanel : MonoBehaviour
    {

        private Dictionary<string, List<UIBehaviour>> uiControllerDic = new Dictionary<string, List<UIBehaviour>>();

        public virtual void Start()
        {
            GetChildrenControl<Button>();
            GetChildrenControl<Image>();
            GetChildrenControl<Text>();
            GetChildrenControl<Slider>();
            GetChildrenControl<Toggle>();
            GetChildrenControl<Dropdown>();
            GetChildrenControl<InputField>();
        }

        protected void GetChildrenControl<T>() where T : UIBehaviour
        {
            T[] uiComponent = this.GetComponentsInChildren<T>();
            string objName;
            for (int i = 0; i < uiComponent.Length; i++)
            {
                objName = uiComponent[i].gameObject.name;
                if (uiControllerDic.ContainsKey(objName))
                {
                    uiControllerDic[objName].Add(uiComponent[i]);
                }
                else
                {
                    uiControllerDic.Add(objName, new List<UIBehaviour>() { uiComponent[i] });
                }

            }

        }

        protected T GetUiController<T>(string name) where T : UIBehaviour
        {
            foreach (var item in uiControllerDic)
            {
                if (name == item.Key)
                {
                    for (int i = 0; i < uiControllerDic[name].Count; i++)
                    {
                        if (uiControllerDic[name][i] is T)
                        {
                            return uiControllerDic[name][i] as T;
                        }
                    }

                }
            }

            Debug.LogError("Framework Cannot Get Ui Component");
            return null;
        }

    }
}