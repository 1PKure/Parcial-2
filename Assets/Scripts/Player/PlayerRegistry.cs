using System;
using UnityEngine;

public static class PlayerRegistry
{
    public static event Action<Transform> OnPlayerRegistered;
    public static Transform Player { get; private set; }

    public static void Register(Transform t)
    {
        Player = t;
        OnPlayerRegistered?.Invoke(Player);
    }

    public static void Unregister(Transform t)
    {
        if (Player == t) Player = null;
    }
}