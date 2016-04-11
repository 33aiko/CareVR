using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent (typeof (NetworkView))]
public class ClientBehaviour : MonoBehaviour {
	public string IP = "";
	public int port = 12321;

	NetworkView mView;

	TouchScreenKeyboard keyboard = null; 

	//network status properties
	public bool Connecting{get;private set;}
	public bool Connected{get;private set;}

	[RPC]
	void RecieveNetworkMessage(string aMsg){
		DebugConsole.Log(aMsg);
	}
		
	[RPC]
	void RecieveObject(string aObject){
	}

	void SendNetworkMessage(string aMsg){
		mView.RPC("RecieveNetworkMessage", RPCMode.All, new object[]{aMsg});

	}

	public void SendObject(string aObject){
		mView.RPC("RecieveObject", RPCMode.Others, new object[]{aObject});
		DebugConsole.Log("send:"+aObject);
	}

	void ClientConnect()
	{
		DebugConsole.Log("trying to connect to server " + IP + " on port " + port);
		Connecting = true;
		var error = Network.Connect(IP, port); //try and connect
		if (error != NetworkConnectionError.NoError)
		{
			Connecting = false;
			DebugConsole.Log("Connection error " + error.ToString());
		}
	}

	void InitializeIpEntry()
	{
		DebugConsole.Log("getting ip via touchscreen keyboard");
		keyboard = TouchScreenKeyboard.Open(IP,TouchScreenKeyboardType.NumbersAndPunctuation);
	}

	// Use this for initialization
	void Start () {
		mView = gameObject.GetComponent<NetworkView>();
		mView.stateSynchronization = NetworkStateSynchronization.Off; 
		InitializeIpEntry();
	}
	
	// Update is called once per frame
	void Update () {
		if(keyboard != null && keyboard.done)
		{
			IP = keyboard.text;
			DebugConsole.Log("IP set to " + IP);
			keyboard = null;
			ClientConnect();
		}

		//EXAMPLE, send a mesasge whenever we register a touch
//		if (Input.GetMouseButtonDown(0))
//		{
//			SendNetworkMessage("touch registered on client");
//		}
	
	}



	public void OnFailedToConnect()
	{
		DebugConsole.Log("Failed to connect... trying again");
		InitializeIpEntry(); //try to reconnect, this will allow you do launch the client befoer you launch the server among other things
	}

	//this is called on the CLIENT when it succesfully connects to the server
	public void OnConnectedToServer()
	{
		Connecting = false;
		Connected = true;
		DebugConsole.Log("Connected to server");
		SendNetworkMessage("Hi, I just connected to your wonderful server");
	}

	//this is called on the CLIENT when it succesfully connects to the server
	public void OnDisconnectedFromServer(NetworkDisconnection info) 
	{
		Connected = false;
		DebugConsole.Log("Disconnected from server");
		InitializeIpEntry(); //try to reconnect
	}
}
