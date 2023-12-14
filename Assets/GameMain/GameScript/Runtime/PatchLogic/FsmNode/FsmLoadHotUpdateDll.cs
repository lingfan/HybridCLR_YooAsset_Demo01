using Cysharp.Threading.Tasks;
using HybridCLR;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UniFramework.Machine;
using UniFramework.Singleton;
using UnityEngine;
using YooAsset;
using Newtonsoft.Json;

public class FsmLoadHotUpdateDll : IStateNode
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
            // 创建游戏管理器
            UniSingleton.CreateSingleton<HotUpdateManager>();
            GameManager.Instance.StartCoroutine(LoadMetadataForAOTAssemblies());
            GameManager.Instance.StartCoroutine(LoadHotUpdateAssemblies());
        }

        _machine.ChangeState<FsmClearPackageCache>();
    }

    void IStateNode.OnExit()
    {
    }

    void IStateNode.OnUpdate()
    {
        
    }
    
    private IEnumerator LoadMetadataForAOTAssemblies()
    {
        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        Debug.Log("LoadMetadataForAOTAssemblies packageName "+packageName);
        
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        var package = YooAssets.TryGetPackage(packageName);
        if (package == null)
        {
            Debug.Log("包获取失败");
        }
        
        var handle = package.LoadRawFileAsync("AOTDLLList");
        yield return handle;
        
        var data = handle.GetRawFileText();

        // var data = "[\"mscorlib.dll\",\"System.Core.dll\",\"System.dll\",\"System.Runtime.dll\",\"UniTask.dll\"]";
        var dllNames = JsonConvert.DeserializeObject<List<string>>(data);
        foreach (var name in dllNames)
        {
            var dataHandle = package.LoadRawFileAsync(name);
            yield return dataHandle;
            var dllData = dataHandle.GetRawFileData();

            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllData, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{name}. mode:{mode} ret:{err}");
        }
    }

    private IEnumerator LoadHotUpdateAssemblies()
    {
        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        Debug.Log("LoadHotUpdateAssemblies packageName "+packageName);

        var package = YooAssets.TryGetPackage(packageName);
        if (package == null)
        {
            Debug.Log("包获取失败");
        }

        var handle = package.LoadRawFileAsync("HotUpdateDLLList");
        yield return handle;
        
        var data = handle.GetRawFileText();
        
        // var data = "[\"HotUpdate.dll\"]";
        var dllNames = JsonConvert.DeserializeObject<List<string>>(data);
        
       
        foreach (var DllName in dllNames)
        {
            var dataHandle = package.LoadRawFileAsync(DllName);
            yield return dataHandle;

            if (dataHandle.Status != EOperationStatus.Succeed)
            {
                Debug.Log("资源加载失败" + DllName);
                yield break;
            }

            var dllData = dataHandle.GetRawFileData();
            if (dllData == null)
            {
                Debug.Log("获取Dll数据失败");
                yield break;
            }

            Assembly assembly = Assembly.Load(dllData);
            HotUpdateManager.Instance.HotUpdateAssemblies.Add(DllName, assembly);
            Debug.Log(assembly.GetTypes());
            Debug.Log($"加载热更新Dll:{DllName}");
        }
    }

    
}