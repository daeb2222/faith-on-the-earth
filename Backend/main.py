from fastapi import FastAPI
from pydantic import BaseModel
from fastapi.middleware.cors import CORSMiddleware
import random
import os
import json
from mistralai import Mistral 
from dotenv import load_dotenv
from libs.prompts import NARRATOR_PROMPT, RIVAL_PROMPT
load_dotenv()

app = FastAPI()

# CORS configuration to allow Unity to connect without issues
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

# CONFIGURATION
# Options: "MISTRAL" or "MOCK" (Use MOCK if API is down or to save credits)
MODE:str = "MISTRAL" 
API_KEY:str = os.getenv("MISTRAL_API_KEY")
MODEL_TEMPERATURE:float = 0.5

# Safety check: Force MOCK mode if no key is found
if MODE == "MISTRAL" and API_KEY:
    client = Mistral(api_key=API_KEY)
else:
    print("⚠️ MOCK MODE ACTIVATED (Missing Key or Mode set to MOCK)")
    MODE = "MOCK"

# DATA MODELS
class GameState(BaseModel):
    player_action: str          # e.g., "Sacrifice a cow"
    history: list[str] = []     # e.g., ["Turn 1: Player burned the village", "Turn 2: ..."]
    absurde_factor: int       # e.g., from 1 to 10, where 1 is epic and 10 is gods poop rainbows
    current_faith: int = 100    # keeping track for rival as well

async def call_mistral(system: str, user:str, mistral_model:str="mistral-medium-latest", max_tokens:int=500, response_format={"type":"json_object"}, temperature:float=0.1):
    try:
        response = client.chat.complete(
            model=mistral_model,
            messages=[{"role":"system","content":system},{"role":"user","content":user}],
            response_format=response_format,
            max_tokens=max_tokens,
            temperature=temperature
        )
        return json.loads(response.choices[0].message.content)
    except:
        return get_mock_response()

@app.post("/api/turn")
async def generate_turn(state: GameState):
    print(f"🔮 Processing turn for action: {state.player_action}")
    short_term_memory = state.history[-5:] 
    previous_context = "\n".join(short_term_memory)
    
    print(f"🧠 Context used: {len(short_term_memory)} items")

    narrator_user = json.dumps({
        "recent_history": previous_context,
        "current_player_action": state.player_action,
        "absurde_factor": state.absurde_factor,
        "current_faith": state.current_faith
    })
    narrator_json = await call_mistral(NARRATOR_PROMPT, narrator_user)

    rival_user = json.dumps({
        "recent_history": previous_context,
        "current_player_action": state.player_action,
        "absurde_factor": state.absurde_factor,
        "current_faith": state.current_faith
    })
    rival_json = await call_mistral(RIVAL_PROMPT, rival_user)
   
    final = {
        **narrator_json,
        "antag_action": rival_json["action"],
        "antag_taunt": rival_json["taunt"],
        "antag_faith_delta": rival_json["faith_stolen"]
    }
    # Update history for next turn (optional: do in Unity or here)
    state.history.append(f"Turn: Narrative - {final['narrative']}; Rival - {final['antag_action']}")
    print(f"✅ Generated turn with narrative and rival action.")
    return final

def get_mock_response():
    """Returns a Python dictionary (not a string) to be serialized automatically"""
    print("🛡️ Using Mock Data")
    situations = [
        {
            "narrative": "The villagers have built a giant statue in your honor, but it accidentally blocks the sun.",
            "options": [
                {
                    "type": "cruel",
                    "desc": "Cause the statue to collapse, crushing a few villagers.",
                    "faith_delta": -10,
                    "antag_delta": 5,
                    "consequence": "The villagers are terrified but secretly relieved."
                },
                {
                    "type": "benevolent",
                    "desc": "Bless the statue to shine light on the village square.",
                    "faith_delta": 15,
                    "antag_delta": -5,
                    "consequence": "The villagers rejoice and hold a festival."
                },
                {
                    "type": "greedy",
                    "desc": "Demand more offerings to maintain the statue.",
                    "faith_delta": 5,
                    "antag_delta": 0,
                    "consequence": "The villagers grumble but comply."
                },
                {
                    "type": "lazy",
                    "desc": "Ignore the statue and let it be.",
                    "faith_delta": -5,
                    "antag_delta": 0,
                    "consequence": "The villagers are confused but carry on."
                }
            ],
            "antag_action": "Lord Mittens knocks over a cart of offerings, scattering them everywhere.",
            "antag_faith_delta": -15
        },
        {
            "narrative": "A sudden rain of frogs has descended upon the village, causing chaos.",
            "options": [
                {
                    "type": "cruel",
                    "desc": "Turn the frogs into toads that stick to people's faces.",
                    "faith_delta": -15,
                    "antag_delta": 10,
                    "consequence": "The villagers are horrified and disgusted."
                },
                {
                    "type": "benevolent",
                    "desc": "Make the frogs harmless and entertaining.",
                    "faith_delta": 20,
                    "antag_delta": -10,
                    "consequence": "The villagers laugh and play with the frogs."
                },
                {
                    "type": "greedy",
                    "desc": "Charge a fee for frog-catching lessons.",
                    "faith_delta": 10,
                    "antag_delta": 0,
                    "consequence": "The villagers begrudgingly pay up."
                },
                {
                    "type": "lazy",
                    "desc": "Do nothing and let the villagers deal with it.",
                    "faith_delta": -10,
                    "antag_delta": 0,
                    "consequence": "The villagers are left to fend for themselves."
                }
                ]
        }
    ]
    return random.choice(situations)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run("main:app", host="127.0.0.1", port=8000, reload=True)