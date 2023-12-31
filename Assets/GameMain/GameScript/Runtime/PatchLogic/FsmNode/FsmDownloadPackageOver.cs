﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;

/// <summary>
/// 下载完毕
/// </summary>
internal class FsmDownloadPackageOver : IStateNode
{
	private StateMachine _machine;

	void IStateNode.OnCreate(StateMachine machine)
	{
		_machine = machine;
	}
	void IStateNode.OnEnter()
	{
		// _machine.ChangeState<FsmClearPackageCache>();
		_machine.ChangeState<FsmLoadHotUpdateDll>();
	}
	
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}
}