# by Pavlo Bazilinskyy <pavlo.bazilinskyy@gmail.com>
# import random
import pandas as pd
import os
# from tqdm import tqdm
import json

import numpy as np
import seaborn as sns
import matplotlib.pyplot as plt
import re

import gtrust as gt
# from gtrust.run import SHOW_OUTPUT

logger = gt.CustomLogger(__name__)  # use custom logger


class CoupledSim:
    # pandas dataframe with extracted data
    sim_data = pd.DataFrame()
    # pickle file for saving data
    file_p = 'sim_data.p'
    # csv file for saving data
    file_csv = 'sim_data.csv'

    def __init__(self,
                 files_data: list,
                 save_p: bool,
                 load_p: bool,
                 save_csv: bool):
        # file with raw data
        self.files_data = files_data
        # save data as pickle file
        self.save_p = save_p
        # load data as pickle file
        self.load_p = load_p
        # save data as csv file
        self.save_csv = save_csv
        # initialize empty list for points
        self.points: dict[str, dict[str, list[float]]] = {'key_presses': {}, 'eye_gaze': {}}

    def set_data(self, sim_data):
        """Setter for the data object.
        """
        old_shape = self.sim_data.shape  # store old shape for logging
        self.sim_data = sim_data
        logger.info('Updated sim_data. Old shape: {}. New shape: {}.',
                    old_shape,
                    self.sim_data.shape)

    def read_data(self, filter_data=True, clean_data=True):
        """Read data into an attribute.

        Args:
            filter_data (bool, optional): flag for filtering data.
            clean_data (bool, optional): clean data.

        Returns:
            dataframe: updated dataframe.
        """
        # load from pickle if available
        if self.load_p:
            df = gt.common.load_from_p(self.file_p, 'sim data')
        else:
            dataframes = []
            
            # iterate over all experiment folders
            for experiment in os.listdir(self.files_data):
                experiment_path = os.path.join(self.files_data, experiment)
                if os.path.isdir(experiment_path):
                    # iterate over role folders (Driver, Passenger (1), Passenger (2))
                    for role in os.listdir(experiment_path):
                        role_path = os.path.join(experiment_path, role)
                        if os.path.isdir(role_path):
                            # iterate over csv files in role folder
                            for file in os.listdir(role_path):
                                filename = os.fsdecode(file)
                                if filename.endswith('.csv'):
                                    file_path = os.path.join(role_path, filename)
                                    logger.info(f'Reading sim data from {file_path}')
                                    
                                    # load csv
                                    df_pp = pd.read_csv(file_path, sep=';')
                                    
                                    # extract condition from filename
                                    match = re.search(r'_(A|M)_(HU|LU)\.csv$', filename)
                                    condition = match.group(1) + '_' + match.group(2) if match else 'Unknown'
                                    
                                    # add metadata columns
                                    df_pp['experiment'] = experiment
                                    df_pp['role'] = role
                                    df_pp['participant'] = filename  # track source file
                                    df_pp['condition'] = condition  # extracted condition
                                    
                                    dataframes.append(df_pp)
            

            # # Process keypress data column outside the loop
            # keypress_column = 'EmperorsRating - UnixTimeSeconds'
            # if keypress_column in df_pp.columns:
            #     dict_row = {}
            #     df_pp['processed_keypresses'] = df_pp[keypress_column].apply(
            #         lambda x: [int(value) for value in str(x).split(',') if value.strip() != '0'] if pd.notna(
            #             x) else []
            #     )
            #     df = pd.concat([df, df_pp], ignore_index=True)
            # # Process each row for gaze and keypress data
            # for _, row in df_pp.iterrows():
            #     stim_name = row.get('ParticipantID', '')  # Replace with actual participant ID column

            #     # --- Keypress Data Logging in dict_row ---
            #     if 'processed_keypresses' in row:
            #         dict_row[stim_name + '-processed_keypresses'] = row['processed_keypresses']

            #     # --- Eye-Tracking Data Processing ---
            #     if 'CombinedGazeForward' in row:
            #         gaze_data = row['CombinedGazeForward']

            #     if isinstance(gaze_data, int):  # Ensure gaze_data is a integer
            #         try:
            #             # Parse gaze data
            #             gaze_points = [tuple(map(float, point.strip('()').split(','))) for point in gaze_data]
            #             x = [point[0] for point in gaze_points]
            #             y = [point[1] for point in gaze_points]
            #             t = [point[2] for point in gaze_points] if len(gaze_points[0]) > 2 else []

            #             # Add gaze data to dict_row
            #             dict_row[stim_name + '-x'] = x
            #             dict_row[stim_name + '-y'] = y
            #             dict_row[stim_name + '-t'] = t

            #         except (ValueError, json.JSONDecodeError):
            #             logger.warning(f"Failed to parse gaze data for {stim_name}")
            #         else:
            #             logger.warning(f"Gaze data for {stim_name} is not in the expected string format.")
            #     # clean data
            #     if clean_data:
            #         df = self.clean_data(df)
            #     # filter data
            #     if filter_data:
            #         df = self.filter_data(df)
            #     else:
            #         continue

            # combine all data into a single dataframe
            df = pd.concat(dataframes, ignore_index=True) if dataframes else pd.DataFrame()
            # df = df.transpose()

            # # Check if 'processed_keypresses' column exists before filtering
            # if 'processed_keypresses' in df.columns:
            #     filtered_df = df[df['processed_keypresses'].apply(lambda x: any(i > 0 for i in x))]
            # else:
            #     logger.warning("Column 'processed_keypresses' not found. Skipping filtering step.")

            # filter data
            if filter_data:
                df = self.filter_data(df)
            # sort columns alphabetically
            df = df.reindex(sorted(df.columns), axis=1)
        # save to pickle
        if self.save_p:
            gt.common.save_to_p(self.file_p, df, 'sim data')
        # save to csv
        if self.save_csv:
            df.to_csv(os.path.join(gt.settings.output_dir, self.file_csv), index=False)
        logger.info('Saved sim data to csv file {}', self.file_csv)
        return df

    def filter_data(self, df):
        """Filtering of data not implemented.
        """
        # return df with data
        return df

    def clean_data(self, df, clean_years=True):
        """Cleaning of data not implemented.
        """
        # return df with data
        return df

    def show_info(self):
        """Output info for data not implemented.
        """
        logger.info('Output of info not implemented.')
