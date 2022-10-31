using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using MyBox;
public class NetworkBlockchainClient : NetworkBehaviour
{
	public static NetworkBlockchainClient localCLient;

	[SerializeField] private Balance balanceOfWallet;
	[SerializeField] public TypeClient typeClient = TypeClient.Client;

	[Space(30)]
	[SerializeField] Transaction transaction;
	[SerializeField] private bool transfer;

	private Stack<Transaction> poolOfTransactions = new Stack<Transaction>();
	private AudioSource soundOfMining;
	private bool isClientMining;

	public override void OnStartAuthority()
	{
		soundOfMining = GetComponent<AudioSource>();
		localCLient = this;

		//* С подкючением нового клиента к сети создаем счет по-умолчанию и присваиваем стартовое количество монет из блокчейна
		balanceOfWallet.wallet = GeneralFunctions.GenerateKeyForClient();
		balanceOfWallet.amountOfCoins = NetworkBlockchain.amountMoneyOfEveryClientInStartingBlockchain;

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
		if (balanceOfWallet.amountOfCoins >= transaction.amountOfTransferingCoins)
		{
			NetworkInternet.singleton.Cmd_SendTransaction(transaction);
		}
		else
		{
			Debug.LogError("You haven't enough money!");
		}
	}

	/*[Command]
	private void Cmd_SendMoney(int amount, NetworkIdentity toSendingClient)
	{
		if (balanceOfWallet.amountOfCoins >= amount)
		{
			balanceOfWallet.amountOfCoins -= amount;
			toSendingClient.GetComponent<NetworkBlockchainClient>().currentBalance += amount;
		}
		else
		{
			Debug.LogError("You haven't enough money!");
		}
	}*/

	private void OnMoneyChanged(int _, int newAmountOfBalance)
	{

	}

	public void CheckTransaction(Transaction newTransaction)
	{
		if (hasAuthority && typeClient == TypeClient.Miner)
		{
			VerifyTransactionAndStartMining(newTransaction);
		}
	}

	private void VerifyTransactionAndStartMining(Transaction newTransaction)
	{
		if (NetworkBlockchain.singleton.IsTransactionValid(newTransaction))
		{
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
			}
		}
		else
		{
			Debug.LogWarning("Oops! Maybe there are some errors! :(");
			if (hasAuthority && typeClient == TypeClient.Miner && poolOfTransactions.Count > 0)
			{
				VerifyTransactionAndStartMining(poolOfTransactions.Pop());
			}
		}
	}

	public void CheckMinedBlockAndAddIntoBlockchain(Block minedBlock)
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
							NetworkBlockchain.singleton.blockchain.Add(minedBlock);
							NetworkBlockchain.singleton.GetBalanceOfWalletInBlockchain(balanceOfWallet);

							CanvasManager.singleton.ShowAmountOfCoins(balanceOfWallet.amountOfCoins);

							if (hasAuthority && typeClient == TypeClient.Miner && poolOfTransactions.Count > 0)
							{
								VerifyTransactionAndStartMining(poolOfTransactions.Pop());
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
		int nonce = 0;
		soundOfMining.Play();
		MineTheBlock(miningBlock, nonce);
		while (miningBlock.hash.Substring(0, NetworkBlockchain.difficultyAmountOfNone) != NetworkBlockchain.difficultyOfProof)
		{
			for (int i = 1; i <= 5000; i++)
			{
				nonce++;
				MineTheBlock(miningBlock, nonce);
				if (miningBlock.hash.Substring(0, NetworkBlockchain.difficultyAmountOfNone) == NetworkBlockchain.difficultyOfProof)
				{
					break;
				}
			}
			Debug.Log("Mining...");
			Resources.UnloadUnusedAssets();
			yield return new WaitForSecondsRealtime(1f);
		}
		Debug.Log("Block has been mined! Sharing with network!");
		miningBlock.nonce = nonce;
		isClientMining = false;
		NetworkInternet.singleton.Cmd_SendMinedBlock(miningBlock);
	}

	private void MineTheBlock(Block block, int newNonce)
	{
		block.hash = GeneralFunctions.sha256(block.hashRoot + newNonce);
	}
}
