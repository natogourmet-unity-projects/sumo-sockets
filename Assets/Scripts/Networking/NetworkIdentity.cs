using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;
using UnityEngine.UI;

public class NetworkIdentity : MonoBehaviour {

    [Header("Helpful Values")]
    [SerializeField]
    [GreyOut]
    public string id;
    [SerializeField]
    [GreyOut]
    private bool isBeingControlled; // if true, it is being controlled by us
    private SocketIOComponent socket;
    public Animator animator;
    public Text name;

    public Vector3 lastPosition;

    void Awake () {
        isBeingControlled = false;
	}
	
	public void SetControllerID (string ID) {
        id = ID;
        //Check incoming ID and the one we have saved from the server
        isBeingControlled = (NetworkClient.clientID == ID) ? true : false;
	}

    public void SetSocketReference(SocketIOComponent _socket)
    {
        socket = _socket;
    }

    public string GetId()
    {
        return id;
    }

    public bool IsBeingControlled()
    {
        return isBeingControlled;
    }

    public SocketIOComponent GetSocket()
    {
        return socket;
    }

    public IEnumerator SetPosition(Vector3 position)
    {
        yield return new WaitForSeconds(0);
        bool b = lastPosition != position;
        animator.SetBool("isWalking", b);
        lastPosition = position;
    }
}
