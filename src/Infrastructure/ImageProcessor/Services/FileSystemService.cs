using System.Text;
using Application.ImageProcessor.Services;
using Domain.ImageProcessor.Models;
using Infrastructure.ImageProcessor.Settings;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Infrastructure.ImageProcessor.Services;

public class FileSystemService(IOptions<StorageOptions> options) : IFileSystemService
{
    private readonly string _storagePath = options.Value.FolderPath;

    public async Task<ImageMatrix> ReadOriginalImageAsync(string systemPath, int countSidePoints, CancellationToken ct = default)
    {
        using var originalImageStream = await OpenFileWithRetryAsync(systemPath, ct);
        using var image = await Image.LoadAsync<Rgba32>(originalImageStream, ct);

        var pixels = new int[image.Width * image.Height];
        var matrix = new ImageMatrix(image.Width, image.Height, countSidePoints);
        for (int y = 0; y < image.Height; y++)
            for (int x = 0; x < image.Width; x++)
                pixels[y * image.Width + x] = (image[x, y].R + image[x, y].G + image[x, y].B) / 3;

        matrix.Pixels = pixels;
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


    public async Task<string> SaveRouteAsync(Route route, CancellationToken cancellationToken = default)
    {
        string resultRouteFileName = Guid.NewGuid().ToString() + ".txt";
        string systemPath = Path.Combine(_storagePath, resultRouteFileName);


        using var resultRouteStream = new FileStream(systemPath, FileMode.Create, FileAccess.Write, FileShare.Read);
        List<string> steps = [.. route.Points.Select(p => p.ToString())];
        byte[] buffer = Encoding.UTF8.GetBytes(string.Join('\n', steps));
        await resultRouteStream.WriteAsync(buffer, cancellationToken);
        await resultRouteStream.FlushAsync(cancellationToken);

        return systemPath;
    }

    public async Task<string> SaveOriginalImageAsync(string fileName, Stream originalStream, CancellationToken cancellationToken = default)
    {
        string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        Directory.CreateDirectory(Path.Combine(_storagePath));

        string systemPath = Path.Combine(_storagePath, newFileName);

        using var fileStream = new FileStream(systemPath, FileMode.Create, FileAccess.Write, FileShare.Read);
        await originalStream.CopyToAsync(fileStream, cancellationToken);
        await fileStream.FlushAsync(cancellationToken);

        return systemPath;
    }

    public async Task<string> SaveResultImageAsync(int innerWidth, int innerHeight, int padding, int[] data, SectorPoint[] sidePoints, CancellationToken ct = default)
    {
        using Image<Rgba32> image = new(innerWidth + 2 * padding, innerHeight + 2 * padding);
        Painter.DrawMatrix(image, data);
        Painter.DrawCoordinateGrid(image, padding, sidePoints);

        string resultImageName = Guid.NewGuid().ToString() + ".png";
        string systemPath = Path.Combine(_storagePath, resultImageName);

        using var resultImageStream = new FileStream(systemPath, FileMode.Create, FileAccess.Write, FileShare.Read);
        await image.SaveAsync(resultImageStream, new PngEncoder(), ct);
        await resultImageStream.FlushAsync(ct);

        return systemPath;
    }
}
