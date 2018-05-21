using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using CommandLine;
using System.IO;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System.Xml;

namespace CRMSolutionImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineArgs>(args).WithParsed((CommandLineArgs c) =>
            {
                int returnCode = Run(c);
                Console.WriteLine($"Complete with code {returnCode}");
                Environment.Exit(returnCode);
            });
        }
        static int Run(CommandLineArgs args)
        {
            List<string> ignoredErrorCodes = new List<string>()
            {
                "0x80045042", // Original workflow definition has been deactivated and replaced
                "0x80045043", // The original sdkmessageprocessingstep has been disabled and replaced.
            };

            CrmServiceClient service = new CrmServiceClient(args.Connection);
            service.OrganizationServiceProxy.Timeout = new TimeSpan(0,30,0);
            if (!string.IsNullOrEmpty(service.LastCrmError))
            {
                Console.WriteLine("Unable to log into CRM");
                Console.WriteLine(service.LastCrmError);
                return 1;
            }

            if (!File.Exists(args.SolutionFilePath))
            {
                Console.WriteLine($"Unable to load file {args.SolutionFilePath} file doesn't exist");
                return 2;
            }

            byte[] solutionFile = File.ReadAllBytes(args.SolutionFilePath);
            Guid jobId = Guid.NewGuid();

            ImportSolutionRequest request = new ImportSolutionRequest()
            {
                PublishWorkflows = true,
                CustomizationFile = solutionFile,
                ImportJobId = jobId,
            };

            bool successful = false;

            Console.WriteLine("Importing solution");
            try
            {
                service.Execute(request);
                successful = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Entity importJob = service.Retrieve("importjob", jobId, new ColumnSet("data"));
            string importData = (string)importJob["data"];

            XmlDocument resultDocument = new XmlDocument();
            List<string> errors = new List<string>();
            resultDocument.LoadXml(importData);
            foreach (XmlNode result in resultDocument.SelectNodes("//result"))
            {
                if (result.Attributes["result"].Value != "success" && !ignoredErrorCodes.Contains(result.Attributes["errorcode"].Value))
                {
                    errors.Add(result.ParentNode.OuterXml);
                }
            }

            if (args.ExportLog)
            {
                File.WriteAllText("output.xml", importData);
            }

            if (errors.Count > 0)
            {
                Console.WriteLine("Import unsuccessful");
                foreach (var item in errors)
                {
                    Console.WriteLine(item);
                }

                return 100;
            }

            if (!successful)
            {
                return 99;
            }

            if (args.Publish)
            {
                Console.WriteLine("Publishing");
                PublishAllXmlRequest publishRequest = new PublishAllXmlRequest();
                service.Execute(publishRequest);
            }

            return 0;
        }
    }
}
