using UnityEngine;
using ShimmerSqlite;
using System.Collections.Generic;

public class SqliteInitManager : MonoBehaviour
{
    [SerializeField]
    private string gameDataBase;

    [SerializeField]
    private string[] tableName;

    [SerializeField]
    private CardEntity[] cardEntities;

    void Start()
    {
        SqlManager.GetInstance().InitDataBase(gameDataBase);
        
        for (int i = 0; i < tableName.Length; i++)
        {
            //dataBaseList.Add(tableName[i],new List<Dictionary<string, object>>());    
        }

        //创建数据表
        for (int i = 0; i < tableName.Length; i++)
        {
            //第一次创建Sqlite表
            if (!SqlManager.GetInstance().DetectionExistTable(tableName[i]))
            {
                SqlManager.GetInstance().CreateTable(tableName[i], new CardEntity());

                if (tableName[i]== "OwnCard")
                {
                    Debug.Log(1111);
                    for (int j = 1; j < 5; j++)
                    {
                        SqlManager.GetInstance().Insert(tableName[i], DataManager.GetInstance().GetCardEntityById(j));
                    }
                }
            }

            SqlManager.GetInstance().PrintValueInDataBase(tableName[i]);
        }
    }
}
