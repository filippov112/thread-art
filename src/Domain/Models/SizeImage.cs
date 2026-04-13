namespace Domain.Models
{
    public readonly struct SizeImage(int width, int height)
    {
        public int Width { get; } = width;
        public int Height { get; } = height;
    }
}
