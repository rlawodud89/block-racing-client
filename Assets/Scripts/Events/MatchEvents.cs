using System;

public static class MatchEvents
{
    public static event Action OnMatchFound;

    public static void RaiseMatchFound()
    {
        OnMatchFound?.Invoke();
    }
}