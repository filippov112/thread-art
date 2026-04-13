using Application.UseCases;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

public class ImageController(ImageProcessor imageService, IWebHostEnvironment env) : Controller
{
    private readonly ImageProcessor _imageService = imageService;
    private readonly IWebHostEnvironment _env = env;

    [HttpPost]
    public async Task<IActionResult> UploadImage(ResultViewModel data)
    {
        if (data.ImageFile == null || data.ImageFile.Length == 0)
            return View("Index", data);

        var request = new ProcessImageRequest
        {
            ImageStream = data.ImageFile.OpenReadStream(),
            FileName = data.ImageFile.FileName,
            Directory = _env.WebRootPath,
            Config = new Config
            {
                CountPoints = data.CountPoints,
                CountSteps = data.CountSteps,
                ContrastLine = data.ContrastLine
            }
        };

        try
        {
            var result = await _imageService.ProcessImageAsync(request);

            var viewModel = new ResultViewModel
            {
                OriginalImagePath = '/' + result.OriginalImagePath,
                ResultImagePath = '/' + result.ResultImagePath,
                ResultRoutePath = '/' + result.RouteFilePath,

                CountPoints = request.Config.CountPoints,
                CountSteps = request.Config.CountSteps,
                ContrastLine = request.Config.ContrastLine
            };

            return View("Result", viewModel);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Ошибка обработки изображения: " + ex.Message);
            return View("Index", new ResultViewModel());
        }
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ResultViewModel());
    }
}
