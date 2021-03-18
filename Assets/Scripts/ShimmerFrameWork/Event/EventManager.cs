using System.Collections.Generic;
using UnityEngine.Events;

namespace ShimmerFramework
{
    public enum EventType
    {
        None_Parameter,
        One_Parameter,
        Two_Parameter,
        Three_Parameter

    }

    public class EventManager : BaseManager<EventManager>
    {
        private Dictionary<string, UnityAction> actionDic_1 = new Dictionary<string, UnityAction>();
        private Dictionary<string, UnityAction<object>> actionDic_2 = new Dictionary<string, UnityAction<object>>();
        private Dictionary<string, UnityAction<object, object>> actionDic_3 = new Dictionary<string, UnityAction<object, object>>();
        private Dictionary<string, UnityAction<object, object, object>> actionDic_4 = new Dictionary<string, UnityAction<object, object, object>>();

        //用于初始化声明所有的委托事件
        //初始化委托事件的集合
        //外部调用清除事件时不会清除当前数据结构中的事件
        private List<UnityAction> eventInitList = new List<UnityAction>() { };

        #region 初始化事件
        public void AddEventInit(UnityAction action)
        {
            eventInitList.Add(action);
        }

        public void EventInitTrigger()
        {
            for (int i = 0; i < eventInitList.Count; i++)
            {
                eventInitList[i]();
            }
        }
        #endregion

        #region 添加委托事件
        public void AddAction(string name, UnityAction action)
        {
            if (actionDic_1.ContainsKey(name))
            {
                actionDic_1[name] += action;
            }
            else
            {
                actionDic_1.Add(name, action);
            }
        }
        public void AddAction(string name, UnityAction<object> action)
        {
            if (actionDic_2.ContainsKey(name))
            {
                actionDic_2[name] += action;
            }
            else
            {
                actionDic_2.Add(name, action);
            }
        }
        public void AddAction(string name, UnityAction<object, object> action)
        {
            if (actionDic_3.ContainsKey(name))
            {
                actionDic_3[name] += action;
            }
            else
            {
                actionDic_3.Add(name, action);
            }
        }
        public void AddAction(string name, UnityAction<object, object, object> action)
        {
            if (actionDic_4.ContainsKey(name))
            {
                actionDic_4[name] += action;
            }
            else
            {
                actionDic_4.Add(name, action);
            }
        }

        #endregion

        #region 移除委托事件
        /// <summary>
        /// 移除当前名称下的所有委托
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAction(string name)
        {
            if (actionDic_1.ContainsKey(name))
            {
                actionDic_1[name] = null;
            }

            if (actionDic_2.ContainsKey(name))
            {
                actionDic_2[name] = null;
            }

            if (actionDic_3.ContainsKey(name))
            {
                actionDic_3[name] = null;
            }


            if (actionDic_4.ContainsKey(name))
            {
                actionDic_4[name] = null;
            }

        }

        /// <summary>
        /// 通过类型移除委托
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eventType"></param>
        public void RemoveAction(string name, EventType eventType)
        {
            switch (eventType)
            {
                case EventType.None_Parameter:
                    if (actionDic_1.ContainsKey(name))
                    {
                        actionDic_1[name] = null;
                    }
                    break;
                case EventType.One_Parameter:
                    if (actionDic_2.ContainsKey(name))
                    {
                        actionDic_2[name] = null;
                    }
                    break;
                case EventType.Two_Parameter:
                    if (actionDic_3.ContainsKey(name))
                    {
                        actionDic_3[name] = null;
                    }
                    break;
                case EventType.Three_Parameter:
                    if (actionDic_4.ContainsKey(name))
                    {
                        actionDic_4[name] = null;
                    }
                    break;
            }
        }
        #endregion

        #region 移除单个委托事件
        public void RemoveSingleAction(string name, UnityAction action)
        {
            if (actionDic_1.ContainsKey(name))
            {
                actionDic_1[name] -= action;
            }
        }
        public void RemoveSingleAction(string name, UnityAction<object> action)
        {
            if (actionDic_2.ContainsKey(name))
            {
                actionDic_2[name] -= action;
            }
        }
        public void RemoveSingleAction(string name, UnityAction<object, object> action)
        {
            if (actionDic_3.ContainsKey(name))
            {
                actionDic_3[name] -= action;
            }
        }
        public void RemoveSingleAction(string name, UnityAction<object, object, object> action)
        {
            if (actionDic_4.ContainsKey(name))
            {
                actionDic_4[name] -= action;
            }
        }

        #endregion

        #region 触发委托事件
        public void ActionTrigger(string name)
        {
            if (actionDic_1.ContainsKey(name))
            {
                actionDic_1[name]();
            }
        }
        public void ActionTrigger(string name, object info)
        {
            if (actionDic_2.ContainsKey(name))
            {
                actionDic_2[name](info);
            }
        }
        public void ActionTrigger(string name, object info_1, object info_2)
        {
            if (actionDic_3.ContainsKey(name))
            {
                actionDic_3[name](info_1, info_2);
            }
        }
        public void ActionTrigger(string name, object info_1, object info_2, object info_3)
        {
            if (actionDic_4.ContainsKey(name))
            {
                actionDic_4[name](info_1, info_2, info_3);
            }
        }

        #endregion

        #region 清除委托事件
        public void ClearAction()
        {
            actionDic_1.Clear();
            actionDic_2.Clear();
            actionDic_3.Clear();
            actionDic_4.Clear();

        }
        #endregion
    }

}
