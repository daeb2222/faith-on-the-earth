# F.ai.th Backend

Backend API for the **Faith on the Earth** project - A Supercell AI: Global AI Game Hackathon submission.

## What is this project?

**F.ai.th** is an AI-powered game where you play as god taking decisions to gain followers.

The backend provides:
- **Narrative Generation**: AI-generated story content and character dialogue using Mistral AI models
- **Asset Management**: Game asset tracking and generation
- **Game Logic**: Core gameplay mechanics powered by large language models
- **Real-time Communication**: API endpoints for the Unity frontend to communicate with AI services

## Overview

This is a FastAPI-based REST API backend that integrates with Mistral AI models to provide intelligent game functionality for the Faith on the Earth game. The backend handles requests from the Unity frontend and leverages advanced LLM capabilities for narrative generation, character interactions, and dynamic game content.

## Technology Stack

- **Framework**: FastAPI
- **Language**: Python 3.12+
- **AI Models**: Mistral AI (Large, Medium, Small variants)
- **Server**: Uvicorn

## Setup

### Prerequisites

- **Python 3.12 or higher** - [Download Python](https://www.python.org/downloads/)
- **Virtual environment** (recommended for isolated dependency management)
- **Mistral AI API Key** - Get one from [Mistral AI Console](https://console.mistral.ai/)

### Step-by-Step Installation

#### 1. Navigate to the Backend directory:

```bash
cd Backend
```

#### 2. Create and activate a virtual environment:

**On macOS/Linux:**
```bash
python3 -m venv venv
source venv/bin/activate
```

**On Windows (PowerShell):**
```powershell
python -m venv venv
.\venv\Scripts\Activate.ps1
```

**On Windows (Command Prompt):**
```cmd
python -m venv venv
venv\Scripts\activate
```

You should see `(venv)` appear in your terminal prompt, indicating the virtual environment is active.

#### 3. Install dependencies using pyproject.toml:

```bash
pip install -e .
```

Alternatively, install individual packages:
```bash
pip install fastapi uvicorn mistralai python-dotenv aiohttp black
```

#### 4. Set up environment variables:

Create a `.env` file in the Backend directory with the following variables:

```
MISTRAL_API_KEY=your_api_key_here
```

You can get your Mistral API key from the [Mistral AI Console](https://console.mistral.ai/).

**⚠️ IMPORTANT:** Never commit `.env` files to version control. The `.env` file is already in `.gitignore`.

#### 5. Verify the installation:

```bash
python -c "from mistralai import Mistral; print('✓ Mistral AI installed successfully')"
python -c "import fastapi; print('✓ FastAPI installed successfully')"
```

## Running the Backend

### Development Mode (with auto-reload)

```bash
python main.py
```

Or use Uvicorn directly:

```bash
uvicorn main:app --reload --host 127.0.0.1 --port 8000
```

The `--reload` flag automatically restarts the server when you modify code.

### Production Mode (without auto-reload)

```bash
uvicorn main:app --host 0.0.0.0 --port 8000
```

### Accessing the API

Once the server is running, you can access:

- **Main API**: http://localhost:8000
- **Interactive API Documentation (Swagger UI)**: http://localhost:8000/docs
  - Here you can test all endpoints directly in your browser
- **Alternative Documentation (ReDoc)**: http://localhost:8000/redoc
- **Health Check**: http://localhost:8000 
  - Returns: `{"message": "Hello, world"}`

## API Endpoints

### Root Endpoint

- **GET** `/`
  - Returns a simple health check message
  - Response: `{"message": "Hello, world"}`

### Narrative Router

Located in `routers/narrative.py` - Handles narrative and dialogue generation for game characters.

### Assets Router

Located in `routers/assets.py` - Manages game assets and related functionality.

For detailed endpoint documentation, start the server and visit `/docs` in your browser.

## Project Structure

```
Backend/
├── main.py                      # FastAPI application entry point
├── config.py                    # Configuration settings
├── pyproject.toml              # Project metadata and dependencies
├── .env                         # Environment variables (not tracked in git)
├── README.md                    # This file
├── libs/
│   ├── models/
│   │   └── mistral_models.json  # Mistral AI model configurations
│   ├── prompts/                 # AI prompt templates
│   └── utils/                   # Utility functions
├── routers/
│   ├── narrative.py             # Narrative generation endpoints
│   └── assets.py                # Asset management endpoints
├── services/                    # Business logic and service layer
└── models/                      # Pydantic models for request/response validation
```

## How Mistral AI Integration Works

The backend uses **Mistral AI** as its large language model provider. Here's how it works:

1. **API Key**: Your Mistral API key (stored in `.env`) authenticates requests
2. **Model Selection**: Different models are used based on the use case (see Available Models table below)
3. **Prompts**: System prompts are defined in `libs/prompts/` to guide AI responses
4. **Requests**: Game logic sends requests to Mistral API endpoints
5. **Responses**: AI-generated content is processed and sent to the Unity frontend

## Available Models

The backend is configured to use the following Mistral AI models:

| Model | Max Tokens | Temperature | Use Case |
|-------|-----------|-------------|----------|
| mistral-large-3 | 4096 | 0.7 | High-quality responses, complex narratives |
| mistral-medium-3.1 | 4096 | 0.7 | Balanced performance and quality |
| ministral-8b | 2048 | 0.6 | Lightweight, faster responses |
| ministral-14b | 2048 | 0.6 | Small but capable model |
| mistral-small-3.2 | 1024 | 0.5 | Minimal tokens, consistent responses |

Current default: `mistral-large-3`

## Development Workflow

### 1. Making Changes

- Edit code in your preferred IDE
- Changes are automatically reloaded when in development mode
- Test your changes through the `/docs` endpoint

### 2. Testing Endpoints

Use the Swagger UI at http://localhost:8000/docs to:
- View all available endpoints
- Test endpoints with different parameters
- See request/response formats
- Check HTTP status codes

### 3. Adding New Endpoints

1. Create a new router file in `routers/` or modify existing ones
2. Use FastAPI decorators like `@router.get()`, `@router.post()`, etc.
3. Include the router in `main.py` with `app.include_router()`
4. Test via Swagger UI

Example:
```python
from fastapi import APIRouter

router = APIRouter(prefix="/example", tags=["Example"])

@router.get("/")
async def example_endpoint():
    return {"message": "Hello from example"}
```

## Configuration

- **Model configurations**: Defined in `libs/models/mistral_models.json`
- **Environment variables**: Defined in `.env` file
- **API settings**: Configured in `main.py` and `config.py`

## Troubleshooting

### Issue: "ModuleNotFoundError: No module named 'fastapi'"

**Solution**: Make sure your virtual environment is activated and dependencies are installed:
```bash
# Activate venv
source venv/bin/activate  # macOS/Linux
# or
.\venv\Scripts\Activate.ps1  # Windows PowerShell

# Install dependencies
pip install -e .
```

### Issue: "MISTRAL_API_KEY not found"

**Solution**: Ensure you have created a `.env` file in the Backend directory with your Mistral API key:
```
MISTRAL_API_KEY=your_actual_api_key_here
```

Get your API key from [Mistral AI Console](https://console.mistral.ai/).

### Issue: "Connection error when testing Mistral API"

**Solution**: 
- Check your internet connection
- Verify your Mistral API key is valid
- Check that your account has available API credits
- Try a simpler request first

### Issue: Port 8000 is already in use

**Solution**: Use a different port:
```bash
uvicorn main:app --reload --port 8001
```

### Need Help?

- Check FastAPI docs: https://fastapi.tiangolo.com/
- Check Mistral AI docs: https://docs.mistral.ai/
- Review server logs for detailed error messages

## Next Steps

- Implement game-specific endpoints
- Add authentication and authorization
- Integrate request/response validation with Pydantic models
- Add logging and error handling
- Deploy to production environment

## License

See the LICENSE file in the root directory.

## Team

Developed as part of the Supercell AI: Global AI Game Hack.
