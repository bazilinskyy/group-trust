# by Pavlo Bazilinskyy <pavlo.bazilinskyy@gmail.com>
import pandas as pd
import os
from tqdm import tqdm
import json

import gtrust as gt

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

    def read_data(self, filter_data=True):
        """Read data into an attribute.

        Args:
            filter_data (bool, optional): flag for filtering data.

        Returns:
            dataframe: updated dataframe.
        """
        # load data
        if self.load_p:
            df = gt.common.load_from_p(self.file_p, 'sim data')
        # process data
        else:
            # read files with sim data one by one
            data_list = []
            data_dict = {}  # dictionary with data
            for file in self.files_data:
                # iterate over csv files in the directory
                # logger.info('Reading sim data from {}.', file)
                # f = open(file, 'r')
                # # add data from the file to the dictionary
                # data_list += f.readlines()
                # f.close()
                continue
            # hold info on previous row for worker
            prev_row_info = pd.DataFrame(columns=['worker_code', 'time_elapsed'])
            prev_row_info.set_index('worker_code', inplace=True)
            # read rows in data
            for row in tqdm(data_list):  # tqdm adds progress bar
                # use dict to store data
                dict_row = {}
                # load data from a single row into a list
                list_row = json.loads(row)
                logger.debug('To implement going over rows of data.')
            # turn into pandas dataframe
            df = pd.DataFrame(data_dict)
            df = df.transpose()
            # # report people that attempted study
            # unique_worker_codes = df['worker_code'].drop_duplicates()
            # logger.info('People who attempted to participate: {}', unique_worker_codes.shape[0])
            # # filter data
            # if filter_data:
            #     df = self.filter_data(df)
            # # sort columns alphabetically
            # df = df.reindex(sorted(df.columns), axis=1)
            # # move worker_code to the front
            # worker_code_col = df['worker_code']
            # df.drop(labels=['worker_code'], axis=1, inplace=True)
            # df.insert(0, 'worker_code', worker_code_col)
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
