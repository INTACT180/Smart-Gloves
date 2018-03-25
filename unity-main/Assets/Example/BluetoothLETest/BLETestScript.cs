using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BLETestScript : MonoBehaviour
{
	public Transform PanelActive;
	public Transform PanelInActive;
	public Transform PanelCentral;

//	public CentralScript PanelCentral;

	static private BLETestScript _bleTestSCript;
	static public void Show (Transform panel)
	{
		if (_bleTestSCript == null)
		{
			GameObject gameObject = GameObject.Find ("Canvas");
			if (gameObject != null)
				_bleTestSCript = gameObject.GetComponent<BLETestScript> ();
		}

		if (_bleTestSCript != null)
		{
			while (_bleTestSCript.PanelActive.childCount > 0)
				_bleTestSCript.PanelActive.GetChild (0).transform.SetParent (_bleTestSCript.PanelInActive);

			panel.SetParent (_bleTestSCript.PanelActive);
		}
	}

	// Use this for initialization
	void Start ()
	{
		BusyScript.IsBusy = false;
		Show (PanelCentral);





	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
