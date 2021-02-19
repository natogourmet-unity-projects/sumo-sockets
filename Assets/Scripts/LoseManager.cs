using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseManager : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
	    bool isBeingControlled = other.GetComponent<NetworkIdentity>().IsBeingControlled();
        if(isBeingControlled)
        {
            CameraFollow cm = Camera.main.GetComponent<CameraFollow>();
            cm.target = NetworkClient.instance.ring;
            cm.offset *= 3;
            ArenaManager.instance.OnPlayerLose();
        }
        
        Destroy(other.gameObject);
        ArenaManager.instance.OnPlayerFall();
    }
}
