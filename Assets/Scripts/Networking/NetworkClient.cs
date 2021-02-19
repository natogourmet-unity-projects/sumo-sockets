using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NetworkClient : SocketIOComponent{

    [Header("Network Client")]
    [SerializeField]
    private Transform networkContainer;
    [SerializeField]
    private GameObject playerPrefab;
    public static Player localPlayer;
    public bool inLobby = true;
    public Transform ring;
    public PlayerManager pm;
    public InputField nicknameTextField;

    public static string clientID {get; private set;}

    private Dictionary<string, NetworkIdentity> serverObjects;
    public int playersCounter = 0;

    public static NetworkClient instance;

    public override void Awake()
    {
        base.Awake();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
        }

    }

    public void EnterServerIP(InputField serverIP)
    {
        localPlayer = new Player();
        url = "ws://" + serverIP.text + "/socket.io/?EIO=4&transport=websocket";
        SetSocket();
        Initialize();
        SetUpEvents();
    }

    public void SetNickName()
    {
        localPlayer.nick = nicknameTextField.text;
        Emit("setNickname", new JSONObject(JsonUtility.ToJson(localPlayer)));
    }

    private void Initialize()
    {
        serverObjects = new Dictionary<string, NetworkIdentity>();
    }

    public void SetUpEvents()
    {

        On("open", (E) =>
        {
            
        });

        On("register", (E) =>
        {
            clientID = E.data["id"].ToString();
            clientID = clientID.Replace("\"", "");
            bool isHost = E.data["isHost"] + "" == "true";
            localPlayer.id = clientID;
            localPlayer.isHost = isHost;
            localPlayer.isReady = E.data["isReady"] + "" == "true";
            LobbyController.instance.SetButtons(localPlayer.isHost);
            SetNickName();
        });

        On("connectToLobby", (E) =>
        {
            string _playerNick = E.data["nick"].ToString().Replace("\"", "");
            string _playerID = E.data["id"].ToString().Replace("\"", "");
            bool isHost = E.data["isHost"] + "" == "true";
            StartCoroutine(LobbyController.instance.InstantiateLobbyPlayer(_playerID, _playerNick, clientID == _playerID, isHost));
        });
        
        On("setReady", (E) =>
        {
            string _playerID = E.data["id"].ToString().Replace("\"", "");
            bool isReady = E.data["isReady"] + "" == "true";
            Debug.Log(E.data);
            LobbyController.instance.SetPlayerReady(_playerID, isReady);
        });

        On("loadGame", (E) =>
        {
            SceneManager.LoadScene("Game");
        });

        On("spawn", (E) =>
        {
            string id = E.data["id"].ToString();
            id = id.Replace("\"", "");
            
            Vector3 pos = new Vector3(Random.Range(-8f, 8f), 3, Random.Range(-8f, 8f));
            GameObject go = Instantiate(playerPrefab, pos, Quaternion.identity, networkContainer);
            go.name = string.Format("Player ({0})", id);
            
            NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
            ni.SetControllerID(id);
            ni.SetSocketReference(this);
            ni.name.text = E.data["nick"].ToString().Replace("\"", "");
            playersCounter++;
            serverObjects.Add(id, ni);
            ArenaManager.instance.SetPlayersLeft();
        });

        On("disconnected", (E) =>
        {
            string id = E.data["id"].ToString();
            id = id.Replace("\"", "");
            if (inLobby) LobbyController.instance.DisconnectPlayer(id);
            else
            {
                GameObject go = serverObjects[id].gameObject;
                Destroy(go);
                serverObjects.Remove(id);
            }
            
        });

        //Positions that are coming from the server (other player object's position)
        On("updatePosition", (E) =>
        {
            string id = E.data["id"].ToString();
            id = id.Replace("\"", "");
            float x = E.data["position"]["x"].f;
            float y = E.data["position"]["y"].f;
            float z = E.data["position"]["z"].f;

            float i = E.data["rotation"]["x"].f;
            float j = E.data["rotation"]["y"].f;
            float k = E.data["rotation"]["z"].f;

            NetworkIdentity ni = serverObjects[id];
            ni.transform.position = new Vector3(x, y, z);
            ni.transform.rotation = Quaternion.Euler(new Vector3(i, j, k));
            StartCoroutine(ni.SetPosition(ni.transform.position));
        });

        On("ringScaleDown", (E) =>
        {
            float ringScale = E.data["rs"].f;
            ring.localScale = new Vector3(ringScale, ring.localScale.y, ringScale);
        });
        
        On("setHit", (E) =>
        {
            string _id = E.data["hited"].ToString().Replace("\"", "");
            Vector3 force = new Vector3(E.data["x"].f, 0, E.data["x"].f);
            if (_id == clientID) pm.GetHited(force);
        });

    }

    public void Hit(Vector3 force, string id)
    {
        HitInfo hit = new HitInfo(force.x, force.z, clientID, id);
        Emit("sendHit", new JSONObject(JsonUtility.ToJson(hit)));
    }

    public void SetReady()
    {
        localPlayer.isReady = !localPlayer.isReady;
        LobbyController.instance.SetPlayerReady(clientID, localPlayer.isReady);
        Emit("sendReady", new JSONObject(JsonUtility.ToJson(localPlayer)));
    }

    public void SendStart()
    {
        Emit("sendStart");
    }

    public void OnGameLoaded()
    {
        Emit("gameLoaded", new JSONObject(JsonUtility.ToJson(localPlayer)));
        inLobby = false;
    }

   

    [Serializable]
    public class Player
    {
        public string id;
        public string nick;
        public Position position;
        public Rotation rotation;
        public bool isHost;
        public bool isReady;
    }

    [Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public class Rotation
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    public class HitInfo
    {
        public HitInfo(float x, float z, string hiter, string hited)
        {
            this.x = x;
            this.z = z;
            this.hiter = hiter;
            this.hited = hited;
        }
        
        public float x;
        public float z;
        public string hiter;
        public string hited;
    }


}
