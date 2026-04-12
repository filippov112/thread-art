using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Domain.Models
{
    public struct SectorPoint(char sector, int number, PixelPoint pixel)
    {
        public char Sector = sector;
        public int Number = number;
        public PixelPoint? Pixel = pixel;

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is SectorPoint p && p.Sector == Sector && p.Number == Number;
        }

        public override readonly int GetHashCode()
        {
            return $"{Sector}_{Number}".GetHashCode();
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
            return $"{Sector}{Number}";
        }
    }
}
