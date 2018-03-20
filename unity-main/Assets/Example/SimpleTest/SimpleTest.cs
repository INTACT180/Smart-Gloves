/* This is a simple example to show the steps and one possible way of
 * automatically scanning for and connecting to a device to receive
 * notification data from the device.
 */

using UnityEngine;
using System.Text;
using System;

public class SimpleTest : MonoBehaviour
{
	public string DeviceName = "Bluefruit52";
	public string ServiceUUID =         "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
	public string ReadCharacteristic =  "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";
	public string WriteCharacteristic = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";
	public float scale = 1.0f;

	public bool position = false;

	enum States
	{
		None,
		Scan,
		ScanRSSI,
		Connect,
		Subscribe,
		Send,
		Unsubscribe,
		Disconnect,
	}

	private bool _connected = false;
	private float _timeout = 0f;
	private float pos_x = 0f;
	private float pos_y = 0f;
	private float pos_z = 0f;

	private States _state = States.None;
	private string _deviceAddress;
	private bool _foundSubscribeID = false;
	private bool _foundWriteID = false;
	private byte[] _dataBytes = null;
	private bool _rssiOnly = false;
	private int _rssi = 0;

	void Reset ()
	{
		_connected = false;
		_timeout = 0f;
		_state = States.None;
		_deviceAddress = null;
		_foundSubscribeID = false;
		_foundWriteID = false;
		_dataBytes = null;
		_rssi = 0;
	}

	void SetState (States newState, float timeout)
	{
		_state = newState;
		_timeout = timeout;
	}

	void StartProcess ()
	{
		Reset ();
		BluetoothLEHardwareInterface.Initialize (true, false, () => {

		SetState (States.Scan , 0.1f);
			
		}, (error) => {
			
			BluetoothLEHardwareInterface.Log ("Error during initialize: " + error);
		});
	}

	// Use this for initialization
	void Start ()
	{
		StartProcess ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_timeout > 0f)
		{
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f)
			{
				_timeout = 0f;

				string state = _state.ToString ();
				print ("running state " + state);// + state); //+ _state.ToString ());// + _state.ToString());

				switch (_state)
				{
				case States.None:
					break;

				case States.Scan:
					print ("Scan running");
					BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (null, (address, name) => {

						// if your device does not advertise the rssi and manufacturer specific data
						// then you must use this callback because the next callback only gets called
						// if you have manufacturer specific data

						print("Found 1.0 " + name);

						print(name);
						print(DeviceName);
						if (name.Contains ("Blue"))
						{

							print("Connecting " + DeviceName);
							BluetoothLEHardwareInterface.StopScan ();

							// found a device with the name we want
							// this example does not deal with finding more than one
							_deviceAddress = address;
							SetState (States.Connect, 0.5f);
						}

					}, (address, name, rssi, bytes) => {

						// use this one if the device responses with manufacturer specific data and the rssi
						print("Found 2.0  " + name);

						if (name.Contains (DeviceName))
						{

							print("Connecting " + name);
							
							BluetoothLEHardwareInterface.StopScan ();
							
							// found a device with the name we want
							// this example does not deal with finding more than one
							_deviceAddress = address;
							SetState (States.Connect, 0.5f);
						}

					}, _rssiOnly); // this last setting allows RFduino to send RSSI without having manufacturer data


					if (_rssiOnly)
						SetState (States.ScanRSSI, 0.5f);
					break;

				case States.ScanRSSI:
					break;

				case States.Send:
					//SendString ("Hello Scott");
					ReadString ();
					SetState (States.Send, 0.5f);
					break;
				

				case States.Connect:
					// set these flags
					_foundSubscribeID = false;
					_foundWriteID = false;

					// note that the first parameter is the address, not the name. I have not fixed this because
					// of backwards compatiblity.
					// also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
					// the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
					// large enough that it will be finished enumerating before you try to subscribe or do any other operations.
					BluetoothLEHardwareInterface.ConnectToPeripheral (_deviceAddress, null, null, (address, serviceUUID, characteristicUUID) => {

						//print("connecting to device");
						//print(_deviceAddress);
						//print(address);
						print("connecting to characteristic " + characteristicUUID);

						if (IsEqual (characteristicUUID, WriteCharacteristic)){
							SetState(States.None,0.5f);
						}else if (IsEqual (characteristicUUID, ReadCharacteristic)){
							print ("subscribing . . . ");
							BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddress, serviceUUID, ReadCharacteristic, null, (address2, characteristicUUID2, bytes) => {
								ReactToInput(bytes);
							});
						}
							

					});
					break;

				case States.Subscribe:

					print ("waiting for input. . . ");
					BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddress, ServiceUUID, ReadCharacteristic, null, (address, characteristicUUID, bytes) => {

						print("input recieved");
						// we don't have a great way to set the state other than waiting until we actually got
						// some data back. For this demo with the rfduino that means pressing the button
						// on the rfduino at least once before the GUI will update.
						_state = States.None;

						// we received some data from the device
						_dataBytes = bytes;
					});
					break;

				case States.Unsubscribe:
					BluetoothLEHardwareInterface.UnSubscribeCharacteristic (_deviceAddress, ServiceUUID, ReadCharacteristic, null);
					SetState (States.Disconnect, 4f);
					break;

				case States.Disconnect:
					if (_connected)
					{
						BluetoothLEHardwareInterface.DisconnectPeripheral (_deviceAddress, (address) => {
							BluetoothLEHardwareInterface.DeInitialize (() => {
								
								_connected = false;
								_state = States.None;
							});
						});
					}
					else
					{
						BluetoothLEHardwareInterface.DeInitialize (() => {
							
							_state = States.None;
						});
					}
					break;
				}
			}
		}
	}

	string FullUUID (string uuid)
	{
		return "0000" + uuid + "-0000-1000-8000-00805f9b34fb";
	}
	
	bool IsEqual(string uuid1, string uuid2)
	{
		if (uuid1.Length == 4)
			uuid1 = FullUUID (uuid1);
		if (uuid2.Length == 4)
			uuid2 = FullUUID (uuid2);
		
		return (uuid1.ToUpper().CompareTo(uuid2.ToUpper()) == 0);
	}

	void SendString (string s)
	{
		byte[] data = Encoding.ASCII.GetBytes(s);
		print("Sending String");
		BluetoothLEHardwareInterface.WriteCharacteristic (_deviceAddress, ServiceUUID, WriteCharacteristic, data, data.Length, true, (characteristicUUID) => {
			BluetoothLEHardwareInterface.Log ("Write Succeeded");
		});
	}

	void ReadString()
	{
		BluetoothLEHardwareInterface.ReadCharacteristic (_deviceAddress, ServiceUUID, ReadCharacteristic, (characteristicUUID, bytes) => {

			print(Encoding.ASCII.GetString(bytes));

			BluetoothLEHardwareInterface.Log ("read Succeeded");
		});
	}

	void ReactToInput(byte[] bytes)
	{



		if (bytes [0] == 'O') {
			float x = BitConverter.ToSingle (bytes, 1);
			float y = BitConverter.ToSingle (bytes, 5);
			float z = BitConverter.ToSingle (bytes, 9);

			//print (x + "\n" + y + "\n" + z);

			Vector3 target = new Vector3 (-y, z, -x);
			transform.eulerAngles = target;



		} else if (position && bytes [0] == 'P') {
			float newPos_x = BitConverter.ToSingle (bytes, 1);
			float newPos_z = BitConverter.ToSingle (bytes, 5);
			float newPos_y = BitConverter.ToSingle (bytes, 9);

			print (newPos_x + "\n" + newPos_y + "\n" + newPos_z);

			Vector3 deltaPosition = new Vector3 (newPos_x - pos_x, newPos_y - pos_y, newPos_z - pos_z);
			deltaPosition *= 50;
			transform.position += deltaPosition;

			pos_x = newPos_x;
			pos_y = newPos_y;
			pos_z = newPos_z;
		}
	}

	public void resetPosition (){
		transform.position = new Vector3 (0f, 0f, 8.21f);
	}

}
