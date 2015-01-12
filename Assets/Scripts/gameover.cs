using UnityEngine;
using System.Collections;

public class gameover : MonoBehaviour {
	
	public GameObject gameOver;
	public GameObject glider;
	public GameObject win;
	public GUIText health;

	void Update () {
		if (gliding.health <= 0) {
			gliding.isAlive = false;
			gameOver.SetActive(true);
			health.text = "Health: " + gliding.health.ToString();
			if (Input.GetKey(KeyCode.R)) {
				Application.LoadLevel(Application.loadedLevel);
			}
		}
		if (!gliding.isAlive && true) glider.SetActive(false);

		if (gliding.legendary_object_count >= 5) {
			gliding.isAlive = false;
			win.SetActive(true);
		}
		
		if (Input.GetKey(KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}