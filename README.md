# Analysing group trust in an automated/manual vehicle

This project defines a framework for the analysis of the level of trust in a traffic environment involving an automated vehicle. In the description below, it is assumed that the repo is stored in the folder `group-trust`. Terminal commands lower assume macOS.

## Setup
Tested with Python 3.9.12. To setup the environment run these two commands in a parent folder of the downloaded repository (replace `/` with `\` and possibly add `--user` if on Windows):
- `pip install -e group-trust` will setup the project as a package accessible in the environment.
- `pip install -r group-trust/requirements.txt` will install required packages.

### Configuration of project
Configuration of the project needs to be defined in `group-trust/config`. Please use the `default.config` file for the required structure of the file. If no custom config file is provided, `default.config` is used. The config file has the following parameters:
* `num_stimuli`: number of stimuli in the study.
* `num_stimuli_participant`: subset of stimuli in the study shown to each participant.
* `num_repeat`: number of times each stimulus is repeated.
* `kp_resolution`: bin size in ms in which keypress data is stored.
* `eg_resolution`: bin size in ms in which eyegaze data is stored.
* `files_data`: files with data from coupled simulator.
* `plotly_template`: template used to make graphs in the analysis.


## Troubleshooting
### Troubleshooting setup
#### ERROR: group-trust is not a valid editable requirement
Check that you are indeed in the parent folder for running command `pip install -e group-trust`. This command will not work from inside of the folder containing the repo.
