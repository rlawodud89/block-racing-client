
public static class ClientContext
{
    public static long PlayerId { get; private set; }

    public static string Nickname { get; private set; }


    public static void SetLogin(
        long playerId,
        string nickname)
    {
        PlayerId = playerId;
        Nickname = nickname;
    }
}