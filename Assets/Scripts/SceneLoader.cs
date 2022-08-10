using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	public void LoadLevel(int level)
	{
		SceneManager.LoadScene("Level" + level);
		PlayerStats.CanMove = true;
		PlayerStats.CanLook = true;
	}
}
