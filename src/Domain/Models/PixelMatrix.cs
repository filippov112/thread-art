using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models;

public class PixelMatrix(double[,] values)
{
    public int Width { get; } = values.GetLength(0); 
    public int Height { get; } = values.GetLength(1);
    public double[,] Values { get; } = values;

    public PixelMatrix(int width, int height) : this(FillZero(width, height)) { }

    private static double[,] FillZero(int width, int height)
    {
        double[,] res = new double[width, height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                res[i, j] = 0;
        return res;
    }
}
