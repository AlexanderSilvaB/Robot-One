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

        float h, s, v;
		
		for(int i = 0; i < H; i++)
		{
			for(int j = 0; j < W; j++)
			{
				color = colors[i*W + j];

                if (color.r < 255 && color.g < 255 && color.b < 255)
				{
					x = X + j;
					z = Y + i;

                    Color.RGBToHSV(color, out h, out s, out v);
                    h *= 360;
                    s *= 100;
                    v *= 100;

                    // Vermelho (Robô)
                    if (h < 20 && s > 50 && v > 50)
					{
						mx += x;
						mz += z;
						mc++;
					}
                    // Amarelo (Linha)
                    else if (h > 40 && h < 70 && s > 50 && v > 50)
                    {
                        /*
                        mx += x;
                        mz += z;
                        mc++;
                        */
                        Instantiate(Cell, new Vector3(x, y, z), Quaternion.identity, transform);
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
            Robot.rotation = Quaternion.Euler(0, 0, 0);
        }
	}

	private void LoadTextMaze(string filePath)
	{
		Clear();
		string[] textLines = File.ReadAllLines(filePath);
	
		float x, z;
		float y = transform.position.y;
		float rx = 0, rz = 0, rth = 0;
		bool changeRobot = false;

        List<Vector4> walls = new List<Vector4>();
        List<Vector2> points = new List<Vector2>();
		List<Vector4> lines = new List<Vector4>();
		List<Vector4> cubes = new List<Vector4>();
		List<Vector3> balls = new List<Vector3>();
        List<DynamicObj> objs = new List<DynamicObj>();

		string id;
		int x1, y1, x2, y2, tmp;
        float s1, s2, s3;
		for(int i = 0; i < textLines.Length; i++)
		{
            if (textLines[i].StartsWith("#"))
            {
                continue;
            }
			string[] parts = textLines[i].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim()).Where( part => !string.IsNullOrEmpty(part)).ToArray();
            if (parts.Length < 3)
            {
                continue;
            }
			id = parts[0].Trim(' ', '\r', '\t');
			if(id == "R")
			{
				rx = float.Parse(parts[1]);
				rz = float.Parse(parts[2]);
                if (parts.Length > 3)
                    rth = float.Parse(parts[3]);
                else
                    rth = 0;
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
                    walls.Add(new Vector4(x1, y1, x2, y2));

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
                if (parts.Length > 3)
                {
                    s1 = float.Parse(parts[3]);
                    if (parts.Length > 4)
                        s2 = float.Parse(parts[4]);
                    else
                        s2 = s1;
                }
                else
                {
                    s1 = 1.0f;
                    s2 = 1.0f;
                }
                cubes.Add(new Vector4(x1, y1, s1, s2));
			}
			else if(id == "B")
			{
				x1 = int.Parse(parts[1]);
				y1 = int.Parse(parts[2]);
                if (parts.Length > 3)
                    s1 = float.Parse(parts[3]);
                else
                    s1 = 1.0f;
                balls.Add(new Vector3(x1, y1, s1));
			}
            else if(id == "O")
            {
                DynamicObj obj = new DynamicObj();

                x1 = int.Parse(parts[1]);
                y1 = int.Parse(parts[2]);

                string root = System.IO.Path.GetDirectoryName(filePath);
                string file = parts[3];

                if (parts.Length > 4)
                {
                    s1 = float.Parse(parts[4]);
                    if (parts.Length > 5)
                    {
                        s2 = float.Parse(parts[5]);
                        if (parts.Length > 6)
                        {
                            s3 = float.Parse(parts[6]);
                        }
                        else
                        {
                            s3 = s1;
                        }
                    }
                    else
                    {
                        s2 = s1;
                        s3 = s1;
                    }
                }
                else
                {
                    s1 = 1.0f;
                    s2 = 1.0f;
                    s3 = 1.0f;
                }

                obj.Scale = new Vector3(s1, s2, s3);
                obj.Position = new Vector3(y1, y + obj.Scale.y, x1);
                obj.Path = System.IO.Path.Combine(root, file);
                
                objs.Add(obj);
            }
		}

		for(int i = 0; i < points.Count; i++)
		{
			x = points[i].y;
			z = points[i].x;
			Instantiate(Cell, new Vector3(x, y, z), Quaternion.identity, transform);
		}

        for (int i = 0; i < walls.Count; i++)
        {
            x1 = (int)walls[i].y;
            y1 = (int)walls[i].x;
            x2 = (int)walls[i].w;
            y2 = (int)walls[i].z;
            float cx = (x1 + x2) / 2.0f;
            float cy = (y1 + y2) / 2.0f;

            Vector2 p1 = new Vector2(x1, y1);
            Vector2 p2 = new Vector2(x2, y2);
            float dist = Vector2.Distance(p1, p2) + 1;

            Vector3 scale = new Vector3(1.0f, 2.0f, 1.0f);
            scale.z = dist;

            float angle = (float)Math.Atan2((double)(y2 - y1), (double)(x2 - x1));

            GameObject wall = Instantiate(Cell, new Vector3(cx, y + scale.y * 0.5f, cy), Quaternion.identity, transform);
            wall.transform.localScale = scale;
            wall.transform.rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg - 90, 0);
        }

        for (int i = 0; i < cubes.Count; i++)
		{
			x = cubes[i].y;
			z = cubes[i].x;
			GameObject cube = Instantiate(Cube, new Vector3(x, y + 1 + cubes[i].w, z), Quaternion.identity, transform);
            cube.transform.localScale = new Vector3(cubes[i].z, cubes[i].w, cubes[i].z);
		}

		for(int i = 0; i < balls.Count; i++)
		{
			x = balls[i].y;
			z = balls[i].x;
            GameObject ball = Instantiate(Ball, new Vector3(x, y + balls[i].z, z), Quaternion.identity, transform);
            ball.transform.localScale = new Vector3(balls[i].z, balls[i].z, balls[i].z);
        }

		for(int i = 0; i < lines.Count; i++)
		{
			x1 = (int)lines[i].y;
			y1 = (int)lines[i].x;
			x2 = (int)lines[i].w;
			y2 = (int)lines[i].z;
			Vector3[] positions = new Vector3[2];
			positions[0].x = lines[i].y;
			positions[0].y = 0;
			positions[0].z = lines[i].x;
			positions[1].x = lines[i].w;
			positions[1].y = 0;
			positions[1].z = lines[i].z;
			GameObject lineObject = Instantiate(Line, new Vector3(0, 0, 0), Quaternion.identity, transform);
			LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
			lineRenderer.positionCount = 2;
			lineRenderer.SetPositions(positions);
		}

        for (int i = 0; i < objs.Count; i++)
        {
            DynamicObj obj = objs[i];

            GameObject model = new Dummiesman.OBJLoader().Load(obj.Path);
            model.transform.SetParent(transform);
            model.transform.position = obj.Position;
            model.transform.localScale = obj.Scale;
            Rigidbody rb = model.AddComponent<Rigidbody>();
            MeshCollider collider = model.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.sharedMesh = model.GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        if (changeRobot)
		{
			Robot.position = new Vector3(rz, 0.86f, rx);
            Robot.rotation = Quaternion.Euler(0, -rth * Mathf.Rad2Deg, 0);
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
