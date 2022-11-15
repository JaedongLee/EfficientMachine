import os.path
import shutil
from pathlib import Path

current_path = os.path.abspath(__file__)
root_path = Path(current_path).resolve().parents[5]
source_file = f'{root_path}/Service/resource/database/EfficientMachine.db'
target_directory = f'{root_path}/Client/EfficientMachine/Resources/Tools/Database'
print(source_file)
print(target_directory)
shutil.copy(source_file, target_directory)
