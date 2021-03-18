using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace ShimmerFramework
{
    /// <summary>
    /// 玩家信息类型
    /// </summary>
    public class GameModelManager : BaseManager<GameModelManager>
    {
        public PlayerDate playerDate;
        public SettingDate settingDate;

        //设置和玩家数据路径
        private string PlayerDate_Url = Application.persistentDataPath + "/PlayerJsonDate.txt";
        private string SettingDate_Url = Application.persistentDataPath + "/SettingJsonDate.txt";

        //json文件的位置，游戏和商城中所有可能存在的道具基本信息
        private string knapsackJsonPath = "Json/KnapsackJsonDate";
        private string shopJsonPath = "Json/ShopJsonDate";


        //背包道具信息的集合
        private Dictionary<int, Item> propInfos = new Dictionary<int, Item>();
        //商品信息集合
        public List<ShopItem> goodsInfos = new List<ShopItem>();

        //数据初始化
        public void Init()
        {

#if PersistentDate
            //玩家背包和商店数据预先写好放在指定路径下，然后程序读取
            #region 玩家背包和商店数据

        //加载背包json文件,并解析背包中所有可能存在的资源基本信息到内存中
        string knapsackInfo = ResourcesManager.GetInstance().LoadAsset<TextAsset>(knapsackJsonPath).text;

        //这里加的前缀需要和数据实体类中的数据结构的名称相同
        string newknapsackInfo = "{ \"info\": " + knapsackInfo + "}";

        //临时转换为list数据结构
        Items items = JsonUtility.FromJson<Items>(newknapsackInfo);
        //List转换为Dictionary数据结构
        for (int i = 0; i < items.info.Count; i++)
        {
            propInfos.Add((int)items.info[i].id, items.info[i]);
        }


        //功能同上 只是用JsonManager管理器类来解析数据
        //propInfos = JsonManager.GetInstance().ReadJson<int, Item>(knapsackJsonPath);


        //加载商品json文件,并解析商品中所有可能存在的资源基本信息到内存中
        string shopInfo = ResourcesManager.GetInstance().LoadAsset<TextAsset>(shopJsonPath).text;

        string newShopInfo = "{ \"info\": " + shopInfo + "}";

        Shops shopItems = JsonUtility.FromJson<Shops>(newShopInfo);

        goodsInfos = shopItems.info;

        //功能同上使用JsonManager管理器类解析
        //goodsInfos = JsonManager.GetInstance().ReadJson<ShopItem>(shopJsonPath);
            #endregion
#endif

            #region 玩家和设置数据
            //如果路径中存在玩家信息文件
            if (File.Exists(PlayerDate_Url))
            {
                //从其他路径里加载字符串的顺序
                //首先先读取到文件中的字节数组
                //然后通过Encoding类中的UTF8静态变量转换获取字符串
                byte[] bytes = File.ReadAllBytes(PlayerDate_Url);

                string json = Encoding.UTF8.GetString(bytes);

                playerDate = JsonUtility.FromJson<PlayerDate>(json);

                SavePlayerInfo();
            }
            else
            {
                //如果不存在玩家信息则初始化一个默认的角色信息写入文件
                playerDate = new PlayerDate();

                SavePlayerInfo();
            }

            //如果路径中存在设置信息文件
            if (File.Exists(SettingDate_Url))
            {
                byte[] bytes = File.ReadAllBytes(SettingDate_Url);
                string json = Encoding.UTF8.GetString(bytes);

                settingDate = JsonUtility.FromJson<SettingDate>(json);

                SaveSettingDate();

            }
            else
            {
                settingDate = new SettingDate();

                SaveSettingDate();
            }
            #endregion

            //Debug.Log("Application.persistentDataPath" + Application.persistentDataPath);

        }

        #region 保存玩家信息，包括玩家基础信息和背包的信息 并且刷新最新的玩家信息

        public void SavePlayerInfo()
        {
            #region 初始化默认角色装备

            #endregion

            //观察者模式通知刷新角色数据
            EventManager.GetInstance().ActionTrigger("RefreshPlayerDate");

            //将json数据写入本地
            string json = JsonUtility.ToJson(playerDate);

            File.WriteAllBytes(PlayerDate_Url, Encoding.UTF8.GetBytes(json));
        }

        //保存设置信息 并且应用中设置最新的设置信息
        public void SaveSettingDate()
        {
            if (settingDate.isAudioMute)
            {
                AudioManager.GetInstance().ChangeAudioVolume(0);
            }
            else
            {
                AudioManager.GetInstance().ChangeAudioVolume(settingDate.audioVolume);
            }

            if (settingDate.isMusicMute)
            {
                AudioManager.GetInstance().ChangeMusicVolume(0);
            }
            else
            {
                AudioManager.GetInstance().ChangeMusicVolume(settingDate.musicVolume);
            }

            EventManager.GetInstance().ActionTrigger("RefreshSettingDate");

            //将json数据写入到本地
            string json = JsonUtility.ToJson(settingDate);

            File.WriteAllBytes(SettingDate_Url, Encoding.UTF8.GetBytes(json));
        }
        #endregion

        //获取背包物品
        public Item GetknapsackInfo(int id)
        {
            if (propInfos.ContainsKey(id))
            {
                return propInfos[id];
            }

            return null;
        }

        //获取商店物品
        public ShopItem GetShopItemInfo(int id)
        {
            if (goodsInfos.Count >= id)
            {
                return goodsInfos[id];
            }

            return null;
        }
    }


}




