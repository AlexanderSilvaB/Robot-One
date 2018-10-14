using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine(LoadAsync(sceneName));
	}

	private IEnumerator LoadAsync(string sceneName)
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		while(!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
