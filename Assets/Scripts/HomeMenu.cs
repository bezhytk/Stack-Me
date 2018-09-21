using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour 
{
	public Text scoreText;
	
	private void Start()
	{
		scoreText.text = PlayerPrefs.GetInt ("score").ToString ();
	}
	public void Play()
	{
		SceneManager.LoadScene ("Game");
	}
}
