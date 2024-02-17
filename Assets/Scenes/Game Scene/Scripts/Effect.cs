public struct Effect
{
    public EffectType type;
    public int magnitude;
    public int delay;
    public int repeat;

    public Effect(EffectType type, int magnitude, int delay, int repeat)
    {
        this.type = type;
        this.magnitude = magnitude;
        this.delay = delay;
        this.repeat = repeat;
    }
}
