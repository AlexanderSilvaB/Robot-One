using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DynamicObj
{
    private string path;
    public string Path
    {
        get
        {
            return path;
        }
        set
        {
            path = value;
            System.IO.Path.GetExtension(path);
        }
    }

    public string Extension
    {
        get
        {
            return System.IO.Path.GetExtension(path);
        }
    }

    public string FileName
    {
        get
        {
            return System.IO.Path.GetFileName(path);
        }
    }

    public string Folder
    {
        get
        {
            return System.IO.Path.GetDirectoryName(path)+"/";
        }
    }

    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }
    public Vector3 Rotation { get; set; }
}