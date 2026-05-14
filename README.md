# Thread Art

---

## 1. Project Description

Thread Art is a web application for visualizing images as paths constructed from straight lines between points on the image boundaries.

### Key Features:
* **Path Generation**: Create a sequence of points connected by straight lines
* **Result Visualization**: Render the processed image with color vertex markers
* **Data Export**: Save the path in text format for later use
* **Asynchronous Processing**: Process large images without blocking the main thread
* **Priority System**: Task Queue with the ability to process multiple files in parallel

---

## 2. Image Processing Examples

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

## 3. Startup Instructions

### Requirements
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Docker and Docker Compose](https://www.docker.com/)
- PostgreSQL Database (enabled via docker-compose)

### Local Development

1. Cloning the Repository:
```bash
git clone https://github.com/filippov112/thread-art.git
cd thread-art
```

2. Setting Environment Variables:
```bash
cp .env.example .env
```

3. Running via Docker Compose:
```bash
docker-compose up
```

4. Initial Database Setup:
```bash
# Migrations will be applied automatically on first run
```

5. Application Access:
- HTTP: `http://localhost:8080`
- HTTPS: `https://localhost:7005`
- Swagger UI API: `https://localhost:7005/swagger`
- Scalar API: `https://localhost:7005/scalar`

### Without Docker (local build)

```bash
dotnet restore
dotnet ef database update
dotnet run --project src/Web
```

### Storage Configuration

Edit the file `src/Web/appsettings.json`:

```json
{
"Storage": {
"FolderPath": "storage",
"StaticFiles": "wwwroot",
"CleanupIntervalHours": 0.1,
"FileAgeHours": 1.1
},
"Processing": {
"MaxConcurrency": 0
}
}
```

---

## 4. Project Implementation Details

### Technology Stack

| Category | Technologies | Version |
|-----------|------------|---------|
| **Language** | C# | 12+ |
| **Framework** | ASP.NET Core | 10.0 |
| **Database** | PostgreSQL | 16 |
| **ORM** | Entity Framework Core | 10.0.6 |
| **Image Processing** | SixLabors.ImageSharp | 3.1.12 |
| **Containerization** | Docker / Docker Compose | 3.8 |
| **Queues** | Channels (MemoryJobQueue) | System.Threading.Channels |

### Project Architectural Map

```
┌─────────────────────────────────────────────────────────────────────┐
│                             WEB LAYER                               │
│  ┌────────────────┐  ┌────────────────┐  ┌─────────────────┐        │
│  │ AuthController │  │ ImageController│  │ Swagger/OpenAPI │        │
│  └────────────────┘  └────────────────┘  └─────────────────┘        │
│           │                   │                   │                 │
│            Middleware (JWT Auth, CORS, Static Files)                │
└─────────────────────────────────────────────────────────────────────┘
                                │
┌─────────────────────────────────────────────────────────────────────┐
│                          APPLICATION LAYER                          │
│  ┌────────────────┐  ┌──────────────┐  ┌──────────────┐             │
│  │ImageProcessor  │  │DTOs          │  │Interfaces    │             │
│  └────────────────┘  └──────────────┘  └──────────────┘             │
│           │               │                  │                      │
│             Services & Use Cases (Business Logic)                   │
└─────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────┐
│                            DOMAIN LAYER                             │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐            │
│  │ImageMatrix    │  │ ProcessingJob │  │ SectorPoint   │            │
│  │ Route         │  │ PixelPoint    │  │ Enum JobStatus│            │
│  └───────────────┘  └───────────────┘  └───────────────┘            │
│                                                                     │
│                        Domain Models                                │
└─────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────┐
│                          INFRASTRUCTURE LAYER                       │
│  ┌──────────────┐  ┌───────────────┐  ┌──────────────┐              │
│  │ Repositories │  │ FileSystem    │  │ Background   │              │
│  │ (EF Core)    │  │ Service       │  │ Workers      │              │
│  └──────────────┘  └───────────────┘  └──────────────┘              │
│                                                                     │
│              Data Access & External Services                        │
└─────────────────────────────────────────────────────────────────────┘
                                    │
                    ┌───────────────┴───────────────┐
                    ▼                               ▼
              ┌───────────────┐          ┌───────────────┐
              │ PostgreSQL DB │          │ File Storage  │
              └───────────────┘          └───────────────┘
```
### Algorithm Description

#### Image Processing Stages:

```
1. Initialization (Process 0%)
├── Reading the image into the brightness matrix
└── Input data validation check

2. Vertex Search (Process 5-10%)
├── Calculating rays from the image center at angles
├── Intersection of rays with image boundaries
└── Generating a SectorPoint array

3. Route construction (Process 10-80%)
├── Selecting the next point with the minimum average brightness value
├── Drawing a line on the matrix with contrast adjustment
├── Updating task progress
└── Cycle through until the specified number of steps is reached

4. Rendering the result (Process 80-90%)
├── Normalizing pixel values
├── Adding a sector color palette
└── Saving the final image

5. Data export (Process 90-100%)
├── Saving the route to a text file
├── Writing metadata to the database
└── Returning the results URL
```

#### FindNextPoint algorithm:

```csharp
// A line is constructed for each possible endpoint
foreach (var end in possibleEndPoints)
{
// The average brightness of all pixels on the line is calculated
double avgBrightness = CalculateLineAverage(start.Pixel, end.Pixel);

// The point with the minimum average brightness is selected
if (avgBrightness < minValue)
bestEndPoint = end;
}

// The line is added to the route with increased contrast
ApplyContrastAdjustment(start.Pixel, bestEndPoint.Pixel);
```

### Interesting technical solutions

#### 1. Automatic calculation of optimal contrast

```csharp
public static int CalculateOptimalContrast(ImageMatrix image, int stepCount)
{
// Formula: x = 3.2 * (2 * S * sigma) / (N * D)
// Where S = area, σ = standard deviation, N = steps, D = diagonal
double rawX = (6.4 * area * sigma) / (stepCount * diagonal);
return (int)Math.Round(rawX);
}
```

Solves the problem of adapting the algorithm to different image sizes and formats without manually selecting parameters.

#### 2. Task queue with limited concurrency

```csharp
private readonly SemaphoreSlim _semaphore = new(maxConcurrency, maxConcurrency);

await _semaphore.WaitAsync(stoppingToken);
_ = Task.Run(() => ProcessJobAsync(jobId, ct), ct);
_semaphore.Release();
```

Prevents server resource exhaustion when processing multiple files simultaneously.

#### 3. Exponential backoff strategy when reading files

```csharp
while (retries > 0)
{
try { return new FileStream(...); }
catch when (retries > 0)
{
await Task.Delay(100 * (int)Math.Pow(2, 5 - retries));
retries--;
}
}
```

Provides resilience to temporary file system access issues.

#### 4. Old File Cleanup System

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
while (!stoppingToken.IsCancellationRequested)
{
CleanupOldFiles(path, options.Value.FileAgeHours);
await Task.Delay(TimeSpan.FromHours(options.Value.CleanupIntervalHours));
}
}
```

Automatically removes files older than the specified age to save disk space.

#### 5. Parallelizing the Route Building Algorithm

```csharp
for (int i = progressStep; i <= request.CountSteps; i += progressStep)
{
start = RouteBuilder.FillRoute(start, ...);
await jobRepo.UpdateProgressAsync(request.JobID, progressPercentage);
// Ability to interrupt or pause processing
}
```

Allows you to pass the current processing status while a long-running operation is running.

---

### API Endpoints

| Method | Endpoint | Description | Auth |
|--------|-------------|-----------|
| POST | `/api/image/upload` | Upload an image | Yes |
| GET | `/api/image/job/{jobId}` | Status of a specific task | Yes |
| GET | `/api/image/jobs` | List of all tasks | Yes |
| GET | `/api/image/all` | List of all processed images | Yes |