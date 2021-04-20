using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EndLevel : MonoBehaviour {

	public Text gameOverText;

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Ball")
		{
			gameOverText.gameObject.SetActive (true);
			other.gameObject.SetActive (false);

            if (SceneManager.GetActiveScene().buildIndex - 1 == SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(0);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
	}
}
