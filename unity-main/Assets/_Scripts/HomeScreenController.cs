using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class HomeScreenController : MonoBehaviour {

	// Use this for initialization
	void Start () {

		if (!File.Exists (Application.persistentDataPath + "/workoutTable.dat")) {
			initExerciseData ();
			Debug.Log ("The file has been created");
		} else {
			Debug.Log ("The file already exists");
		}

	}
	
	// Update is called once per frame
	void Update () {
	}

	void initExerciseData () {
	
		BinaryFormatter binaryFormatter = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/workoutTable.dat");

		WorkoutList workoutList = new WorkoutList ();

		List<String> workoutDetailsList = new List<String> ();

		// Adds the Bicep_Curl to the file.
		workoutDetailsList.Add ("Bicep_Curl"); // exercise name
		workoutDetailsList.Add ("3"); // number of sets
		workoutDetailsList.Add ("10"); // start timer
		workoutDetailsList.Add ("15"); // rest timer
		workoutDetailsList.Add ("5"); // goal number of reps
		workoutList.workoutTable.Add("Bicep Curl", workoutDetailsList);

		workoutDetailsList = new List<String> ();

		// Adds Bench Press to the file.
		workoutDetailsList.Add ("Bench_Press"); // exercise name
		workoutDetailsList.Add ("3"); // number of sets
		workoutDetailsList.Add ("5"); // start timer
		workoutDetailsList.Add ("10"); // rest timer
		workoutDetailsList.Add ("6"); // goal number of reps
		workoutList.workoutTable.Add("Bench Press", workoutDetailsList);

		// Saves the file.
		binaryFormatter.Serialize (file, workoutList);

	}
}
