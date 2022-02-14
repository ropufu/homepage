namespace Ropufu.Homepage;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// https://en.wikipedia.org/wiki/Xorshift#xorwow
/// </remarks>
public struct XorWowRandom
{
    private uint state0;
    private uint state1;
    private uint state2;
    private uint state3;
    private uint state4;
    private uint counter = 0;

    public XorWowRandom()
        : this(
              (uint)Guid.NewGuid().GetHashCode(),
              (uint)Guid.NewGuid().GetHashCode(),
              (uint)Guid.NewGuid().GetHashCode(),
              (uint)Guid.NewGuid().GetHashCode())
    {
    }

    public XorWowRandom(uint seed0, uint seed1, uint seed2, uint seed3, uint seed4 = 0)
    {
        if (seed0 == 0)
            throw new ArgumentException("Seed 0 cannot be zero.", nameof(seed0));
        if (seed1 == 0)
            throw new ArgumentException("Seed 1 cannot be zero.", nameof(seed1));
        if (seed2 == 0)
            throw new ArgumentException("Seed 2 cannot be zero.", nameof(seed2));
        if (seed3 == 0)
            throw new ArgumentException("Seed 3 cannot be zero.", nameof(seed3));

        this.state0 = seed0;
        this.state1 = seed1;
        this.state2 = seed2;
        this.state3 = seed3;
        this.state4 = seed4;
    }

    public uint Next()
    {
        uint t = this.state4;

        uint s = this.state0;
        this.state4 = this.state3;
        this.state3 = this.state2;
        this.state2 = this.state1;
        this.state1 = s;

        t ^= (t >> 2);
        t ^= (t << 1);
        t ^= (s ^ (s << 4));
        this.state0 = t;
        this.counter += 362437U;
        return t + this.counter;
    }
}
