

using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases
{
    public class ImageHandling(ISaveManager saveManager, IPainter painter)
    {
        public async Task ProcessImage(IFormFile file, Config config)
        {
            config.OriginalImagePath = await saveManager.SaveImageAsync(file, config);

            // Получим данные об изображении
            double[,] gray_negative_matrix = await painter.GetImageGrayNegativeMatrix(config.OriginalImagePath);
            SizeImage size = painter.Size ?? new(0, 0);

            // Построим матрицу
            var matrix = new Matrix(size.Width, size.Height, config.CountPoints);
            
            // Выберем стартовую точку
            var start = matrix.SelectBeginPoint();
            
            // Найдем маршрут
            var route = await matrix.BuildRoute(start, gray_negative_matrix, config.ContrastLine, config.CountSteps);

            // Заполним список шагов для сохранения в файл
            var listCoordinates = new List<string>() { matrix.ConvertToCoordinate(start, 0).ToString() };
            foreach (var line in route)
                listCoordinates.Add(matrix.ConvertToCoordinate(line.Points.Last(), 0).ToString());

            // Отрисуем маршрут на большом холсте
            double[,] renderMatrix = await matrix.RenderRoute(route);

            // Перенесем матрицу на изображение
            int padding = (int)(Math.Max(size.Width, size.Height) * 0.05f);
            await painter.DrawImage(renderMatrix, padding);

            // Нанесем координатную сетку
            foreach (PixelPoint point in matrix.NodesAndPaths.Keys)
            {
                var sectorPoint = matrix.ConvertToCoordinate(point, padding);
                painter.DrawCoordinate(sectorPoint);
            }
                    
            // Сохраним изображение
            await painter.SaveImage(config.ResultImagePath);

            // Сохраним список координат маршрута
            await saveManager.SaveRouteAsync(listCoordinates, config.ResultRoutePath);
        }
    }
}
