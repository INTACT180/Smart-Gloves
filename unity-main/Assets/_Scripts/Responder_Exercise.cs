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

	// Goal Stats input fields.
	public InputField numberOfSetsField;
	public InputField startTimerField;
	public InputField restTimerField;
	public InputField goalRepsField;

	// Current Stats input fields.
	public InputField currentSetField;

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


	public static Controller controller;


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
		goalRepsField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[4];

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
//				decreasestartTimer = false;
				currentStageText.text = "Waiting for User to Finish Exercise";
				currentStageText.color = Color.red;
				startTimerVal = float.Parse (startTimerField.text);
				currentState = States.InProgress;
			}

			break;

		case States.InProgress:
			//run exercise itteration for each glove
			//
//			Debug.Log("running state IN PROGRESS");

			if(GetReps() > int.Parse(numberOfSetsField.text ))
			{
				
			}		
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

			break;

		case States.Rest:
			// Check if rest timer expired... if yes goto -> Inprogress..
		

			// Decreses the rest timer.
			restTimerVal-= Time.deltaTime;
			gameTimerText.text = restTimerVal.ToString ("f0");

			// The rest timer completes.
			if (restTimerVal <= 0) {
				currentStageText.text = "Counting Down Start Timer";
				currentStageText.color = Color.red;

				currentState = States.SetUp;
				excerciseTimer = 0f;
				restTimerVal = float.Parse (restTimerField.text);
			}

			break;

		case States.Finish:

			//Play audio wrap up exercise // show stats?
			break;
		
		}
	}


	void ExerciseUpdate()
	{

		switch (Routine)
		{
		case Exercise.Bench_Press:
			if (BenchPress.instance == null) {
				BenchPress.Initilize (controller.OrientationLeft, controller.OrientationRight);
			} else {
				BenchPress.instance.Update (
					controller.OrientationLeft, 
					controller.AccelerationLeft,
					controller.OrientationRight,
					controller.AccelerationRight);
			}
			break;

		case Exercise.Bicep_Curl:

			break;
		}
			
	}

	int GetReps()
	{

		switch (Routine)
		{
		case Exercise.Bench_Press:
			if (BenchPress.instance != null)
			{
				return BenchPress.instance.Repitition;
			}
			break;

		case Exercise.Bicep_Curl:

			break;
		}

		return 0;

	}

	void FinishExercise()
	{

		switch (Routine)
		{
		case Exercise.Bench_Press:
			BenchPress.Complete ();
			break;

		case Exercise.Bicep_Curl:

			break;
		}
	}

	public void ActionButton () {
		// State in which the exercise needs to be started.
		if (actionButton.GetComponentInChildren<Text> ().text.ToLower ().Equals ("start exercise")) {
			actionButton.GetComponentInChildren<Text> ().text = "Stop Set";
			currentStageText.text = "Counting Down Start Timer";
			currentSetField.text = "1";
			currentState = States.SetUp;	
		} else if (actionButton.GetComponentInChildren<Text> ().text.ToLower ().Equals ("stop set")) {

			// Checks if the current number of sets and the goal number of sets match.
			if (!currentSetField.text.Equals (numberOfSetsField.text)) {
				actionButton.GetComponentInChildren<Text> ().text = "Stop Set";
				currentStageText.text = "Counting Down Rest Timer";

				// Updates the current set number.
				int currentSet = int.Parse (currentSetField.text);
				currentSet += 1;
				currentSetField.text = currentSet.ToString ();
				currentState = States.Rest;
			} else {
				actionButton.GetComponentInChildren<Text> ().text = "Start Exercise";
				currentStageText.text = "Exercise Has Been Completed";
				currentState = States.Finish;
				gameTimerText.text = "00:00";
			}
		}
	}

	public static void BeepLeft()
	{
		controller.SendStringLeft ("B");
	}
	public static void BeepRight()
	{
		controller.SendStringRight ("B");
	}
	public static void BeepBoth()
	{
		controller.SendStringBoth ("B");
	}
		
}

class BenchPress
{
	public static BenchPress instance = null; 

	public Vector3 refrenceOrientationLeft;
	public Vector3 refrenceOrientationRight;

    Vector3 bounds  = new Vector3(12.0f,12.0f,12.0f);

	public int Repitition = 0;

	enum GloveState
	{
		Awaiting,
		ReadyToTransition
	}

	enum Stage
	{
		Exodus,
		Return
	}

	Stage currentStage = Stage.Exodus;

	GloveState leftState = GloveState.Awaiting;
	GloveState rightState = GloveState.Awaiting;

	public BenchPress(Vector3 oLeft, Vector3 oRight)
	{
		UpdateRefrenceOrienation (oLeft, oRight);

	}

	public static void Initilize(Vector3 oLeft, Vector3 oRight)
	{
		instance = new BenchPress (oLeft, oRight);
	}

	public static void Complete()
	{
		instance = null;
	}

	public void UpdateRefrenceOrienation (Vector3 oLeft, Vector3 oRight)
	{
		refrenceOrientationLeft = oLeft;
		refrenceOrientationRight = oRight;
	}


	public void Update(Vector3 oLeft, Vector3 aLeft, Vector3 oRight, Vector3 aRight)
	{
		if (leftState == GloveState.Awaiting) {
			leftState = DetermineTransition (refrenceOrientationLeft, oLeft, aLeft);
		}

		if (rightState == GloveState.Awaiting) {
			rightState = DetermineTransition (refrenceOrientationRight, oRight, aRight);
		}

		if (!areWeInBounds (oLeft, refrenceOrientationLeft, bounds)) {
			if (!areWeInBounds (oRight, refrenceOrientationRight, bounds)) {
				Responder_Exercise.BeepBoth ();
			} else {
				Responder_Exercise.BeepLeft();
			}
		}
		else if (!areWeInBounds (oRight, refrenceOrientationRight, bounds)) {
			Responder_Exercise.BeepRight();
		}

		if (isReadyToTransition ()) {
			currentStage = Transition ();
		}

	
	}

 Stage Transition()
	{

		switch (currentStage) 
		{
		case Stage.Exodus:
			rightState = GloveState.Awaiting;
			leftState = GloveState.Awaiting;
			return Stage.Return;

		case Stage.Return:
			Repitition++;
			rightState = GloveState.Awaiting;
			leftState = GloveState.Awaiting;
			return Stage.Exodus;
		}

		return Stage.Exodus;


	}

	public bool isReadyToTransition()
	{
		return rightState == GloveState.ReadyToTransition && leftState == GloveState.ReadyToTransition;
	}

   GloveState DetermineTransition(Vector3 oRef, Vector3 o, Vector3 a)
	{
		switch (currentStage) 
		{
		case Stage.Exodus:
			if (a.x > 0.8f)
				return GloveState.ReadyToTransition;
			break;
		case Stage.Return:
			if (a.x < -0.8f)
				return GloveState.ReadyToTransition;
			break;
		}

		return GloveState.Awaiting;
	}

	public static bool areWeInBounds(Vector3 currentOrientation, Vector3 originalReferencePoint, Vector3 boundaryOffset)
	{
		bool inBound = true;
		Vector3 high, low;
		Vector3 a, b, c;

		//For the X axis
		high.x = Math.Max(originalReferencePoint.x, currentOrientation.x);
		low.x  = Math.Min(originalReferencePoint.x, currentOrientation.x);

		a.x = high.x - low.x;
		b.x = 360 - high.x + low.x;
		c.x = Math.Min(a.x, b.x);

		if (c.x > boundaryOffset.x)
			inBound = false;

		//For the Y axis
		high.y = Math.Max(originalReferencePoint.y, currentOrientation.y);
		low.y = Math.Min(originalReferencePoint.y, currentOrientation.y);

		a.y = high.y - low.y;
		b.y = 360 - high.y + low.y;
		c.y = Math.Min(a.y, b.y);

		if (c.y > boundaryOffset.y)
			inBound = false;

		//For the Z axis
		high.z = Math.Max(originalReferencePoint.z, currentOrientation.z);
		low.z = Math.Min(originalReferencePoint.z, currentOrientation.z);

		a.z = high.z - low.z;
		b.z = 360 - high.z + low.z;
		c.z = Math.Min(a.z, b.z);

		if (c.z > boundaryOffset.z)
			inBound = false;

		return inBound;
	}


}
