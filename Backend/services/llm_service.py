import json
import asyncio
from mistralai import Mistral
from config import MODE, MISTRAL_API_KEY
from libs.prompts import NARRATOR_PROMPT, RIVAL_PROMPT

client = None
if MODE == "MISTRAL" and MISTRAL_API_KEY:
    client = Mistral(api_key=MISTRAL_API_KEY)


async def _call_mistral_api(
    system: str,
    user: str,
    model: str = "mistral-small-latest",
    max_tokens: int = 500,
    response_format={"type": "json_object"},
    temperature: float = 0.1,
):
    if not client:
        return get_mock_response()
    try:
        response = await client.chat.complete_async(
            model=model,
            messages=[
                {"role": "system", "content": system},
                {"role": "user", "content": user},
            ],
            max_tokens=max_tokens,
            response_format=response_format,
            temperature=temperature,
        )
        return json.loads(response.choices[0].message.content)
    except Exception as e:
        print(f"🔥 Mistral API Error: {e}")
        return get_mock_response()


def get_mock_response():
    """Fallback mock data."""
    return {
        "narrative": "The gods are silent. A giant cat is chewing on the internet cables.",
        "action": "Connection Lost",  # Normalized key for internal use
        "taunt": "Try again later, human.",
        "faith_stolen": 0,
        "options": [],  # In case narrator needs it
    }


async def generate_turn_narrative(
    previous_context: str,
    action: str,
    absurde_factor: int,
    faith: int,
    rival_faith: int = 0,
) -> dict:
    """
    Orchestrates the parallel calls to Narrator and Rival.
    Returns the combined dictionary.
    """

    # Prepare the JSON payloads for the prompts
    user_payload = json.dumps(
        {
            "current_player_action": action,
            "recent_history": previous_context,
            "absurde_factor": absurde_factor,
            "current_faith": faith,
            "rival_faith": rival_faith,
        }
    )

    # Run both personalities in parallel
    narrator_task = _call_mistral_api(
        NARRATOR_PROMPT, user_payload, temperature=0.3
    )  # Narrator is more consistent
    rival_task = _call_mistral_api(
        RIVAL_PROMPT, user_payload, temperature=0.6
    )  # Rival might be more unpredictable

    narrator_json, rival_json = await asyncio.gather(narrator_task, rival_task)

    final = {
        **narrator_json,
        "antag_action": rival_json.get("action", "Glaring menacingly"),
        "antag_taunt": rival_json.get("taunt", "..."),
        "antag_faith_delta": rival_json.get("faith_stolen", 0),
    }
    # Expected structured by the frontend/Unity
    return final
