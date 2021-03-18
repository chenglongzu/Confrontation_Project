using UnityEngine;
using ShimmerSqlite;

public class SqliteInitManager : MonoBehaviour
{
    [SerializeField]
    private string gameDataBase;

    [SerializeField]
    private string tableName;

    void Start()
    {
        SqlManager.GetInstance().InitDataBase(gameDataBase);

        if (!SqlManager.GetInstance().DetectionExistTable(tableName))
        {
            SqlManager.GetInstance().CreateTable(tableName, new CardEntity());
        }
       


        SqlManager.GetInstance().PrintValueInDataBase(tableName);


    }
}
