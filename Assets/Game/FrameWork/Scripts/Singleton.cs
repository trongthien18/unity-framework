using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
                CreateInstance();

            return _instance;
        }
    }

    private static void CreateInstance()
    {
        var type = typeof(T);
        var objects = FindObjectsOfType<T>();
        if (objects.Length > 0)
        {
            _instance = objects[0];
            _instance.gameObject.SetActive(true);
            if (objects.Length > 1)
            {
#if UNITY_EDITOR
                MyLog.LogWarning("There is more than one instance of Singleton of type \"" + type + "\". Keeping the first one. Destroying the others.");
#endif
                for (var i = 1; i < objects.Length; i++)
                    Destroy(objects[i].gameObject);
            }

            return;
        }

        string prefabName;
        GameObject gameObject;

        prefabName = type.ToString();
        gameObject = new GameObject();

        gameObject.name = prefabName;
        if (_instance == null)
            _instance = gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }

    public bool Persistent;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            if (Persistent)
            {
                CreateInstance();
                DontDestroyOnLoad(gameObject);
            }
            return;
        }
        if (Persistent)
            DontDestroyOnLoad(gameObject);

        if (GetInstanceID() != _instance.GetInstanceID())
            Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}