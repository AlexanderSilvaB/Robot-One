using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TerrainManager : MonoBehaviour 
{
	public TerrainData terrain;
	private float[,] heights;
	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}

	void SaveTerrain(string filename) 
	{
        float[,] dat = terrain.GetHeights(0,0,terrain.heightmapWidth,terrain.heightmapHeight);
        FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
        BinaryWriter bw = new BinaryWriter(fs);
        for(int i = 0; i < terrain.heightmapWidth; i++) 
		{
            for(int j = 0; j < terrain.heightmapHeight; j++) 
			{
                bw.Write(dat[i,j]);
            }
        }
        bw.Close();
    }
    
    void LoadTerrain(string filename) 
	{
        float[,] dat = terrain.GetHeights(0,0,terrain.heightmapWidth,terrain.heightmapHeight);
        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
        BinaryReader br = new BinaryReader(fs);
        br.BaseStream.Seek(0, SeekOrigin.Begin);
        for(int i = 0; i < terrain.heightmapWidth; i++) 
		{
            for(int j = 0; j < terrain.heightmapHeight; j++) 
			{
                dat[i,j] = (float)br.ReadSingle();
            }
        }
        br.Close();
        terrain.SetHeights(0,0,dat);
        heights = terrain.GetHeights(50,50,1024,1024);
    }
}
