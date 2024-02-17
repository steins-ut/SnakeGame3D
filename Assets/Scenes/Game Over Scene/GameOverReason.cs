public enum GameOverReason
{
    HYPERTERMIA,
    HYPOTHERMIA,
    INSANITY,
    OUT_OF_HP
}

public static class GameOverReasonUtility
{
    public static string GetDescription(GameOverReason reason)
    {
        switch (reason)
        {
            case GameOverReason.HYPERTERMIA:
                return "You let the Egg get too hot...";

            case GameOverReason.HYPOTHERMIA:
                return "You let the Egg get too cold...";

            case GameOverReason.OUT_OF_HP:
                return "The Egg got too hurt...";

            case GameOverReason.INSANITY:
                return "You let your stress go to your head...";
            
            default:
                return "wat";
        }
    }
}
