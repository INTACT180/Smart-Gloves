using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Responder_Test : MonoBehaviour {

	public LevelManager lvlMng;

	public Transform Cube;

	public Text[] xyz;





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

		Vector3 temp;

		if (currentHand == Hand.Left) {
			Cube.transform.eulerAngles = controller.OrientationLeft;
			temp = controller.AccelerationLeft;
		} else {
			Cube.transform.eulerAngles = controller.OrientationRight;
			temp = controller.AccelerationRight;
		}

		xyz [0].text = "Or X: " + Cube.transform.eulerAngles.x.ToString ();
		xyz [1].text = "Or Y: " + Cube.transform.eulerAngles.y.ToString ();
		xyz [2].text = "Or Z: " + Cube.transform.eulerAngles.z.ToString ();

		xyz [3].text = " Ac X: " + temp.x.ToString();
		xyz [4].text = " Ac Y: " + temp.y.ToString();
		xyz [5].text = " Ac Z: " + temp.z.ToString();




	}
}
