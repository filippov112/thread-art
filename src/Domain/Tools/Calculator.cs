using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Tools
{
    public static class Calculator
    {
        /// <summary>
        /// Вычисляет координаты точек матрицы, лежащие на прямой линии между двумя точками на её краях
        /// </summary>
        /// <param name="a">Точка начала линии</param>
        /// <param name="b">Точка конца линии</param>
        /// <returns></returns>
        public static List<PixelPoint> BresenhamLine(PixelPoint a, PixelPoint b)
        {
            var (x1, y1) = a;
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
