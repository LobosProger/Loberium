using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class CanvasManager : MonoBehaviour
{
	[SerializeField] private Text textOfAmountCoins;
	[SerializeField] private Text textOfIdWallet;
	[Space]
	[SerializeField] private Sprite spriteOfUser;
	[SerializeField] private Sprite spriteOfMiner;
	[SerializeField] private Image iconOfRole;
	[Space]
	[SerializeField] private InputField inputID;
	[SerializeField] private InputField inputAmount;
	[Space]
	[SerializeField] private GameObject clientsUI;

	public static CanvasManager singleton;

	private IEnumerator Start()
	{
		singleton = this;
		yield return NetworkInternet.singleton != null;
		if (NetworkInternet.singleton.isServer)
			clientsUI.SetActive(false);
	}

	public void ShowAmountOfCoins(int amount)
	{
		textOfAmountCoins.text = amount.ToString();
	}

	public void ShowIdOfWallet(string id)
	{
		textOfIdWallet.text = id;
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

	public void SendTransaction()
	{
		NetworkBlockchainClient.localCLient.SendCoins(inputID.text, int.Parse(inputAmount.text));
	}
}
