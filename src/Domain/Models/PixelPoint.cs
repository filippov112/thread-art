using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;

namespace Domain.Models
{
    public class PixelPoint(int x, int y)
    {
        public int X = x; 
        public int Y = y;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is PixelPoint p && p.X == X && p.Y == Y;
        }

        public override int GetHashCode()
        {
            return $"{X}_{Y}".GetHashCode();
        }

        public static bool operator ==(PixelPoint left, PixelPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PixelPoint left, PixelPoint right)
        {
            return !(left == right);
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }
}
