using Microsoft.AspNetCore.SignalR;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public class ImageProcessor
{
    private readonly IHubContext<ProgressHub> _hubContext;
    private static Random random = new Random();

    public ImageProcessor(IHubContext<ProgressHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    private static (int, int) SelectBeginPoint(List<(int, int)> keys)
    {
        return keys[random.Next(keys.Count)];
    }

    private static List<(int, int)> BresenhamLine((int, int) b, (int, int) e)
    {
        var (x1, y1) = b;
        var (x2, y2) = e;
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

    private static Dictionary<(int, int), List<List<(int, int)>>> GeneratePaths(int width, int height, int n, bool isEllipseMatrix)
    {
        var sidePoints = new List<(int, int)>();
        int stepPixel = 2 * (height + width) / n;

        if (isEllipseMatrix)
        {
            double a = width / 2.0 - 1;
            double b = height / 2.0 - 1;
            for (double angle = 0; angle < 2 * Math.PI; angle += 2 * Math.PI / n)
            {
                int x = Math.Clamp((int)(a * Math.Cos(angle) + a), 0, width - 1);
                int y = Math.Clamp((int)(b * Math.Sin(angle) + b), 0, height - 1);
                sidePoints.Add((y, x));
            }
        }
        else
        {
            for (int i = 0; i < width; i += stepPixel)
            {
                sidePoints.Add((0, i)); 
                sidePoints.Add((height - 1, i)); 
            }
            for (int j = 0; j < height; j += stepPixel)
            {
                sidePoints.Add((j, 0));
                sidePoints.Add((j, width - 1));
            }
        }

        var paths = new Dictionary<(int, int), List<List<(int, int)>>>();
        foreach (var start in sidePoints)
        {
            paths[start] = new List<List<(int, int)>>();
            foreach (var end in sidePoints)
            {
                if (start != end && (isEllipseMatrix || !(
                    (start.Item1 == 0 && end.Item1 == 0) || 
                    (start.Item1 == height - 1 && end.Item1 == height - 1) || 
                    (start.Item2 == 0 && end.Item2 == 0) || 
                    (start.Item2 == width - 1 && end.Item2 == width - 1)
                )))
                {
                    paths[start].Add(BresenhamLine(start, end));
                }
            }
        }
        return paths;
    }

    private static List<(int, int)> EvaluatePath((int, int) start, List<(int, int)> route, Dictionary<(int, int), List<List<(int, int)>>> paths, double[,] exemplaryMatrix)
    {
        double maxTotal = double.MinValue;
        List<(int, int)> bestPath = new List<(int, int)>();

        foreach (var path in paths[start])
        {
            double sum = 0;
            int count = 0;

            foreach (var p in path)
            {
                sum += exemplaryMatrix[p.Item1, p.Item2];
                count++;
            }

            if (count > 0)
            {
                double avgProb = sum / count;
                if (route.Count > 1)
                    if (path.Last() == route[^2])
                        continue;
                if (avgProb > maxTotal)
                {
                    maxTotal = avgProb;
                    bestPath = path;
                }
            }
        }

        paths[start].Remove(bestPath);
        return bestPath;
    }

    private static (int, int) SelectPath((int, int) start, List<(int, int)> route, Dictionary<(int, int), List<List<(int, int)>>> paths, double[,] exemplaryMatrix, int dx)
    {
        var bestPath = EvaluatePath(start, route, paths, exemplaryMatrix);
        if (bestPath.Count == 0) return start;

        foreach (var p in bestPath)
        {
            exemplaryMatrix[p.Item1, p.Item2] -= dx;
        }
        return bestPath.Last();
    }

    private static (int, int) ScalePoint((int, int) point, (int, int) smallShape, (int, int) largeShape)
    {
        double scaleY = (double)largeShape.Item1 / smallShape.Item1;
        double scaleX = (double)largeShape.Item2 / smallShape.Item2;
        return ((int)(point.Item1 * scaleY), (int)(point.Item2 * scaleX));
    }

    private static void DrawLineOnLargeMatrix((int, int) start, (int, int) end, Image<Rgba32> largeImage, int dx)
    {
        var linePoints = BresenhamLine(start, end);
        foreach (var p in linePoints)
        {
            if (p.Item1 >= 0 && p.Item1 < largeImage.Height && p.Item2 >= 0 && p.Item2 < largeImage.Width)
            {
                var pixel = largeImage[p.Item2, p.Item1];
                int newValue = Math.Clamp(pixel.R - dx, 0, 255);
                largeImage[p.Item2, p.Item1] = new Rgba32((byte)newValue, (byte)newValue, (byte)newValue);
            }
        }
    }

    private static void SaveRouteToFile(List<string> route, string filename)
    {
        using (var writer = new StreamWriter(filename))
        {
            foreach (var point in route)
            {
                writer.WriteLine(point);
            }
        }
    }

    private static void DrawCoordinateGrid(Image<Rgba32> image, int step = 50)
    {
        var font = SystemFonts.CreateFont("Arial", 12); 
        var pen = Pens.Solid(Color.Gray, 1);
        var brush = Brushes.Solid(Color.Gray);

        for (int x = 0; x < image.Width; x += step)
        {
            image.Mutate(ctx => ctx.DrawLine(pen, new PointF(x, 0), new PointF(x, image.Height)));
            if (x > 0)
            {
                var text = x.ToString();
                var textOptions = new RichTextOptions(font)
                {
                    Origin = new PointF(x, 5), 
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                };
                image.Mutate(ctx => ctx.DrawText(textOptions, text, brush));
            }
        }

        for (int y = 0; y < image.Height; y += step)
        {
            image.Mutate(ctx => ctx.DrawLine(pen, new PointF(0, y), new PointF(image.Width, y)));
            if (y > 0)
            {
                var text = y.ToString();
                var textOptions = new RichTextOptions(font)
                {
                    Origin = new PointF(5, y),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };
                image.Mutate(ctx => ctx.DrawText(textOptions, text, brush));
            }
        }
    }

    // Новый метод для генерации точек сетки
    private static List<(int, int)> GenerateGridPoints(int width, int height, int n, bool isEllipseMatrix)
    {
        var gridPoints = new List<(int, int)>();
        int stepPixel = 2 * (height + width) / n;

        if (isEllipseMatrix)
        {
            double a = width / 2.0 - 1;
            double b = height / 2.0 - 1;
            for (double angle = 0; angle < 2 * Math.PI; angle += 2 * Math.PI / n)
            {
                int x = Math.Clamp((int)(a * Math.Cos(angle) + a), 0, width - 1);
                int y = Math.Clamp((int)(b * Math.Sin(angle) + b), 0, height - 1);
                gridPoints.Add((y, x));
            }
        }
        else
        {
            int lostPixel = 0;
            for (int i = 0; i < width; i += stepPixel) // top
            {
                gridPoints.Add((0, i));
            }
            lostPixel = stepPixel - (width - gridPoints.Last().Item2);
            for (int j = lostPixel; j < height; j += stepPixel ) // right
            {
                gridPoints.Add((j, width - 1));
            }
            lostPixel = stepPixel - (height - gridPoints.Last().Item1);
            for (int i = width - lostPixel; i > 0; i -= stepPixel) // bottom
            {
                gridPoints.Add((height - 1, i));
            }
            lostPixel = stepPixel - gridPoints.Last().Item2;
            for (int j = height - lostPixel; j > 0; j -= stepPixel) // left
            {
                gridPoints.Add((j, 0));
            }
        }

        return gridPoints;
    }

    // Новый метод для преобразования координат холста в координаты сетки
    public static string ConvertToGridCoordinate((int, int) canvasPoint, List<(int, int)> gridPoints, int width, int height, bool isEllipseMatrix)
    {
        // Находим ближайшую точку сетки
        var closestPoint = gridPoints.OrderBy(p => 
            Math.Sqrt(Math.Pow(p.Item1 - canvasPoint.Item1, 2) + Math.Pow(p.Item2 - canvasPoint.Item2, 2)))
            .First();

        int index = gridPoints.IndexOf(closestPoint);

      
        return ConvertToCoordinate(index, gridPoints.Count, isEllipseMatrix);
       
    }

    private static string ConvertToCoordinate(int index, int totalPoints, bool isEllipseMatrix)
    {
        // Разделяем эллипс на 4 сектора
        int pointsPerSector = totalPoints / 4;
        int sector = index / pointsPerSector;
        int positionInSector = index % pointsPerSector;
        char sectorLetter;
        if (isEllipseMatrix)
            sectorLetter = sector switch
            {
                0 => 'A', // Правая часть (0° - 90°)
                1 => 'B', // Верхняя часть (90° - 180°)
                2 => 'C', // Левая часть (180° - 270°)
                3 => 'D', // Нижняя часть (270° - 360°)
                _ => 'A'
            };
        else
            sectorLetter = sector switch
            {
                0 => 'T',
                1 => 'R',
                2 => 'B',
                3 => 'L',
                _ => 'T'
            };
        return $"{sectorLetter}{positionInSector + 1}";
    }

    private static string ConvertToRectangleCoordinate((int, int) point, int width, int height)
    {
        var (y, x) = point;

        if (y == 0) // Верхняя сторона
        {
            int position = x * height / width + 1; // Нормализуем позицию
            return $"T{position}";
        }
        else if (y == height - 1) // Нижняя сторона
        {
            int position = x * height / width + 1;
            return $"B{position}";
        }
        else if (x == 0) // Левая сторона
        {
            int position = (height - 1 - y) + 1; // Снизу вверх
            return $"L{position}";
        }
        else if (x == width - 1) // Правая сторона
        {
            int position = (height - 1 - y) + 1; // Снизу вверх
            return $"R{position}";
        }

        return $"({y},{x})"; // Fallback для внутренних точек
    }

    public async Task<(List<(int, int)> GridPoints, List<string> FormattedRoute)> ProcessImage(string inputImagePath, string outputImagePath, string routeFilePath, (int, int) smallSize, (int, int) largeSize, int n, int numSteps, bool isEllipseMatrix, int dx)
    {
        using (var baseImage = Image.Load<Rgba32>(inputImagePath))
        using (var smallImage = baseImage.Clone(ctx => ctx.Resize(smallSize.Item1, smallSize.Item2)))
        {
            var smallMatrix = new double[smallSize.Item2, smallSize.Item1];
            for (int y = 0; y < smallSize.Item1; y++)
            {
                for (int x = 0; x < smallSize.Item2; x++)
                {
                    smallMatrix[x, y] = 255 - smallImage[y, x].R;
                }
            }

            var paths = GeneratePaths(smallSize.Item1, smallSize.Item2, n, isEllipseMatrix);
            var gridPoints = GenerateGridPoints(smallSize.Item1, smallSize.Item2, n, isEllipseMatrix);
            var start = SelectBeginPoint(paths.Keys.ToList());


            var route = new List<(int, int)> { start };
            for (int i = 0; i < numSteps; i++)
            {
                start = SelectPath(start, route, paths, smallMatrix, dx);
                if (start == route.Last()) break;
                route.Add(start);

                int progress = (int)((i + 1) / (double)numSteps * 100);
                await _hubContext.Clients.All.SendAsync("ReceiveProgress", progress);
            }

            // Преобразуем маршрут в координаты сетки
            var formattedRoute = route.Select(point => 
                ConvertToGridCoordinate(point, gridPoints, smallSize.Item1, smallSize.Item2, isEllipseMatrix))
                .ToList();

            using (var largeImage = new Image<Rgba32>(largeSize.Item1, largeSize.Item2))
            {
                largeImage.Mutate(ctx => ctx.BackgroundColor(Color.White));
                for (int i = 0; i < route.Count - 1; i++)
                {
                    var startScaled = ScalePoint(route[i], smallSize, largeSize);
                    var endScaled = ScalePoint(route[i + 1], smallSize, largeSize);
                    DrawLineOnLargeMatrix(startScaled, endScaled, largeImage, dx);
                }
                DrawCoordinateGrid(largeImage);
                SaveRouteToFile(formattedRoute, routeFilePath);
                largeImage.Save(outputImagePath);
            }

            return (gridPoints, formattedRoute);
        }
    }
}