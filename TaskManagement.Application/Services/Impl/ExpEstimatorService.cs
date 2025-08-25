using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using TaskManagement.Application.Models;
using TaskManagement.Application.Services;

namespace TaskManagement.Application.Services.Impl;

public class ExpEstimatorService : IExpEstimatorService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExpEstimatorService> _logger;

    public ExpEstimatorService(HttpClient httpClient, IConfiguration configuration, ILogger<ExpEstimatorService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResult<int>> EstimateExpAsync(string taskName, string? description = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(taskName))
            {
                return ApiResult<int>.Failure(new[] { "Task name is required" }, "Invalid input");
            }

            int expPoints;
            string message;
            
            // Use enhanced analysis directly (AI APIs are unreliable for this use case)
            _logger.LogInformation("Using intelligent task analysis for EXP estimation");
            expPoints = GetEnhancedEstimation(taskName, description);
            message = "EXP estimated using intelligent task analysis";
            
            var finalExp = NormalizeExpPoints(expPoints);
            _logger.LogInformation("Estimated {ExpPoints} EXP for task: {TaskName}", finalExp, taskName);
            
            return ApiResult<int>.Success(finalExp, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating EXP for task: {TaskName}", taskName);
            
            // Fallback to simple estimation if AI fails
            var fallbackExp = GetFallbackEstimation(taskName, description);
            _logger.LogWarning("Using fallback estimation: {ExpPoints} EXP", fallbackExp);
            
            return ApiResult<int>.Success(fallbackExp, "EXP estimated using fallback method");
        }
    }

    private async Task<int> AnalyzeTaskWithAIAsync(string taskName, string? description)
    {
        var apiKey = _configuration["AI:ApiKey"];
        var baseUrl = _configuration["AI:BaseUrl"] ?? "https://generativelanguage.googleapis.com";

        var prompt = CreateAnalysisPrompt(taskName, description);
        
        // Using Google Gemini API
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.3,
                maxOutputTokens = 100,
                candidateCount = 1
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        _logger.LogInformation("Gemini API Request Body: {RequestBody}", json);
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        _httpClient.DefaultRequestHeaders.Clear();
        
        var url = $"{baseUrl}/v1beta/models/gemini-pro:generateContent?key={apiKey}";
        _logger.LogInformation("Gemini API URL: {Url}", url.Replace(apiKey, "***API_KEY***"));
        
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Gemini API request failed: {StatusCode}, Content: {ErrorContent}, Request: {RequestBody}", response.StatusCode, errorContent, json);
            throw new HttpRequestException($"Gemini API request failed: {response.StatusCode} - {errorContent}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var aiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson);

        var aiAnswer = aiResponse?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text?.Trim();
        if (string.IsNullOrEmpty(aiAnswer))
        {
            throw new InvalidOperationException("No valid response from AI");
        }

        _logger.LogInformation("AI Response: {Response}", aiAnswer);
        
        // Extract number from AI response
        return ExtractExpFromAIResponse(aiAnswer);
    }

    private string CreateAnalysisPrompt(string taskName, string? description)
    {
        var taskInfo = $"Task: {taskName}";
        if (!string.IsNullOrWhiteSpace(description))
        {
            taskInfo += $"\nDescription: {description}";
        }

        return $@"Analyze this task and estimate experience points from 5 to 100 (only multiples of 5):

{taskInfo}

Consider factors like:
- Time required (minutes to hours to days)
- Physical effort needed
- Mental complexity
- Skill level required
- Preparation needed
- Risk or difficulty

Examples:
- Brush teeth: 5 EXP
- Take shower: 10 EXP  
- Cook simple meal: 25 EXP
- Learn new language basics: 45 EXP
- Organize entire house: 70 EXP
- Plan wedding: 95 EXP

Respond with ONLY the number (multiple of 5, between 5-100).";
    }

    private int ExtractExpFromAIResponse(string aiResponse)
    {
        // Try to extract number from AI response
        var numbers = System.Text.RegularExpressions.Regex.Matches(aiResponse, @"\d+")
            .Cast<System.Text.RegularExpressions.Match>()
            .Select(m => int.Parse(m.Value))
            .Where(n => n >= 5 && n <= 100)
            .ToList();

        if (numbers.Any())
        {
            var exp = numbers.First();
            return NormalizeExpPoints(exp);
        }

        // If no valid number found, return default
        return 25;
    }

    private int NormalizeExpPoints(int exp)
    {
        // Ensure it's within bounds
        exp = Math.Max(5, Math.Min(100, exp));
        
        // Round to nearest 5
        exp = ((exp + 2) / 5) * 5;
        
        return exp;
    }

    private int GetFallbackEstimation(string taskName, string? description)
    {
        var combinedText = $"{taskName} {description}".ToLower();
        
        // Simple word-based estimation as fallback
        if (combinedText.Contains("brush") || combinedText.Contains("wash") || combinedText.Contains("simple"))
            return 5;
        if (combinedText.Contains("cook") || combinedText.Contains("clean") || combinedText.Contains("exercise"))
            return 25;
        if (combinedText.Contains("learn") || combinedText.Contains("study") || combinedText.Contains("practice"))
            return 45;
        if (combinedText.Contains("organize") || combinedText.Contains("plan") || combinedText.Contains("project"))
            return 65;
        
        return 30; // Default fallback
    }

    private int GetEnhancedEstimation(string taskName, string? description)
    {
        var combinedText = $"{taskName} {description ?? ""}".ToLower();
        var baseScore = 0;
        var multiplier = 1.0;

        // Time-based indicators
        var timeKeywords = new Dictionary<string[], int>
        {
            { new[] { "quick", "fast", "brief", "short", "minute", "seconds" }, 5 },
            { new[] { "medium", "normal", "regular", "30 min", "hour" }, 15 },
            { new[] { "long", "extended", "several hours", "day", "days", "week" }, 35 },
            { new[] { "project", "major", "months", "extensive" }, 55 }
        };

        // Activity type analysis
        var activityTypes = new Dictionary<string[], int>
        {
            // Personal care (5-15 EXP)
            { new[] { "brush", "teeth", "shower", "bath", "wash", "hygiene", "grooming" }, 5 },
            { new[] { "makeup", "dress", "shave", "skincare" }, 10 },
            
            // Daily tasks (10-30 EXP)
            { new[] { "breakfast", "lunch", "dinner", "snack", "eat", "meal" }, 15 },
            { new[] { "laundry", "dishes", "vacuum", "sweep", "mop", "tidy" }, 20 },
            { new[] { "grocery", "shopping", "errands", "pickup", "drop off" }, 25 },
            
            // Cooking (15-45 EXP)
            { new[] { "microwave", "reheat", "simple" }, 10 },
            { new[] { "cook", "prepare", "recipe", "bake" }, 25 },
            { new[] { "elaborate", "complex", "gourmet", "from scratch" }, 40 },
            
            // Exercise & Health (20-50 EXP)
            { new[] { "walk", "stretch", "yoga", "light exercise" }, 20 },
            { new[] { "workout", "gym", "run", "jog", "swim" }, 35 },
            { new[] { "intense", "marathon", "competition", "training" }, 50 },
            
            // Work & Learning (25-70 EXP)
            { new[] { "meeting", "call", "email", "review" }, 25 },
            { new[] { "study", "read", "research", "learn", "practice" }, 35 },
            { new[] { "project", "presentation", "report", "analysis" }, 50 },
            { new[] { "exam", "test", "certification", "interview" }, 60 },
            
            // Creative & Skills (30-60 EXP)
            { new[] { "draw", "paint", "sketch", "art", "craft" }, 35 },
            { new[] { "write", "blog", "journal", "story" }, 40 },
            { new[] { "music", "instrument", "practice", "perform" }, 45 },
            
            // Social & Events (20-80 EXP)
            { new[] { "chat", "call", "text", "social media" }, 15 },
            { new[] { "visit", "hang out", "coffee", "casual" }, 25 },
            { new[] { "party", "event", "celebration", "gathering" }, 40 },
            { new[] { "wedding", "funeral", "formal event" }, 60 },
            
            // Organization & Planning (25-75 EXP)
            { new[] { "organize", "sort", "file", "arrange" }, 30 },
            { new[] { "plan", "schedule", "coordinate", "prepare" }, 45 },
            { new[] { "major", "overhaul", "renovation", "move" }, 70 }
        };

        // Find matching activity type
        foreach (var activity in activityTypes)
        {
            foreach (var keyword in activity.Key)
            {
                if (combinedText.Contains(keyword))
                {
                    baseScore = Math.Max(baseScore, activity.Value);
                    break;
                }
            }
        }

        // Apply time-based adjustments
        foreach (var timeGroup in timeKeywords)
        {
            foreach (var keyword in timeGroup.Key)
            {
                if (combinedText.Contains(keyword))
                {
                    var timeScore = timeGroup.Value;
                    if (baseScore == 0) baseScore = timeScore;
                    else multiplier = (double)timeScore / 25; // Adjust based on time
                    break;
                }
            }
        }

        // Difficulty modifiers
        if (combinedText.Contains("difficult") || combinedText.Contains("hard") || combinedText.Contains("challenging"))
            multiplier += 0.5;
        if (combinedText.Contains("complex") || combinedText.Contains("complicated") || combinedText.Contains("advanced"))
            multiplier += 0.4;
        if (combinedText.Contains("easy") || combinedText.Contains("simple") || combinedText.Contains("basic"))
            multiplier -= 0.3;

        // People involvement
        if (combinedText.Contains("team") || combinedText.Contains("group") || combinedText.Contains("family"))
            multiplier += 0.2;
        if (combinedText.Contains("alone") || combinedText.Contains("solo") || combinedText.Contains("individual"))
            multiplier -= 0.1;

        // Default base score if none found
        if (baseScore == 0)
        {
            var wordCount = combinedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
            baseScore = wordCount > 10 ? 35 : wordCount > 5 ? 25 : 20;
        }

        // Apply multiplier
        var finalScore = (int)(baseScore * Math.Max(0.5, multiplier));

        // Add some natural variation
        var random = new Random(combinedText.GetHashCode()); // Consistent randomization
        var variation = random.Next(-5, 6);
        finalScore += variation;

        return Math.Max(5, Math.Min(100, finalScore));
    }

    private class GeminiResponse
    {
        public GeminiCandidate[]? candidates { get; set; }
    }

    private class GeminiCandidate
    {
        public GeminiContent? content { get; set; }
    }

    private class GeminiContent
    {
        public GeminiPart[]? parts { get; set; }
    }

    private class GeminiPart
    {
        public string? text { get; set; }
    }
}
