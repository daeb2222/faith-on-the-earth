from fastapi import APIRouter, HTTPException
from models.schemas import RodinGen2Request, AssetResult
from services.rodin_service import generate_asset_gen2

router = APIRouter(prefix="/api/assets", tags=["Asset Generation"])


@router.post("/rodin/gen2", response_model=AssetResult)
def generate_rodin_gen2(req: RodinGen2Request):
    try:
        result = generate_asset_gen2(req)
        return AssetResult(task_uuid=result["task_uuid"], files=result["files"])
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))
