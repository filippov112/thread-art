using Domain.Models;

namespace Application.Interfaces;

public interface IRouteRenderer
{
    public PixelMatrix RenderRoute(Route route, int padding, int width, int height);
}
