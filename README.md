# Thread Art

---

## Project Description

Thread Art is a application for visualizing images as paths constructed from straight lines between points on the image boundaries.

### Key Features:
* **Path Generation**: Create a sequence of points connected by straight lines
* **Result Visualization**: Render the processed image with color vertex markers
* **Asynchronous Processing**: Process large images without blocking the main thread
* **Priority System**: Task Queue with the ability to process multiple files in parallel

---

## Image Processing Examples

| Original Image | Path Result |
|---------------------|-------------------|
| ![Original](/assets/screens/original.jpg) | ![Result](/assets/screens/result.png) |

### Processing Specifications:
```
Points per Boundary: 300
Algorithm Steps: 4000
Padding: 10px
Contrast: Automatically Calculated
```

---

## Project Implementation Details

### Technology Stack

| Category | Technologies |
|-----------|------------|
| **Language** | C# |
| **Database** | SQLite |
| **ORM** | Entity Framework Core |
| **Image Processing** | SixLabors.ImageSharp |

