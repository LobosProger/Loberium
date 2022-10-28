using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MyBox;
public class NetworkBlockchainClient : NetworkBehaviour
{
	public static NetworkBlockchainClient localCLient;

	[SerializeField] private int currentBalance = 10;
	[SerializeField] private TypeClient typeClient = TypeClient.Client;

	[Space(30)]
	[SerializeField] Transaction transaction;
	[SerializeField] private bool transfer;

	public override void OnStartAuthority()
	{
		localCLient = this;
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
		if (currentBalance >= transaction.amountOfTransferingCoins)
		{
			NetworkInternet.singleton.Cmd_SendTransaction(transaction.fromWallet, transaction.toWallet, transaction.amountOfTransferingCoins);
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

	public void CheckTransaction(Transaction newTransaction)
	{
		if (hasAuthority && typeClient == TypeClient.Miner)
		{
			Debug.LogWarning(NetworkBlockchain.singleton.GetBalanceOfWalletInBlockchain(transaction.fromWallet).amountOfCoins);
			if (NetworkBlockchain.singleton.IsTransactionValid(newTransaction))
			{
				Debug.Log("YEEEEAH! We can create new block!");
				Block creatingNewBlock = new Block(NetworkBlockchain.singleton.blockchain.Count - 1, netIdentity, newTransaction, NetworkBlockchain.singleton.previousHashOfLastBlock);
				creatingNewBlock.hashRoot = GeneralFunctions.sha256(creatingNewBlock.index.ToString() + creatingNewBlock.timestamp + creatingNewBlock.currentTransaction + creatingNewBlock.rewardTransaction + creatingNewBlock.previousHash);

				MineTheBlock(creatingNewBlock, 0);
				NetworkBlockchain.singleton.blockchain.Add(creatingNewBlock);
			}
			else
			{
				Debug.LogWarning("Oops! Maybe there are some errors! :(");
				if (newTransaction.fromWallet != newTransaction.toWallet)
					Debug.LogWarning("Adress to sending the same");
				Debug.LogWarning(newTransaction.fromWallet.netId);
				Debug.LogWarning(newTransaction.toWallet.netId);
				Debug.LogWarning(NetworkBlockchain.singleton.GetBalanceOfWalletInBlockchain(transaction.fromWallet).amountOfCoins + " amount: " + newTransaction.amountOfTransferingCoins);
			}

		}
	}

	private void MineTheBlock(Block block, int newNonce)
	{
		block.hash = GeneralFunctions.sha256(block.hashRoot + newNonce);
	}
}
