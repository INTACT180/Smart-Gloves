using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimerScript : MonoBehaviour {

	public Text gameTimerText;
	float gameTimer = 0f;
	private bool increaseTimer = false;
	private bool decreaseTimer = false;
	private float timer = 30;

	// Update is called once per frame
	void Update () {

		if (increaseTimer) {
			gameTimer += Time.deltaTime;

			int seconds = (int)(gameTimer % 60);
			int minutes = (int)(gameTimer / 60) % 60;
			int hours = (int)(gameTimer / 3600) % 24;

			string timerString = string.Format("{1:00}:{2:00}", hours, minutes, seconds);

			gameTimerText.text = timerString;	
		}

		if (decreaseTimer) {
			timer -= Time.deltaTime;
			gameTimerText.text = timer.ToString ("f0");

		}
	}

	public void startTimer () {

		decreaseTimer = true;

	}

}