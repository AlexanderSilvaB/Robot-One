using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FileManager
{
    public static string OpenFile(string title ="", string directory = "", string extension = "")
    {
        return title;//EditorUtility.OpenFilePanel(title, directory, extension);
    }
}