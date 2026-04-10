using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public interface IStartPointSelector
    {
        public PixelPoint SelectBeginPoint(List<PixelPoint> keys);
    }
}
