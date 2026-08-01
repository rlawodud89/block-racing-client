using System;

public static class MatchEvents
{
    public static event Action OnMatchFound;
    public static event Action OnMatchCanceled;

    public static void RaiseMatchFound()
    {
        OnMatchFound?.Invoke();
    }

    public static void RaiseMatchCanceled()
    {
        OnMatchCanceled?.Invoke();
    }
}