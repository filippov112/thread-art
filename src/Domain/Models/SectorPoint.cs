using System.Diagnostics.CodeAnalysis;

namespace Domain.Models
{
    public struct SectorPoint
    {
        public SectorPoint(PixelPoint pixel, SizeImage size)
        {
            Sector = 'U';
            if (pixel.X == 0)
                Sector = 'L';
            if (pixel.X == size.Width - 1)
                Sector = 'R';
            if (pixel.Y == 0)
                Sector = 'T';
            if (pixel.Y == size.Height - 1)
                Sector = 'B';
            Pixel = pixel;
        }
        public char Sector { get; }
        public PixelPoint Pixel { get; }

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is SectorPoint p && p.Sector == Sector && p.Pixel.Number == Pixel.Number;
        }

        public override readonly int GetHashCode()
        {
            return $"{Sector}_{Pixel.Number}".GetHashCode();
        }

        public static bool operator ==(SectorPoint left, SectorPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SectorPoint left, SectorPoint right)
        {
            return !(left == right);
        }

        public override readonly string ToString()
        {
            return $"{Sector}{Pixel.Number}";
        }
    }
}
