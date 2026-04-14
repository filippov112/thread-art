namespace Domain.Models;

public class PixelMatrix
{
    public int Width { get; }
    public int Height { get; }
    public double[,] Values { get; } // 255-based

    private readonly int _padding = 10;

    public PixelMatrix(PixelData[,] data)
    {
        double[,] values = new double[data.GetLength(0), data.GetLength(1)];
        for (int i = 0; i < data.GetLength(0); i++)
            for (int j = 0; j < data.GetLength(1); j++)
                values[i, j] = (data[i, j].R + data[i, j].G + data[i, j].B) / 3;

        Values = values;
        Width = data.GetLength(0);
        Height = data.GetLength(1);
    }

    public PixelMatrix(int width, int height, Route route, int padding)
    {
        Width = width + padding * 2;
        Height = height + padding * 2;
        _padding = padding;
        Values = RenderRoute(route);
    }

    private double[,] FillValues()
    {
        double[,] result = new double[Width, Height];
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                result[i, j] = 0;
        return result;
    }

    public double[,] RenderRoute(Route route)
    {
        var lineMatrix = FillValues();
        foreach (var line in route.Lines)
            foreach (var point in line.Points)
                lineMatrix[point.X + _padding, point.Y + _padding] += 1;

        double maxValue = CalcContrast(lineMatrix);
        NormalizeValues(lineMatrix, maxValue);
        return lineMatrix;
    }

    private double CalcContrast(double[,] values)
    {
        double maxValue = 0;
        for (int i = _padding; i < Width - _padding; i++)
            for (int j = _padding; j < Height - _padding; j++)
                if (i != _padding && i != Width - 1 - _padding && j != _padding && j != Height - 1 - _padding) // Пропускаем вершины, как самые плотные узлы
                    maxValue = Math.Max(maxValue, values[i, j]);
        return maxValue;
    }
    private void NormalizeValues(double[,] values, double maxValue)
    {
        for (int i = _padding; i < Width - _padding; i++)
            for (int j = _padding; j < Height - _padding; j++)
                values[i, j] = 255 - 255 * values[i, j] / maxValue;
    }
}
