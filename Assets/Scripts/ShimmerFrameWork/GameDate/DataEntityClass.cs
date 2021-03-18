using System;
using System.Collections.Generic;

namespace ShimmerFramework
{
    /// <summary>
    /// 数据实体类集合
    /// </summary>
    public class DataEntityClass
    {
    }

    /// <summary>
    /// 玩家的数据实体类
    /// </summary>
    public class PlayerDate
    {
        public int id;
        public string name;
        public string passward;

        //构造方法
        public PlayerDate()
        {
            this.id = 1;

            this.name = "龍城";
            this.passward = "123456";
        }


        #region 修改玩家信息

        //修改自己的名称
        public void ChangeName(string name)
        {
            this.name = name;
        }

        //修改密码
        public void ChangePassward(string passward)
        {
            this.passward = passward;
        }
        #endregion

    }

    /// <summary>
    /// 设置数据实体类
    /// </summary>
    public class SettingDate
    {
        public float audioVolume;
        public float musicVolume;

        public bool isAudioMute;
        public bool isMusicMute;

        public SettingDate()
        {
            audioVolume = 1;
            musicVolume = 1;

            isAudioMute = false;
            isMusicMute = false;
        }

        #region 修改设置选项

        public void ChangeGameAudio(float Value)
        {
            audioVolume = Value;
            AudioManager.GetInstance().ChangeAudioVolume(Value);

        }
        public void ChangeGameMusic(float Value)
        {
            musicVolume = Value;
            AudioManager.GetInstance().ChangeMusicVolume(Value);
        }

        public void ChangeMute(int index, bool isMute)
        {
            switch (index)
            {
                case 0:
                    if (isMute)
                    {
                        AudioManager.GetInstance().ChangeAudioVolume(0);
                        AudioManager.GetInstance().ChangeMusicVolume(0);
                    }
                    else
                    {
                        AudioManager.GetInstance().ChangeAudioVolume(GameModelManager.GetInstance().settingDate.audioVolume);
                        AudioManager.GetInstance().ChangeMusicVolume(GameModelManager.GetInstance().settingDate.musicVolume);
                    }

                    break;
                case 1:
                    if (isMute)
                    {
                        AudioManager.GetInstance().ChangeMusicVolume(0);
                    }
                    else
                    {
                        AudioManager.GetInstance().ChangeMusicVolume(GameModelManager.GetInstance().settingDate.musicVolume);
                    }

                    this.isMusicMute = isMute;

                    break;
                case 2:
                    if (isMute)
                    {
                        AudioManager.GetInstance().ChangeAudioVolume(0);
                    }
                    else
                    {
                        AudioManager.GetInstance().ChangeAudioVolume(GameModelManager.GetInstance().settingDate.audioVolume);
                    }

                    this.isAudioMute = isMute;

                    break;

                default:
                    break;
            }

        }

        public void ChangeAudioMute(bool value)
        {
            isAudioMute = value;

        }
        public void ChangeMusicMute(bool value)
        {
            isMusicMute = value;


        }
        #endregion
    }


    //道具数据实体类
    [Serializable]
    public class Item : ContentBase
    {
        //数据实体类中的内容与Json文件中的内容保持一致
        public int id;
        public string name;

    }

    //背包道具数据结构
    [Serializable]
    public class Items
    {
        public List<Item> info;
    }



    //商城中的商品基本信息
    [Serializable]
    public class ShopItem : ContentBase
    {
        //数据实体类中的内容与Json文件中的内容保持一致
    }

    //临时的存储商品信息的数据结构
    [Serializable]
    public class Shops
    {
        public List<ShopItem> info;
    }

}
