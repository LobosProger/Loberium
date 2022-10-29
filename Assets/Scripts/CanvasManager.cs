using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
	[SerializeField] private Text textOfAmountCoins;
	public static CanvasManager singleton;

	private void Awake()
	{
		singleton = this;
	}

	public void ShowAmountOfCoins(int amount)
	{
		textOfAmountCoins.text = amount.ToString();
	}
}
