# Faith on the Earth - AI God Game

A dynamic narrative videogame where **you are a god** making decisions that shape the narrative, while a rival AI is sabotaging you. Every choice you make is immediately processed by LLMs systems that generate new story content and unseen consequences. Your rival AI continuously interprets and reacts to your actions, creating an ever-evolving narrative that's never the same twice.

## Overview

This is an AI-powered narrative game where:
- **Dynamic AI Narratives**: Every action triggers real-time story generation using advanced LLMs (Mistral AI)
- **Ever-Changing Story**: No two playthroughs are identical—the narrative adapts to your choices and the rival AI's interference

## Project Structure

```
faith-on-the-earth/
├── Backend/                 # FastAPI server with AI orchestration
│   ├── routers/            # API endpoints (narrative, assets)
│   ├── services/           # AI integration services (LLM, asset generation)
│   ├── models/             # Data schemas and structures
│   ├── libs/               # Shared utilities and prompts
│   ├── main.py             # FastAPI application
│   ├── config.py           # Configuration and environment variables
│   └── pyproject.toml      # Python dependencies
│
└── Faith on the earth - Supercell Hackathon AI/  # Unity game client
    └── [Unity project files]
```

## Key Features

### AI-Powered Narrative Generation
- Real-time story generation based on your decisions
- Narrator prompts and rival god prompts that compete for narrative influence
- Context-aware storytelling using a short-term memory of recent events (last 5 turns)

### Dynamic Asset Creation Is Ready
- 3D character and environment generation using Rodin AI
- On-demand generation of game world visuals based on narrative events

### Game Mechanics
- **Player Actions**: Make choices that drive the narrative forward
- **Absurdity Factor**: A chaos parameter that adds unpredictability to narratives
- **Rival AI Interference**: The rival god's decisions can counteract, enhance, or complicate the narrative

## Setup & Installation

### Prerequisites
- **Python 3.12+** installed on your system
- **uv** package manager
- **API Keys** for external AI services

### Step 1: Install uv

If you don't have `uv` installed, install it first (it's a modern Python package manager):

```bash
pip install uv
```

Or follow the official guide: https://docs.astral.sh/uv/

### Step 2: Configure Environment Variables

Create a `.env` file in the `Backend/` directory based on the `.env.template`:

```bash
cd Backend
cp .env.template .env
```

Edit `.env` and add your API keys:

```env
MISTRAL_API_KEY="your-mistral-api-key-here"
RODIN_API_KEY="your-rodin-api-key-here"
```

**Required Keys:**
- **MISTRAL_API_KEY**: Get from [Mistral AI Console](https://console.mistral.ai/)
- **RODIN_API_KEY**: Get from [Rodin AI (Hyper3D)](https://www.hyper3d.com/rodin)

### Step 3: Install Backend Dependencies

From the `Backend/` directory, use `uv` to sync and install dependencies:

```bash
cd Backend
uv sync
```

This will install all required packages specified in `pyproject.toml`:
- FastAPI (web framework)
- Mistral AI (LLM for narrative generation)
- Rodin integration (3D asset generation)
- UVicorn (ASGI server)
- And other dependencies

### Step 4: Start the Backend Server

Run the FastAPI server:

```bash
cd Backend
uv run python main.py
```

The server will start on `http://127.0.0.1:8000`

You can view the interactive API documentation at: `http://127.0.0.1:8000/docs`

### Step 5: Run the Game

Open and run the Unity project from the `Faith on the earth - Supercell Hackathon AI/` directory in Unity to play the game. The client will connect to the running backend server.

## API Endpoints

### Narrative Generation
- **POST** `/api/narrative/turn` - Generate the next narrative turn based on player action, game state, and faith
  - Receives: `GameState` (player action, history, faith, absurdity factor)
  - Returns: Generated narrative text and world changes


## Game State Model

The game tracks:
- **History**: Last 5 turns of narrative events
- **Player Action**: Current decision being made
- **Current Faith**: How strong the player's influence is
- **Absurde Factor**: Randomness/chaos parameter (adds unpredictability)

## Technologies Used

- **Backend**: FastAPI (Python)
- **LLM**: Mistral AI
- **Asset Generation**: Rodin AI (3D models)
- **Frontend**: Unity
- **Package Management**: uv + Python 3.12+
- **API Server**: Uvicorn

## Development Notes

- The backend uses CORS middleware to accept requests from the Unity client and any other clients
- LLM responses are logged for debugging purposes
- The system supports switching between different LLM providers in the future (currently configured for Mistral)
- Mock mode is available for development/testing without API keys (see `config.py`)

## Running in Different Modes

By default, the backend uses **Mistral API**. To switch modes, edit `Backend/config.py`:

```python
MODE = "MISTRAL"  # Use "MOCK" for testing without API calls
```

## Troubleshooting

**Connection Error to Backend**: Ensure the FastAPI server is running on `127.0.0.1:8000`

**API Key Errors**: Verify your `.env` file is properly configured in the `Backend/` directory

**Missing Dependencies**: Run `uv sync` to ensure all packages are installed

**Python Version Issues**: Ensure you're using Python 3.12 or higher (`python --version`)

# REMEMBER, it is possible to easily change the style from the AI by going to libs->prompts.py and changing the strings. But make sure that the output format stays the same. Have fun!

## License

See [LICENSE](LICENSE) file for details.
