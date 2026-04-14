namespace Domain.Models;

public readonly struct PixelData(byte r, byte g, byte b)
{
    public byte R { get; } = r;
    public byte G { get; } = g;
    public byte B { get; } = b;
}
