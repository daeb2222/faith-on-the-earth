from fastapi import APIRouter
from models.schemas import GameState
from services import llm_service

router = APIRouter(prefix="/api/narrative", tags=["Narrative"])


@router.post("/turn")
async def generate_turn(state: GameState):
    short_term_memory = state.history[-5:]
    previous_context = "\n".join(short_term_memory)

    result = await llm_service.generate_turn_narrative(
        previous_context=previous_context,
        action=state.player_action,
        absurde_factor=state.absurde_factor,
        faith=state.current_faith,
    )
    print(f"Generated Result: {result}")  # Debug log to see the raw output from LLM
    return result
