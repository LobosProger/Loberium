using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class CanvasManager : MonoBehaviour
{
	[SerializeField] private Text textOfAmountCoins;
	[SerializeField] private Text textOfIdWallet;
	[Space]
	[SerializeField] private Sprite spriteOfUser;
	[SerializeField] private Sprite spriteOfMiner;
	[SerializeField] private Image iconOfRole;
	[Space]
	[SerializeField] private InputField inputID;
	[SerializeField] private InputField inputAmount;
	[Space]
	[SerializeField] private Animator sentMoneyBox;
	[SerializeField] private Animator receiveMoneyBox;
	[SerializeField] private Animator processingNewTransactionBox;
	[SerializeField] private Animator verifiedNewTransactionBox;
	[SerializeField] private Animator verifiedNewTransactionAndAddIntoPoolBox;
	[SerializeField] private Animator checkWithErrorNewTransactionBox;
	[Space]
	[SerializeField] private GameObject processOfMiningBox;
	[SerializeField] private Text smallHashText;
	[Space]
	[SerializeField] private Animator startMineNewBlockBox;
	[SerializeField] private Animator verifiedNewBlockBox;
	[SerializeField] private Animator getRewardForNewMinedBlockBox;
	[Space]
	[SerializeField] private GameObject clientsUI;

	public static CanvasManager singleton;

	private IEnumerator Start()
	{
		singleton = this;
		yield return NetworkInternet.singleton != null;
		if (NetworkInternet.singleton.isServer)
			clientsUI.SetActive(false);
	}

	public void ShowAmountOfCoins(int amount)
	{
		textOfAmountCoins.text = amount.ToString();
	}

	public void ShowIdOfWallet(string id)
	{
		textOfIdWallet.text = id;
	}

	public void ChangeRole()
	{
		if (NetworkInternet.singleton.isServer)
			return;

		switch (NetworkBlockchainClient.localClient.typeClient)
		{
			case TypeClient.Client: NetworkBlockchainClient.localClient.typeClient = TypeClient.Miner; ShowRoleOnUI(TypeClient.Miner); break;
			case TypeClient.Miner: NetworkBlockchainClient.localClient.typeClient = TypeClient.Client; ShowRoleOnUI(TypeClient.Client); break;
		}
	}

	public void ShowRoleOnUI(TypeClient typeClient)
	{
		if (typeClient == TypeClient.Client)
			iconOfRole.sprite = spriteOfUser;
		else
			iconOfRole.sprite = spriteOfMiner;
	}

	public void SendTransaction()
	{
		NetworkBlockchainClient.localClient.SendCoins(inputID.text, int.Parse(inputAmount.text));
	}

	public void OnProcessingNewTransaction()
	{
		processingNewTransactionBox.SetTrigger("Show");
	}

	public void OnVerifiedNewTransaction()
	{
		verifiedNewTransactionBox.SetTrigger("Show");
	}

	public void OnVerifiedNewTransactionAndAddIntoPool()
	{
		verifiedNewTransactionAndAddIntoPoolBox.SetTrigger("Show");
	}

	public void OnCheckWithErrorNewTransaction()
	{
		checkWithErrorNewTransactionBox.SetTrigger("Show");
	}

	public void OnStartMiningBlock()
	{
		startMineNewBlockBox.SetTrigger("Show");
	}

	public void OnProcessOfMiningBlock(bool showBox, string smallHash)
	{
		processOfMiningBox.SetActive(showBox);
		smallHashText.text = smallHash;
	}

	public void OnVerifiedNewBlock()
	{
		verifiedNewBlockBox.SetTrigger("Show");
	}

	public void OnSentMoney()
	{
		sentMoneyBox.SetTrigger("Show");
	}

	public void OnReceivedMoney()
	{
		receiveMoneyBox.SetTrigger("Show");
	}

	public void OnReceivedRewardForMinedBlock()
	{
		getRewardForNewMinedBlockBox.SetTrigger("Show");
	}
}
