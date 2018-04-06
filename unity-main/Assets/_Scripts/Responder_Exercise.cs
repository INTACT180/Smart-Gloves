using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Responder_Exercise : MonoBehaviour {

	public LevelManager lvlMng;

	public Transform CubeLeft, CubeRight;

	public Text[] xyz;





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

		controller = controllerObj.GetComponent<Controller> ();
		controller.ReadBoth ();

		currentState = States.Pre;

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
			break;

		case States.InProgress:
			//run exercise itteration for each glove

			//

			//Are reps finished? and are sets done? -> Finished state
			//Are reps finished? -> Rest State ++Sets Set Rest Count down
			//continue in progress...
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
		
}
