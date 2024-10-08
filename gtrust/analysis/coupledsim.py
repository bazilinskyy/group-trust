# by Pavlo Bazilinskyy <pavlo.bazilinskyy@gmail.com>
import pandas as pd
import os
from tqdm import tqdm
import json

import gtrust as gt
from gtrust.run import SHOW_OUTPUT

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
                    df_pp = pd.read_csv(os.path.join(self.files_data, filename), sep=';')
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
            # filter data
            if filter_data:
                df = self.filter_data(df)
            # sort columns alphabetically
            df = df.reindex(sorted(df.columns), axis=1)

            # keypresses
            if 'EmperorsRating - UnixTimeSeconds' in self.sim_data:
                # record given keypresses
                responses = pd.DataFrame['EmperorsRating - UnixTimeSeconds']
                logger.debug('Found {} points in keypress data.',
                             len(responses))
                # extract pressed keys and rt values
                key = [point['key'] for point in responses]
                rt = [point['rt'] for point in responses]
                # check if values were recorded previously
               # if stim_name + '-key' not in dict_row.keys():
                    # first value
               #    dict_row[stim_name + '-key'] = key
               # else:
                    # previous values found
               #     dict_row[stim_name + '-key'].extend(key)
                # check if values were recorded previously
               # if stim_name + '-rt' not in dict_row.keys():
                    # first value
               #     dict_row[stim_name + '-rt'] = rt
               # else:
               #     # previous values found
               #     dict_row[stim_name + '-rt'].extend(rt)

        # save to pickle
        if self.save_p:
            gt.common.save_to_p(self.file_p, df, 'sim data')
        # save to csv
        if self.save_csv:
            df.to_csv(os.path.join(gt.settings.output_dir, self.file_csv), index=False)
            logger.info('Saved sim data to csv file {}', self.file_csv + '.csv')
        # update attribute
        self.heroku_data = df
        # return df with data
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
