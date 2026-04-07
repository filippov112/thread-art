public class ImageModel
{
    public IFormFile ImageFile { get; set; }
    public CalculationParameters Parameters { get; set; } = new CalculationParameters();
    public string OriginalImagePath { get; set; }
    public string ResultImagePath { get; set; }
    public string RouteFilePath { get; set; }
    public List<(int, int)> Route { get; set; }
    public List<string> FormattedRoute { get; set; } // Новое поле для форматированного маршрута
    public List<(int, int)> GridPoints { get; set; } // Координаты точек сетки
}

public class CalculationParameters
{
    public int SmallWidth { get; set; } = 270;
    public int SmallHeight { get; set; } = 270;
    public int LargeWidth { get; set; } = 540;
    public int LargeHeight { get; set; } = 540;
    public int N { get; set; } = 240;
    public int NumSteps { get; set; } = 4000;
    public bool IsEllipseMatrix { get; set; } = true;
    public int Dx { get; set; } = 8;
}