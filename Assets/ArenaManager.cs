using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaManager : MonoBehaviour
{
	public int playersLeftC;
	public int totPlayers;
	public Text playersLeft;
	public GameObject winTxt, loseTxt;
	public bool lose;
	public Transform ring;

	public static ArenaManager instance;
	private void Awake()
	{
		instance = this;
	}

	void Start () {
		NetworkClient.instance.OnGameLoaded();
		NetworkClient.instance.ring = ring;

	}

	public void SetPlayersLeft()
	{
		totPlayers = NetworkClient.instance.playersCounter;
		playersLeftC = totPlayers;
		playersLeft.text = "Players Left\n" + playersLeftC + "/" + totPlayers;
	}
	public void OnPlayerFall()
	{
		playersLeftC--;
		playersLeft.text = "Players Left\n" + playersLeftC + "/" + totPlayers;
		if (playersLeftC == 1 && !lose)
		{
			winTxt.SetActive(true);
		}
	}

	public void OnPlayerLose()
	{
		lose = true;
		loseTxt.SetActive(true);
	}

}
