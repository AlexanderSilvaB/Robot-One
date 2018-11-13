using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Maze : MonoBehaviour 
{
	public string MazeFile;
	public GameObject Cell;
	public GameObject Line;
	public Transform Robot;
	void Start () 
	{
		if(MazeFile != null && MazeFile.Length > 0)
			LoadMaze(MazeFile);
	}
	
	void Update () 
	{

	}

	public void LoadMazeFromFile()
	{
		string path = FileManager.OpenFile("Select a maze file", "", "png");
		if(path.Length > 0)
			LoadMaze(path);
	}

	public void LoadMaze(string filePath)
	{
		if (!File.Exists(filePath))    
			return;

		try
		{
		if(filePath.ToLowerInvariant().EndsWith(".png"))
			LoadImageMaze(filePath);
		else if(filePath.ToLowerInvariant().EndsWith(".txt"))
			LoadTextMaze(filePath);
		}
		catch(System.Exception ex)
		{

		}
	}

	private void LoadImageMaze(string filePath)
	{
		Clear();
		byte[] fileData = File.ReadAllBytes(filePath);
		Texture2D tex = new Texture2D (2, 2, TextureFormat.RGBA32, false);
		tex.LoadImage(fileData);

		Color32[] colors = tex.GetPixels32();
		Color32 color;
		int c;
		int max = 256*256*256 - 1;
		int red = 255 << 16;

		float x, z;
		float y = transform.position.y;
		float mx = 0, mz = 0;
		int mc = 0;

		int W = tex.width;
		int H = tex.height;
		int X = -W/2;
		int Y = -H/2;
		
		for(int i = 0; i < H; i++)
		{
			for(int j = 0; j < W; j++)
			{
				color = colors[i*W + j];
				if(color.r < 255 && color.g < 255 && color.b < 255)
				{
					x = X + j;
					z = Y + i;
					if(color.r > 200)
					{
						mx += x;
						mz += z;
						mc++;
					}
					else
					{
						Instantiate(Cell, new Vector3(x, y, z), Quaternion.identity, transform);
					}
				}
			}
		}
		if(mc > 0)
		{
			mx /= mc;
			mz /= mc;
			Robot.position = new Vector3(mx, 0.86f, mz);
		}
	}

	private void LoadTextMaze(string filePath)
	{
		Clear();
		string[] textLines = File.ReadAllLines(filePath);
	
		float x, z;
		float y = transform.position.y;
		float rx = 0, rz = 0;
		bool changeRobot = false;

		List<Vector2> points = new List<Vector2>();
		List<Vector4> lines = new List<Vector4>();

		string id;
		int x1, y1, x2, y2, tmp;
		for(int i = 0; i < textLines.Length; i++)
		{
			string[] parts = textLines[i].Split(',');
			id = parts[0].Trim(' ', '\r', '\t');
			if(id == "R")
			{
				rx = float.Parse(parts[1]);
				rz = float.Parse(parts[2]);
				changeRobot = true;
			}
			else if(id == "W")
			{
				x1 = int.Parse(parts[1]);
				y1 = int.Parse(parts[2]);
				if(parts.Length == 3)
					points.Add(new Vector2(x1, y1));
				else if(parts.Length == 5)
				{
					x2 = int.Parse(parts[3]);
					y2 = int.Parse(parts[4]);
					if(x2 < x1)
					{
						tmp = x1;
						x1 = x2;
						x2 = tmp;
					}
					if(y2 < y1)
					{
						tmp = y1;
						y1 = y2;
						y2 = tmp;
					}
					while(y1 <= y2 || x1 <= x2)
					{
						points.Add(new Vector2(x1, y1));
						if(x1 <= x2)
							x1++;
						if(y1 <= y2)
							y1++;
					}
				}
			}
			else if(id == "L")
			{
				x1 = int.Parse(parts[1]);
				y1 = int.Parse(parts[2]);
				x2 = int.Parse(parts[3]);
				y2 = int.Parse(parts[4]);
				lines.Add(new Vector4(x1, y2, x2, y2));
			}
		}

		for(int i = 0; i < points.Count; i++)
		{
			x = points[i].x;
			z = points[i].y;
			Instantiate(Cell, new Vector3(x, y, z), Quaternion.identity, transform);
		}

		for(int i = 0; i < lines.Count; i++)
		{
			x1 = (int)lines[i].x;
			y1 = (int)lines[i].y;
			x2 = (int)lines[i].z;
			y2 = (int)lines[i].w;
			Vector3[] positions = new Vector3[2];
			positions[0].x = lines[i].x;
			positions[0].y = 0;
			positions[0].z = lines[i].y;
			positions[1].x = lines[i].z;
			positions[1].y = 0;
			positions[1].z = lines[i].w;
			GameObject lineObject = Instantiate(Line, new Vector3(0, 0, 0), Quaternion.identity, transform);
			LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
			lineRenderer.positionCount = 2;
			lineRenderer.SetPositions(positions);

		}
		
		if(changeRobot)
		{
			Robot.position = new Vector3(rx, 0.86f, rz);
		}
	}

	private void Clear()
	{
		foreach (Transform child in transform) 
		{
     		GameObject.Destroy(child.gameObject);
 		}
	}
}
