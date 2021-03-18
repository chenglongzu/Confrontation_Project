using System;
using UnityEngine;

namespace ShimmerFramework
{
    public class SystemManager : BaseManager<SystemManager>
    {
        /// <summary>
        /// 判断当前网络是否有连接wifi
        /// </summary>
        public void GetNetworkInfo()
        {
            //通过Application.internetReachability这个字段可以判断他当前是否有连接wifi，如果非手持设备，默认连接wifi的
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    Debug.Log("网络已断开，请重连");
                    //没有网络
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    Debug.Log("当前使用的是移动网络，请问是否更新");
                    //移动网络
                    break;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    Debug.Log("当前使用的是wifi网络，请问是否更新");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 获取和显示时间的方法
        /// </summary>
        private void GetTime()
        {
            Debug.Log("当前时间" + DateTime.Now.ToString("T"));
        }

        /// <summary>
        /// 获取并设置电量的显示
        /// </summary>
        private void GetBattery()
        {
            if (SystemInfo.batteryLevel != -1)
            {
                Debug.Log("当前电量值为" + SystemInfo.batteryLevel);
            }
            else
            {
                Debug.Log("无电量");
            }
        }
    }
}