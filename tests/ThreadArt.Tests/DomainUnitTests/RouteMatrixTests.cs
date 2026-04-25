using Domain.Models;

namespace ThreadArt.Tests.DomainUnitTests
{
    public class RouteMatrixTests
    {
        [Theory]
        [InlineData(0, 0, 0)] // Нулевой размер и 0 точек
        [InlineData(0, 0, 5)] // Нулевой размер
        [InlineData(2, 2, 4)] // Стороны слишком короткие (в углах точки не ставятся)
        [InlineData(10, 10, 1)] // 1 точка
        [InlineData(10, 10, 2)] // Расстояние между соседними точками больше стороны
        public void RouteMatrix_CountPoint_Should_BeEmpty(int width, int height, int n)
        {
            // A
            var matrix = new RouteMatrix(width, height, n);

            // A
            Assert.Empty(matrix.Points);
        }

        [Theory]
        [InlineData(3, 3, 4)] // Перекрестие в форме "+"
        [InlineData(5, 5, 17)] // Пикселей меньше чем точек
        public void RouteMatrix_CountPoint_Should_BeNotEmpty(int width, int height, int n)
        {
            // A
            var matrix = new RouteMatrix(width, height, n);

            // A
            Assert.NotEmpty(matrix.Points);
        }

        [Theory]
        [InlineData(3, 3, 4)]
        [InlineData(3, 3, 3)]
        [InlineData(5, 5, 12)]
        public void RouteMatrix_CountPoint_Should_BeEqual(int width, int height, int n)
        {
            // A
            var matrix = new RouteMatrix(width, height, n);

            // A
            Assert.Equal(n, matrix.Points.Length);
        }

        [Theory]
        [InlineData(10, 20, 4, 9)]
        [InlineData(15, 40, 6, 18)]
        [InlineData(45, 40, 19, 16)]
        public void RouteMatrix_The_aspect_ratio_must_be_maintained(int width, int height, int widthCount, int heightCount)
        {
            // A
            var matrix = new RouteMatrix(width, height, (heightCount + widthCount) * 2);

            // A
            Assert.Equal(heightCount, matrix.Points.Where(p => p.Sector == 'L').Count());
            Assert.Equal(heightCount, matrix.Points.Where(p => p.Sector == 'R').Count());
            Assert.Equal(widthCount, matrix.Points.Where(p => p.Sector == 'T').Count());
            Assert.Equal(widthCount, matrix.Points.Where(p => p.Sector == 'B').Count());
        }
    }
}
