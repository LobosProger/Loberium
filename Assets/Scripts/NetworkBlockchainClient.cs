using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using MyBox;
using System.Text;
public class NetworkBlockchainClient : NetworkBehaviour
{
	public static NetworkBlockchainClient localClient;

	private Balance balanceOfWallet;
	[HideInInspector] public TypeClient typeClient = TypeClient.Client;

	private Transaction transaction;
	private bool transfer;

	private Stack<Transaction> poolOfTransactions = new Stack<Transaction>();
	private AudioSource soundOfMining;
	private bool isClientMining;

	public override void OnStartAuthority()
	{
		soundOfMining = GetComponent<AudioSource>();
		localClient = this;

		//* С подкючением нового клиента к сети создаем счет по-умолчанию и присваиваем стартовое количество монет из блокчейна
		balanceOfWallet = new Balance(GeneralFunctions.GenerateKeyForClient(), NetworkBlockchain.amountMoneyOfEveryClientInStartingBlockchain);

		CanvasManager.singleton.ShowAmountOfCoins(balanceOfWallet.amountOfCoins);
		CanvasManager.singleton.ShowIdOfWallet(balanceOfWallet.wallet);

		if (!isServer)
			CanvasManager.singleton.ShowRoleOnUI(typeClient);
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
		if (balanceOfWallet.amountOfCoins >= transaction.amountOfTransferingCoins)
		{
			NetworkInternet.singleton.Cmd_SendTransaction(transaction);
		}
		else
		{
			Debug.LogError("You haven't enough money!");
		}
	}

	public void SendCoins(string idOfWallet, int amountOfSendingMoney)
	{
		Transaction transaction = new Transaction(balanceOfWallet.wallet, idOfWallet, amountOfSendingMoney);
		NetworkInternet.singleton.Cmd_SendTransaction(transaction);
	}

	public void CheckTransaction(Transaction newTransaction)
	{
		if (hasAuthority && typeClient == TypeClient.Miner)
		{
			StartCoroutine(VerifyTransactionAndStartMining(newTransaction));
		}
	}

	private IEnumerator VerifyTransactionAndStartMining(Transaction newTransaction)
	{
		OnProcessingNewTransaction();
		yield return new WaitForSeconds(3f);
		if (NetworkBlockchain.singleton.IsTransactionValid(newTransaction))
		{
			OnVerifiedNewTransaction();
			yield return new WaitForSeconds(3f);
			if (!isClientMining)
			{
				Block creatingNewBlock = new Block
				{
					index = NetworkBlockchain.singleton.blockchain.Count + 1,
					timestamp = DateTime.Now.ToString(),
					rewardTransaction = new RewardTransaction(balanceOfWallet.wallet),
					currentTransaction = newTransaction,
					previousHash = NetworkBlockchain.singleton.previousHashOfLastBlock,
				};
				creatingNewBlock.hashRoot = GeneralFunctions.sha256(creatingNewBlock.index.ToString() + creatingNewBlock.timestamp + creatingNewBlock.currentTransaction + creatingNewBlock.rewardTransaction + creatingNewBlock.previousHash);

				isClientMining = true;
				StartCoroutine(MiningOfBlock(creatingNewBlock));
			}
			else
			{
				Debug.Log("Add transaction into queue and then check");
				poolOfTransactions.Push(newTransaction);
				OnVerifiedNewTransactionAndAddIntoPool();
			}
		}
		else
		{
			Debug.LogWarning("Oops! Maybe there are some errors! :(");
			OnCheckWithErrorNewTransaction();
			yield return new WaitForSeconds(3f);
			if (hasAuthority && typeClient == TypeClient.Miner && poolOfTransactions.Count > 0)
			{
				StartCoroutine(VerifyTransactionAndStartMining(poolOfTransactions.Pop()));
			}
		}
	}

	public IEnumerator CheckMinedBlockAndAddIntoBlockchain(Block minedBlock)
	{
		NetworkBlockchain.singleton.minedBlock = minedBlock;
		if (IsIndexOfBlockValid(minedBlock))
		{
			if (NetworkBlockchain.singleton.IsTransactionValid(minedBlock.currentTransaction))
			{
				if (IsPreviousHashIsValid(minedBlock))
				{
					if (IsHashRootIsValid(minedBlock))
					{
						if (IsHashOfBlockIsValidAndProofed(minedBlock))
						{
							Debug.Log("New block is veryfied! Adding into blockchain!");
							StopAllCoroutines();
							soundOfMining.Stop();
							isClientMining = false;
							OnProcessOfMiningBlock(false, "");
							OnVerifiedNewBlock();
							yield return new WaitForSeconds(3f);

							if (minedBlock.currentTransaction.fromWallet == balanceOfWallet.wallet)
								OnSentMoney();
							if (minedBlock.currentTransaction.toWallet == balanceOfWallet.wallet)
								OnReceivedMoney();

							NetworkBlockchain.singleton.blockchain.Add(minedBlock);
							NetworkBlockchain.singleton.GetBalanceOfWalletInBlockchain(balanceOfWallet);
							CanvasManager.singleton.ShowAmountOfCoins(balanceOfWallet.amountOfCoins);

							yield return new WaitForSeconds(3f);
							if (minedBlock.rewardTransaction.toWallet == balanceOfWallet.wallet)
								OnReceivedRewardForMinedBlock();

							yield return new WaitForSeconds(3f);
							if (hasAuthority && typeClient == TypeClient.Miner && poolOfTransactions.Count > 0)
							{
								StartCoroutine(VerifyTransactionAndStartMining(poolOfTransactions.Pop()));
							}
						}
						else
						{
							Debug.Log("Invalid hash!");
						}
					}
					else
					{
						Debug.Log("Invalid hashroot!");
					}
				}
				else
				{
					Debug.Log("Invalid previous hash!");
				}
			}
			else
			{
				Debug.Log("Invalid transaction!");
			}
		}
		else
		{
			Debug.Log("Invalid index or added new block");
			//! Invalid index or added new block
		}
	}

	private bool IsIndexOfBlockValid(Block minedBlock) => minedBlock.index == NetworkBlockchain.singleton.blockchain.Count + 1;

	private bool IsPreviousHashIsValid(Block minedBlock) => NetworkBlockchain.singleton.previousHashOfLastBlock == minedBlock.previousHash;

	private bool IsHashRootIsValid(Block minedBlock) => GeneralFunctions.sha256(minedBlock.index.ToString() + minedBlock.timestamp + minedBlock.currentTransaction + minedBlock.rewardTransaction + minedBlock.previousHash) == minedBlock.hashRoot;

	private bool IsHashOfBlockIsValidAndProofed(Block minedBlock) => GeneralFunctions.sha256(minedBlock.hashRoot + minedBlock.nonce) == minedBlock.hash && minedBlock.hash.Substring(0, NetworkBlockchain.difficultyAmountOfNone) == NetworkBlockchain.difficultyOfProof;


	private IEnumerator MiningOfBlock(Block miningBlock)
	{
		OnStartMiningBlock();
		yield return new WaitForSeconds(3f);
		int nonce = 0;
		soundOfMining.Play();
		MineTheBlock(miningBlock, nonce);
		while (miningBlock.hash.Substring(0, NetworkBlockchain.difficultyAmountOfNone) != NetworkBlockchain.difficultyOfProof)
		{
			for (int i = 1; i <= 5000; i++)
			{
				nonce++;
				MineTheBlock(miningBlock, nonce);

				if (i % 500 == 0)
				{
					StringBuilder smallHashForShowingInBox = new StringBuilder();
					for (int index = 0; index < 8; index++)
						smallHashForShowingInBox.Append(miningBlock.hash[index]);

					OnProcessOfMiningBlock(true, smallHashForShowingInBox.ToString());
					yield return new WaitForSecondsRealtime(0.1f);
				}

				if (miningBlock.hash.Substring(0, NetworkBlockchain.difficultyAmountOfNone) == NetworkBlockchain.difficultyOfProof)
				{
					break;
				}
			}
			Debug.Log("Mining...");
			Resources.UnloadUnusedAssets();

			StringBuilder smallHash = new StringBuilder();
			for (int index = 0; index < 8; index++)
				smallHash.Append(miningBlock.hash[index]);

			OnProcessOfMiningBlock(true, smallHash.ToString());

			yield return new WaitForSecondsRealtime(1f);
		}
		Debug.Log("Block has been mined! Sharing with network!");
		OnProcessOfMiningBlock(false, "");
		miningBlock.nonce = nonce;
		isClientMining = false;
		NetworkInternet.singleton.Cmd_SendMinedBlock(miningBlock);
	}

	private void MineTheBlock(Block block, int newNonce)
	{
		block.hash = GeneralFunctions.sha256(block.hashRoot + newNonce);
	}

	private void OnProcessingNewTransaction()
	{
		CanvasManager.singleton.OnProcessingNewTransaction();
	}

	private void OnVerifiedNewTransaction()
	{
		CanvasManager.singleton.OnVerifiedNewTransaction();
	}

	private void OnVerifiedNewTransactionAndAddIntoPool()
	{
		CanvasManager.singleton.OnVerifiedNewTransactionAndAddIntoPool();
	}

	private void OnCheckWithErrorNewTransaction()
	{
		CanvasManager.singleton.OnCheckWithErrorNewTransaction();
	}

	private void OnStartMiningBlock()
	{
		CanvasManager.singleton.OnStartMiningBlock();
	}

	private void OnProcessOfMiningBlock(bool showBox, string hash)
	{
		CanvasManager.singleton.OnProcessOfMiningBlock(showBox, hash);
	}

	private void OnVerifiedNewBlock()
	{
		CanvasManager.singleton.OnVerifiedNewBlock();
	}

	private void OnSentMoney()
	{
		CanvasManager.singleton.OnSentMoney();
	}

	private void OnReceivedMoney()
	{
		CanvasManager.singleton.OnReceivedMoney();
	}

	private void OnReceivedRewardForMinedBlock()
	{
		CanvasManager.singleton.OnReceivedRewardForMinedBlock();
	}
}
