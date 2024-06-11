using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;

public class AddressableManager
{
    // ����Asset�ķ��ͷ���
    public static AsyncOperationHandle<T> LoadAssetAsync<T>(string address) where T : UnityEngine.Object
    {
        // ʹ��Addressables.LoadAssetAsync�����ʲ�
        return Addressables.LoadAssetAsync<T>(address);
    }

    // ����Asset�ķ��ͷ��������лص�
    public static void LoadAssetAsync<T>(string address, Action<T> callback) where T : UnityEngine.Object
    {
        // ��ʼ����Asset
        var handle = LoadAssetAsync<T>(address);

        // �ȴ��������
        handle.Completed += operation =>
        {
            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                // ���سɹ���ִ�лص�
                callback?.Invoke(operation.Result);
            }
            else
            {
                // ����ʧ�ܣ����������ﴦ�����
                Debug.LogError($"Failed to load asset at address: {address}");
            }
        };
    }

    // �ͷ�Asset
    public static void ReleaseAsset<T>(AsyncOperationHandle<T> handle) where T : UnityEngine.Object
    {
        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }
    }
}