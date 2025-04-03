using UnityEngine;

//Base written by: Christian Irvine

/// <summary>
/// A Generic singleton parent class which can be inherited from.
/// Just keep in mind if you use Awake in the child class, you must call base.Awake() first (or after but probably first in most cases).
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {   
        if (Instance != null && Instance != this as T)
        {
            Debug.LogWarning("Two Instances of a singleton class exist. Deleting second of instances on: " + gameObject + ". Other instance can be found at :" + Instance);
            Destroy(this);
            return;
        }

        Instance = this as T;
    }
}