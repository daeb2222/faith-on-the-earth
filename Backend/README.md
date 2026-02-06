# Faith on the Earth - Backend

Backend API for the **Faith on the Earth** project - A Supercell AI: Global AI Game Hackathon submission.

## Overview

This is a FastAPI-based REST API backend that integrates with Mistral AI models to provide intelligent game functionality for the Faith on the Earth game. The backend handles requests from the Unity frontend and leverages advanced LLM capabilities for game logic and features.

## Technology Stack

- **Framework**: FastAPI
- **Language**: Python 3.12+
- **AI Models**: Mistral AI (Large, Medium, Small variants)
- **Server**: Uvicorn

## Project Structure

```
Backend/
├── main.py                 # FastAPI application entry point
├── pyproject.toml         # Project configuration and dependencies
├── libs/
│   ├── models/
│   │   └── mistral_models.json    # Mistral AI model configurations
│   └── utils/             # Utility functions (placeholder)
└── README.md              # This file
```

## Available Models

The backend is configured to use the following Mistral AI models:

| Model | Max Tokens | Temperature | Use Case |
|-------|-----------|-------------|----------|
| mistral-large-3 | 4096 | 0.7 | Default, high-quality responses |
| mistral-medium-3.1 | 4096 | 0.7 | Balanced performance/quality |
| ministral-8b | 2048 | 0.6 | Lightweight, faster responses |
| ministral-14b | 2048 | 0.6 | Small but capable model |
| mistral-small-3.2 | 1024 | 0.5 | Minimal tokens, consistent responses |

## Setup

### Prerequisites

- Python 3.12 or higher
- Virtual environment (recommended)

### Installation

1. **Navigate to the Backend directory:**
   ```bash
   cd Backend
   ```

2. **Create and activate a virtual environment:**
   ```bash
   python -m venv venv
   source venv/bin/activate  # On Windows: venv\Scripts\activate
   ```

3. **Install dependencies:**
   ```bash
   pip install fastapi uvicorn mistralai
   ```

## Running the Backend

Start the development server with hot reload:

```bash
uvicorn main:app --reload
```

The API will be available at: `http://localhost:8000`

- **Interactive API docs**: http://localhost:8000/docs (Swagger UI)
- **Alternative API docs**: http://localhost:8000/redoc (ReDoc)

## API Endpoints

### Root Endpoint

- **GET** `/`
  - Returns a simple health check message
  - Response: `{"message": "Hello, world"}`

## Configuration

Model configurations are defined in `libs/models/mistral_models.json`. The default model is `mistral-large-3`, but you can switch models by updating the configuration.

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
