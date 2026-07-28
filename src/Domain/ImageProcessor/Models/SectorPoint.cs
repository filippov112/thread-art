using System.Diagnostics.CodeAnalysis;

namespace Domain.ImageProcessor.Models
{
    /// <summary>
    /// Узловые точки по краям изображения
    /// </summary>
    public readonly struct SectorPoint
    {
        public SectorPoint(PixelPoint pixel, int width, int height, int number)
        {
            Number = number;
            Sector = 'U';
            if (pixel.X == 0)
                Sector = 'L';
            if (pixel.X == width - 1)
                Sector = 'R';
            if (pixel.Y == 0)
                Sector = 'T';
            if (pixel.Y == height - 1)
                Sector = 'B';
            Pixel = pixel;
        }
        public char Sector { get; }
        public int Number { get; }
        public PixelPoint Pixel { get; }

        public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is SectorPoint p && p.GetHashCode() == GetHashCode();
        public override readonly int GetHashCode() => HashCode.Combine(Sector, Number);
        public static bool operator ==(SectorPoint left, SectorPoint right) => left.Equals(right);
        public static bool operator !=(SectorPoint left, SectorPoint right) => !(left == right);
        public override readonly string ToString() => $"{Sector}{Number}";

    }
}
