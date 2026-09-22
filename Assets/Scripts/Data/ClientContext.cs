
public static class ClientContext
{
    public static long PlayerId { get; private set; }


    public static void SetLogin(
        long playerId)
    {
        PlayerId = playerId;
    }
}