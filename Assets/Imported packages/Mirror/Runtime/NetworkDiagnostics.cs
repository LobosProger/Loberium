using System;
using UnityEngine;
namespace Mirror
{
	/// <summary>Profiling statistics for tool to subscribe to (profiler etc.)</summary>
	public static class NetworkDiagnostics
	{
		/// <summary>Describes an outgoing message</summary>
		public readonly struct MessageInfo
		{
			/// <summary>The message being sent</summary>
			public readonly NetworkMessage message;
			/// <summary>channel through which the message was sent</summary>
			public readonly int channel;
			/// <summary>how big was the message (does not include transport headers)</summary>
			public readonly int bytes;
			/// <summary>How many connections was the message sent to.</summary>
			public readonly int count;

			internal MessageInfo(NetworkMessage message, int channel, int bytes, int count)
			{
				this.message = message;
				this.channel = channel;
				this.bytes = bytes;
				this.count = count;
			}
		}

		/// <summary>Event for when Mirror sends a message. Can be subscribed to.</summary>
		public static event Action<MessageInfo> OutMessageEvent;

		internal static void OnSend<T>(T message, int channel, int bytes, int count)
			 where T : struct, NetworkMessage
		{
#if UNITY_EDITOR
			if (count > 0)
				Debug.LogWarning("Send: \t Name: " + typeof(T).Name + "\t Connections count: " + count + "\n\t\t\t" + "Amount of bytes: " + bytes + "\t\t" + "Total amount of bytes: " + bytes * count + " \t " + "Channel: " + (channel == 0 ? "reliable" : "unreliable"));
#endif
		}

		/// <summary>Event for when Mirror receives a message. Can be subscribed to.</summary>
		public static event Action<MessageInfo> InMessageEvent;

		internal static void OnReceive<T>(T message, int channel, int bytes)
			 where T : struct, NetworkMessage
		{
#if UNITY_EDITOR
			Debug.LogWarning("Receive: \t Name: " + typeof(T).Name + "\t Connections count: " + 1 + "\n\t\t\t" + "Amount of bytes: " + bytes + "\t\t" + "Total amount of bytes: " + bytes + " \t " + "Channel: " + (channel == 0 ? "reliable" : "unreliable"));
#endif
		}
	}
}
