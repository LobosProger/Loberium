using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class GeneralFunctions : MonoBehaviour
{
	private static int bufferedIndexForRandom = 0;
	private static readonly int maxBufferedIntForRandom = 2147483647;

	public static string GenerateKeyForClient(string additionalData)
	{
		string creatingKey = SystemInfo.deviceModel + SystemInfo.deviceName + SystemInfo.deviceType + Random.Range(-2147483648, 2147483647) + additionalData;
		creatingKey = sha256(creatingKey);
		StringBuilder key = new StringBuilder();
		for (int i = 0; i < 6; i++)
			key.Append(creatingKey[i]);

		return key.ToString();
	}

	public static string GenerateKeyForClient()
	{
		string creatingKey = SystemInfo.deviceModel + SystemInfo.deviceName + SystemInfo.deviceType + Random.Range(-2147483648, 2147483647);
		creatingKey = sha256(creatingKey);
		StringBuilder key = new StringBuilder();
		for (int i = 0; i < 6; i++)
			key.Append(creatingKey[i]);

		return key.ToString();
	}

	public static string sha256(string randomString)
	{
		var crypt = new System.Security.Cryptography.SHA256Managed();
		var hash = new System.Text.StringBuilder();
		byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
		foreach (byte theByte in crypto)
		{
			hash.Append(theByte.ToString("x2"));
		}
		return hash.ToString();
	}

	public static int GetRandomNumber(int min, int max)
	{
		bufferedIndexForRandom++;
		Random.InitState((bufferedIndexForRandom + System.DateTime.UtcNow.Millisecond) * 2 - 1);
		if (bufferedIndexForRandom == maxBufferedIntForRandom)
			bufferedIndexForRandom = 0;

		return Random.Range(min, max + 1);
	}

	public static int GetRandomNonceForMining()
	=> GetRandomNumber(-2147483648, 2147483647);
}
