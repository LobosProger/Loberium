using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkBlockchain : NetworkBehaviour
{
	public const string difficultyOfProof = "0000";
	public const int amountMoneyOfEveryClientInStartingBlockchain = 10;
	public const int amountRewardMoney = 50;

	public static NetworkBlockchain singleton;

	public List<Block> blockchain = new List<Block>();

	public Block lastBlock => GetLastBlock();
	public string previousHashOfLastBlock { get { if (lastBlock != null) return lastBlock.hash; else return "0"; } }

	private Block GetLastBlock()
	{
		if (!IsBlockchainInitialized())
			return null;
		else
			return blockchain[^1];
	}

	private bool IsBlockchainInitialized() => blockchain.Count != 0;

	public bool IsTransactionValid(Transaction transaction) => GetBalanceOfWalletInBlockchain(transaction.fromWallet).amountOfCoins <= transaction.amountOfTransferingCoins && transaction.fromWallet != transaction.toWallet;

	public Balance GetBalanceOfWalletInBlockchain(NetworkIdentity gottenWallet)
	{
		Balance currentBalanceOfWallet = new Balance(gottenWallet, amountMoneyOfEveryClientInStartingBlockchain);

		for (int i = 0; i < blockchain.Count; i++)
		{
			CalculateBalanceInBlock(blockchain[i], currentBalanceOfWallet);
		}
		return currentBalanceOfWallet;
	}

	private bool IsTransactionConsistsThisWallet(Transaction transaction, NetworkIdentity wallet) => transaction.fromWallet == wallet || transaction.toWallet == wallet;

	private bool IsTransactionConsistsThisWallet(RewardTransaction transaction, NetworkIdentity wallet) => transaction.toWallet == wallet;

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
		else if (IsTransactionConsistsThisWallet(block.rewardTransaction, balance.wallet))
			CalculateThisBalanceOnInputAndOutput(block.rewardTransaction, balance);
	}
}
