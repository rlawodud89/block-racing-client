
public static class ClientContext
{
    public static int PlayerId { get; private set; }

    public static string Nickname { get; private set; }


    public static void SetLogin(
        int playerId,
        string nickname)
    {
        PlayerId = playerId;
        Nickname = nickname;
    }
}