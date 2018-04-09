using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ViewExerciseDetails : MonoBehaviour {

	public InputField workoutNameField;
	public InputField numberOfSetsField;
	public InputField startTimerField;
	public InputField restTimerField;
	public InputField goalRepsField;

	private bool validInput = true;

	public Text debugText;

	static public string exerciseName;

	// Use this for initialization
	void Start () {
		Debug.Log ("This is the exercise name: " + exerciseName);

		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream file = File.Open (Application.persistentDataPath + "/workoutTable.dat", FileMode.Open);
		WorkoutList workoutHistory = (WorkoutList) binaryFormatter.Deserialize(file);
		file.Close ();

		List<string> exerciseDetails = (List<string>)workoutHistory.workoutTable [exerciseName];
	
		workoutNameField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[0];
		numberOfSetsField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[1];
		startTimerField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[2];
		restTimerField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[3];
		goalRepsField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[4];
	}

	public void save () {

		// Loads the file with the corresponding workouts.
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream file = File.Open (Application.persistentDataPath + "/workoutTable.dat", FileMode.Open);
		WorkoutList workoutHistory = (WorkoutList) binaryFormatter.Deserialize(file);
		file.Close ();

		int number;

		// Checks if the inputs are numbers and not 
		if (!int.TryParse (numberOfSetsField.text, out number)) {
			validInput = false;
			numberOfSetsField.transform.FindChild ("Text").GetComponent<Text> ().color = Color.red;
		} else if (numberOfSetsField.transform.FindChild ("Text").GetComponent<Text> ().color.Equals(Color.red))  {
			validInput = true;
			numberOfSetsField.transform.FindChild ("Text").GetComponent<Text> ().color = Color.black;
		}

		if (!int.TryParse (startTimerField.text, out number)) {
			validInput = false;
			startTimerField.transform.FindChild ("Text").GetComponent<Text> ().color = Color.red;
		} else if (startTimerField.transform.FindChild ("Text").GetComponent<Text> ().color.Equals(Color.red))  {
			validInput = true;
			startTimerField.transform.FindChild ("Text").GetComponent<Text> ().color = Color.black;
		}

		if (!int.TryParse (restTimerField.text, out number)) {
			validInput = false;
			restTimerField.transform.FindChild ("Text").GetComponent<Text> ().color = Color.red;
		} else if (restTimerField.transform.FindChild ("Text").GetComponent<Text> ().color.Equals(Color.red))  {
			validInput = true;
			restTimerField.transform.FindChild ("Text").GetComponent<Text> ().color = Color.black;
		}

		if (!int.TryParse (goalRepsField.text, out number)) {
			validInput = false;
			goalRepsField.transform.FindChild ("Text").GetComponent<Text> ().color = Color.red;
		} else if (goalRepsField.transform.FindChild ("Text").GetComponent<Text> ().color.Equals(Color.red))  {
			validInput = true;
			goalRepsField.transform.FindChild ("Text").GetComponent<Text> ().color = Color.black;
		}

		// If all the inputs are valid, it will be updated on the file.
		if (validInput) {
			
					((List<string>)workoutHistory.workoutTable [exerciseName]) [0] = workoutNameField.text;
					((List<string>)workoutHistory.workoutTable [exerciseName]) [1] = numberOfSetsField.text;
					((List<string>)workoutHistory.workoutTable [exerciseName]) [2] = startTimerField.text;
					((List<string>)workoutHistory.workoutTable [exerciseName]) [3] = restTimerField.text;
					((List<string>)workoutHistory.workoutTable [exerciseName]) [4] = goalRepsField.text;
			
					file = File.Create (Application.persistentDataPath + "/workoutTable.dat");
					binaryFormatter.Serialize (file, workoutHistory);
					file.Close ();

			debugText.text = "Inserted into the workout table file.";
			debugText.GetComponent<Text>().color = Color.green;
		} else {
			debugText.text = "Please ensure that number of sets, start timer, rest timer, and goal reps are numbers.";
			debugText.color = Color.red;
		}


	}
}
