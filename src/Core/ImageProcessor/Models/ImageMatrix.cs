using Core.ImageProcessor.Services;

namespace Core.ImageProcessor.Models;

/// <summary>
/// Математическое представление изображения для обработки
/// </summary>
public record ImageMatrix
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int CountSidePoints { get; private set; }
    public SectorPoint[]? SidePoints { get; private set; }

    public int[]? Pixels { get; set; }

    /// <summary>
    /// Базовый конструктор с моделированием точек
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="n"></param>
    /// <exception cref="Exception"></exception>
    public ImageMatrix(int width, int height, int n)
    {
        Width = width;
        Height = height;
        CountSidePoints = n;

        FillSidePoints();
        if (SidePoints is null || SidePoints.Length == 0)
            throw new Exception("Ошибка! Недостаточное количество крайних точек!");
    }

    /// <summary>
    /// На основе готовой матрицы
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="sidePoints"></param>
    public ImageMatrix(int width, int height, SectorPoint[] sidePoints)
    {
        Width = width;
        Height = height;
        SidePoints = sidePoints;
        CountSidePoints = sidePoints.Length;
    }

    private void FillSidePoints()
    {
        if (CountSidePoints < 2 || Width < 3 || Height < 3)
            SidePoints = [];

        CountSidePoints = Math.Min(CountSidePoints, 2 * (Width + Height) - 8); // Не разрешаем число точек большее, чем число пикселей
        var selectedPoints = FindPoints();
        List<SectorPoint> results = [];

        // Вычисляем вершины
        int counterT = 0;
        for (int j = 1; j < Width - 1; j++) // top
            if (selectedPoints.Contains(new(j, 0)))
                results.Add(new(new(j, 0), Width, Height, counterT + 1));
        int counterR = 0;
        for (int j = 1; j < Height - 1; j++) // right
            if (selectedPoints.Contains(new(Width - 1, j)))
                results.Add(new(new(Width - 1, j), Width, Height, counterR + 1));
        int counterB = 0;
        for (int j = 1; j < Width - 1; j++) // bottom
            if (selectedPoints.Contains(new(Width - 1 - j, Height - 1)))
                results.Add(new(new(Width - 1 - j, Height - 1), Width, Height, counterB + 1));
        int counterL = 0;
        for (int j = 1; j < Height - 1; j++) // left
            if (selectedPoints.Contains(new(0, Height - 1 - j)))
                results.Add(new(new(0, Height - 1 - j), Width, Height, counterL + 1));
        SidePoints = [.. results];
    }

    /// <summary>
    /// Находит вершины на пересечении лучей с границей изображения
    /// </summary>
    private HashSet<PixelPoint> FindPoints()
    {
        HashSet<PixelPoint> selectedPoints = [];
        // Находим центр
        var max = Math.Max(Width, Height);
        double x0 = (int)Math.Round(Width / 2.0);
        double y0 = (int)Math.Round(Height / 2.0);

        for (double angle = 0; angle < 2 * Math.PI; angle += 2 * Math.PI / CountSidePoints)
        {
            int x = (int)(2 * max * Math.Cos(angle) + x0);
            int y = (int)(2 * max * Math.Sin(angle) + y0);
            IEnumerable<PixelPoint> line = BresenhamAlgorithm.GetLineIterator(
                new((int)x0, (int)y0),
                new(x, y)
            );
            foreach (var point in line) // Выбираем первую точку, которая попадает на край изображения
            {
                if ((point.X == 0 || point.X == Width - 1 || point.Y == 0 || point.Y == Height - 1) // Определяем по крайнему значению любой из координат
                    && !(point.X == 0 && point.Y == 0) && !(point.X == Width - 1 && point.Y == Height - 1) // Исключаем углы главной диагонали
                    && !(point.X == 0 && point.Y == Height - 1) && !(point.X == Width - 1 && point.Y == 0) // Исключаем углы побочной диагонали
                )
                {
                    selectedPoints.Add(point);
                    break;
                }
            }
        }
        return selectedPoints;
    }
}
