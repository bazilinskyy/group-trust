# by Pavlo Bazilinskyy <pavlo.bazilinskyy@gmail.com>
# import random
import pandas as pd
import os
# from tqdm import tqdm
import json

import numpy as np
import seaborn as sns
import matplotlib.pyplot as plt

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
        # load data
        if self.load_p:
            df = gt.common.load_from_p(self.file_p, 'sim data')
        # process data
        else:
            # read files with sim data one by one
            # TODO: check what's best, combining all pp into 1 dataframe or have individual dataframe
            df = pd.DataFrame()  # dictionary with data
            # iterate over files in the directory
            for file in os.listdir(self.files_data):
                filename = os.fsdecode(file)
                if filename.endswith('.csv'):
                    logger.info('Reading sim data from {}.', os.path.join(self.files_data, filename))
                    # load from csv
                    df_pp = pd.read_csv(os.path.join(self.files_data, filename), sep=',')

            # Process keypress data column outside the loop
            keypress_column = 'EmperorsRating - UnixTimeSeconds'
            if keypress_column in df_pp.columns:
                dict_row = {}
                df_pp['processed_keypresses'] = df_pp[keypress_column].apply(
                    lambda x: [int(value) for value in str(x).split(',') if value.strip() != '0'] if pd.notna(
                        x) else []
                )
                df = pd.concat([df, df_pp], ignore_index=True)
            # Process each row for gaze and keypress data
            for _, row in df_pp.iterrows():
                stim_name = row.get('ParticipantID', '')  # Replace with actual participant ID column

                # --- Keypress Data Logging in dict_row ---
                if 'processed_keypresses' in row:
                    dict_row[stim_name + '-processed_keypresses'] = row['processed_keypresses']

                # --- Eye-Tracking Data Processing ---
                if 'CombinedGazeForward' in row:
                    gaze_data = row['CombinedGazeForward']

                if isinstance(gaze_data, int):  # Ensure gaze_data is a integer
                    try:
                        # Parse gaze data
                        gaze_points = [tuple(map(float, point.strip('()').split(','))) for point in gaze_data]
                        x = [point[0] for point in gaze_points]
                        y = [point[1] for point in gaze_points]
                        t = [point[2] for point in gaze_points] if len(gaze_points[0]) > 2 else []

                        # Add gaze data to dict_row
                        dict_row[stim_name + '-x'] = x
                        dict_row[stim_name + '-y'] = y
                        dict_row[stim_name + '-t'] = t

                    except (ValueError, json.JSONDecodeError):
                        logger.warning(f"Failed to parse gaze data for {stim_name}")
                    else:
                        logger.warning(f"Gaze data for {stim_name} is not in the expected string format.")
                # clean data
                if clean_data:
                    df = self.clean_data(df)
                # filter data
                if filter_data:
                    df = self.filter_data(df)
                else:
                    continue

            # turn into pandas dataframe
            df = pd.concat([df, df_pp], ignore_index=True)
            # df = df.transpose()

            # Check if 'processed_keypresses' column exists before filtering
            if 'processed_keypresses' in df.columns:
                filtered_df = df[df['processed_keypresses'].apply(lambda x: any(i > 0 for i in x))]
                print(filtered_df[['processed_keypresses']].head())
            else:
                logger.warning("Column 'processed_keypresses' not found. Skipping filtering step.")

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
            logger.info('Saved sim data to csv file {}', self.file_csv + '.csv')
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
