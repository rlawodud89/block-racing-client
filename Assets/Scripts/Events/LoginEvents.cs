using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoginEvents
{
    public static event Action OnLoginSuccess;


    public static void RaiseSuccess()
    {
        OnLoginSuccess?.Invoke();
    }
}
