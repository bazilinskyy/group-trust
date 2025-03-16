# by Pavlo Bazilinskyy <pavlo.bazilinskyy@gmail.com>
import matplotlib.pyplot as plt
import matplotlib._pylab_helpers
import gtrust as gt

gt.logs(show_level='debug', show_color=True)
logger = gt.CustomLogger(__name__)  # use custom logger

# const
SAVE_P = True  # save pickle files with data
LOAD_P = False  # load pickle files with data
SAVE_CSV = True  # save csv files with data
FILTER_DATA = True  # filter Appen and heroku data
CLEAN_DATA = False  # clean Appen data
SHOW_OUTPUT = True  # should figures be plotted


if __name__ == '__main__':
    # create object for working with data
    files_data = gt.common.get_configs('files_data')
    csim = gt.analysis.CoupledSim(files_data=files_data,
                                  save_p=SAVE_P,
                                  load_p=LOAD_P,
                                  save_csv=SAVE_CSV)

    # read simulator data
    sim_data = csim.read_data(filter_data=FILTER_DATA, clean_data=CLEAN_DATA)

    if sim_data is not None and not sim_data.empty:
        logger.info('{} rows of data included in analysis.', sim_data.shape[0])
    else:
        logger.warning('No data loaded into sim_data. Check file paths and data loading steps.')
    
    # update original data files
    csim.show_info()  # show info for filtered data

    if SHOW_OUTPUT:
        # Output
        analysis = gt.analysis.Analysis()
        logger.info('Creating figures.')

        # histogram of AOIs
        logger.info('Unique values in EyeTracking_FocusName: ')
        print(sim_data['EyeTracking_FocusName'].unique())
        analysis.hist_aoi(sim_data)

        logger.info('Unique values in EmperorsRating_UnixTime: ')
        print(sim_data['EmperorsRating_UnixTime'].unique())
        # analysis.hist(sim_data, 'EmperorsRating_UnixTime')

        figures = [manager.canvas.figure
                   for manager in
                   matplotlib._pylab_helpers.Gcf.get_all_fig_managers()]
        # show figures, if any
        if figures:
            plt.show()
