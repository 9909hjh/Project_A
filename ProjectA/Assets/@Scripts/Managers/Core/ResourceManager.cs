using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public class ResourceManager
{
    Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, Object>();
    private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>(); 

    public T Load<T>(string key) where T : Object
    {
        if (_resources.TryGetValue(key, out Object resource))
            return resource as T;

        if (typeof(T) == typeof(Sprite))
        {
            key = key + ".sprite";
            if (_resources.TryGetValue(key, out Object temp))
            {
                return temp as T;
            }
        }

        return null;
    }

    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>($"{key}");
        if (prefab == null)
        {
            Debug.Log("Failed to load prefab : " + key);
            return null;
        }

        //Pooling
        if (pooling)
        {
            //return Managers.Pool.Pop(prefab);
        }

        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        //if (Managers.Pool.Push(go))
            //return;

        Object.Destroy(go);
    }


    #region 어드레서블
    // 비동기 방식
    public void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        // 캐시 확인
        if (_resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        // 텍스쳐로 갖고 오는걸 스프라이트로 변환
        string loadkey = key;
        if (key.Contains(".sprite"))
            loadkey = $"{key}[{key.Replace(".sprite", "")}]"; //$"{key}[{key.Replace(".sprite", "")}]"

        var asyncOperation = Addressables.LoadAssetAsync<T>(loadkey);
        asyncOperation.Completed += (op) =>
        {
            if (op.Result is Texture2D texture)
            {
                // Texture2D를 Sprite로 변환.
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                // 리소스에 스프라이트를 추가하고 콜백을 호출.
                _resources.Add(key, sprite);
                callback?.Invoke(sprite as T);
                return;
            }

            _resources.Add(key, op.Result);
            callback?.Invoke(op.Result);
        };
    }

    public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        opHandle.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            foreach (var result in op.Result)
            {
                LoadAsync<T>(result.PrimaryKey, (obj) =>
                {
                    loadCount++;
                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount); // 몇개의 프리펩이 있고 몇번째의 프리팹을 갖고 오는 중인지 채크
                });
            }
        };
    }
    #endregion

}
