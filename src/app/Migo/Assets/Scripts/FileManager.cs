using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FileManager
{
    public static string OpenFile(string title ="", string directory = "", string extension = "")
    {
        #if UNITY_EDITOR
        return EditorUtility.OpenFilePanel(title, directory, extension);
        #else
        return title;
        #endif
    }
}