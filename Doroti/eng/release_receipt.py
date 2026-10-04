"""A native receipt belongs to exactly one local candidate execution."""
import json
import os
from pathlib import Path

def receipt_environment(base, path, run_id, version):
    result = {key: value for key, value in base.items() if key not in
              ('DOROTI_RELEASE_RECEIPT', 'DOROTI_RELEASE_RUN_ID', 'DOROTI_RELEASE_VERSION')}
    result.update(DOROTI_RELEASE_SMOKE='1', DOROTI_RELEASE_RECEIPT=str(Path(path).resolve()),
                  DOROTI_RELEASE_RUN_ID=run_id, DOROTI_RELEASE_VERSION=version)
    return result

def validate_receipt(path, run_id, version):
    value = json.loads(Path(path).read_text(encoding='utf-8'))
    if not isinstance(value, dict) or value.get('runId') != run_id or value.get('version') != version:
        raise ValueError('Native receipt belongs to another run or candidate.')
    if any(value.get(key) is not True for key in ('nativeFirstFrame', 'twoWindows', 'secondResizedAndClosed')) or type(value.get('survivorsBeforeMainClose')) is not int or value['survivorsBeforeMainClose'] != 1:
        raise ValueError('Native consumer did not complete its two-window fixture.')
    return value
