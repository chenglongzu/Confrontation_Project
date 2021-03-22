using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorExtend : Editor
{
    [MenuItem("Tools/ClearLevelData")]
    public static void ClearLevelData()
    {
        PlayerPrefs.SetInt("LevelIndex",1);
    }
}
