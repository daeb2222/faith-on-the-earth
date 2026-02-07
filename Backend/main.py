from fastapi import FastAPI
from pydantic import BaseModel
from fastapi.middleware.cors import CORSMiddleware
import random
import os
import json
import asyncio
from mistralai import Mistral
from dotenv import load_dotenv
from libs.prompts import NARRATOR_PROMPT, RIVAL_PROMPT

load_dotenv()

from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

# Import your new routers
from routers import narrative, assets

app = FastAPI(title="God Game AI Engine")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

# Connect the routers to the main app
app.include_router(
    narrative.router
)  # For llm, currently using Mistral API, but could be switched in the future to other models
app.include_router(assets.router)  # Assets generation

if __name__ == "__main__":
    import uvicorn

    uvicorn.run("main:app", host="127.0.0.1", port=8000, reload=True)
