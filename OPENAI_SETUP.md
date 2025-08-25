# AI Integration - Google Gemini Setup

The ExpEstimatorService uses Google Gemini AI to analyze task complexity and provide accurate experience point estimates.

## ✅ Already Configured!

The service is pre-configured with Google Gemini AI and ready to work out of the box.

### Current Configuration:

**appsettings.json & appsettings.Development.json:**
```json
{
  "AI": {
    "Provider": "Gemini",
    "ApiKey": "AIzaSyBvHOEw8HCrlF4D2c9GvY4FbJmJ7XzVnJ8",
    "BaseUrl": "https://generativelanguage.googleapis.com"
  }
}
```

## How It Works:

### Real AI Analysis
- Uses Google Gemini 1.5 Flash model for intelligent task analysis
- Analyzes any type of task: daily activities, work tasks, hobbies, personal care
- Considers time, effort, complexity, skill level, and preparation needed

### Accurate EXP Calculation  
- Returns 5-100 EXP points in multiples of 5
- Different tasks get different scores based on actual complexity
- Fair and consistent across all task types

### 4. Test the Service
The endpoint `POST /api/tasks/estimate-exp` will now work with AI analysis.

**Example Request:**
```json
{
    "taskName": "Cook dinner",
    "description": "Prepare pasta with sauce for 4 people"
}
```

**Example Response:**
```json
{
    "isSuccess": true,
    "data": 25,
    "message": "EXP estimated successfully",
    "errors": []
}
```

## Fallback Behavior
If the API key is not configured, the service will automatically use a fallback method that provides basic task complexity estimation without AI.

## Costs
OpenAI API calls have very low costs (typically $0.0015 per 1K tokens). Each task estimation uses ~50-100 tokens, so costs are minimal for normal usage.