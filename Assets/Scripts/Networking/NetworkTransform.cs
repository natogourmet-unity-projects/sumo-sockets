using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkTransform : MonoBehaviour {

    [SerializeField]
    [GreyOut]
    private Vector3 oldPosition;
    private NetworkIdentity networkIdentity;
    private NetworkClient.Player player;
    private float stillCounter = 0;

	void Start () {
        networkIdentity = GetComponent<NetworkIdentity>();
        oldPosition = transform.position;
        player = NetworkClient.localPlayer;
        player.position = new NetworkClient.Position();
        player.rotation = new NetworkClient.Rotation();

        if (!networkIdentity.IsBeingControlled())
        {
            enabled = false;
            return;
        }
        Camera.main.GetComponent<CameraFollow>().target = gameObject.transform;

    }
	
	void Update () {
		if(networkIdentity.IsBeingControlled())
        {
            if (oldPosition != transform.position)
            {
                oldPosition = transform.position;
                stillCounter = 0;
                SendData();
            }
            else
            {
                //It's going to tell the server where I am every second if i'm doing nothing
                stillCounter += Time.deltaTime;
                if (stillCounter >= 1)
                {
                    stillCounter = 0;
                    SendData();
                }
            }
        }
	}   

    public void SendData()
    {
        player.position.x = Mathf.Round(transform.position.x * 1000.0f) / 1000.0f;
        player.position.y = Mathf.Round(transform.position.y * 1000.0f) / 1000.0f;
        player.position.z = Mathf.Round(transform.position.z * 1000.0f) / 1000.0f;

        player.rotation.x = Mathf.Round(transform.rotation.eulerAngles.x * 1000.0f) / 1000.0f;
        player.rotation.y = Mathf.Round(transform.rotation.eulerAngles.y * 1000.0f) / 1000.0f;
        player.rotation.z = Mathf.Round(transform.rotation.eulerAngles.z * 1000.0f) / 1000.0f;
        networkIdentity.GetSocket().Emit("updatePosition", new JSONObject(JsonUtility.ToJson(player)));
    }
}
