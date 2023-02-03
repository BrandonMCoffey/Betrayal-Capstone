﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	[SerializeField] private Player _localPlayerPrefab;
	[SerializeField] private Player _remotePlayerPrefab;
	
	[SerializeField, ReadOnly] private Player _localPlayer;
	
	public static List<Player> Players = new List<Player>();
	
	private void Start()
	{
		if (NetworkManager.Instance)
		{
			NetworkManager.OnGameLoaded();
			foreach (var pair in NetworkManager.AllUsers)
			{
				var user = pair.Value;
				if (user.Character > 0)
				{
					bool isLocal =user.IsLocal;
					var player = Instantiate(user.IsLocal ? _localPlayerPrefab : _remotePlayerPrefab, transform);
					user.SetPlayer(player);
					Players.Add(player);
					if (user.IsLocal) _localPlayer = player;
				}
			}
		}
		else _localPlayer = Instantiate(_localPlayerPrefab, transform);
	}
}
