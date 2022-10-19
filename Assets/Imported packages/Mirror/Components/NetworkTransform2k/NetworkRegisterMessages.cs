using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkRegisterMessages : MonoBehaviour
{
	public static Dictionary<ushort, NetworkTransform> TransformObjects = new Dictionary<ushort, NetworkTransform>();

	private void Awake()
	{
		NetworkServer.delegatesServerMessages = RegisterHandlersServer;
		NetworkClient.delegatesClientMessages = RegisterHandlersClient;
	}
	public void OnClientRpcTransformMessage(RpcTransform message)
	{
		if (TransformObjects.ContainsKey(message.netId))
			TransformObjects[message.netId].OnClientRpcTransformMessage(message);
		else
		{
#if UNITY_EDITOR
			Debug.LogWarning("Can't find Transform Object on netId!");
#endif
		}
	}

	public void OnCommandTransformMessage(NetworkConnection conn, CmdTransform message)
	{
		if (TransformObjects.ContainsKey(message.netId))
			TransformObjects[message.netId].OnCommandTransformMessage(conn, message);
		else
		{
#if UNITY_EDITOR
			Debug.LogWarning("Can't find Transform Object on netId!");
#endif
		}
	}

	private void RegisterHandlersServer()
	{
		NetworkServer.RegisterHandler<CmdTransform>(OnCommandTransformMessage);
	}

	private void RegisterHandlersClient()
	{
		NetworkClient.RegisterHandler<RpcTransform>(OnClientRpcTransformMessage);
	}
}
