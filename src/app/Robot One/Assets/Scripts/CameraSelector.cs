using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityEngine.UI;

public class CameraSelector : MonoBehaviour 
{
	public enum Cameras { FirstPerson = 0, ThirdPerson = 1, Top = 2 };
	public Cameras SelectedCamera = Cameras.ThirdPerson;
	public float Distance = 0.1f;
	public string[] Titles = new string[3]{"First", "Third", "Top"};
	public Text CameraTitle;
	
	public GameObject FirstPersonCamera;
	public GameObject ThirdPersonCamera;
	public GameObject TopCamera;

	private Cameras selectedCamera;
	private float distance = 0;

	void Start () 
	{
		selectedCamera = SelectedCamera;
		if(Distance < 0.0f)
			Distance = 0.0f; 
		else if(Distance > 1.0f)
			Distance = 1.0f;
		distance = Distance;
		ChooseCamera();
		UpdateCamera();
	}
	
	void Update () 
	{
		if(SelectedCamera != selectedCamera)
		{
			selectedCamera = SelectedCamera;
			ChooseCamera();
			UpdateCamera();
		}
		if(Distance != distance)
		{
			if(Distance < 0.0f)
				Distance = 0.0f; 
			else if(Distance > 1.0f)
				Distance = 1.0f;
			distance = Distance;
			UpdateCamera();
		}
	}

	public void NextCamera()
	{
		int cam = (int)SelectedCamera;
		cam++;
		if(cam > 2)
			cam = 0;
		SelectedCamera = (Cameras)cam;
	}

	public void DistanceChanged(float d)
	{
		Distance = d;
	}

    public void ChooseCamera(string camera)
    {
        for (int i = 0; i < Titles.Length; i++)
        {
            if (camera == Titles[i])
            {
                selectedCamera = (Cameras)i;
                ChooseCamera();
                break;
            }
        }
    }

    public void ChooseCamera(Cameras camera)
    {
        selectedCamera = camera;
        ChooseCamera();
    }

	private void ChooseCamera()
	{
		switch (selectedCamera)
		{
			case Cameras.FirstPerson:
				FirstPersonCamera.SetActive(true);
				ThirdPersonCamera.SetActive(false);
				TopCamera.SetActive(false);
				break;
			case Cameras.ThirdPerson:
				FirstPersonCamera.SetActive(false);
				ThirdPersonCamera.SetActive(true);
				TopCamera.SetActive(false);
				break;
			case Cameras.Top:
				FirstPersonCamera.SetActive(false);
				ThirdPersonCamera.SetActive(false);
				TopCamera.SetActive(true);
				break;
			default:
				break;
		}
		if(CameraTitle != null)
			CameraTitle.text = Titles[(int)selectedCamera];
	}

	private void UpdateCamera()
	{
		float d = 0;
		switch (selectedCamera)
		{
			case Cameras.FirstPerson:
				break;
			case Cameras.ThirdPerson:
				d = (distance * (20.0f - 3.0f)) + 3.0f;
				ThirdPersonCamera.GetComponent<ProtectCameraFromWallClip>().closestDistance = d;
				break;
			case Cameras.Top:
				d = (distance * (60.0f - 4.0f)) + 4.0f;
				TopCamera.transform.localPosition = new Vector3(0.0f, d, 0.0f);
				break;
			default:
				break;
		}
	}
}
