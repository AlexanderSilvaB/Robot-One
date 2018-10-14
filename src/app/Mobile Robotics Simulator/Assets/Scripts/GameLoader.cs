using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour {

	public float ShowTime = 3.0f;
	void Start () {
		StartCoroutine(StartCountdown());
	}
	
	void Update () {
		
	}

	float currCountdownValue;
	public IEnumerator StartCountdown(float countdownValue = 10)
	{
		currCountdownValue = countdownValue;
		while (currCountdownValue > 0)
		{
			yield return new WaitForSeconds(1.0f);
			currCountdownValue--;
		}
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("World");
		while(!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
