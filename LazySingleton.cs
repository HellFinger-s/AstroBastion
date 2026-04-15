using UnityEngine;
using System;

public abstract class LazySingleton<T> : MonoBehaviour where T : LazySingleton<T>
{
    public static T instance;

    protected bool IsInitialized { get; private set; }

    protected virtual void Awake()
    {
        instance = (T)this;

        StartCoroutine(InitializeAfterDependencies());
    }

    public static event Action OnInitialized;

    protected abstract bool CheckDependencies();
    protected abstract void OnDependenciesReady();

    private System.Collections.IEnumerator InitializeAfterDependencies()
    {
        while (!CheckDependencies())
        {
            yield return null;
        }

        OnDependenciesReady();
        IsInitialized = true;

        OnInitialized?.Invoke();
    }

    public static T GetInstance()
    {
        return instance;
    }
}