using Application.UseCases;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

public class ImageController : Controller
{
    private readonly ImageHandling _imageService;
    private readonly IWebHostEnvironment _env;

    public ImageController(ImageHandling imageService, IWebHostEnvironment env)
    {
        _imageService = imageService;
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(ImageDto parameters)
    {
        if (parameters.ImageFile != null && parameters.ImageFile.Length > 0)
        {
            var imagesFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }
            Guid guid = Guid.NewGuid();
            Config config = new()
            {
                WebRootPath = imagesFolder
            };
            var outputImagePath = $"{guid.ToString()}_output.png";
            var routeFilePath = $"{guid.ToString()}_route.txt";

            config.ResultImagePath = Path.Combine(config.WebRootPath, outputImagePath);
            config.ResultRoutePath = Path.Combine(config.WebRootPath, routeFilePath);
            config.ContrastLine = parameters.ContrastLine;
            config.IsEllipse = parameters.IsEllipse;
            config.CountSteps = parameters.CountSteps;
            config.CountPoints = parameters.CountPoints;
            config.LargeSize = parameters.LargeSize;
            config.SmallSize = parameters.SmallSize;

            await _imageService.ProcessImage(parameters.ImageFile, config);

            parameters.OriginalImagePath = Path.Combine("/Images", Path.GetFileName(config.OriginalImagePath));
            parameters.ResultImagePath = Path.Combine("/Images", outputImagePath);
            parameters.ResultRoutePath = Path.Combine("/Images", routeFilePath);

            return View("Result", parameters);
        }

        return View("Index", parameters);
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ImageDto());
    }
}