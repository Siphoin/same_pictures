using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SiphoinUnityHelpers
{
    public static class AddressablesHelperUniTask
    {
        private static TimeSpan _timeoutUnloading;

        private static Dictionary<string, UnityEngine.Object> _cacheSingle;

        private static Dictionary<string, IEnumerable<UnityEngine.Object>> _cacheMultiple;

        private static Queue<UnityEngine.Object> _unloadingQueue;

        private static bool _initialized = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _cacheSingle = new Dictionary<string, UnityEngine.Object>();

            _cacheMultiple = new Dictionary<string, IEnumerable<UnityEngine.Object>>();

            _unloadingQueue = new Queue<UnityEngine.Object>();

            Addressables.InitializeAsync().Completed += OnInitializeAddressables;
        }

        private static async void OnInitializeAddressables(AsyncOperationHandle<IResourceLocator> handle)
        {
            handle.Completed -= OnInitializeAddressables;

            _initialized = true;

            Debug.Log("Addressables up");

            _timeoutUnloading = TimeSpan.FromSeconds(60);

            await UniTask.Delay(10, true);

            Unloading().Forget();
        }

        public static async UniTask<T> Get<T>(bool isUnload = true) where T : UnityEngine.Object
        {
            Type type = typeof(T);

            string key = type.Name;

            var asset = await Get<T>(key, isUnload);

            return asset;
        }

        public static async UniTask<T> Get<T>(string key, bool isUnload = true) where T : UnityEngine.Object
        {
            await UniTask.WaitUntil(() => _initialized);

            Type type = typeof(T);

            string keyName = type.Name;

            if (_cacheSingle.TryGetValue(key, out var result))
            {
                return (T)result;
            }

            else
            {
                var operation = Addressables.LoadAssetAsync<T>(key);

                var loadedAsset = await operation.Task;

                if (operation.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception($"asset {key} not found on Addressables");
                }
                if (!_cacheSingle.ContainsKey(keyName))
                {
                    _cacheSingle.Add(keyName, loadedAsset);
                }

                if (isUnload)
                {
                    _unloadingQueue.Enqueue(loadedAsset);
                }

                return loadedAsset;


            }
        }

        public static async UniTask<T> GetPrefab<T>(bool isUnload = true) where T : UnityEngine.Object
        {
            await UniTask.WaitUntil(() => _initialized);

            Type type = typeof(T);

            string key = $"{type.Name}";

            if (_cacheSingle.TryGetValue(key, out var result))
            {
                GameObject prefab = (GameObject)result;

                return prefab.GetComponent<T>();
            }

            else
            {
                var operation = Addressables.LoadAssetAsync<GameObject>(key);

                var loadedAsset = await operation.Task;

                if (operation.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception($"asset {key} not found on Addressables");
                }
                if (!_cacheSingle.ContainsKey(key))
                {
                    _cacheSingle.Add(key, loadedAsset);

                    if (isUnload)
                    {
                        _unloadingQueue.Enqueue(loadedAsset);
                    }
                }

                if (!loadedAsset.TryGetComponent(out T component))
                {
                    throw new Exception($"Prefab {loadedAsset.name} not have component {key}");
                }

                else
                {
                    return component;
                }


            }
        }

        public static async UniTask<IEnumerable<T>> GetMore<T>(bool isUnload = true, string key = "") where T : UnityEngine.Object
        {
            await UniTask.WaitUntil(() => _initialized);

            if (string.IsNullOrEmpty(key))
            {
                Type type = typeof(T);

                key = $"{type.Name}s";
            }

            if (_cacheMultiple.TryGetValue(key, out var result))
            {
                return result as IEnumerable<T>;
            }

            else
            {
                
                var operation = Addressables.LoadAssetsAsync<T>(key, null);

                var loadedAssets = await operation.Task;

                if (operation.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception($"asset {key} not found on Addressables");
                }

                if (!_cacheMultiple.ContainsKey(key))
                {
                    _cacheMultiple.Add(key, loadedAssets);

                    if (isUnload)
                    {
                        foreach (var item in loadedAssets)
                        {
                            _unloadingQueue.Enqueue(item);
                        }
                    }
                }

                return loadedAssets;


            }
        }

        public static async UniTask<IEnumerable<T>> GetPrefabs<T>(bool isUnload = true, string key = "") where T : UnityEngine.Object
        {
            await UniTask.WaitUntil(() => _initialized);

            Type type = typeof(T);

            key = $"{type.Name}s";

            if (_cacheMultiple.TryGetValue(key, out var result))
            {
                var enumerable = result as IEnumerable<GameObject>;

                List<T> list = new List<T>();

                foreach (var item in enumerable)
                {
                    list.Add(item.GetComponent<T>());
                }

                return list;
            }

            else
            {

                var operation = Addressables.LoadAssetsAsync<GameObject>(key, null);

                var loadedAssets = await operation.Task;

                if (operation.Status != AsyncOperationStatus.Succeeded)
                {
                    throw new Exception($"assets with {key} not found on Addressables");
                }
                if (!_cacheMultiple.ContainsKey(key))
                {
                    _cacheMultiple.Add(key, loadedAssets);

                    if (isUnload)
                    {
                        foreach (var item in loadedAssets)
                        {
                            _unloadingQueue.Enqueue(item);
                        }
                    }
                }

                List<T> list = new List<T>();

                foreach (var item in loadedAssets)
                {
                    list.Add(item.GetComponent<T>());
                }

                return list;


            }
        }

        private static async UniTask Unloading ()
        {
            while (true)
            {

                await UniTask.Delay(_timeoutUnloading, true);

                Debug.Log($"New {nameof(Unloading)} {nameof(Addressables)}...");

                _unloadingQueue = new Queue<UnityEngine.Object>(_unloadingQueue.Distinct());

                

                for (int i = 0; i < _unloadingQueue.Count; i++)
                {
                    await UniTask.Delay(100, true);

                    var asset = _unloadingQueue?.Dequeue();

                    string key = nameof(asset);

                    Addressables.Release(asset);

                    bool isEnumerable = asset is IEnumerable;

                    if (isEnumerable)
                    {
                        _cacheMultiple.Remove(key);
                    }

                    else
                    {
                        _cacheSingle.Remove(key);
                    }
                }

               
            }
        }
    }
}
