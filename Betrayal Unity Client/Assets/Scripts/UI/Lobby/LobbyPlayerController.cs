using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerController : MonoBehaviour
{
	[SerializeField] private LobbyPlayerDisplay _baseDisplay;
	[SerializeField] private List<LobbyPlayerDisplay> _displays;
	
	private void OnEnable()
	{
		NetworkManager.OnUpdateAllUsers += UpdateCharacterList;
		User.OnUpdatePlayerStates += UpdateCharacterList;
	}
	
	private void OnDisable()
	{
		NetworkManager.OnUpdateAllUsers -= UpdateCharacterList;
		User.OnUpdatePlayerStates -= UpdateCharacterList;
	}
	
	private void Start()
	{
		_baseDisplay.gameObject.SetActive(false);
	}
	
	private void UpdateCharacterList()
	{
		int i = 0;
		foreach ((ushort id, User user) in NetworkManager.AllUsers)
		{
			TryGetCreateDisplay(i++, user);
		}
		for (; i < _displays.Count; i++)
		{
			_displays[i].gameObject.SetActive(false);
		}
	}
	
	private void TryGetCreateDisplay(int index, User user)
	{
		if (index >= _displays.Count)
		{
			_displays.Add(Instantiate(_baseDisplay, transform));
		}
		_displays[index].SetName(user.UserName, user.Character > 0 ? GameData.GetCharacter(user.Character).Name : "Spectator");
		_displays[index].SetReady(user.Ready);
		_displays[index].gameObject.SetActive(true);
	}
}
