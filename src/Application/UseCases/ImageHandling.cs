

using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases
{
    public class ImageHandling(ISaveManager saveManager, IStartPointSelector startPointSelecter, IPainter painter)
    {
        public async Task ProcessImage(IFormFile file, Config config)
        {
            config.OriginalImagePath = await saveManager.SaveImageAsync(file, config);

            // Получим данные об изображении
            double[,] gray_negative_matrix = await painter.GetImageGrayNegativeMatrix(config.OriginalImagePath, config.SmallSize);
            
            // Построим матрицу
            var smallMatrix = new Matrix(config.SmallSize.Width, config.SmallSize.Height, config.CountPoints, config.IsEllipse);
            
            // Выберем стартовую точку
            var start = startPointSelecter.SelectBeginPoint(smallMatrix.NodesAndPaths.Keys.ToList());
            
            // Найдем маршрут
            var route = await smallMatrix.BuildRoute(start, gray_negative_matrix, config.ContrastLine, config.CountSteps);

            // Заполним список шагов для сохранения в файл
            var listCoordinates = new List<string>() { smallMatrix.ConvertToCoordinate(start).ToString() };
            foreach (var line in route)
                listCoordinates.Add(smallMatrix.ConvertToCoordinate(line.Points.Last()).ToString());

            // Отрисуем маршрут на большом холсте
            var canvas = new Canvas(smallMatrix, config.LargeSize.Width, config.LargeSize.Height);
            double[,] renderMatrix = await canvas.ProjectRouteOntoRenderingMatrix(route);

            // Перенесем матрицу на изображение
            int padding = (int)(Math.Max(config.LargeSize.Width, config.LargeSize.Height) * 0.05f);
            await painter.DrawImage(renderMatrix, padding);

            // Нанесем координатную сетку
            foreach (PixelPoint point in canvas.LargeMatrix.NodesAndPaths.Keys)
            {
                var imagePoint = new PixelPoint(point.X + padding, point.Y + padding);
                var sectorPoint = canvas.LargeMatrix.ConvertToCoordinate(point);
                painter.DrawCoordinate(imagePoint, sectorPoint);
            }
                    
            // Сохраним изображение
            await painter.SaveImage(config.ResultImagePath);

            // Сохраним список координат маршрута
            await saveManager.SaveRouteAsync(listCoordinates, config.ResultRoutePath);
        }
    }
}
