using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Picture
    {
        public string OriginalImagePath { get; set; } = string.Empty;
        public string ResultImagePath { get; set; } = string.Empty;
        public string RouteFilePath { get; set; } = string.Empty;
        public List<string> Route { get; set; } = [];

        public int SmallMatrixWidth { get; set; } = 270;
        public int SmallMatrixHeight { get; set; } = 270;
        public int LargeMatrixWidth { get; set; } = 540;
        public int LargeMatrixHeight { get; set; } = 540;
        public int CountPoints { get; set; } = 240;
        public int CountLines { get; set; } = 4000;
        public bool IsEllipseMatrix { get; set; } = true;
        public int DistanceBetweenNeighboringPoints { get; set; } = 8;
    }
}
