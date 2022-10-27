using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MyBox;
public class NetworkBlockchainClient : NetworkBehaviour
{
	[SyncVar(hook = nameof(OnMoneyChanged))] public int currentBalance = 10;
	[SerializeField] private int sendAmountOfCoins;
	[SerializeField] private NetworkIdentity toSendingClient;
	[SerializeField] private bool transfer;

	[Space]
	[SerializeField] private string privateKeyOfClient;
	[SerializeField] private string publicKeyOfClient;

	private void Start()
	{
		publicKeyOfClient = GeneralFunctions.GenerateKeyForClient("Yarik");
		privateKeyOfClient = GeneralFunctions.GenerateKeyForClient("Password");
	}

	private void Update()
	{
		if (transfer)
		{
			transfer = false;
			SendCoins();
		}
	}

	private void SendCoins()
	{
		if (currentBalance >= sendAmountOfCoins)
		{
			Cmd_SendMoney(sendAmountOfCoins, toSendingClient);
		}
		else
		{
			Debug.LogError("You haven't enough money!");
		}
	}

	[Command]
	private void Cmd_SendMoney(int amount, NetworkIdentity toSendingClient)
	{
		if (currentBalance >= amount)
		{
			currentBalance -= amount;
			toSendingClient.GetComponent<NetworkBlockchainClient>().currentBalance += amount;
		}
		else
		{
			Debug.LogError("You haven't enough money!");
		}
	}

	private void OnMoneyChanged(int _, int newAmountOfBalance)
	{

	}
}
