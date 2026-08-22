using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkEvents
{
    public static event Action OnConnected;
    public static event Action OnDisconnected;

    public static void RaiseConnected()
    {
        OnConnected?.Invoke();
    }

    public static void RaiseDisconnected()
    {
        OnDisconnected?.Invoke();
    }
}
