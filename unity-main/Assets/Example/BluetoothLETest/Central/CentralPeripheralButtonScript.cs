using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CentralPeripheralButtonScript : MonoBehaviour
{
	public CentralRFduinoScript PanelCentralRFduino;
	public CentralTISensorTagScript PanelCentralTISensorTag;
	public CentralNordicScript PanelCentralNordic;
	public Transform PanelInactive;
	public Text TextName;
	public Text TextAddress;

	Controller controller;
	
	public void OnPeripheralSelected ()
	{
		if (TextName.text.Contains ("fruit"))
		{
			//wat
			BluetoothLEHardwareInterface.StopScan ();
			controller.connectToNew (TextName.text, TextAddress.text);
			SceneManager.LoadScene ("GloveConfig");
		}

	}
	
	// Use this for initialization
	void Start ()
	{
		GameObject controllerObj = GameObject.Find ("Controller");
		controller = controllerObj.GetComponent<Controller> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
