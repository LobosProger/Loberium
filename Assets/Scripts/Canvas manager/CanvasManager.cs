using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
	[SerializeField] private Text textOfAmountCoins;
	[Space]
	[SerializeField] private Sprite spriteOfUser;
	[SerializeField] private Sprite spriteOfMiner;
	[SerializeField] private Image iconOfRole;

	public static CanvasManager singleton;

	private void Awake()
	{
		singleton = this;
	}

	public void ShowAmountOfCoins(int amount)
	{
		textOfAmountCoins.text = amount.ToString();
	}

	public void ChangeRole()
	{
		if (NetworkInternet.singleton.isServer)
			return;

		switch (NetworkBlockchainClient.localCLient.typeClient)
		{
			case TypeClient.Client: NetworkBlockchainClient.localCLient.typeClient = TypeClient.Miner; ShowRoleOnUI(TypeClient.Miner); break;
			case TypeClient.Miner: NetworkBlockchainClient.localCLient.typeClient = TypeClient.Client; ShowRoleOnUI(TypeClient.Client); break;
		}
	}

	public void ShowRoleOnUI(TypeClient typeClient)
	{
		if (typeClient == TypeClient.Client)
			iconOfRole.sprite = spriteOfUser;
		else
			iconOfRole.sprite = spriteOfMiner;
	}
}
