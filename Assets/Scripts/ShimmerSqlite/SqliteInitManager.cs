using UnityEngine;
using ShimmerSqlite;
using System.Collections.Generic;
using ShimmerFramework;

public class SqliteInitManager : SingletonMono<SqliteInitManager>
{
    [SerializeField]
    private string gameDataBase;

    [SerializeField]
    private string[] tableName;

    [SerializeField]
    private CardEntity[] cardEntities;

    protected override void Awake()
    {
        base.Awake();

        SqlManager.GetInstance().InitDataBase(gameDataBase);

        //创建数据表
        for (int i = 0; i < tableName.Length; i++)
        {
            //第一次创建Sqlite表
            if (!SqlManager.GetInstance().DetectionExistTable(tableName[i]))
            {
                SqlManager.GetInstance().CreateTable(tableName[i], new CardEntity());

                if (tableName[i] == "OwnCard")
                {
                    for (int j = 1; j < 5; j++)
                    {
                        SqlManager.GetInstance().Insert(tableName[i], DataManager.GetInstance().GetCardEntityById(j));
                    }
                }
            }

            SqlManager.GetInstance().PrintValueInDataBase(tableName[i]);
        }

    }

    public bool CheackExitData(string tableName,int id)
    {
         return SqlManager.GetInstance().CheackExitData(tableName,id);
    }
}
