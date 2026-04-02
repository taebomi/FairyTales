using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static bool _shuttingDown;
    private static object LockObject = new();
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_shuttingDown)
            {
                Debug.LogWarning($"{typeof(T)} 싱글톤 인스턴스 이미 파괴됨. null 리턴.");
#if !UNITY_EDITOR
                return null;
#else // 에디터 상에서 플레이 중인 경우와 아닌 경우 구분
                if (Application.isPlaying)
                {
                    return null;
                }
                else
                {
                    _shuttingDown = false;
                }
#endif
            }

            lock (LockObject)
            {
                if (_instance)
                {
                    return _instance;
                }

                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance)
                {
                    return _instance;
                }

                var singletonObject = new GameObject(nameof(T));
                _instance = singletonObject.AddComponent<T>();

                return _instance;
            }
        }
        private set => _instance = value;
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = (T)this;
        AwakeAfter();
    }

    protected abstract void AwakeAfter();

    private void OnApplicationQuit()
    {
        _shuttingDown = true;
        _instance = null;
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}