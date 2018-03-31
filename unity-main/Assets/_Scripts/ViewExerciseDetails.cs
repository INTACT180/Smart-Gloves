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

	// Use this for initialization
	void Start () {
		Debug.Log ("This is the exercise name: " + exerciseName);

		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream file = File.Open (Application.persistentDataPath + "/workoutTable.dat", FileMode.Open);
		WorkoutList workoutHistory = (WorkoutList) binaryFormatter.Deserialize(file);
		file.Close ();

		List<string> exerciseDetails = (List<string>)workoutHistory.workoutTable [exerciseName];

		workoutNameField.text = exerciseDetails[0];
		numberOfSetsField.text = exerciseDetails [1];
		startTimerField.text = exerciseDetails [2];
		restTimerField.text = exerciseDetails [3];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
