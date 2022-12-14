using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public enum TypeClient { Client, Miner }

[System.Serializable]
public struct Transaction
{
	public string fromWallet;
	public string toWallet;
	public int amountOfTransferingCoins;

	public Transaction(string fromWallet, string toWallet, int amount)
	{
		this.fromWallet = fromWallet;
		this.toWallet = toWallet;
		amountOfTransferingCoins = amount;
	}
}

[System.Serializable]
public struct RewardTransaction
{
	public string toWallet;

	public RewardTransaction(string toWallet)
	{
		this.toWallet = toWallet;
	}
}
[System.Serializable]
public class Balance
{
	public string wallet;
	public int amountOfCoins;

	public Balance(string wallet, int amountOfCoins)
	{
		this.wallet = wallet;
		this.amountOfCoins = amountOfCoins;
	}
}
[System.Serializable]
public class Block
{
	public int index = 0;
	public string timestamp;
	public Transaction currentTransaction;
	public RewardTransaction rewardTransaction;
	public string previousHash = "0";
	public string hashRoot;
	public int nonce = 0;
	public string hash;
	/*
		public Block(int index, NetworkIdentity rewardTransaction, Transaction currentTransaction, string previousHash = "0", int nonce = 0)
		{
			this.index = index;
			this.timestamp = DateTime.Now;
			this.rewardTransaction = new RewardTransaction(rewardTransaction);
			this.currentTransaction = currentTransaction;
			this.previousHash = previousHash;
			this.nonce = nonce;
		}*/
}
