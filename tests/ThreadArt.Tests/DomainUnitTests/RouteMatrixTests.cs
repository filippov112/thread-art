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
        [InlineData(3, 3, 3)] //  Расстояние между соседними точками больше стороны
        [InlineData(10, 10, 2)] // Расстояние между соседними точками больше стороны
        public void RouteMatrix_Should_BeEmpty(int width, int height, int n)
        {
            // A
            var matrix = new RouteMatrix(width, height, n);

            // A
            Assert.Empty(matrix.Paths);
        }

        [Theory]
        [InlineData(3, 3, 4)] // Перекрестие в форме "+"
        [InlineData(5, 5, 17)] // Пикселей меньше чем точек
        public void RouteMatrix_Should_BeNotEmpty(int width, int height, int n)
        {
            // A
            var matrix = new RouteMatrix(width, height, n);

            // A
            Assert.NotEmpty(matrix.Paths);
        }
    }
}
