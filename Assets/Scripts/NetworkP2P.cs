using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum TypeClient { Client, Miner, Internet }

public struct TransferInfo
{
	public string fromWallet;
	public string toWallet;
	public int amountOfTransferingCoins;
	public string signature; //! Сигнатура представляет собой подписанное сообщение ключом. Так как у меня возникли сложности с интеграцией ассиметричного шифрования, то я буду использовать симметричное

}

public class NetworkP2P : NetworkBehaviour
{
	public readonly SyncDictionary<NetworkIdentity, TypeClient> clientsOfP2P = new SyncDictionary<NetworkIdentity, TypeClient>();
	//public readonly SyncDictionary<string, TypeClient> clientsOfP2P = new SyncDictionary<NetworkIdentity, TypeClient>();
}
