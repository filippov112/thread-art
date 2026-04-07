using Microsoft.AspNetCore.Mvc;

public class ImageController : Controller
{
    private readonly IImageService _imageService;
    private readonly IWebHostEnvironment _env;

    public ImageController(IImageService imageService, IWebHostEnvironment env)
    {
        _imageService = imageService;
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(ImageModel model)
    {
        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            var imagesFolder = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            var originalImagePath = await _imageService.SaveImageAsync(model.ImageFile);
            model.OriginalImagePath = originalImagePath;

            var result = await _imageService.ProcessImageAsync(originalImagePath, model.Parameters);
            model.ResultImagePath = result.ResultImagePath;
            model.RouteFilePath = result.RouteFilePath;
            model.Route = result.Route;
            model.GridPoints = result.GridPoints;
            model.FormattedRoute = result.FormattedRoute;

            return View("Result", model);
        }

        return View("Index", model);
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = new ImageModel
        {
            Parameters = new CalculationParameters()
        };
        return View(model);
    }
}