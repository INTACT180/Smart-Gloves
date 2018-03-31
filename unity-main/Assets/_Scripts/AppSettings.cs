using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class AppSettings : MonoBehaviour {

	// Singleton that will hold the app settings.
	public static AppSettings appSettings;

	public InputField workoutNameField;
	public InputField numberOfSetsField;
	public InputField startTimerField;
	public InputField restTimerField;

	public Text debugText;

	// Use this for initialization
	// This could potentially be used for loading user settings when the app starts.
	void Awake () {

//		File.Delete (Application.persistentDataPath + "/workoutTable.dat");
//
//		Debug.Log ("The workout table was deleted");
//
//		DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
//		FileInfo[] info = dir.GetFiles("*.*");
//		Debug.Log (info.Length);
//		foreach (FileInfo f in info) {
//			Debug.Log (f.Name);
//		}

//		if (File.Exists (Application.persistentDataPath + "/workoutTable.dat")) {
//			debugText.text = "The file still exists. + There are " + info.Length + "files in this directory";
//		} else {
//			debugText.text = "The file doesn't exist. + There are " + info.Length + "files in this directory";
//		}

		if (File.Exists (Application.persistentDataPath + "/workoutTable.dat")) {
			debugText.text = "The workout table file already exists.";
			Debug.Log ("The file already exists");
		} else {
			BinaryFormatter binaryFormatter = new BinaryFormatter ();
			FileStream file = File.Create (Application.persistentDataPath + "/workoutTable.dat");
			binaryFormatter.Serialize (file, new WorkoutList());
			Debug.Log ("The file was created");
			debugText.text = "The workout table file was created for the first time.";
		}
	}

	// Awake happens before Start()
//	void Awake () {
//
////		debugText = GetComponent<Text> ();
////
////		debugText.text = "Yay";
//
//
//	}
	
	// Update is called once per frame
//	void Update () {
//
//	}

	public void save () {

		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream file = File.Open (Application.persistentDataPath + "/workoutTable.dat", FileMode.Open);
		WorkoutList workoutHistory = (WorkoutList) binaryFormatter.Deserialize(file);
		file.Close ();


		Debug.Log ("The current length of the workoutTable is: " + workoutHistory.workoutTable.Count);

		List<String> workoutDetailsList = new List<String> ();
		workoutDetailsList.Add (this.workoutNameField.text);
		workoutDetailsList.Add (this.numberOfSetsField.text);
		workoutDetailsList.Add (this.startTimerField.text);
		workoutDetailsList.Add (this.restTimerField.text);

		workoutHistory.workoutTable.Add (this.workoutNameField.text, workoutDetailsList);

		Debug.Log ("This is the table size before insertion: " + workoutHistory.workoutTable.Count);
			
		file = File.Create (Application.persistentDataPath + "/workoutTable.dat");
		binaryFormatter.Serialize (file, workoutHistory);
		file.Close ();

		debugText.text = "Inserted into the workout table file.";
	}

//	public void Load () {
//
//		if (File.Exists(Application.persistentDataPath + "/playerInfo.dat")) {
//			BinaryFormatter binaryFormatter = new BinaryFormatter();
//			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
//		
//			Workout workout = (Workout) binaryFormatter.Deserialize(file);
//			file.Close();
//
//			workoutName = workout.workoutName;
//			numberOfSets = workout.numberOfSets;
//			startTimer = workout.startTimer;
//			restTimer = workout.restTimer;
//		}
//
//	}
}

[Serializable]
class WorkoutList {

	public Hashtable workoutTable;
	public List<String> workoutList;

	public string workoutName;
	public string numberOfSets;
	public string startTimer;
	public string restTimer;

	// Constructor Creates Hashtable that will have all workours and List that will store all workout details.
	public WorkoutList () {
		this.workoutTable = new Hashtable ();
		this.workoutList = new List<String> ();
	}

}
