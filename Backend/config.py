from dotenv import load_dotenv
import os

load_dotenv()

# Global Settings
MODE = os.getenv(
    "APP_MODE", "MISTRAL"
)  # Default to MISTRAL if not set, the deal is to use oLLama or similar locally for faster responses


# Mistral Settings
MISTRAL_API_KEY = os.getenv("MISTRAL_API_KEY")


# Rodin Settings
RODIN_API_KEY = os.getenv("RODIN_API_KEY")
RODIN_API_ENDPOINT = os.getenv("RODIN_API_URL", "https://api.hyper3d.com/api/v2/rodin")
RODIN_STATUS_ENDPOINT = os.getenv(
    "RODIN_STATUS_URL", "https://api.hyper3d.com/api/v2/status"
)
RODIN_DOWNLOAD_ENDPOINT = os.getenv(
    "RODIN_DOWNLOAD_URL", "https://api.hyper3d.com/api/v2/download"
)

# if not MISTRAL_API_KEY or not RODIN_API_KEY:
#     print("⚠️  Keys missing. Forcing MOCK mode.")
#     MODE = "MOCK"
