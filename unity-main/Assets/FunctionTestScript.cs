using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionTestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 currentO = new Vector3 (1, 1, 1);

		Vector3 refO = new Vector3 (350, 350, 350);

		Vector3 bounds = new Vector3 (20, 20, 20);

		areInBoundsCustom (currentO, refO, bounds, bounds);

		Debug.Log (" are we in bounds " + areInBoundsCustom (currentO, refO, bounds, bounds).ToString ());



		
	}

	public static bool areInBoundsCustom(Vector3 currentOrientation, Vector3 originalReferencePoint, Vector3 lowerBounds, Vector3 upperBounds)
	{
		bool inBound = true;

		Vector3 upper = correctForDegrees(originalReferencePoint + upperBounds);
		Vector3 lower = correctForDegrees(originalReferencePoint - lowerBounds);

		Debug.Log ("upper " + upper);
		Debug.Log ("lower " + lower);

		if (!isAngleBetween(upper.x, lower.x, currentOrientation.x))
			inBound = false;
		if (isAngleBetween(upper.y, lower.y, currentOrientation.y))
			inBound = false;
		if (isAngleBetween(upper.z, lower.z, currentOrientation.z))
			inBound = false;

		return inBound;
	}

	public static bool isAngleBetween(float upper, float lower, float angle)
	{
		print ("upper " + upper + " lower " + lower + " angle " + angle);
		if(upper > lower)
		{
			print ("here");
			return (angle <upper) && (angle > lower);
		}else {
			print ("here 2");
			print ("comp one  " + ((angle <360) && (angle > lower)).ToString() + " comp2 " + ((angle > 0) && (angle < upper)).ToString() );
			print ("comp three " + ((angle < 360) && (angle > lower) || (angle > 0) && (angle < upper)).ToString());
			return ((angle <360) && (angle > lower) || (angle > 0) && (angle < upper));
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
