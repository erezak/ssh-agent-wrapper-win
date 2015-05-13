using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sshagentwrapper {
    class Program {

        const string AGENT = "ssh-agent";
        const string AGENT_KILL_PARAM = "-k";
        const string ADD_KEYS = "ssh-add";
        const string ADD_KEYS_PARAMS = "/.ssh/id*";

        static void Main(string[] args) {

            System.IO.StreamReader result = null;

            // Start the new agent
            result = RunCommand(AGENT);

            // Iterate on output lines
            string line;
            while ((line = result.ReadLine()) != null) {
                var variablesToSet = line.Split('=');
                if (variablesToSet.Length == 2) {
                    var variableName = variablesToSet[0];
                    var variableValue = variablesToSet[1];
                    // The value may contain a command to set the actual variable, we don't need this
                    if (variableValue.Contains(';')) {
                        variableValue = variableValue.Split(';')[0];
                    }
                    Environment.SetEnvironmentVariable(variableName, variableValue, EnvironmentVariableTarget.User);
                    // We also need to set the agent for this process to allow adding identities later
                    Environment.SetEnvironmentVariable(variableName, variableValue, EnvironmentVariableTarget.Process);
                }
            }

            // Try to add keys to id chain
            try {
                var userProfile = Environment.GetEnvironmentVariable("USERPROFILE", EnvironmentVariableTarget.Process);
                userProfile = userProfile.Replace("\\", "/");
                result = RunCommand(ADD_KEYS, userProfile + ADD_KEYS_PARAMS);
                Console.WriteLine(result.ReadToEnd());
                result = RunCommand("ssh-add", "-l");
                Console.WriteLine(result.ReadToEnd());
            } catch (Win32Exception e) {
                Console.WriteLine("Coulnd't add keys. Try manually.");
                Console.WriteLine("Error was: " + e.Message);
                Environment.Exit(2);
            }

            Console.WriteLine("Keys added successfully.");
            Console.WriteLine("Please CLOSE this window and open a new command prompt to see changes.");
        }

        private static System.IO.StreamReader RunCommand(string commandName, string commandParams = null, bool redirectOutput = true) {
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo(commandName, commandParams);

            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            procStartInfo.RedirectStandardOutput = redirectOutput;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;
            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            if (redirectOutput) {
                return proc.StandardOutput;
            } else {
                return null;
            }
        }
    }
}
