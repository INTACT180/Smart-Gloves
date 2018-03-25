/* This is a simple example to show the steps and one possible way of
 * automatically scanning for and connecting to a device to receive
 * notification data from the device.
 */

using UnityEngine;
using System.Text;
using System;

public class SimpleMultiTest : MonoBehaviour
{
	public string BlueName = "Bluefruit52";
	public string RedName = "Redfruit52";
	public GameObject blueCube;
	public GameObject redCube;
	public string ServiceUUID =         "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
	public string ReadCharacteristic =  "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";
	public string WriteCharacteristic = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";

	enum States
	{
		None,
		Scan,
		ScanRSSI,
		Connect,
		ConnectSecondary,
		Subscribe,
		Send,
		Unsubscribe,
		Disconnect,
	}

	private bool _connected = false;
	private float _timeout = 0f;
	private States _state = States.None;
	private string _deviceAddressBlue;
	private string _deviceAddressRed;
	private bool _foundSubscribeID = false;
	private bool _foundWriteID = false;
	private byte[] _dataBytes = null;
	private bool _rssiOnly = false;
	private int _rssi = 0;
	private int found =0;

	void Reset ()
	{
		_connected = false;
		_timeout = 0f;
		_state = States.None;
		_deviceAddressBlue = null;
		_deviceAddressRed = null;
		_foundSubscribeID = false;
		_foundWriteID = false;
		_dataBytes = null;
		_rssi = 0;
		found = 0;
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
		// here goes the code for automatic connecting
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

						print("Found " + name);
						if (name.Contains (RedName) || name.Contains(BlueName))
						{

							print("Connecting " + name);


							// found a device with the name we want
							// this example does not deal with finding more than one
							if (name.Contains(BlueName))
								_deviceAddressBlue = address;
							else if (name.Contains(RedName))
								_deviceAddressRed = address;

							if ( ++found >= 2){
								SetState (States.Connect, 0.5f);
								BluetoothLEHardwareInterface.StopScan ();
							}
						}

					}, (address, name, rssi, bytes) => {

						// use this one if the device responses with manufacturer specific data and the rssi
						print("Found 2.0  " + name);

						if (name.Contains (RedName) || name.Contains(BlueName))
						{

							print("Connecting " + name);
							
							BluetoothLEHardwareInterface.StopScan ();
							
							// found a device with the name we want
							// this example does not deal with finding more than one
							if (name.Contains(BlueName))
								_deviceAddressBlue = address;
							else if (name.Contains(RedName))
								_deviceAddressRed = address;

							if ( ++found >= 2){
								SetState (States.Connect, 0.5f);
								BluetoothLEHardwareInterface.StopScan ();
							}
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

					print ("Connect Primary running for Blue! " + BlueName);

					// note that the first parameter is the address, not the name. I have not fixed this because
					// of backwards compatiblity.
					// also note that I am note using the first 2 callbacks. If you are not looking for specific characteristics you can use one of
					// the first 2, but keep in mind that the device will enumerate everything and so you will want to have a timeout
					// large enough that it will be finished enumerating before you try to subscribe or do any other operations.
					BluetoothLEHardwareInterface.ConnectToPeripheral (_deviceAddressBlue, null, null, (address, serviceUUID, characteristicUUID) => {

						//print("connecting to device");
						//print(_deviceAddress);
						//print(address);
						print ("connecting to characteristic Blue!" + characteristicUUID);

						/*if (IsEqual (characteristicUUID, WriteCharacteristic)) {
							//SetState (States.None, 0.5f);
						} else */
						if (IsEqual (characteristicUUID, ReadCharacteristic)) {
							//print ("subscribing . . . ");
							print("Blue!: Device Address Blue: " + _deviceAddressBlue + "\n" +
								"Blue: Service UUID: " + serviceUUID + "/n" +
								"Blue: Read Characteristic: " + ReadCharacteristic + "\n");

							BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddressBlue, serviceUUID, ReadCharacteristic, null, (address2, characteristicUUID2, bytes) => {
								print("Blue frame update");
								ReactToInput (bytes, address2);

							});
						}
						SetState (States.ConnectSecondary, 0.5f);
					});

					//SetState (States.ConnectSecondary, 10f);

					break;

				case States.ConnectSecondary:
					_foundSubscribeID = false;
					_foundWriteID = false;

					print ("Connect Secondary running for " + RedName);

					BluetoothLEHardwareInterface.ConnectToPeripheral (_deviceAddressRed, null, null, (address2, serviceUUID2, characteristicUUID2) => {

						//print("connecting to device");
						//print(_deviceAddress);
						//print(address);
						print ("connecting to characteristic " + characteristicUUID2);

						/*if (IsEqual (characteristicUUID2, WriteCharacteristic)) {
							SetState (States.None, 0.5f);
						} else */
						if (IsEqual (characteristicUUID2, ReadCharacteristic)) {
//							print ("subscribing . . . ");
							print("Red!: Device Address Red: " + _deviceAddressRed + "\n" +
								"Red: Service UUID: " + serviceUUID2 + "/n" +
								"Red: Read Characteristic: " + ReadCharacteristic + "\n");
							BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddressRed, serviceUUID2, ReadCharacteristic, null, (address3, characteristicUUID3, bytes) => {
								print("Red frame update");
								ReactToInput (bytes, address3);
							});
						}
					});
					break;

				case States.Subscribe:

					print ("waiting for input. . . ");
					BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddressBlue, ServiceUUID, ReadCharacteristic, null, (address, characteristicUUID, bytes) => {

						print("input recieved");
						// we don't have a great way to set the state other than waiting until we actually got
						// some data back. For this demo with the rfduino that means pressing the button
						// on the rfduino at least once before the GUI will update.
						_state = States.None;

						// we received some data from the device
						_dataBytes = bytes;
					});
					BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddressRed, ServiceUUID, ReadCharacteristic, null, (address, characteristicUUID, bytes) => {

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
					BluetoothLEHardwareInterface.UnSubscribeCharacteristic (_deviceAddressBlue, ServiceUUID, ReadCharacteristic, null);
					// no red support
					SetState (States.Disconnect, 4f);
					break;

				case States.Disconnect:
					if (_connected)
					{
						// no red support
						BluetoothLEHardwareInterface.DisconnectPeripheral (_deviceAddressBlue, (address) => {
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
		//TODO: need to specify who we are sending what to
		byte[] data = Encoding.ASCII.GetBytes(s);
		print("Sending String");
		BluetoothLEHardwareInterface.WriteCharacteristic (_deviceAddressBlue, ServiceUUID, WriteCharacteristic, data, data.Length, true, (characteristicUUID) => {
			BluetoothLEHardwareInterface.Log ("Write Succeeded");
		});
	}

	void ReadString()
	{
		//TODO: adapt for multiple config
		BluetoothLEHardwareInterface.ReadCharacteristic (_deviceAddressBlue, ServiceUUID, ReadCharacteristic, (characteristicUUID, bytes) => {

			print(Encoding.ASCII.GetString(bytes));

			BluetoothLEHardwareInterface.Log ("read Succeeded");
		});
	}

	void ReactToInput(byte[] bytes, string address)
	{
		if(bytes[0] == 'O')
		{
			float x = BitConverter.ToSingle (bytes, 1);
			float y = BitConverter.ToSingle (bytes, 5);
			float z = BitConverter.ToSingle (bytes, 9);
			print (x + "\n" + y + "\n" + z);

			Vector3 target = new Vector3(-y, z, -x);

			if (address.Equals (_deviceAddressBlue))
				blueCube.transform.eulerAngles = target;
			else if (address.Equals (_deviceAddressRed))
				redCube.transform.eulerAngles = target;
			else
				print (address + " XXXXX " + _deviceAddressBlue);

		}
	}

}
