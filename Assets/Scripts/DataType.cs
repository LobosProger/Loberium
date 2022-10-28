using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public enum TypeClient { Client, Miner }

[System.Serializable]
public struct Transaction
{
	public NetworkIdentity fromWallet;
	public NetworkIdentity toWallet;
	public int amountOfTransferingCoins;
}

[System.Serializable]
public struct RewardTransaction
{
	public NetworkIdentity toWallet;

	public RewardTransaction(NetworkIdentity toWallet)
	{
		this.toWallet = toWallet;
	}
}
[System.Serializable]
public class Balance
{
	public NetworkIdentity wallet;
	public int amountOfCoins;

	public Balance(NetworkIdentity wallet, int amountOfCoins)
	{
		this.wallet = wallet;
		this.amountOfCoins = amountOfCoins;
	}
}
[System.Serializable]
public class Block
{
	public int index = 0;
	public DateTime timestamp;
	public Transaction currentTransaction;
	public RewardTransaction rewardTransaction;
	public string previousHash = "0";
	public string hashRoot;
	public int nonce = 0;
	public string hash;

	public Block(int index, NetworkIdentity rewardTransaction, Transaction currentTransaction, string previousHash = "0", int nonce = 0)
	{
		this.index = index;
		this.timestamp = DateTime.Now;
		this.rewardTransaction = new RewardTransaction(rewardTransaction);
		this.currentTransaction = currentTransaction;
		this.previousHash = previousHash;
		this.nonce = nonce;
	}
}
