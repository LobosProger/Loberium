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

	[Command]
	public void Cmd_SendTransaction(Transaction transactionInfo)
	{
		Rpc_SendToAllMinersTransaction(transactionInfo);
	}

	[ClientRpc]
	public void Rpc_SendToAllMinersTransaction(Transaction transactionInfo)
	{
		NetworkBlockchainClient.localCLient.CheckTransaction(transactionInfo);
	}
}
