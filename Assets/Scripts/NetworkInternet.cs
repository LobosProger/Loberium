using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkInternet : NetworkBehaviour
{
	public static NetworkInternet singleton;

	private void Start()
	{
		singleton = this;
	}

	[Command(requiresAuthority = false)]
	public void Cmd_SendTransaction(NetworkIdentity fromWallet, NetworkIdentity toWallet, int amountOfTransferingCoins)
	{
		Rpc_SendToAllMinersTransaction(fromWallet, toWallet, amountOfTransferingCoins);
	}

	[ClientRpc]
	public void Rpc_SendToAllMinersTransaction(NetworkIdentity from, NetworkIdentity to, int amount)
	{
		Transaction transaction = new Transaction
		{
			fromWallet = from,
			toWallet = to,
			amountOfTransferingCoins = amount
		};
		if (from != null && to != null)
			Debug.LogError("OK");
		NetworkBlockchainClient.localCLient.CheckTransaction(transaction);
	}

	[Command(requiresAuthority = false)]
	public void Cmd_SendMinedBlock(Block minedBlock)
	{
		Rpc_SendMinedBlock(minedBlock);
	}

	[ClientRpc]
	public void Rpc_SendMinedBlock(Block minedBlock)
	{
		NetworkBlockchainClient.localCLient.CheckMinedBlockAndAddIntoBlockchain(minedBlock);
	}
}
