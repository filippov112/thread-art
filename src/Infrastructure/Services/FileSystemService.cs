using System.Text;
using Application.Interfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Infrastructure.Services;

public class FileSystemService(IOptions<StorageOptions> options) : IFileSystemService
{
    private readonly string _webPath = options.Value.StaticFiles;
    private readonly string _storagePath = options.Value.FolderPath;

    public async Task<ImageMatrix> ReadOriginalImageAsync(string systemPath, CancellationToken ct = default)
    {
        using var originalImageStream = await OpenFileWithRetryAsync(systemPath, ct);
        using var image = await Image.LoadAsync<Rgba32>(originalImageStream, ct);
        var matrix = new ImageMatrix(image.Width, image.Height, new int[image.Width * image.Height]);
        for (int y = 0; y < image.Height; y++)
            for (int x = 0; x < image.Width; x++)
                matrix.Pixels[y * image.Width + x] = (image[x, y].R + image[x, y].G + image[x, y].B) / 3;
        return matrix;
    }
    private static async Task<Stream> OpenFileWithRetryAsync(string filePath, CancellationToken ct)
    {
        int retries = 5;
        while (retries > 0)
        {
            try
            {
                return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 4096, useAsync: true);
            }
            catch when (retries > 0)
            {
                retries--;
                // Ждем экспоненциально возрастающее время: 100мс, 200мс, 400мс...
                await Task.Delay(100 * (int)Math.Pow(2, 5 - retries), ct);
            }
        }
        throw new IOException($"Не удалось получить доступ к файлу {filePath} после нескольких попыток.");
    }


    public async Task<(string, string)> SaveRouteAsync(Route route, CancellationToken cancellationToken = default)
    {
        string resultRouteFileName = Guid.NewGuid().ToString() + ".txt";
        string systemPath = Path.Combine(_webPath, _storagePath, resultRouteFileName);
        string webPath = "/" + string.Join("/", _storagePath, resultRouteFileName);

        using var resultRouteStream = new FileStream(systemPath, FileMode.Create, FileAccess.Write, FileShare.Read);
        List<string> steps = [.. route.Points.Select(p => p.ToString())];
        byte[] buffer = Encoding.UTF8.GetBytes(string.Join('\n', steps));
        await resultRouteStream.WriteAsync(buffer, cancellationToken);
        await resultRouteStream.FlushAsync(cancellationToken);

        return (systemPath, webPath);
    }

    public async Task<(string, string)> SaveOriginalImageAsync(string fileName, Stream originalStream, CancellationToken cancellationToken = default)
    {
        string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        Directory.CreateDirectory(Path.Combine(_webPath, _storagePath));

        string systemPath = Path.Combine(_webPath, _storagePath, newFileName);
        string webPath = "/" + string.Join("/", _storagePath, newFileName);

        using var fileStream = new FileStream(systemPath, FileMode.Create, FileAccess.Write, FileShare.Read);
        await originalStream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);

        return (systemPath, webPath);
    }

    public async Task<(string, string)> SaveResultImageAsync(int padding, SectorPoint[] points, ImageMatrix matrix, CancellationToken ct = default)
    {
        using Image<Rgba32> image = new(matrix.Width, matrix.Height);
        Painter.DrawMatrix(image, matrix.Pixels);
        Painter.DrawCoordinateGrid(image, padding, points);

        string resultImageName = Guid.NewGuid().ToString() + ".png";
        string systemPath = Path.Combine(_webPath, _storagePath, resultImageName);
        string webPath = "/" + string.Join("/", _storagePath, resultImageName);

        using var resultImageStream = new FileStream(systemPath, FileMode.Create, FileAccess.Write, FileShare.Read);
        await image.SaveAsync(resultImageStream, new PngEncoder(), ct);
        await resultImageStream.FlushAsync(ct);

        return (systemPath, webPath);
    }
}
