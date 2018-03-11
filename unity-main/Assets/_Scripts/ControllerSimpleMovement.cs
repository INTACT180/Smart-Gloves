using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSimpleMovement : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		float move = 1f;
		float x=0f,y=0f,z=0f;

		Vector3 translation = new Vector3 ();
		if(Input.GetKey(KeyCode.D))
		{
			x += move;
		}

		if(Input.GetKey(KeyCode.A))
		{
			x -=move;
		}

		if(Input.GetKey(KeyCode.Q))
		{
			z +=move;
		}
		if(Input.GetKey(KeyCode.E))
		{
			z -=move;
		}

		if(Input.GetKey(KeyCode.S))
		{
			y -=move;
		}
		if(Input.GetKey(KeyCode.W))
		{
			y +=move;
		}

		this.transform.position = this.transform.position + new Vector3 (x, y, z);
		

		
		
	}
}
