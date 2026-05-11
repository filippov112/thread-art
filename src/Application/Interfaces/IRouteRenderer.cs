using Domain.Models;

namespace Application.Interfaces;

public interface IRouteRenderer
{
    public ImageMatrix RenderRoute(Route route, int padding, int width, int height);
}
