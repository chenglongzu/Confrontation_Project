using UnityEngine;
using System;
using ShimmerSqlite;

[Serializable]
public class CardEntity : DataBase
{
    public string name;
    public string fileName;
    public int resourcesPoint;
    public int hurtValue;
    public string character;

    public CardEntity()
    {
    }

    public CardEntity(int id, string name,string fileName, int resourcesPoint, int hurtValue, string character)
    {
        this.id = id;
        this.name = name;
        this.fileName = fileName;
        this.resourcesPoint = resourcesPoint;
        this.hurtValue = hurtValue;
        this.character = character;
    }

    //输出变量的值
    public override object[] DataToArray()
    {
        return new object[] { id, name,fileName, resourcesPoint, hurtValue, character };
    }

    //输出变量的名称
    public override string[] NameToArray()
    {
        return new string[] { "id", "name", "fileName", "resourcesPoint", "hurtValue", "character" };
    }

    //输出变量的类型
    public override string[] TypeToArray()
    {
        return new string[] { "int", "string", "string", "int", "int", "string" };
    }
}
