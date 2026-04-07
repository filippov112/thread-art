using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;

namespace Domain.Models
{
    public struct PixelPoint(int x, int y)
    {
        public int X = x; 
        public int Y = y;

        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is PixelPoint p && p.X == X && p.Y == Y;
        }

        public override readonly int GetHashCode()
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

        /// <summary>
        /// Вычисляет координаты точек матрицы, лежащие на прямой линии между двумя точками на её краях
        /// </summary>
        /// <param name="b">Точка конца линии</param>
        /// <returns></returns>
        List<PixelPoint> BresenhamLine(PixelPoint b)
        {
            var (x1, y1) = this;
            var (x2, y2) = b;
            var points = new List<(int, int)>();
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                points.Add((x1, y1));
                if (x1 == x2 && y1 == y2) break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x1 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y1 += sy;
                }
            }
            return points;
        }
    }
}
