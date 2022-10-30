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
	public void Cmd_SendTransaction(Transaction transaction)
	{
		StartCoroutine(SendTransactionToMiners(transaction));
	}

	private IEnumerator SendTransactionToMiners(Transaction transaction)
	{
		foreach (NetworkConnectionToClient everyConnection in NetworkServer.connections.Values)
		{
			//* На первое время код подойдет, так как различные майнеры должнв получать вознаграждение случайным путем
			//* и так как, nonce подбирается неслучайно, то элемент отправки транзакции с "задержкой"
			//* => Это сделано для того, чтобы разные майнеры имели возможность получить награду, а не только один, так как nonce подбирается путем прибавления единицы
			float randomTimeToSendTransaction = ((float)GeneralFunctions.GetRandomNumber(500, 2000)) / 1000;
			yield return new WaitForSeconds(randomTimeToSendTransaction);
			Target_SendToAllMinersTransaction(everyConnection, transaction);
		}
	}

	[TargetRpc]
	public void Target_SendToAllMinersTransaction(NetworkConnection connection, Transaction transaction)
	{
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
