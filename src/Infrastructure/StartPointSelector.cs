using Application.Services;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class StartPointSelector : IStartPointSelector
    {
        public PixelPoint SelectBeginPoint(List<PixelPoint> keys)
        {
            return keys[new Random().Next(keys.Count)];
        }
    }
}
