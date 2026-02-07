import time
import requests
from typing import Dict, Any
from models.schemas import RodinGen2Request
from config import (
    RODIN_API_KEY,
    RODIN_API_ENDPOINT,
    RODIN_DOWNLOAD_ENDPOINT,
    RODIN_STATUS_ENDPOINT,
)

HEADERS = {
    "Authorization": f"Bearer {RODIN_API_KEY}",
    "Content-Type": "application/json",
}


def build_payload(req: RodinGen2Request) -> Dict[str, Any]:
    """Convierte el schema RodinGen2Request en el payload correcto para Gen‑2."""
    payload = {
        "prompt": req.prompt,
        "tier": "Gen-2",
        "geometry_file_format": "glb",
        "quality": req.quality,
        "mesh_mode": req.mesh_mode,
        "material": req.material,
    }
    if req.quality_override is not None:
        payload["quality_override"] = req.quality_override
    if req.bbox_condition is not None:
        payload["bbox_condition"] = req.bbox_condition
    if req.seed is not None:
        payload["seed"] = req.seed
    return payload


def submit_gen2(req: RodinGen2Request) -> Dict[str, Any]:
    """Envía una generación Gen‑2 a Hyper3D."""
    body = build_payload(req)

    r = requests.post(RODIN_API_ENDPOINT, json=body, headers=HEADERS)
    r.raise_for_status()
    return r.json()


def wait_for_completion(subscription_key: str, poll_interval: float = 10.0) -> None:
    """Espera hasta que TODOS los jobs estén en Done."""
    while True:
        r = requests.post(
            RODIN_STATUS_ENDPOINT,
            json={"subscription_key": subscription_key},
            headers=HEADERS,
        )
        r.raise_for_status()
        data = r.json()

        statuses = [job["status"] for job in data["jobs"]]

        # Si hay algún job en Generating o Waiting → no está listo
        if any(s in ("Generating", "Waiting") for s in statuses):
            time.sleep(poll_interval)
            continue

        # Si todos están en Done → salir
        if all(s == "Done" for s in statuses):
            return

        # Si aparece Failed → lanzar error
        if "Failed" in statuses:
            raise RuntimeError("Rodin Gen‑2 job failed.")

        time.sleep(poll_interval)


def download_results(task_uuid: str) -> Dict[str, Any]:
    """Descarga los resultados usando task_uuid."""
    r = requests.post(
        RODIN_DOWNLOAD_ENDPOINT, json={"task_uuid": task_uuid}, headers=HEADERS
    )
    r.raise_for_status()
    return r.json()


def generate_asset_gen2(req: RodinGen2Request) -> Dict[str, Any]:
    """Pipeline completo: submit → wait → download."""
    # lets do some prints to see the flow with emojis as well
    print("🚀 Submitting Gen‑2 job to Rodin...")
    submit = submit_gen2(req)
    
    task_uuid = submit["uuid"]
    subscription_key = submit["jobs"]["subscription_key"]
    print(f"📥 Job submitted. Task UUID: {task_uuid}.  Subscription Key: {subscription_key} waiting for completion...")
    wait_for_completion(subscription_key)
    print("✅ Job completed. Downloading results...")
    download_info = download_results(task_uuid)
    print(f"📂 Download completed. Files: {download_info.get('list', [])}")
    return {"task_uuid": task_uuid, "files": download_info.get("list", [])}
