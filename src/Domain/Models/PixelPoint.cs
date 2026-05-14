using System.Diagnostics.CodeAnalysis;

namespace Domain.Models
{
    public class PixelPoint(int x, int y)
    {
        public int X { get; } = x;
        public int Y { get; } = y;

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is PixelPoint p && p.GetHashCode() == GetHashCode();
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public static bool operator ==(PixelPoint left, PixelPoint right) => left.Equals(right);
        public static bool operator !=(PixelPoint left, PixelPoint right) => !(left == right);
        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }
}
