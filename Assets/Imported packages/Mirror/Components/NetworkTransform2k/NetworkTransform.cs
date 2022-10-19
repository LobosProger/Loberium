//!!! ВАЖНОЕ ЗАМЕЧАНИЕ НАСЧЕТ ДАННОГО СКРИПТА
//! Скрипт работает ТОЛЬКО тогда, когда появляется или спаунится игрок на сцене!
//! Так как RegisterHandler вызывается из функции OnStartAuthority
//! Если игра запущена, а игрок не появился на сцене, то все остальные игровые объекты НЕ БУДУТ СИНХРОНИЗИРОВАТЬ ПОЗИЦИЮ!!!


using UnityEngine;
using System;
using System.Collections.Generic;
using Mirror;
[DisallowMultipleComponent]
public class NetworkTransform : NetworkTransformBase
{
	protected override Transform targetComponent => transform;

	private bool AddedToDictionary = false;
	public override void OnStartClient()
	{
		if (!AddedToDictionary)
		{
			AddedToDictionary = true;
			if (NetworkRegisterMessages.TransformObjects.ContainsKey(netId))
			{
				Debug.LogError("Trying to register existing NetworkTransform with netId. Maybe is this component located in child? This not allowed!");
				return;
			}
			NetworkRegisterMessages.TransformObjects.Add(netId, this);
		}
	}

	private void OnDestroy() => NetworkRegisterMessages.TransformObjects.Remove(netId);
}