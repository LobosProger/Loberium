using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkBlockchain : NetworkBehaviour
{
	public static readonly string difficultyOfProof = "0000";
	public static readonly int difficultyAmountOfNone = difficultyOfProof.Length;
	public static readonly int amountMoneyOfEveryClientInStartingBlockchain = 10;
	public static readonly int amountRewardMoney = 50;

	public static NetworkBlockchain singleton;

	public List<Block> blockchain = new List<Block>();
	public Block minedBlock;

	public Block lastBlock => GetLastBlock();
	public string previousHashOfLastBlock { get { if (lastBlock != null) return lastBlock.hash; else return "0"; } }

	private void Start()
	{
		singleton = this;
	}

	private Block GetLastBlock()
	{
		if (!IsBlockchainInitialized())
			return null;
		else
			return blockchain[^1];
	}

	private bool IsBlockchainInitialized() => blockchain.Count != 0;

	public bool IsTransactionValid(Transaction transaction) => GetBalanceOfWalletInBlockchain(transaction.fromWallet).amountOfCoins >= transaction.amountOfTransferingCoins && transaction.amountOfTransferingCoins >= 0;// && transaction.fromWallet.netId != transaction.toWallet.netId;

	public Balance GetBalanceOfWalletInBlockchain(string gottenWallet)
	{
		Balance currentBalanceOfWallet = new Balance(gottenWallet, amountMoneyOfEveryClientInStartingBlockchain);
		for (int i = 0; i < blockchain.Count; i++)
		{
			CalculateBalanceInBlock(blockchain[i], currentBalanceOfWallet);
		}
		return currentBalanceOfWallet;
	}

	public void GetBalanceOfWalletInBlockchain(Balance gottenWallet)
	{
		gottenWallet.amountOfCoins = amountMoneyOfEveryClientInStartingBlockchain;
		for (int i = 0; i < blockchain.Count; i++)
		{
			CalculateBalanceInBlock(blockchain[i], gottenWallet);
		}
	}

	private bool IsTransactionConsistsThisWallet(Transaction transaction, string wallet) => transaction.fromWallet == wallet || transaction.toWallet == wallet;

	private bool IsTransactionConsistsThisWallet(RewardTransaction transaction, string wallet) => transaction.toWallet == wallet;

	private void CalculateThisBalanceOnInputAndOutput(Transaction transaction, Balance balance)
	{
		if (transaction.fromWallet == balance.wallet)
			balance.amountOfCoins -= transaction.amountOfTransferingCoins;
		else
			balance.amountOfCoins += transaction.amountOfTransferingCoins;
	}

	private void CalculateThisBalanceOnInputAndOutput(RewardTransaction transaction, Balance balance)
	{
		balance.amountOfCoins += amountRewardMoney;
	}

	private void CalculateBalanceInBlock(Block block, Balance balance)
	{
		if (IsTransactionConsistsThisWallet(block.currentTransaction, balance.wallet))
			CalculateThisBalanceOnInputAndOutput(block.currentTransaction, balance);
		if (IsTransactionConsistsThisWallet(block.rewardTransaction, balance.wallet))
			CalculateThisBalanceOnInputAndOutput(block.rewardTransaction, balance);
	}
}
