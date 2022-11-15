import datetime
import os
import shutil
from pathlib import Path

import yaml


def build_archive_name(resources_path):
    config_path = f'{resources_path}/config.yml'
    with open(config_path, 'r', encoding='UTF-8') as file:
        config = yaml.safe_load(file)
        app_name = config['application']['name']
        app_version = config['application']['version']
    now = datetime.datetime.now()
    return f'{app_name}-{app_version}.{now.strftime("%Y%m%d%H%M%S")}'


def pack():
    print("===== start pack =====")
    # 压缩文件
    current_path = os.path.abspath(__file__)
    resources_path = Path(current_path).resolve().parents[2]
    solution_path = Path(current_path).resolve().parents[3]
    bin_directory = f'{solution_path}/bin'
    release_directory = f'{bin_directory}/Release'
    archive_name = build_archive_name(resources_path)
    file_name = shutil.make_archive(archive_name, "zip", release_directory)
    # 移动文件
    shutil.move(file_name, bin_directory)
    print("===== pack success =====")


if __name__ == '__main__':
    pack()
