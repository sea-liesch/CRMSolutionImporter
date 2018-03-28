using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CRMSolutionImporter
{
    class CommandLineArgs
    {
        [Option('c', "connection", Required = true, HelpText = "Dynamics CRM connection string")]
        public string Connection { get; set; }

        [Option('s', "solution", Required = true, HelpText = "Solution file to import")]
        public string SolutionFilePath { get; set; }

        [Option("export", HelpText = "Whether or not to export the solution log")]
        public bool ExportLog { get; set; }

        [Option("publish", HelpText = "Whether or not to publish the solution afterwards")]
        public bool Publish { get; set; }
    }
}
