using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Responder_Test : MonoBehaviour {

	public LevelManager lvlMng;

	public Transform Cube;





	enum Hand {
		Left,
		Right
	};

	public enum Exercise{
		Bicep_Curl,
		Bench_Press
	}

	public Exercise Routine;

	Hand currentHand;


	Controller controller;


	// Use this for initialization
	void Start () {
		GameObject controllerObj = GameObject.Find ("Controller");

		controller = controllerObj.GetComponent<Controller> ();

		if (controller.ModifyLeft) {
			controller.ReadLeft ();
			currentHand = Hand.Left;
		}else {
			controller.ReadRight ();
			currentHand = Hand.Right;
		}



	}

	public void Back_Button()
	{
		controller.ReadStop ();
		lvlMng.LoadLevel ("GloveConfig");
	}

	// Update is called once per frame
	void Update () {

		if (currentHand == Hand.Left) {
			Cube.transform.eulerAngles = controller.OrientationLeft;
		} else {
			Cube.transform.eulerAngles = controller.OrientationRight;
		}

	}
}
