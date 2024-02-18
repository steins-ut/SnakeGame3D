public enum GameOverReason
{
    HYPERTHERMIA,
    HYPOTHERMIA,
    BLOODTHIRST,
    INSANITY,
    MONSTER,
    OUT_OF_HP
}

public static class GameOverReasonUtility
{
    public static string GetDescription(GameOverReason reason)
    {
        switch (reason)
        {
            case GameOverReason.HYPERTHERMIA:
                return "You let the Egg get too hot...";

            case GameOverReason.HYPOTHERMIA:
                return "You let the Egg get too cold...";

            case GameOverReason.BLOODTHIRST:
                return "The Egg got a little too bloodthirsty...";

            case GameOverReason.OUT_OF_HP:
                return "The Egg got too hurt...";

            case GameOverReason.INSANITY:
                return "You let your stress go to your head...";

            default:
                return "wat";
        }
    }
}