using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class EditWorkouts : MonoBehaviour {

	// These fields are added from the inspector in Unity.
	[SerializeField] Transform menuPanel;
	[SerializeField] GameObject buttonPrefab;

	public Hashtable listOfWorkouts;
	public string message;
//	public Text pageTitle;
	public Button pageTitle;

	// Use this for initialization
	void Start () {
		
		// Check if the file exists.
		if (File.Exists (Application.persistentDataPath + "/workoutTable.dat")) {

			// Open the file and retrieve the workout list.
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/workoutTable.dat", FileMode.Open);
			WorkoutList workoutHistory = (WorkoutList) binaryFormatter.Deserialize(file);
			file.Close ();

			// Iterates through all of the created workouts.
			foreach (DictionaryEntry workout in workoutHistory.workoutTable) {
				Debug.Log (workout.Key);

				// Creates the button.
				GameObject button = (GameObject)Instantiate (buttonPrefab);
				button.GetComponentInChildren<Text> ().text = (string)workout.Key;

				// Give the buttons an action to load a screen that displays its details. 
				button.GetComponent<Button> ().onClick.AddListener (
					() => {
						if (pageTitle.GetComponentInChildren<Text>().text.Equals ("Create/Edit Workout")) {
							goToViewExerciseDetailsScene (button);
						} else if (pageTitle.GetComponentInChildren<Text>().text.Equals ("Select Exercise")) {
							goToExerciseScreenScene(button);
						}
					}
				);
				button.transform.parent = menuPanel;
			}
		} 
	}

	private void goToViewExerciseDetailsScene(GameObject button) {
		ViewExerciseDetails.exerciseName = button.GetComponentInChildren<Text> ().text;
		SceneManager.LoadScene("ViewExeciseDetails");
	}

	private void goToExerciseScreenScene(GameObject button) {
		ExerciseScreen.exerciseName = button.GetComponentInChildren<Text> ().text;
		SceneManager.LoadScene("ExerciseScreen");
	}
		
}
