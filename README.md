# ssh-agent-wrapper-win
A simple wrapper for ssh-agent to run from Windows command line

The wrapper fires up ssh-agent, sets the required environment variables
and adds the private identities to the agent

## Usage recommendation
Add a shortcut to the file to the Windows Startup folder

## Assumptions
For the first release, the wrapper assumes the following:
- ssh-agent in path
- shh-add in path
- private keys are in %USERPROFILE%/.ssh folder
- private keys names start with id (e.g. id_rsa)



