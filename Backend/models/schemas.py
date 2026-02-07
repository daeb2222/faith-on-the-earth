from pydantic import BaseModel, Field
from typing import Optional


# For Narrative Models
class GameState(BaseModel):
    player_action: str
    history: list[str] = (
        []
    )  # New: ["Turn 0: Player Action: 'You open your eyes. You are a divinity. There is a village,with small and big situations to be attended. You are curious.' -> Outcome: '' || Rival Intervention: ''"]
    absurde_factor: float
    current_faith: int = 100
    rival_faith: int = 0  # New: Track rival's score for taunts


# For Rodin API
class RodinGen2Request(BaseModel):
    """
    Payload for generating a 3D asset via Rodin Gen-2.
    """

    prompt: str = Field(..., description="Text description of the object to generate.")

    # Options for modification from Unity
    quality: str = Field(
        "extra-low",
        description="Speed vs Detail. Options: 'extra-low' (fastest), 'low', 'medium', 'high'.",
    )
    mesh_mode: str = Field(
        "Raw",
        description="Geometry type. 'Raw' (Triangles) is best for game engines. 'Quad' is for editing.",
    )
    material: str = Field(
        "PBR",
        description="Material type. 'PBR' (Realistic), 'Shaded' (Baked lighting). or 'All'",
    )
    quality_override: Optional[str] = Field(
        None,
        description="""Customize poly count for generation, providing more accurate control over mesh face count. When mesh_mode = Raw: Range from 500 to 1,000,000. Default is 500,000.
When mesh_mode = Quad: Range from 1,000 to 200,000. Default is 18,000.
Recommend 150,000+ faces for Gen-2.
This parameter is an advanced parameter of quality. When this parameter is invoked, the quality parameter will not take effect."
    """,
    )
    bbox_condition: Optional[list[int]] = Field(
        None,
        description="""This is a controlnet that controls the maxmimum sized of the generated model.
This array must contain 3 elements, Width(Y-axis), Height(Z-axis), and Length(X-axis), in this exact fixed sequence (y, z, x).
""",
    )
    seed: Optional[int] = Field(
        None,
        description="Random seed (0-65535). Use the same seed for reproducible results.",
    )


class GenerationResponse(BaseModel):
    task_uuid: str
    subscription_key: str


class StatusRequest(BaseModel):
    subscription_key: str


class DownloadRequest(BaseModel):
    task_uuid: str


class AssetResult(BaseModel):
    status: str
    model_url: Optional[str] = None
    error: Optional[str] = None
