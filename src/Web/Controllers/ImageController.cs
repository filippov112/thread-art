using Application.DTO;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;

namespace Web.Controllers;

public class ImageController : Controller
{
    private readonly ImageProcessor _imageService;
    private readonly IWebHostEnvironment _env;
    private readonly IStreamController _streamController;
    private readonly IPathManager _pathManager;

    public ImageController(ImageProcessor imageService, IWebHostEnvironment env, IStreamController streamController, IPathManager pathManager)
    {
        _imageService = imageService;
        _env = env;
        _streamController = streamController;
        _pathManager = pathManager;
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(ResultViewModel data)
    {
        if (data.ImageFile == null || data.ImageFile.Length == 0)
            return View("Index", data);

        _pathManager.InitNamesAndPaths(_env.WebRootPath, data.ImageFile.FileName);

        var viewModel = new ResultViewModel
        {
            OriginalImagePath = _pathManager.OriginalImagePathVM,
            ResultImagePath = _pathManager.ResultImagePathVM,
            ResultRoutePath = _pathManager.ResultRouteFilePathVM,

            CountPoints = data.CountPoints,
            CountSteps = data.CountSteps,
            ContrastLine = data.ContrastLine
        };

        try
        {
            using (var memoryStream = new MemoryStream())
            {
                await data.ImageFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                await _streamController.SaveFromMemory(_pathManager.OriginalImagePath, memoryStream);
                using var resultImageStream = _streamController.MakeStream(_pathManager.ResultImagePath);
                using var resultRouteStream = _streamController.MakeStream(_pathManager.ResultRouteFilePath);

                var request = new ProcessingRequest
                {
                    OriginalImageStream = memoryStream,
                    ResultImageStream = resultImageStream,
                    ResultRouteStream = resultRouteStream,
                    CountPoints = data.CountPoints,
                    CountSteps = data.CountSteps,
                    ContrastLine = data.ContrastLine
                };
                await _imageService.ProcessImageAsync(request);
            }
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
