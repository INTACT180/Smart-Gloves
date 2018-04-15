using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class HomeScreenController : MonoBehaviour {

	// Use this for initialization
	void Start () {

//		File.Delete (Application.persistentDataPath + "/workoutTable.dat");

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

		// Adds the Bicep_Curl to the file.
		workoutDetailsList.Add ("Bicep_Curl"); // exercise name
		workoutDetailsList.Add ("3"); // number of sets
		workoutDetailsList.Add ("10"); // start timer
		workoutDetailsList.Add ("15"); // rest timer
		workoutDetailsList.Add ("5"); // goal number of reps
		workoutList.workoutTable.Add("Wrist Curl", workoutDetailsList);

		workoutDetailsList = new List<String> ();
		// Adds Bench Press to the file.
		workoutDetailsList.Add ("Bench_Press"); // exercise name
		workoutDetailsList.Add ("3"); // number of sets
		workoutDetailsList.Add ("5"); // start timer
		workoutDetailsList.Add ("10"); // rest timer
		workoutDetailsList.Add ("6"); // goal number of reps
		workoutList.workoutTable.Add("Bench Press", workoutDetailsList);

		workoutDetailsList = new List<String> ();

		// Adds Bench Press to the file.
		workoutDetailsList.Add ("Bench_Press"); // exercise name
		workoutDetailsList.Add ("3"); // number of sets
		workoutDetailsList.Add ("5"); // start timer
		workoutDetailsList.Add ("10"); // rest timer
		workoutDetailsList.Add ("6"); // goal number of reps
		workoutList.workoutTable.Add("Bent Over Rows", workoutDetailsList);

		workoutDetailsList = new List<String> ();

		// Adds Bench Press to the file.
		workoutDetailsList.Add ("Bench_Press"); // exercise name
		workoutDetailsList.Add ("3"); // number of sets
		workoutDetailsList.Add ("5"); // start timer
		workoutDetailsList.Add ("10"); // rest timer
		workoutDetailsList.Add ("6"); // goal number of reps
		workoutList.workoutTable.Add("Squats", workoutDetailsList);

		workoutDetailsList = new List<String> ();

		// Adds Bench Press to the file.
		workoutDetailsList.Add ("Inverted_Bench_Press"); // exercise name
		workoutDetailsList.Add ("2"); // number of sets
		workoutDetailsList.Add ("4"); // start timer
		workoutDetailsList.Add ("8"); // rest timer
		workoutDetailsList.Add ("5"); // goal number of reps
		workoutList.workoutTable.Add("Military Press", workoutDetailsList);

		// Saves the file.
		binaryFormatter.Serialize (file, workoutList);

	}
}
