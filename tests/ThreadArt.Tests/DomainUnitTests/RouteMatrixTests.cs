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
        [InlineData(30, 30, 4)] // Перекрестие в форме "+"
        [InlineData(50, 50, 17)] // Пикселей меньше чем точек
        public void RouteMatrix_CountPoint_Should_BeNotEmpty(int width, int height, int n)
        {
            // A
            var matrix = new RouteMatrix(width, height, n);

            // A
            Assert.NotEmpty(matrix.Points);
        }

        [Theory]
        [InlineData(30, 30, 4)]
        [InlineData(30, 30, 3)]
        [InlineData(50, 50, 12)]
        public void RouteMatrix_CountPoint_Should_BeEqual(int width, int height, int n)
        {
            // A
            var matrix = new RouteMatrix(width, height, n);

            // A
            Assert.Equal(n, matrix.Points.Length);
        }

        [Theory]
        [InlineData(100, 200, 8)]
        [InlineData(150, 400, 16)]
        [InlineData(450, 380, 24)]
        [InlineData(1000, 380, 24)]
        [InlineData(50, 380, 24)]
        public void RouteMatrix_The_aspect_ratio_must_be_maintained(int width, int height, int count)
        {
            // A
            var matrix = new RouteMatrix(width, height, count);

            // A
            Assert.True(Math.Abs((double)count / 2 * height / (width + height) - matrix.Points.Where(p => p.Sector == 'L').Count()) < 1.5);
            Assert.True(Math.Abs((double)count / 2 * height / (width + height) - matrix.Points.Where(p => p.Sector == 'R').Count()) < 1.5);
            Assert.True(Math.Abs((double)count / 2 * width / (width + height) - matrix.Points.Where(p => p.Sector == 'T').Count()) < 1.5);
            Assert.True(Math.Abs((double)count / 2 * width / (width + height) - matrix.Points.Where(p => p.Sector == 'B').Count()) < 1.5);
        }
    }
}
