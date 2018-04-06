//using System.Collections;
//using System;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//
//public class Responder_Settings : MonoBehaviour {
//
//	public Text LeftGloveStatus;
//	public Text RightGloveStatus;
//	public Button[] LeftButtons;
//	public Button[] RightButtons;
//	public LevelManager lvlMng;
//
//	Boolean currentLeftStatus;
//	Boolean currentRightStatus;
//
//	enum Hand {
//		Left,
//		Right
//	};
//
//
//	Controller controller;
//
//
//	public void Modify(bool isLeft)
//	{
//		controller.ModifyLeft = isLeft;
//		if (isLeft && currentLeftStatus) {
//			controller.disconnectLeft ();
//		}else if (!isLeft && currentRightStatus) {
//			controller.disconnectRight();
//		}
//		lvlMng.LoadLevel("Scan");
//	}
//
//	public void Disconnect(bool isLeft)
//	{
//		controller.ModifyLeft = isLeft;
//		if (isLeft && currentLeftStatus) {
//			controller.disconnectLeft ();
//		}else if (!isLeft && currentRightStatus) {
//			controller.disconnectRight();
//		}
//	}
//
//	// Use this for initialization
//	void Start () {
//		GameObject controllerObj = GameObject.Find ("Controller");
//
//		controller = controllerObj.GetComponent<Controller> ();
//
//		currentLeftStatus = controller.ConnectedLeft;
//		UpdateHandStatus (Hand.Left, currentLeftStatus);
//
//		currentRightStatus = controller.ConnectedRight;
//		UpdateHandStatus (Hand.Right, currentRightStatus);
//
//	}
//	
//	// Update is called once per frame
//	void Update () {
//
//		if (currentLeftStatus != controller.ConnectedLeft) {
//			currentLeftStatus = controller.ConnectedLeft;
//			UpdateHandStatus (Hand.Left, currentLeftStatus);
//		}
//		if (currentRightStatus != controller.ConnectedRight) {
//			currentRightStatus = controller.ConnectedRight;
//			UpdateHandStatus (Hand.Right, currentRightStatus);
//		}
//		
//	}
//
//	void UpdateHandStatus(Hand hand, Boolean status)
//	{
//		Text glove;
//		Button[] buttons;
//		if (hand == Hand.Left) {
//			glove = LeftGloveStatus;
//			buttons = LeftButtons;
//		}else{
//			glove = RightGloveStatus;
//			buttons = RightButtons;
//		}
//
//		if (status) {
//			glove.text = "Connected";
//			glove.color = Color.green;
//			foreach (Button button in buttons)
//				button.gameObject.SetActive (true); 
//			
//		} else if(!status) {
//			glove.text = "Disconnected";
//			glove.color = Color.red;
//			foreach (Button button in buttons)
//				button.gameObject.SetActive (false); 
//		}
//	}
//
//
//	public void LoadTestScene(bool isLeft)
//	{
//		controller.ModifyLeft = isLeft;
//		lvlMng.LoadLevel ("cube_test");
//	}
//}
