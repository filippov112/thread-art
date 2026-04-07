using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Domain.Models
{
    public struct MatrixPoint(char sector, int number)
    {
        public char Sector = sector;
        public int Number = number;

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is MatrixPoint p && p.Sector == Sector && p.Number == Number;
        }

        public override readonly int GetHashCode()
        {
            return $"{Sector}_{Number}".GetHashCode();
        }

        public static bool operator ==(MatrixPoint left, MatrixPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MatrixPoint left, MatrixPoint right)
        {
            return !(left == right);
        }
    }
}
