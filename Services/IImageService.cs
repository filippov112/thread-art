using Microsoft.AspNetCore.SignalR;

public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile imageFile);
    Task<(string ResultImagePath, string RouteFilePath, List<(int, int)> Route, List<(int, int)> GridPoints, List<string> FormattedRoute)> ProcessImageAsync(string imagePath, CalculationParameters parameters);
}

public class ImageService : IImageService
{
    ImageProcessor processor;
    private readonly IWebHostEnvironment _env;
    public ImageService(IHubContext<ProgressHub> hubContext, IWebHostEnvironment env)
    {
        processor = new(hubContext);
        _env = env;
    }

    public async Task<string> SaveImageAsync(IFormFile imageFile)
    {
        var imagesFolder = Path.Combine(_env.WebRootPath, "images");
        if (!Directory.Exists(imagesFolder))
        {
            Directory.CreateDirectory(imagesFolder);
        }
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(imagesFolder, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return $"/images/{fileName}";
    }

    public async Task<(string ResultImagePath, string RouteFilePath, List<(int, int)> Route, List<(int, int)> GridPoints, List<string> FormattedRoute)> ProcessImageAsync(string imagePath, CalculationParameters parameters)
    {
        Guid guid = Guid.NewGuid();
        var outputImagePath = Path.Combine("/images", $"{guid.ToString()}_output.png");
        var routePixelFilePath = Path.Combine("/images", $"{guid.ToString()}_routepixel.txt");
        var routeFilePath = Path.Combine("/images", $"{guid.ToString()}_route.txt");

        var (gridPoints, formattedRoute) = await processor.ProcessImage(
            _env.WebRootPath + imagePath,
            _env.WebRootPath + outputImagePath,
            _env.WebRootPath + routePixelFilePath,
            _env.WebRootPath + routeFilePath,
            (parameters.SmallWidth, parameters.SmallHeight),
            (parameters.LargeWidth, parameters.LargeHeight),
            parameters.N,
            parameters.NumSteps,
            parameters.IsEllipseMatrix,
            parameters.Dx
        );

        var route = File.ReadAllLines(_env.WebRootPath + routePixelFilePath)
                        .Select(line => line.Split(','))
                        .Select(parts => (int.Parse(parts[0]), int.Parse(parts[1])))
                        .ToList();

        return (outputImagePath, routeFilePath, route, gridPoints, formattedRoute);
    }
}