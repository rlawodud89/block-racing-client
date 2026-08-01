using block_racing_common.Game.Enums;

public static class ResultData
{
    public static GameResultType Result { get; set; }

    public static void SetResult(GameResultType result)
    {
        Result = result;
    }

    public static void Clear()
    {
        Result = default;
    }
}