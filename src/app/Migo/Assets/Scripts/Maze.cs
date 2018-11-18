using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Linq;
using SFB;

public class Maze : MonoBehaviour 
{
	public string MazeFile;
	public GameObject Cell;
	public GameObject Line;
	public GameObject Cube;
	public GameObject Ball;
	public Transform Robot;

	public GameObject MazeEditPanel;
	public Image MazeImage;
	public InputField InputWidth, InputHeight;

	private int MaxWidth = 500;
	private int MaxHeight = 500;
	private int LastWidth = 0, LastHeight = 0;
	private int ImageWidth = 0, ImageHeight = 0;

	private string filePath = null;
	void Start () 
	{
		if(MazeFile != null && MazeFile.Length > 0)
			LoadMaze(MazeFile);
		MazeEditPanel.SetActive(false);
	}
	
	void Update () 
	{

	}

	public void LoadMazeFromFile()
	{
		var extensions = new [] 
		{
			new ExtensionFilter("Maze Files", "png", "jpg", "jpeg", "bmp", "txt" ),
			new ExtensionFilter("Image Files", "png", "jpg", "jpeg", "bmp" ),
			new ExtensionFilter("Text files", "txt" ),
		};
        string[] path = StandaloneFileBrowser.OpenFilePanel("Select a maze file", "", extensions, false);
		if(path.Length > 0)
			LoadMaze(path[0]);
	}

	public void LoadMaze(string filePath)
	{
		if (!File.Exists(filePath))    
			return;

		try
		{
			string lower = filePath.ToLowerInvariant();
			if(lower.EndsWith(".png") || lower.EndsWith(".jpg") || lower.EndsWith(".jpeg") || lower.EndsWith(".bmp"))
				PrepareImageMaze(filePath);
			else if(lower.EndsWith(".txt"))
				LoadTextMaze(filePath);
		}
		catch(System.Exception ex)
		{

		}
	}

	private void PrepareImageMaze(string filePath)
	{
		byte[] fileData = File.ReadAllBytes(filePath);
		Texture2D tex = new Texture2D (2, 2, TextureFormat.RGBA32, false);
		tex.LoadImage(fileData);

		int width = tex.width;
		int height = tex.height;

		MazeImage.overrideSprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));

		ImageWidth = width;
		ImageHeight = height;

		if(width > MaxWidth)
			width = MaxWidth;
		if(height > MaxHeight)
			height = MaxHeight;
		this.filePath = filePath;
		InputWidth.text = width+"";
		InputHeight.text = height+"";
		LastWidth = width;
		LastHeight = height;
		MazeEditPanel.SetActive(true);
	}

	public void CancelImageMaze()
	{
		filePath = null;
		MazeEditPanel.SetActive(false);
	}

	public void ContinueImageMaze()
	{
		if(filePath == null)
		{
			CancelImageMaze();
			return;
		}

		int width = int.Parse(InputWidth.text);
		int height = int.Parse(InputHeight.text);
		
		LoadImageMaze(filePath, width, height);
		CancelImageMaze();
	}

	public void FixWidthValue(string value)
	{
		int v = 0;
		if(int.TryParse(value, out v))
		{
			if(v < ImageWidth)
			{
				v = ImageWidth;
				InputWidth.text = v+"";
			}
			else if(v > MaxWidth)
			{
				v = MaxWidth;
				InputWidth.text = v+"";
			}
			else
				LastWidth = v;
		}
		else
		{
			InputWidth.text = LastWidth+"";
		}
	}

	public void FixHeightValue(string value)
	{
		int v = 0;
		if(int.TryParse(value, out v))
		{
			if(v < ImageHeight)
			{
				v = ImageHeight;
				InputHeight.text = v+"";
			}
			else if(v > MaxHeight)
			{
				v = MaxHeight;
				InputHeight.text = v+"";
			}
			else
				LastHeight = v;
		}
		else
		{
			InputHeight.text = LastHeight+"";
		}
	}

	private void LoadImageMaze(string filePath, int newWidth, int newHeight)
	{
		Clear();
		byte[] fileData = File.ReadAllBytes(filePath);
		Texture2D tex = new Texture2D (2, 2, TextureFormat.RGBA32, false);
		tex.LoadImage(fileData);

		tex = TextureScaler.Scaled(tex, newWidth, newHeight);

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
		List<Vector2> cubes = new List<Vector2>();
		List<Vector2> balls = new List<Vector2>();

		string id;
		int x1, y1, x2, y2, tmp;
		for(int i = 0; i < textLines.Length; i++)
		{
			string[] parts = textLines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim()).Where( part => !string.IsNullOrEmpty(part)).ToArray();
			if(parts.Length < 3)
				continue;
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
					while(y1 != y2 || x1 != x2)
					{
						points.Add(new Vector2(x1, y1));
						if(x1 < x2)
							x1++;
						else if(x1 > x2)
							x1--;
						if(y1 < y2)
							y1++;
						else if(y1 > y2)
							y1--;
					}
					points.Add(new Vector2(x1, y1));
				}
			}
			else if(id == "L")
			{
				x1 = int.Parse(parts[1]);
				y1 = int.Parse(parts[2]);
				x2 = int.Parse(parts[3]);
				y2 = int.Parse(parts[4]);
				lines.Add(new Vector4(x1, y1, x2, y2));
			}
			else if(id == "C")
			{
				x1 = int.Parse(parts[1]);
				y1 = int.Parse(parts[2]);
				cubes.Add(new Vector2(x1, y1));
			}
			else if(id == "B")
			{
				x1 = int.Parse(parts[1]);
				y1 = int.Parse(parts[2]);
				balls.Add(new Vector2(x1, y1));
			}
		}

		for(int i = 0; i < points.Count; i++)
		{
			x = points[i].x;
			z = points[i].y;
			Instantiate(Cell, new Vector3(x, y, z), Quaternion.identity, transform);
		}

		for(int i = 0; i < cubes.Count; i++)
		{
			x = cubes[i].x;
			z = cubes[i].y;
			Instantiate(Cube, new Vector3(x, y, z), Quaternion.identity, transform);
		}

		for(int i = 0; i < balls.Count; i++)
		{
			x = balls[i].x;
			z = balls[i].y;
			Instantiate(Ball, new Vector3(x, y, z), Quaternion.identity, transform);
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

	public void Clear()
	{
		foreach (Transform child in transform) 
		{
     		GameObject.Destroy(child.gameObject);
 		}
	}
}
