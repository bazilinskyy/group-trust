"""Contains various function used throughout this project."""
from typing import Dict
import os
import json
import pickle
import sys

import gtrust as gt

logger = gt.CustomLogger(__name__)  # use custom logger


def get_secrets(entry_name: str, secret_file_name: str = 'secret') -> Dict[str, str]:
    """
    Open the secrets file and return the requested entry.
    """
    with open(os.path.join(gt.settings.root_dir, secret_file_name)) as f:
        return json.load(f)[entry_name]


def get_configs(entry_name: str, config_file_name: str = 'config', config_default_file_name: str = 'default.config'):
    """
    Open the config file and return the requested entry.
    If no config file is found, open default.config.
    """
    # check if config file is updated
    if not gt.common.check_config():
        sys.exit()
    try:
        with open(os.path.join(gt.settings.root_dir, config_file_name)) as f:
            content = json.load(f)
    except FileNotFoundError:
        with open(os.path.join(gt.settings.root_dir, config_default_file_name)) as f:
            content = json.load(f)
    return content[entry_name]


def check_config(config_file_name: str = 'config', config_default_file_name: str = 'default.config'):
    """
    Check if config file has at least as many rows as default.config.
    """
    # load config file
    try:
        with open(os.path.join(gt.settings.root_dir, config_file_name)) as f:
            config = json.load(f)
    except FileNotFoundError:
        logger.error('Config file {} not found.', config_file_name)
        return False
    except json.decoder.JSONDecodeError:
        logger.error('Config file badly formatted. Please update based on' +
                     ' default.config.', config_file_name)
        return False
    # load default.config file
    try:
        with open(os.path.join(gt.settings.root_dir,
                               config_default_file_name)) as f:
            default = json.load(f)
    except FileNotFoundError:
        logger.error('Default config file {} not found.', config_file_name)
        return False
    except json.decoder.JSONDecodeError:
        logger.error('Config file badly formatted. Please update based on' +
                     ' default.config.', config_file_name)
        return False
    # check length of each file
    if len(config) < len(default):
        logger.error('Config file has {} variables, which is fewer than {}'
                     + ' variables in default.config. Please update.',
                     len(config),
                     len(default))
        return False
    else:
        return True


def search_dict(dictionary, search_for, nested=False):
    """
    Search if dictionary value contains certain string search_for. If
    nested=True multiple levels are traversed.
    """
    for k in dictionary:
        if nested:
            for v in dictionary[k]:
                if search_for in v:
                    return k
                elif v in search_for:
                    return k
        else:
            if search_for in dictionary[k]:
                return k
            elif dictionary[k] in search_for:
                return k
    return None


def save_to_p(file, data, desription_data='data'):
    """
    Save data to a pickle file.
    """
    path = os.path.join(os.path.join(gt.settings.root_dir, 'gtrust'), file)
    with open(path, 'wb') as f:
        pickle.dump(data, f)
    logger.info('Saved ' + desription_data + ' to pickle file {}.', file)


def load_from_p(file, desription_data='data'):
    """
    Load data from a pickle file.
    """
    path = os.path.join(os.path.join(gt.settings.root_dir, 'gtrust'), file)
    with open(path, 'rb') as f:
        data = pickle.load(f)
    logger.info('Loaded ' + desription_data + ' from pickle file {}.',
                file)
    return data
