using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour {

    public static LobbyController instance;
    public Dictionary<string, LobbyPlayer> lobbyPlayers;
    public Transform panelContainer;
    public GameObject lobbyPlayer;
    public GameObject buttonReady;
    public GameObject buttonStart;

    void Awake () {
        
        lobbyPlayers = new Dictionary<string, LobbyPlayer>();
        
		if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
	}

    public void SetButtons(bool isHost)
    {
        buttonStart.SetActive(isHost);
        buttonReady.SetActive(!isHost);
    }

    public void SetPlayerReady(string id, bool rdy)
    {
        lobbyPlayers[id].SetColor(rdy ? 1 : 0);
    }

    public void DisconnectPlayer(string id)
    {
        DestroyImmediate(lobbyPlayers[id].gameObject);
        lobbyPlayers.Remove(id);
    }

    public IEnumerator InstantiateLobbyPlayer(string id, string nick, bool isMe, bool isHost)
    {
        GameObject go = Instantiate(lobbyPlayer, panelContainer);
        yield return new WaitForEndOfFrame();
        LobbyPlayer lp = go.GetComponent<LobbyPlayer>();
        
        if (isMe) lp.SetName(nick + " - Me");
        else lp.SetName(nick);
        
        if (isHost) lp.SetColor(2);
        
        lobbyPlayers.Add(id, lp);
    }
}
