using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;

public class AddressableManager
{
    // 加载Asset的泛型方法
    public static AsyncOperationHandle<T> LoadAssetAsync<T>(string address) where T : UnityEngine.Object
    {
        // 使用Addressables.LoadAssetAsync加载资产
        return Addressables.LoadAssetAsync<T>(address);
    }

    // 加载Asset的泛型方法，带有回调
    public static void LoadAssetAsync<T>(string address, Action<T> callback) where T : UnityEngine.Object
    {
        // 开始加载Asset
        var handle = LoadAssetAsync<T>(address);

        // 等待加载完成
        handle.Completed += operation =>
        {
            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                // 加载成功，执行回调
                callback?.Invoke(operation.Result);
            }
            else
            {
                // 加载失败，可以在这里处理错误
                Debug.LogError($"Failed to load asset at address: {address}");
            }
        };
    }

    // 释放Asset
    public static void ReleaseAsset<T>(AsyncOperationHandle<T> handle) where T : UnityEngine.Object
    {
        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }
    }
}