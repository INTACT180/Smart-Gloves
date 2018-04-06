using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ExerciseScreen : MonoBehaviour {

	static public string exerciseName;

	public Button sceneTitle;
	public Button actionButton;

	public InputField numberOfSetsField;
	public InputField startTimerField;
	public InputField restTimerField;

	public Text gameTimerText;
	public Text currentStageText;
	public Text exerciseNameLabel;

	// Timer boolean flags. 
	private bool increaseExerciseTimer = false;
	private bool decreasestartTimer = false;

	// Timer values.
	private float startTimerVal = 0;
	private float restTimerVal = 0;
	private float excerciseTimer = 0f;

	// Use this for initialization
	void Start () {

		// Sets the top button to the chosen exercise name.
		sceneTitle.GetComponentInChildren<Text> ().text = exerciseName;

		// Sets the bottom button's text to "Start Exercise".
		actionButton.GetComponentInChildren<Text> ().text = "Start Exercise";

		// Sets the current stage text.
		currentStageText.text = "Waiting to start exercise...";

		// Retrieves file with same exercise details.
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream file = File.Open (Application.persistentDataPath + "/workoutTable.dat", FileMode.Open);
		WorkoutList workoutHistory = (WorkoutList) binaryFormatter.Deserialize(file);
		file.Close ();

		// Sets the text fields to the stored values.
		numberOfSetsField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[1];
		startTimerField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[2];
		restTimerField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[3];

		// Parses the start time entered by the user to a float and sets the start timer
		startTimerVal = float.Parse (startTimerField.text);
		restTimerVal = float.Parse (restTimerField.text);

		
	}

	void Update () {

		if (increaseExerciseTimer) {
			excerciseTimer += Time.deltaTime;

			int seconds = (int)(excerciseTimer % 60);
			int minutes = (int)(excerciseTimer / 60) % 60;
			int hours = (int)(excerciseTimer / 3600) % 24;

			string timerString = string.Format("{1:00}:{2:00}", hours, minutes, seconds);

			gameTimerText.text = timerString;	
		}

		// Decreases the start timer.
		if (decreasestartTimer) {
			startTimerVal -= Time.deltaTime;
			gameTimerText.text = startTimerVal.ToString ("f0");
			if (startTimerVal <= 0) {
				decreasestartTimer = false;
				currentStageText.text = "Waiting for User to Finish Exercise";
				currentStageText.color = Color.red;

				increaseExerciseTimer = true;
			}
		}
	}

	public void startTimer () {

		if (actionButton.GetComponentInChildren<Text> ().text.ToLower ().Equals ("start exercise")) {
			actionButton.GetComponentInChildren<Text> ().text = "Stop";
			currentStageText.text = "Counting Down Start Timer";
			decreasestartTimer = true;
		}


	}
}
