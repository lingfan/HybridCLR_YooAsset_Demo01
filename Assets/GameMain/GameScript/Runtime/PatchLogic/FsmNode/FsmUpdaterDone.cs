using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Singleton;
using YooAsset;

/// <summary>
/// 流程更新完毕
/// </summary>
internal class FsmUpdaterDone : IStateNode
{
	private StateMachine _machine;
	void IStateNode.OnCreate(StateMachine machine)
	{
		_machine = machine;
	}
	void IStateNode.OnEnter()
	{
		var buildPipeline = (string)_machine.GetBlackboardValue("BuildPipeline");
		if (buildPipeline == EDefaultBuildPipeline.RawFileBuildPipeline.ToString())
		{
			PatchEventDefine.PatchStatesChange.SendEventMessage("开始游戏！");

			// 开启游戏流程
			GameManager.Instance.Run();
		}
	}
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}
}