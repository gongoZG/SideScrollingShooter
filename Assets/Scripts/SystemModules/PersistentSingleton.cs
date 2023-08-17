using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    protected virtual void Awake() {
        if (Instance == null) {
            Instance = this as T;
        }
        else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);  // 加载新场景不会摧毁
    }    
}
