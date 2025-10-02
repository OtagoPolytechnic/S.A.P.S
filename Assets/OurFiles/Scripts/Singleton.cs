using UnityEngine;

//Base written by: Christian Irvine

/// <summary>
/// Generic base class for creating MonoBehaviour singletons.
/// Inherit from this class to ensure only one instance of a component exists at runtime.
/// </summary>
/// <typeparam name="T">
/// The type of the singleton class (the child type that derives from this).
/// </typeparam>
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