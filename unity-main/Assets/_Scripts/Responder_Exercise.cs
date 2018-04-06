using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class Responder_Exercise : MonoBehaviour {

	public LevelManager lvlMng;

	public Transform CubeLeft, CubeRight;

	public Text[] xyz;

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



	enum Hand {
		Left,
		Right
	};

	enum States {
		Pre,
		SetUp,
		InProgress,
		Rest,
		Finish
	};

	public enum Exercise{
		Bicep_Curl,
		Bench_Press
	}

	Exercise Routine = Exercise.Bench_Press;

	Hand currentHand;

	States currentState;


	Controller controller;


	// Use this for initialization
	void Start () {

		GameObject controllerObj = GameObject.Find ("Controller");

		// Connects both gloves.
		controller = controllerObj.GetComponent<Controller> ();
		controller.ReadBoth ();

		// Sets the current state to "Pre".
		currentState = States.Pre;

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

		Debug.Log (workoutHistory.workoutTable.Count);

		// Sets the text fields to the stored values.
		numberOfSetsField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[1];
		startTimerField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[2];
		restTimerField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[3];

		// Parses the start time entered by the user to a float and sets the start timer
		startTimerVal = float.Parse (startTimerField.text);
		restTimerVal = float.Parse (restTimerField.text);


	}

	public void Back_Button()
	{
		controller.ReadStop ();
		lvlMng.LoadLevel ("GloveConfig");
	}

	// Update is called once per frame
	void Update () {

		CubeLeft.transform.eulerAngles = controller.OrientationLeft;
		CubeRight.transform.eulerAngles = controller.OrientationRight;

		switch (currentState) {

		case States.Pre:
			//if button is pressed, set setup Time, advance to setup
			break;

		case States.SetUp:
			//Await for user to get in position, buzz gloves or notify when begin,
			//Advance to exercise state
		
			startTimerVal -= Time.deltaTime;
			gameTimerText.text = startTimerVal.ToString ("f0");
			if (startTimerVal <= 0) {
				decreasestartTimer = false;
				currentStageText.text = "Waiting for User to Finish Exercise";
				currentStageText.color = Color.red;

//				increaseExerciseTimer = true;
				currentState = States.InProgress;
			}

			break;

		case States.InProgress:
			//run exercise itteration for each glove

			//
//			Debug.Log("running state IN PROGRESS");

			//Are reps finished? and are sets done? -> Finished state
			//Are reps finished? -> Rest State ++Sets Set Rest Count down
			//continue in progress...

			// Decrements timer while user is doing exercise.
			excerciseTimer += Time.deltaTime;
			int seconds = (int)(excerciseTimer % 60);
			int minutes = (int)(excerciseTimer / 60) % 60;
			int hours = (int)(excerciseTimer / 3600) % 24;
			string timerString = string.Format("{1:00}:{2:00}", hours, minutes, seconds);
			gameTimerText.text = timerString;

//			currentState = States.Rest;

			break;

		case States.Rest:
			// Check if rest timer expired... if yes goto -> Inprogress..
	
			break;

		case States.Finish:

			//Play audio wrap up exercise // show stats?
			break;
		
		}
	}


	void StartExercise()
	{

//		switch ()
//		{
//
//		}
			
	}

	public void ActionButton () {

		if (actionButton.GetComponentInChildren<Text> ().text.ToLower ().Equals ("start exercise")) {
			actionButton.GetComponentInChildren<Text> ().text = "Stop";
			currentStageText.text = "Counting Down Start Timer";
			currentState = States.SetUp;	
		}

	}
		
}
