using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent (typeof (NetworkView))]
public class ServerBehaviour : MonoBehaviour {

	public int port = 12321;

	NetworkView mView;

	private GameObject vase;


	[RPC]
	void RecieveNetworkMessage(string aMsg){
		DebugConsole.Log(aMsg);
	}

	[RPC]
	void RecieveObject(string aObject){
		DebugConsole.Log(aObject);
		vase = GameObject.Find (aObject);
		if (vase != null) {
			DebugConsole.Log("I have a vase");
		}
		vase.GetComponent<MeshRenderer> ().enabled = true;
	}


	void SendNetworkMessage(string aMsg){
		mView.RPC("RecieveNetworkMessage", RPCMode.All, new object[]{aMsg});
	}

	void Start () {
		mView = gameObject.GetComponent<NetworkView> ();
		mView.stateSynchronization = NetworkStateSynchronization.Off;

		DebugConsole.Log("initializing server");

		//Initialize the server
		Network.InitializeServer (1, port, false);
	
	}
	

	void Update () {

		if (Input.GetKeyDown ("a")) {
			SendNetworkMessage("I pressed A");
		}
	}

	public void OnPlayerConnected(NetworkPlayer aPlayer){
		DebugConsole.Log(aPlayer.ipAddress + "connected");
		SendNetworkMessage ("Hi, welcome to my server");
	}

	public void OnPlayerDisconnected (NetworkPlayer aPlayer){
		DebugConsole.Log(aPlayer.ipAddress + "disconnected :(");
	}


}
