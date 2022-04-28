using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardHacks : MonoBehaviour
{
	void Awake ()
	{
		Time.timeScale = PlayerPrefs.GetFloat("SavedTimeScale", 1f);
		Application.targetFrameRate = PlayerPrefs.GetInt("SavedFrameRate", 60);
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.F5))
		{
			Time.timeScale = Mathf.Max(Mathf.RoundToInt(Time.timeScale * 10) - 2, 0) / 10f;
			PlayerPrefs.SetFloat("SavedTimeScale", Time.timeScale);
			Debug.Log("Time scale is now " + Time.timeScale);
		}
		else if (Input.GetKeyDown(KeyCode.F6))
		{
			Time.timeScale = Mathf.Min(Mathf.RoundToInt(Time.timeScale * 10) + 2, 30) / 10f;
			PlayerPrefs.SetFloat("SavedTimeScale", Time.timeScale);
			Debug.Log("Time scale is now " + Time.timeScale);
		}
		else if (Input.GetKeyDown(KeyCode.F7))
		{
			Application.targetFrameRate = Mathf.Max(Application.targetFrameRate - 5, 5);
			PlayerPrefs.SetInt("SavedFrameRate", Application.targetFrameRate);
			Debug.Log("Target frame rate is now " + Application.targetFrameRate);
		}
		else if (Input.GetKeyDown(KeyCode.F8))
		{
			Application.targetFrameRate = Mathf.Min(Application.targetFrameRate + 5, 40);
			PlayerPrefs.SetInt("SavedFrameRate", Application.targetFrameRate);
			Debug.Log("Target frame rate is now " + Application.targetFrameRate);
		}
	}
}
