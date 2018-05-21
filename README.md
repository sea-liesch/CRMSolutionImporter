# CRMSolutionImporter
A utitlity to import solutions into CRM

# Building
Uses the standard .net build process via Visual Studio or msbuild

# Usage

Usage is as follows

```
CRMSolutionImporter.exe --connection="<crm-connection-string>" --solution=<solutionfile>.zip
```

This will import a solution file into the target dynamics instance. The solution also parses the result of the solution import
and will fail the workflow if any warnings or errors besides your standard "Workflow/SDK Messaging Step has been deactivated and replaced"
occur the solution import will return a non-zero exit code.

There are also a couple other arguments that I would reccommend using

`--export` will export the result of the solution import as output.xml
`--publish` will publish the solution after everything is done

# Contributing
If you find a bug or have a need that this program doesn't meet feel free to submit an issue.

If you are a developer PRs are always welcome