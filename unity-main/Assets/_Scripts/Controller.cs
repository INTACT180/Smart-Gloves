/* This is a simple example to show the steps and one possible way of
 * automatically scanning for and connecting to a device to receive
 * notification data from the device.
 */

using UnityEngine;
using System.Text;
using System;
using System.Timers;

public class Controller : MonoBehaviour
{

	private static Controller instance;

	public string LeftName  = "Bluefruit52";
	public string RightName = "Redfruit52";
	public GameObject blueCube;
	public GameObject redCube;
	public string ServiceUUID =         "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
	public string ReadCharacteristic =  "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";
	public string WriteCharacteristic = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";

	Timer leftTimer;
	Timer rightTimer;



	public Vector3 OrientationLeft   = new Vector3();
	public Vector3 OrientationRight  = new Vector3();

	public Vector3 AccelerationLeft  = new Vector3 ();
	public Vector3 AccelerationRight = new Vector3 ();

	public bool ConnectedLeft	= false;
	public bool ConnectedRight   = false;

	public bool ModifyLeft;

	public bool reading = false;

	enum States
	{
		None,
		Scan,
		ScanRSSI,
		Connect,
		CheckConnected,
		CheckResultsOfConnection,
		ConnectPrimary,
		ConnectSecondary,
		Subscribe,
		Send,
		Unsubscribe,
		DisconnectLeft,
		DisconnectRight,
		ReadLeft,
		ReadRight,
		ReadBoth
	}

	private bool _connected = false;
	private bool subscribedLeft = false;
	private bool subscribedRight = false;
	private float _timeout = 0f;
	private States _state = States.None;

	private string storedDeviceAddressLeft;
	private string storedDeviceAddressRight;

	private string _deviceAddressLeft;
	private string _deviceAddressRight;
	private string[] services;
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
	void Awake(){	
		if (instance == null) {
			GameObject.DontDestroyOnLoad (gameObject);
			instance = this;
		} else {
			Destroy (gameObject);
			print ("Duplicate controller player destroyed");
		}
	}
		
	// Use this for initialization
	void Start ()
	{
		StartProcess ();

		leftTimer = new System.Timers.Timer();
		leftTimer.Elapsed+=new ElapsedEventHandler(TimerLeftExpired);
		leftTimer.Interval=250;
		leftTimer.Enabled=false;

		rightTimer = new System.Timers.Timer();
		rightTimer.Elapsed+=new ElapsedEventHandler(TimerRightExpired);
		rightTimer.Interval=250;
		rightTimer.Enabled=false;

		LeftName = "";
		storedDeviceAddressLeft = _deviceAddressLeft  = PlayerPrefs.GetString ("Left Address");

		RightName = "";
		storedDeviceAddressRight = _deviceAddressRight = PlayerPrefs.GetString ("Right Address");
		string[] services = { ServiceUUID };
		this.services = services;

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_timeout > 0f) {
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f) {
				_timeout = 0f;

				string state = _state.ToString ();
				print ("running state " + state);// + state); //+ _state.ToString ());// + _state.ToString());

				switch (_state) {
				case States.None:
					break;

				case States.Scan:
					print ("Scan running");

					if (ConnectedLeft == null || !ConnectedLeft) {
						_deviceAddressLeft = "";
					}

					if (ConnectedRight == null || !ConnectedRight) {
						_deviceAddressRight = "";
					}

					print ("Scan running");
					BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (null, (address, name) => {

						// if your device does not advertise the rssi and manufacturer specific data
						// then you must use this callback because the next callback only gets called
						// if you have manufacturer specific data

						print ("Found " + name);
						if (!name.Contains ("fruit")) {
							
						} else if (string.IsNullOrEmpty (storedDeviceAddressLeft) && string.IsNullOrEmpty (storedDeviceAddressRight)) {
							storedDeviceAddressLeft = address;
							_deviceAddressLeft = address;
							LeftName = name;
							++found;
						} else if (string.IsNullOrEmpty (storedDeviceAddressRight) && !address.Equals (storedDeviceAddressLeft)) {
							storedDeviceAddressRight = address;
							_deviceAddressRight = address;
							RightName = name;
							++found;
						} else if (string.IsNullOrEmpty (storedDeviceAddressLeft) && !address.Equals (storedDeviceAddressRight)) {
							storedDeviceAddressLeft = address;
							_deviceAddressLeft = address;
							LeftName = name;
							++found;
						} else if (address.Equals (storedDeviceAddressLeft) && ConnectedLeft != null && !ConnectedLeft) {
							LeftName = name;
							_deviceAddressLeft = address;
							++found;

						} else if (address.Equals (storedDeviceAddressRight) && ConnectedRight != null && !ConnectedRight) {
							RightName = name;
							_deviceAddressRight = address;
							++found;
						}
						if (found >= 2 || _timeout < -4) {
							found = 0; 
							SetState (States.Connect, 0.5f);
							BluetoothLEHardwareInterface.StopScan ();
						}



					}, (address, name, rssi, bytes) => {

						print ("Found " + name);
						if (string.IsNullOrEmpty (storedDeviceAddressLeft) && string.IsNullOrEmpty (storedDeviceAddressRight)) {
							storedDeviceAddressLeft = address;
							_deviceAddressLeft = address;
							LeftName = name;
							++found;
						} else if (string.IsNullOrEmpty (storedDeviceAddressRight) && !address.Equals (storedDeviceAddressLeft)) {
							storedDeviceAddressRight = address;
							_deviceAddressRight = address;
							RightName = name;
							++found;
						} else if (string.IsNullOrEmpty (storedDeviceAddressLeft) && !address.Equals (storedDeviceAddressRight)) {
							storedDeviceAddressLeft = address;
							_deviceAddressLeft = address;
							LeftName = name;
							++found;
						} else if (address.Equals (storedDeviceAddressLeft) && ConnectedLeft != null && !ConnectedLeft) {
							LeftName = name;
							_deviceAddressLeft = address;
							++found;

						} else if (address.Equals (storedDeviceAddressRight) && ConnectedRight != null && !ConnectedRight) {
							RightName = name;
							_deviceAddressRight = address;
							++found;
						}
						if (found >= 2 || _timeout < -4) {
							found = 0; 
							SetState (States.Connect, 0.5f);
							BluetoothLEHardwareInterface.StopScan ();
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

					print ("Connect Primary running for Left! " + LeftName);

					print ("Scott12: Connected LEFT: " + ConnectedLeft + " Connected Right " + ConnectedRight + " Device Address Left: " + _deviceAddressLeft + " Device Address Right: " + _deviceAddressRight);

					if (ConnectedLeft == null)
						ConnectedLeft = false;
					if (ConnectedLeft || string.IsNullOrEmpty (_deviceAddressLeft)) {
						SetState (States.ConnectSecondary, 0.1f);
						return;
					}

					BluetoothLEHardwareInterface.ConnectToPeripheral (_deviceAddressLeft, (address) => {
					},
						(address, serviceUUID) => {
						},
						(address, serviceUUID, characteristicUUID) => {

							print ("connecting to characteristic Blue!" + characteristicUUID);
							if (IsEqual (characteristicUUID, ReadCharacteristic)) {
								//print ("subscribing . . . ");
								print ("Blue!: Device Address Blue: " + _deviceAddressLeft + "\n" +
								"Blue: Service UUID: " + serviceUUID + "/n" +
								"Blue: Read Characteristic: " + ReadCharacteristic + "\n");

//							BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddressLeft, serviceUUID, ReadCharacteristic, null, (address2, characteristicUUID2, bytes) => {
//								print ("Blue frame update");
//								ReactToInput (bytes, address2);
//
								SetState (States.ConnectSecondary, 0.5f);
								ConnectedLeft = true;
							}
						}, (address) => {

						// this will get called when the device disconnects
						// be aware that this will also get called when the disconnect
						// is called above. both methods get call for the same action
						// this is for backwards compatibility
							if(address.Equals(_deviceAddressLeft)){
								ConnectedLeft = false;
								subscribedLeft =false;
								_deviceAddressLeft = "";
							}else if( address.Equals(_deviceAddressRight))
							{
								ConnectedRight = false;
								subscribedRight = false;
								_deviceAddressRight = "";
							}
					});

					break;

				case States.ConnectPrimary:
					// set these flags
					print ("Connect Primary running for Left! " + LeftName);

					if (ConnectedLeft == null)
						ConnectedLeft = false;
					if (ConnectedLeft || string.IsNullOrEmpty (_deviceAddressLeft)) {
						return;
					}

					BluetoothLEHardwareInterface.ConnectToPeripheral (_deviceAddressLeft, (address) => {
					},
						(address, serviceUUID) => {
						},
						(address, serviceUUID, characteristicUUID) => {

							print ("connecting to characteristic Blue!" + characteristicUUID);
							if (IsEqual (characteristicUUID, ReadCharacteristic)) {
								//print ("subscribing . . . ");
								print ("Blue!: Device Address Blue: " + _deviceAddressLeft + "\n" +
								"Blue: Service UUID: " + serviceUUID + "/n" +
								"Blue: Read Characteristic: " + ReadCharacteristic + "\n");

								//							BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddressLeft, serviceUUID, ReadCharacteristic, null, (address2, characteristicUUID2, bytes) => {
								//								print ("Blue frame update");
								//								ReactToInput (bytes, address2);
								//
								//							SetState (States.ConnectSecondary, 0.5f);
								ConnectedLeft = true;
							}
						}, (address) => {

							// this will get called when the device disconnects
							// be aware that this will also get called when the disconnect
							// is called above. both methods get call for the same action
							// this is for backwards compatibility
							if(address.Equals(_deviceAddressLeft)){
								ConnectedLeft = false;
								subscribedLeft =false;
								_deviceAddressLeft = "";
							}else if( address.Equals(_deviceAddressRight))
							{
								ConnectedRight = false;
								subscribedRight = false;
								_deviceAddressRight = "";
							}

					});

					break;

				case States.ConnectSecondary:
					_foundSubscribeID = false;
					_foundWriteID = false;

					print ("Connect Secondary running for " + RightName);

					if (ConnectedRight|| string.IsNullOrEmpty (_deviceAddressRight)) {
						//TODO: hook this up to normal mode
						return;
					}

					BluetoothLEHardwareInterface.ConnectToPeripheral (_deviceAddressRight, null, null, (address2, serviceUUID2, characteristicUUID2) => {

						//print("connecting to device");
						//print(_deviceAddress);
						//print(address);
						print ("connecting to characteristic " + characteristicUUID2);

						/*if (IsEqual (characteristicUUID2, WriteCharacteristic)) {
							SetState (States.None, 0.5f);
						} else */
						if (IsEqual (characteristicUUID2, ReadCharacteristic)) {
//							print ("subscribing . . . ");
							print ("Red!: Device Address Red: " + _deviceAddressRight + "\n" +
							"Red: Service UUID: " + serviceUUID2 + "/n" +
							"Red: Read Characteristic: " + ReadCharacteristic + "\n");
//							BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddressRight, serviceUUID2, ReadCharacteristic, null, (address3, characteristicUUID3, bytes) => {
//								print ("Red frame update");
//								ReactToInput (bytes, address3);
//							});
							print("connected right");
							ConnectedRight = true;

							//TODO: hook this up to normal mode
						}
					}, (address) => {

						// this will get called when the device disconnects
						// be aware that this will also get called when the disconnect
						// is called above. both methods get call for the same action
						// this is for backwards compatibility

						if(address.Equals(_deviceAddressLeft)){
							ConnectedLeft = false;
							subscribedLeft =false;
							_deviceAddressLeft = "";
						}else if( address.Equals(_deviceAddressRight))
						{
							ConnectedRight = false;
							subscribedRight = false;
							_deviceAddressRight = "";
						}
					});
					break;

				case States.Subscribe:

					if (ConnectedLeft && !subscribedLeft) 
					{
						BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddressLeft, ServiceUUID, ReadCharacteristic, null, (address, characteristicUUID, bytes) => {

							print ("input recieved");
							// we don't have a great way to set the state other than waiting until we actually got
							// some data back. For this demo with the rfduino that means pressing the button
							// on the rfduino at least once before the GUI will update.
							_state = States.None;

							// we received some data from the device
							_dataBytes = bytes;
						});
						subscribedLeft = true;
					}

					if (ConnectedRight && !subscribedRight) 
					{
						BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (_deviceAddressRight, ServiceUUID, ReadCharacteristic, null, (address, characteristicUUID, bytes) => {

							print ("input recieved");
							// we don't have a great way to set the state other than waiting until we actually got
							// some data back. For this demo with the rfduino that means pressing the button
							// on the rfduino at least once before the GUI will update.
							_state = States.None;

							// we received some data from the device
							_dataBytes = bytes;
						});
						subscribedRight = true;
					}
					break;
				
				case States.ReadLeft:
					BluetoothLEHardwareInterface.ReadCharacteristic (_deviceAddressLeft, ServiceUUID, ReadCharacteristic, ( characteristicUUID, bytes) => {
						ReactToInput( bytes,_deviceAddressLeft);
						if(reading)
							SetState (States.ReadLeft, 0.01f);
					});
					//SetState (States.ReadLeft, 0.01f);
					break;
				
				case States.ReadRight:
					BluetoothLEHardwareInterface.ReadCharacteristic (_deviceAddressRight, ServiceUUID, ReadCharacteristic, ( characteristicUUID, bytes) => {
						ReactToInput ( bytes, _deviceAddressRight);
						if(reading)
							SetState (States.ReadRight, 0.01f);
					});
					//SetState (States.ReadRight, 0.01f);
					break;

				case States.ReadBoth:
					BluetoothLEHardwareInterface.ReadCharacteristic (_deviceAddressRight, ServiceUUID, ReadCharacteristic, ( characteristicUUID, bytes) => {
						ReactToInput ( bytes, _deviceAddressRight);
						BluetoothLEHardwareInterface.ReadCharacteristic (_deviceAddressLeft, ServiceUUID, ReadCharacteristic, ( charUID, bits) => {
							ReactToInput( bits,_deviceAddressLeft);
							if(reading)
								SetState (States.ReadBoth, 0.001f);
						});
					});
					//SetState (States.ReadBoth, 0.02f);
					break;


				case States.Unsubscribe:
					BluetoothLEHardwareInterface.UnSubscribeCharacteristic (_deviceAddressLeft, ServiceUUID, ReadCharacteristic, null);
					// no red support
					SetState (States.DisconnectLeft, 4f);
					break;

				case States.DisconnectLeft:
					BluetoothLEHardwareInterface.DisconnectPeripheral (_deviceAddressLeft, (address) => {});
					break;
				case States.DisconnectRight:
					BluetoothLEHardwareInterface.DisconnectPeripheral (_deviceAddressRight, (address) => {});
					break;

				}
			}
		} else {
			_timeout -= Time.deltaTime;

			if (_state == States.Scan && _timeout < -5.0f) {
				BluetoothLEHardwareInterface.StopScan ();
				SetState(States.Connect, 0.1f);
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

	public void SendStringLeft (string s)
	{
		//TODO: need to specify who we are sending what t
		if (leftTimer.Enabled)
			return;
		leftTimer.Enabled = true;

		byte[] data = Encoding.ASCII.GetBytes(s);
		print("Sending String");
		BluetoothLEHardwareInterface.WriteCharacteristic (_deviceAddressLeft, ServiceUUID, WriteCharacteristic, data, data.Length, true, (characteristicUUID) => {
			BluetoothLEHardwareInterface.Log ("Write Succeeded");
		});
	}
	public void SendStringRight (string s)
	{
		if (rightTimer.Enabled)
			return;
		rightTimer.Enabled = true;

		//TODO: need to specify who we are sending what t
		byte[] data = Encoding.ASCII.GetBytes(s);
		print("Sending String");
		BluetoothLEHardwareInterface.WriteCharacteristic (_deviceAddressRight, ServiceUUID, WriteCharacteristic, data, data.Length, true, (characteristicUUID) => {
			BluetoothLEHardwareInterface.Log ("Write Succeeded");
		});
	}

	public void SendStringBoth (string s)
	{
		if (leftTimer.Enabled) {
			SendStringRight (s);
			return;
		}
		leftTimer.Enabled = true;

		//TODO: need to specify who we are sending what t
		byte[] data = Encoding.ASCII.GetBytes(s);
		print("Sending String");
		BluetoothLEHardwareInterface.WriteCharacteristic (_deviceAddressLeft, ServiceUUID, WriteCharacteristic, data, data.Length, true, (characteristicUUID) => {
			SendStringRight(s);
		});
	}

	void ReadString()
	{
		//TODO: adapt for multiple config
		BluetoothLEHardwareInterface.ReadCharacteristic (_deviceAddressLeft, ServiceUUID, ReadCharacteristic, (characteristicUUID, bytes) => {

			print(Encoding.ASCII.GetString(bytes));

			BluetoothLEHardwareInterface.Log ("read Succeeded");
		});
	}

	void ReactToInput(byte[] bytes, string address)
	{
		if(bytes[0] == 'O')
		{
			UInt16 tempX = BitConverter.ToUInt16 (bytes, 1);
			UInt16 tempY = BitConverter.ToUInt16 (bytes, 3);
			UInt16 tempZ = BitConverter.ToUInt16 (bytes, 5);

			float accX   = BitConverter.ToSingle (bytes, 7);
			float accY   = BitConverter.ToSingle (bytes, 11);
			float accZ   = BitConverter.ToSingle (bytes, 15);

			float x = ((float)tempX) / 100.0f;
			float y = ((float)tempY) / 100.0f;
			float z = ((float)tempZ) / 100.0f;
			//print (x + "\n" + y + "\n" + z);

			Vector3 target = new Vector3(z,y,x);
			Vector3 targetAcc = new Vector3 (accX, accY, accZ);

			if (address.Equals (_deviceAddressLeft)) {
				OrientationLeft = target;
				AccelerationLeft = targetAcc;
			} else if (address.Equals (_deviceAddressRight)) {
				OrientationRight = target;
				AccelerationRight = targetAcc;
			}else
				print (address + " XXXXX " + _deviceAddressLeft);

		}
	}

	public void disconnectLeft(){
		if(ConnectedLeft)
			SetState (States.DisconnectLeft, 0.1f);
	}
	public void disconnectRight(){
		if(ConnectedRight)
			SetState (States.DisconnectRight, 0.1f);
	}

	public void connectToNew(string name, string address)
	{
		if (ModifyLeft) {
			storedDeviceAddressLeft = _deviceAddressLeft = address;
			LeftName = name;

			string rightHandTemp = PlayerPrefs.GetString ("Right Address");
			if(address.Equals(rightHandTemp)){
				PlayerPrefs.DeleteKey("Right Address");
				storedDeviceAddressRight = "";
			}

			PlayerPrefs.SetString ("Left Address", address);
			SetState (States.Connect, 0.1f);
		} else {
			storedDeviceAddressRight = _deviceAddressRight = address;
			RightName = name;

			string leftHandTemp = PlayerPrefs.GetString ("Left Address");
			if(address.Equals(leftHandTemp)){
				PlayerPrefs.DeleteKey("Left Address");
				storedDeviceAddressLeft = "";
			}

			PlayerPrefs.SetString ("Right Address", address);
			SetState (States.ConnectSecondary, 0.1f);
		}

		PlayerPrefs.Save ();
	}

	public void ReadLeft()
	{
		if (ConnectedLeft) {
			reading = true;
			SetState (States.ReadLeft, 0.1f);
		}

	}

	public void ReadRight()
	{
		if (ConnectedRight) {
			reading = true;
			SetState (States.ReadRight, 0.1f);
		}

	}

	public void ReadBoth()
	{
		if (ConnectedLeft && ConnectedRight) {
			reading = true;
			SetState (States.ReadBoth, 0.1f);
		}
	}


	public void ReadStop()
	{
		reading = false;
		SetState (States.None, 0.1f);
	}

	void TimerLeftExpired(object source, ElapsedEventArgs e)
	{
		leftTimer.Enabled = false;
	}
		void TimerRightExpired(object source, ElapsedEventArgs e)
	{
		rightTimer.Enabled = false;
	}

}
