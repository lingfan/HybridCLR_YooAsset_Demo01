using System.Collections;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;


internal class FsmLunchHotUpdate : IStateNode
{
	private StateMachine _machine;

	void IStateNode.OnCreate(StateMachine machine)
	{
		_machine = machine;
	}
	void IStateNode.OnEnter()
	{
		GameManager.Instance.StartCoroutine(Prepare());
	}
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}

	private IEnumerator Prepare()
	{
		
		var package = YooAssets.GetPackage("DefaultPackage");
		var loadHandle = package.LoadAssetAsync<GameObject>("HotUpdateLuncher");
		yield return loadHandle;
		
		if(loadHandle.Status== EOperationStatus.Succeed)
        {
			var instantiateHandle = loadHandle.InstantiateAsync();
			yield return instantiateHandle;
			
			var obj = instantiateHandle.Result;
			
			GameObject.DontDestroyOnLoad(obj);
			Debug.Log("加载热更新预制体完成");
		}
		package.UnloadUnusedAssets();
	}
}