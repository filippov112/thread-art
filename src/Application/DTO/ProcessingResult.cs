using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO;

public class ProcessingResult
{
    public string OriginalImagePath { get; set; } = string.Empty;
    public string ResultImagePath { get; set; } = string.Empty;
    public string RouteFilePath { get; set; } = string.Empty;
}
