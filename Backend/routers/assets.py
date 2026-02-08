from fastapi import APIRouter, HTTPException
from models.schemas import RodinRequest, AssetResult
from services.rodin_service import generate_asset_rodin
router = APIRouter(prefix="/api/assets", tags=["Asset Generation"])


@router.post("/rodin", response_model=AssetResult)
def generate_rodin(req: RodinRequest):
    try:
        result = generate_asset_rodin(req)
        return AssetResult(task_uuid=result["task_uuid"], files=result["files"])
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
