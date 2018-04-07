using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ViewExerciseDetails : MonoBehaviour {

	static public string exerciseName;
	public InputField workoutNameField;
	public InputField numberOfSetsField;
	public InputField startTimerField;
	public InputField restTimerField;
	public InputField goalRepsField;

	public Text debugText;

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

		((List<string>)workoutHistory.workoutTable [exerciseName]) [0] = workoutNameField.text;
		((List<string>)workoutHistory.workoutTable [exerciseName]) [1] = numberOfSetsField.text;
		((List<string>)workoutHistory.workoutTable [exerciseName]) [2] = startTimerField.text;
		((List<string>)workoutHistory.workoutTable [exerciseName]) [3] = restTimerField.text;
		((List<string>)workoutHistory.workoutTable [exerciseName]) [4] = goalRepsField.text;

		file = File.Create (Application.persistentDataPath + "/workoutTable.dat");
		binaryFormatter.Serialize (file, workoutHistory);
		file.Close ();

		debugText.text = "Inserted into the workout table file.";
	}
}
