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

	public AudioClip halo;
	public static AudioClip RepComplete;
	public AudioClip yaySound;
	public AudioClip _RepComplete;
	public AudioClip SetComplete;
	public AudioClip RestComplete;



	// string to be used to get what the current exercise is.
	public string exerciseNameEnumValue;

	public Button sceneTitle;
	public Button actionButton;

	// Goal Stats input fields.   
	public InputField numberOfSetsField;
	public InputField startTimerField;
	public InputField restTimerField;
	public InputField goalRepsField;

	// Current Stats input fields.
	public InputField currentSetField;
	public InputField currentRepField;
	public InputField totalNumberRepField;

	public Text gameTimerText;
	public Text currentStageText;
	public Text exerciseNameLabel;

	public Text exerciseNameTest;

	// Timer boolean flags. 
	private bool increaseExerciseTimer = false;
	private bool decreasestartTimer = false;
	private bool repsChanged = false;

	// Timer values.
	private float startTimerVal = 0;
	private float restTimerVal = 0;
	private float excerciseTimer = 0f;

	private bool haloPlayed = false;



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
		Bench_Press,
		Inverted_Bench_Press
	}

	Exercise Routine = Exercise.Bench_Press;

	Hand currentHand;

	States currentState;


	public static Controller controller;


	// Use this for initialization
	void Start () {

		RepComplete = _RepComplete;

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

		exerciseNameEnumValue = ((List<string>)workoutHistory.workoutTable[exerciseName])[0];

		if(exerciseNameEnumValue.Equals(Exercise.Bench_Press.ToString()))
			Routine = Exercise.Bench_Press;
		else if(exerciseNameEnumValue.Equals(Exercise.Inverted_Bench_Press.ToString()))
			Routine = Exercise.Inverted_Bench_Press;
		else if(exerciseNameEnumValue.Equals(Exercise.Bicep_Curl.ToString()))
			Routine = Exercise.Bicep_Curl;

		// Sets the text fields to the stored values.
		numberOfSetsField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[1];
		startTimerField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[2];
		restTimerField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[3];
		goalRepsField.text = ((List<string>)workoutHistory.workoutTable[exerciseName])[4];

		// Parses the start time entered by the user to a float and sets the start timer
		startTimerVal = float.Parse (startTimerField.text);
		restTimerVal = float.Parse (restTimerField.text);

//		exerciseNameTest.text = Routine.ToString ();

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

			if (!haloPlayed && startTimerVal < 3.75 && startTimerVal >= 3) {
				AudioSource.PlayClipAtPoint(halo,Camera.main.transform.position);
				haloPlayed = true;
			}

			if (startTimerVal <= 0) {
				haloPlayed = false;
//				decreasestartTimer = false;
				currentStageText.text = "Waiting for User to Finish Exercise";
//				currentStageText.color = Color.red;
				startTimerVal = float.Parse (startTimerField.text);

//				int currentSetNumber = int.Parse (currentSetField.text);
//				currentSetField.text = "1";
				currentState = States.InProgress;

				FinishExercise ();
			}
			break;

		case States.InProgress:
			//run exercise itteration for each glove
			//
//			Debug.Log ("running state IN PROGRESS");



			ExerciseUpdate ();

			int currentFinishedReps = GetReps ();

//			int num = int.Parse (currentRepField.text);
//
//			// When the number of reps needs to be updated, currentFinishedReps will be greater that currentRepField.
//			// The total number of reps should also be updated.
//			if (currentFinishedReps >= num) {
//				int currentTotalRepNumber = int.Parse (totalNumberRepField.text);
//				totalNumberRepField.text = (++currentTotalRepNumber).ToString ();
//				currentRepField.text = currentFinishedReps.ToString ();
//			}
				
			currentRepField.text = currentFinishedReps.ToString ();

//			int totalNumReps = int.Parse (totalNumberRepField.text);
//			totalNumberRepField.text = (++totalNumReps).ToString ();

			//update current rep text
			if (currentFinishedReps >= int.Parse (goalRepsField.text)) { //check if reps >= number

//				FinishExercise ();

//				int currentSetNumber = int.Parse (currentSetField.text);
//
//				currentSetField.text = (++currentSetNumber).ToString ();
					
				if (currentSetField.text.Equals (numberOfSetsField.text)) {
					currentState = States.Finish;
					AudioSource.PlayClipAtPoint(yaySound,Camera.main.transform.position);
				} else {
					AudioSource.PlayClipAtPoint(SetComplete,Camera.main.transform.position);
					currentState = States.Rest;
				}

				//sets ++
				//if sets >= total -> finished
				//transition -> rest
				
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
		
			currentStageText.text = "Counting Down Rest Timer";

			// Decreses the rest timer.
			restTimerVal-= Time.deltaTime;
			gameTimerText.text = restTimerVal.ToString ("f0");

			// The rest timer completes.
			if (restTimerVal <= 0 && int.Parse(currentSetField.text) < int.Parse(numberOfSetsField.text)) {

				Debug.Log (int.Parse (currentSetField.text));
				Debug.Log (int.Parse(numberOfSetsField.text));

				currentStageText.text = "Counting Down Start Timer";
//				currentStageText.color = Color.red;

				AudioSource.PlayClipAtPoint(RestComplete,Camera.main.transform.position);

				int currentSetNumber = int.Parse (currentSetField.text);

				currentSetField.text = (++currentSetNumber).ToString ();

				currentState = States.SetUp;
				excerciseTimer = 0f;
				restTimerVal = float.Parse (restTimerField.text);
			} else if (restTimerVal <= 0 && int.Parse(currentSetField.text) == int.Parse(numberOfSetsField.text)) {
				currentState = States.Finish;
			}

			break;

		case States.Finish:

			actionButton.GetComponentInChildren<Text> ().text = "Start Exercise";
			currentStageText.text = "Exercise Has Been Completed";
			currentState = States.Finish;
			gameTimerText.text = "00:00";
			excerciseTimer = 0f;

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
			if (BicepCurl.instance == null) {
				BicepCurl.Initilize (controller.OrientationLeft, controller.OrientationRight);
			} else {
				BicepCurl.instance.Update (
					controller.OrientationLeft, 
					controller.AccelerationLeft,
					controller.OrientationRight,
					controller.AccelerationRight);
			}
			break;

		case Exercise.Inverted_Bench_Press:
			if (InvertedBenchPress.instance == null) {
				InvertedBenchPress.Initilize (controller.OrientationLeft, controller.OrientationRight);
			} else {
				InvertedBenchPress.instance.Update (
					controller.OrientationLeft, 
					controller.AccelerationLeft,
					controller.OrientationRight,
					controller.AccelerationRight);
			}
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
			if (BicepCurl.instance != null)
			{
				return BicepCurl.instance.Repitition;
			}
			break;

		case Exercise.Inverted_Bench_Press:
			if (InvertedBenchPress.instance != null)
			{
				return InvertedBenchPress.instance.Repitition;
			}
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
			BicepCurl.Complete ();
			break;

		case Exercise.Inverted_Bench_Press:
			InvertedBenchPress.Complete ();
			break;
		}
	}

	public void ActionButton () {
		// State in which the exercise needs to be started.
		if (actionButton.GetComponentInChildren<Text> ().text.ToLower ().Equals ("start exercise")) {
			actionButton.GetComponentInChildren<Text> ().text = "Stop Set";
			currentStageText.text = "Counting Down Start Timer";
			currentSetField.text = "1";
			gameTimerText.text = "00:00";
			currentState = States.SetUp;	
		} else if (actionButton.GetComponentInChildren<Text> ().text.ToLower ().Equals ("stop set")) {

			if (!currentState.Equals (States.Rest) || !currentState.Equals (States.SetUp)) {
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

	Vector3 bounds  = new Vector3(15.0f,15.0f,15.0f);

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
			AudioSource.PlayClipAtPoint(Responder_Exercise.RepComplete,Camera.main.transform.position);
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

class InvertedBenchPress
{
	public static InvertedBenchPress instance = null; 

	public Vector3 refrenceOrientationLeft;
	public Vector3 refrenceOrientationRight;

	Vector3 bounds  = new Vector3(15.0f,15.0f,15.0f);

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

	public InvertedBenchPress(Vector3 oLeft, Vector3 oRight)
	{
		UpdateRefrenceOrienation (oLeft, oRight);

	}

	public static void Initilize(Vector3 oLeft, Vector3 oRight)
	{
		instance = new InvertedBenchPress (oLeft, oRight);
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
			AudioSource.PlayClipAtPoint(Responder_Exercise.RepComplete,Camera.main.transform.position);
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
			if (a.x < -0.8f)
				return GloveState.ReadyToTransition;
			break;
		case Stage.Return:
			if (a.x > 0.8f)
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

class BicepCurl
{
	public static BicepCurl instance = null; 

	public Vector3 refrenceOrientationLeft;
	public Vector3 refrenceOrientationRight;

	private bool leftAcc = false;
	private bool leftRotBreach = false;

	private bool rightAcc = false;
	private bool rightRotBreach = false;

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

	enum Hand
	{
		Left,
		Right
	}

	Vector3 lowerBoundry = new Vector3 (41, 145, 30);
	Vector3 upperBoundry = new Vector3 (14, 14, 14);

	Stage currentStage = Stage.Exodus;

	GloveState leftState = GloveState.Awaiting;
	GloveState rightState = GloveState.Awaiting;

	public BicepCurl(Vector3 oLeft, Vector3 oRight)
	{
		UpdateRefrenceOrienation (oLeft, oRight);

	}

	public static void Initilize(Vector3 oLeft, Vector3 oRight)
	{
		instance = new BicepCurl (oLeft, oRight);
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
			leftState = DetermineTransition (refrenceOrientationLeft, oLeft, aLeft,Hand.Left);
		}

		if (rightState == GloveState.Awaiting) {
			rightState = DetermineTransition (refrenceOrientationRight, oRight, aRight, Hand.Right);
		}

		if (!areInBoundsCustom2 (oLeft, refrenceOrientationLeft, lowerBoundry,upperBoundry)) {
			if (!areInBoundsCustom2 (oRight, refrenceOrientationRight, lowerBoundry,upperBoundry)) {
				Responder_Exercise.BeepBoth ();
			} else {
				Responder_Exercise.BeepLeft();
			}
		}
		else if (!areInBoundsCustom2 (oRight, refrenceOrientationRight, lowerBoundry,upperBoundry)) {
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
			resetFlags ();
			return Stage.Return;

		case Stage.Return:
			Repitition++;
			AudioSource.PlayClipAtPoint(Responder_Exercise.RepComplete,Camera.main.transform.position);
			rightState = GloveState.Awaiting;
			leftState = GloveState.Awaiting;
			resetFlags ();
			return Stage.Exodus;
		}

		return Stage.Exodus;


	}

	public bool isReadyToTransition()
	{
		return rightState == GloveState.ReadyToTransition && leftState == GloveState.ReadyToTransition;
	}

	GloveState DetermineTransition(Vector3 oRef, Vector3 o, Vector3 a, Hand hand)
	{
		switch (currentStage) 
		{
		case Stage.Exodus:
			if (!hasAccelerated (hand)) {
				updateHasAccelerated (hand, a);
			}
			if (!hasBreachedBounds (hand)) {
				updateHasBreachedBounds (hand, o);
			}

			if (hasAccelerated (hand) && hasBreachedBounds (hand))
				return GloveState.ReadyToTransition;

			break;
		case Stage.Return:
			if (!hasAccelerated (hand)) {
				updateHasReversedAcceleration (hand, a);
			}
			if (!hasBreachedBounds (hand)) {
				updateReverseHasBreachedBounds (hand, o);
			}
			if (hasAccelerated (hand) && hasBreachedBounds (hand))
				return GloveState.ReadyToTransition;
			break;
		}

		return GloveState.Awaiting;
	}

	void updateHasAccelerated(Hand hand, Vector3 a)
	{
		if (hand == Hand.Left)
			leftAcc = a.z < -0.8f;
		else
			rightAcc = a.z < -0.8f;
		return;
	}

	void updateHasReversedAcceleration(Hand hand, Vector3 a)
	{
		if (hand == Hand.Left)
			leftAcc = a.z > 0.8f;
		else
			rightAcc = a.z >0.8f;
		return;
	}

	void updateHasBreachedBounds (Hand hand, Vector3 o)
	{
		Vector3 refer;

		if (hand == Hand.Left)
			refer = refrenceOrientationLeft;
		else
			refer = refrenceOrientationRight;
		
		if( !areInBoundsCustom(
			o,
			refer,
			new Vector3(180, 50, 180),
			new Vector3(180, 180, 180)))
		{
			if(hand == Hand.Left)
				leftRotBreach = true;
			else
				rightRotBreach = true;
		}
				
	}

	void updateReverseHasBreachedBounds (Hand hand, Vector3 o)
	{
		Vector3 refer;

		if (hand == Hand.Left)
			refer = refrenceOrientationLeft;
		else
			refer = refrenceOrientationRight;

		if( !areInBoundsCustom(
			o,
			refer,
			new Vector3(180, 220, 180),
			new Vector3(180, -50, 180)))
		{
			if(hand == Hand.Left)
				leftRotBreach = true;
			else
				rightRotBreach = true;
		}

	}

	void resetFlags()
	{
		leftAcc = false;
		rightAcc = false;
		leftRotBreach = false;
		rightRotBreach = false;
	}

	bool hasAccelerated( Hand hand)
	{
		if (hand == Hand.Left)
			return leftAcc;

		return rightAcc;
	}

	bool hasBreachedBounds( Hand hand)
	{
		if (hand == Hand.Left)
			return leftRotBreach;
		
		return rightRotBreach;
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

	public static bool areInBoundsCustom(Vector3 currentOrientation, Vector3 originalReferencePoint, Vector3 lowerBounds, Vector3 upperBounds)
	{
		bool inBound = true;

		Vector3 upper = correctForDegrees(originalReferencePoint + upperBounds);
		Vector3 lower = correctForDegrees(originalReferencePoint - lowerBounds);

		if (!isAngleBetween(upper.x, lower.x, currentOrientation.x))
			inBound = false;
		if (!isAngleBetween(upper.y, lower.y, currentOrientation.y))
			inBound = false;
		if (!isAngleBetween(upper.z, lower.z, currentOrientation.z))
			inBound = false;

		return inBound;
	}

	public static bool areInBoundsCustom2(Vector3 currentOrientation, Vector3 originalReferencePoint, Vector3 lowerBounds, Vector3 upperBounds)
	{
		bool inBound = true;

		Vector3 upper = correctForDegrees(originalReferencePoint + upperBounds);
		Vector3 lower = correctForDegrees(originalReferencePoint - lowerBounds);

		if (!isAngleBetween(upper.x, lower.x, currentOrientation.x))
			inBound = false;
		if (!isAngleBetween(upper.y, lower.y, currentOrientation.y))
			inBound = false;
		if (!isAngleBetween(upper.z, lower.z, currentOrientation.z))
			inBound = false;

		if (!inBound)
			Debug.Log ("scoot currentOrientation: " + currentOrientation + " originalRef: " + originalReferencePoint + " lower: " + lower + " upper: " + upper + " lower bounds" + lowerBounds + " upperBounds: " + upperBounds);
		
		return inBound;
	}

	public static bool isAngleBetween(float upper, float lower, float angle)
	{
		if(upper > lower)
		{
			return (angle <upper) && (angle > lower);
		}else {
			return ((angle <= 360) && (angle > lower) || (angle >= 0) && (angle < upper));
		}
	}

	public static Vector3 correctForDegrees(Vector3 original)
	{
		float x, y, z;

		if (original.x >= 360.0f) 
			x = original.x-360.0f;
		else if( original.x <0)
			x = 360.0f + original.x;
		else
			x = original.x;

		if (original.y >= 360.0f)
			y = original.y - 360.0f;
		else if (original.y < 0.0f)
			y = 360.0f + original.y;
		else
			y = original.y;

		if (original.z >= 360.0f) 
			z = original.z-360.0f;
		else if( original.z <0.0f)
			z = 360.0f + original.z;
		else
			z = original.z;

		return new Vector3(x,y,z);
		
	}


}

