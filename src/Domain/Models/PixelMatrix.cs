namespace Domain.Models;

public class PixelMatrix
{
    public int Width { get; }
    public int Height { get; }
    public double[,] Values { get; } // 255-based


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

    public PixelMatrix(int width, int height, double[,] values)
    {
        Width = width;
        Height = height;
        Values = values;
    }


}
