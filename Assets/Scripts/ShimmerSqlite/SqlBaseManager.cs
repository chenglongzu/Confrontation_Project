using UnityEngine;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using ShimmerFramework;

namespace ShimmerSqlite
{
    public class SqlManager : BaseManager<SqlManager>
    {
        //链接数据库
        private SqliteConnection connection;
        //数据库命令
        private SqliteCommand command;
        //数据库阅读器
        private SqliteDataReader reader;

        public void InitDataBase(string dbName)
        {
            OpenDB(dbName);
        }

        #region 初始化
        /// <summary>
        /// 创建或连接数据库
        /// </summary>
        /// <param name="dbName"></param>
        private void OpenDB(string dbName)
        {
            try
            {   //链接数据库操作
                string dbPath = Application.streamingAssetsPath + "/" + dbName + ".db";
                //固定sqlite格式data source
                connection = new SqliteConnection(@"Data Source = " + dbPath);
                connection.Open();

                Debug.Log("DataBase Connect");
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        #endregion

        #region 增
        /// <summary>
        /// 创建数据库表
        /// </summary>
        /// <param name="_tableName"></param>
        /// <param name="dataBase"></param>
        public void CreateTable(string tableName, DataBase dataBase)
        {
            if (DetectionExistTable(tableName)) return;

            string sql = "CREATE TABLE " + tableName + "(";

            for (int i = 0; i < dataBase.NameToArray().Length; i++)
            {
                sql += dataBase.NameToArray()[i] + " " + dataBase.TypeToArray()[i] + ",";
            }

            sql = sql.TrimEnd(',');
            sql += ")";

            ExcuteSql(sql);

        }

        /// <summary>
        /// 在数据库中插入数据
        /// </summary>
        /// <param name="_tableName"></param>
        /// <param name="values"></param>
        public void Insert(string tableName, DataBase dataBase)
        {
            List<Dictionary<string, object>> tableData = GetTableData(tableName);

            //如果表中存在这段数据则返回
            for (int i = 0; i < tableData.Count; i++)
            {
                if (tableData[i].ContainsKey(dataBase.id.ToString()))
                {
                    Debug.Log("当前字典长度："+ tableData[i].Keys);

                    foreach (var item in tableData[i])
                    {
                        Debug.Log(item.ToString());
                    }

                    Debug.Log("当前插入数据已存在：id:"+ dataBase.id +"名称："+ dataBase.NameToArray());
                    return;
                }

            }

            Debug.Log("当前插入数据不存在：id:" + dataBase.id + "名称：" + dataBase.NameToArray());

            string sql = "INSERT INTO " + tableName + " VALUES(";

            foreach (object value in dataBase.DataToArray())
            {
                sql += "'" + value.ToString() + "'" + ",";
            }

            sql = sql.TrimEnd(',');
            sql += ")";

            ExcuteSql(sql);
        }

        #endregion

        #region 删
        /// <summary>
        /// 通过账户名称删除单条数据
        /// 语法：delete from <表名> [where <删除条件>]　　  
        /// 例：delete from a where name='王伟华'（删除表a中列值为王伟华的行）　
        /// </summary>
        public void DeleteData(string tableName, string id)
        {
            ExcuteSql(string.Format("delete from {0} where id = '{1}'", tableName, id));
        }

        /// <summary>
        /// 删除表中的所有数据
        /// </summary>
        /// <param name="tableName"></param>
        public void DeleTableAllData(string tableName)
        {
            List<Dictionary<string, object>> temp = GetTableData(tableName);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].ContainsKey("id"))
                {
                    DeleteData(tableName,temp[i]["id"].ToString());
                }
            }
        }
        
        /// <summary>
        /// 从数据库中删除表
        /// </summary>
        public void DeleteTable(string tableName)
        {
            ExcuteSql(string.Format("truncate table {0}", tableName));
        }


        #endregion

        #region 改
        /// <summary>
        /// 更新string类型的值
        /// 对应的SQL语句
        /// update <表名> set <列名=更新值> [where <更新条件>]
        /// update addressList set 年龄=18 where 姓名='王伟华'
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void UpdateStringValue(string tableName, string accountName, string key, string value)
        {
            ExecuteNonQuery(string.Format("update {0} set {1}={2} where {3}='{4}'; ", tableName, key, value, "account", accountName));
        }

        #endregion

        #region 查
        /// <summary>
        /// 查询数据库中是否存在某张表
        /// </summary>
        /// <param name="tableName"></param>
        public bool DetectionExistTable(string tableName)
        {
            return ExecuteScalar("SELECT COUNT(*) FROM sqlite_master where type='table' and name='" + tableName + "';");
        }

        /// <summary>
        /// 获取表中的所有数据
        /// 返回所有数据库的ListDic的数据嵌套结构
        /// </summary>
        /// <param name="_tableName"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> GetTableData(string _tableName)
        {
            string sql = "SELECT * FROM " + _tableName;

            List<Dictionary<string, object>> dataArr = new List<Dictionary<string, object>>();

            reader = ExcuteSql(sql);

            while (reader.Read())
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string key = reader.GetName(i);
                    object value = reader.GetValue(i);
                    data.Add(key, value);
                }

                dataArr.Add(data);
            }

            return dataArr;
        }

        /// <summary>
        /// 获取表中的所有数据的长度
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int GetTableLength(string tableName)
        {
            return GetTableData(tableName).Count;
        }

        public bool CheackExitData(string tableName, int id)
        {
            List<Dictionary<string, object>> dataArr=GetTableData(tableName);
            for (int i = 0; i < dataArr.Count; i++)
            {
                if (dataArr[i].ContainsKey("id"))
                {
                    if (dataArr[i]["id"].ToString() == id.ToString())
                    {
                        return true;
                    } 
                }
            }

            return false;
        }

        /// <summary>
        /// 按顺序打印数据库中的数值
        /// </summary>
        public void PrintValueInDataBase(string tableName)
        {
            List<Dictionary<string, object>> tableData = GetTableData(tableName);

            //如果表中存在这段数据则返回
            for (int i = 0; i < tableData.Count; i++)
            {
                string value ="数据库内容 表： "+tableName+":"+ i + ":";

                foreach (var item in tableData[i])
                {
                    value += item.Key + ":";
                    value += item.Value + " ";
                }

                Debug.Log(value);
            }
        }

        #endregion

        #region 析构
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseDataBase()
        {
            if (reader != null)
                reader.Close();

            if (command != null)
                command.Dispose();

            if (connection != null)
                connection.Close();

            Debug.Log("DataBase Close");
        }

        #endregion

        #region 工具函数 执行Sql函数
        private SqliteDataReader ExcuteSql(string _sql)
        {
            //Debug.Log("Excute Sql :" + _sql);

            //创建数据库连接命令（事务管理、命令管理：向数据库发送指令）
            command = connection.CreateCommand();

            //设置命令语句
            command.CommandText = _sql;

            reader = command.ExecuteReader();

            return reader;
        }

        private bool ExecuteScalar(string _sql)
        {
            //Debug.Log("ExecuteScalar Sql :" + _sql);

            //创建数据库连接命令（事务管理、命令管理：向数据库发送指令）
            command = connection.CreateCommand();

            //设置命令语句
            command.CommandText = _sql;

            int result = System.Convert.ToInt32(command.ExecuteScalar());

            //Debug.Log("result " + (result > 0));

            return (result > 0);
        }

        private void ExecuteNonQuery(string _sql)
        {
            //Debug.Log("ExecuteNonQuery Sql :" + _sql);

            //创建数据库连接命令（事务管理、命令管理：向数据库发送指令）
            command = connection.CreateCommand();

            //设置命令语句
            command.CommandText = _sql;

            command.ExecuteNonQuery();
        }

        #endregion

    }


}