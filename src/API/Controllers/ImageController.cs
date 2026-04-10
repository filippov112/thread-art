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
            config.ResultImagePath = Path.Combine(config.WebRootPath, $"{guid}_output.png");
            config.ResultRoutePath = Path.Combine(config.WebRootPath, $"{guid}_route.txt");
            config.ContrastLine = parameters.ContrastLine;
            config.IsEllipse = parameters.IsEllipse;
            config.CountSteps = parameters.CountSteps;
            config.CountPoints = parameters.CountPoints;
            config.LargeSize = parameters.LargeSize;
            config.SmallSize = parameters.SmallSize;

            await _imageService.ProcessImage(parameters.ImageFile, config);

            parameters.OriginalImagePath = config.OriginalImagePath;
            parameters.ResultImagePath = config.ResultImagePath;
            parameters.ResultRoutePath = config.ResultRoutePath;

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