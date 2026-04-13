namespace Domain.Models
{
    public class Line(PixelPoint A, PixelPoint B)
    {
        public List<PixelPoint> Points = GetBresenhamLine(A, B);

        /// <summary>
        /// Вычисляет координаты точек матрицы, лежащие на прямой линии между двумя точками на её краях
        /// </summary>
        private static List<PixelPoint> GetBresenhamLine(PixelPoint A, PixelPoint B)
        {
            var (x1, y1) = A;
            var (x2, y2) = B;
            var points = new List<PixelPoint>();
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                points.Add(new(x1, y1, A.Number));
                if (x1 == x2 && y1 == y2)
                {
                    points.Last().Number = B.Number;
                    break;
                }
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

        public bool IsRevert(Line other)
        {
            if (other.Points.Count == 0 || Points.Count == 0)
                return false;
            return other.Points.First() == Points.Last() && other.Points.Last() == Points.First();
        }
    }
}
