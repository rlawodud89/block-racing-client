using block_racing_common.Game.Enums;

public static class ResultData
{
    public static GameResultType Result { get; set; }
    public static GameEndReason Reason { get; set; }

    public static void SetResult(GameResultType result, GameEndReason reason)
    {
        Result = result;
        Reason = reason;
    }

    public static void Clear()
    {
        Result = default;
        Reason = default;
    }
}