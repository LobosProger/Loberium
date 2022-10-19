using System;
using UnityEngine;

namespace Mirror
{
	public struct ReadyMessage : NetworkMessage { }

	public struct NotReadyMessage : NetworkMessage { }

	public struct AddPlayerMessage : NetworkMessage { }

	public struct SceneMessage : NetworkMessage
	{
		public string sceneName;
		// Normal = 0, LoadAdditive = 1, UnloadAdditive = 2
		public SceneOperation sceneOperation;
		public bool customHandling;
	}

	public enum SceneOperation : byte
	{
		Normal,
		LoadAdditive,
		UnloadAdditive
	}

	public struct CommandMessage : NetworkMessage
	{
		public ushort netId;
		public byte componentIndex;
		public short functionHash;
		// the parameters for the Cmd function
		// -> ArraySegment to avoid unnecessary allocations
		public ArraySegment<byte> payload;
	}

	public struct RpcMessage : NetworkMessage
	{
		public ushort netId;
		public byte componentIndex;
		public short functionHash;
		// the parameters for the Cmd function
		// -> ArraySegment to avoid unnecessary allocations
		public ArraySegment<byte> payload;
	}

	public struct SpawnMessage : NetworkMessage
	{
		// netId of new or existing object
		public ushort netId;
		public bool isLocalPlayer;
		// Sets hasAuthority on the spawned object
		public bool isOwner;
		public ulong sceneId;
		// If sceneId != 0 then it is used instead of assetId
		public Guid assetId;
		// Local position
		//public Vector3 position;
		public Vector2 position;
		public float rotationZ;
		// Local rotation
		//public Quaternion rotation;
		// Local scale

		//public Vector3 scale;

		// serialized component data
		// ArraySegment to avoid unnecessary allocations
		public ArraySegment<byte> payload;
	}

	public struct CmdTransform : NetworkMessage
	{
		public ushort netId;
		public ushort x;
		public ushort y;
		public ushort rotZ;
	}

	public struct RpcTransform : NetworkMessage
	{
		public ushort netId;
		public ushort x;
		public ushort y;
		public ushort rotZ;
	}

	public struct ChangeOwnerMessage : NetworkMessage
	{
		public ushort netId;
		public bool isOwner;
	}

	public struct ObjectSpawnStartedMessage : NetworkMessage { }

	public struct ObjectSpawnFinishedMessage : NetworkMessage { }

	public struct ObjectDestroyMessage : NetworkMessage
	{
		public ushort netId;
	}

	public struct ObjectHideMessage : NetworkMessage
	{
		public ushort netId;
	}

	public struct EntityStateMessage : NetworkMessage
	{
		public ushort netId;
		// the serialized component data
		// -> ArraySegment to avoid unnecessary allocations
		public ArraySegment<byte> payload;
	}

	// A client sends this message to the server
	// to calculate RTT and synchronize time
	public struct NetworkPingMessage : NetworkMessage
	{
		public double clientTime;

		public NetworkPingMessage(double value)
		{
			clientTime = value;
		}
	}

	// The server responds with this message
	// The client can use this to calculate RTT and sync time
	public struct NetworkPongMessage : NetworkMessage
	{
		public double clientTime;
		public double serverTime;
	}
}
