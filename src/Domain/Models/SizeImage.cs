using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public struct SizeImage(int width, int height)
    {
        public int Width = width;
        public int Height = height;
    }
}
