﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions.Common;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Helpers;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi;


namespace TechTalk.SpecFlow.Specs.Support
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly CurrentVersionDriver _currentVersionDriver;


        public Hooks(ScenarioContext scenarioContext, CurrentVersionDriver currentVersionDriver)
        {
            _scenarioContext = scenarioContext;
            _currentVersionDriver = currentVersionDriver;
        }

        [BeforeScenario()]
        public void BeforeScenario()
        {
            _currentVersionDriver.NuGetVersion = VersionInfo.NuGetVersion;
            _scenarioContext.ScenarioContainer.RegisterTypeAs<OutputConnector, IOutputWriter>();
        }

        [BeforeTestRun]
        public static void BeforTestRun()
        {
            var appConfigDriver = new AppConfigDriver();
            var folders = new Folders(appConfigDriver);


            DeletePackageVersionFolders();
            DeleteOldTestRunData(folders);
        }

        private static void DeleteOldTestRunData(Folders folders)
        {
            try
            {
                FileSystemHelper.DeleteFolderContent(folders.FolderToSaveGeneratedSolutions);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void DeletePackageVersionFolders()
        {

            var currentVersionDriver = new CurrentVersionDriver {NuGetVersion = VersionInfo.NuGetVersion};

            string[] packageNames = { "SpecFlow", "SpecFlow.CustomPlugin", "SpecFlow.MsTest", "SpecFlow.NUnit", "SpecFlow.NUnit.Runners", "SpecFlow.Tools.MsBuild.Generation", "SpecFlow.xUnit" };
            
            foreach (var name in packageNames)
            {
                string hooksPath = Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), ".nuget", "packages", name, currentVersionDriver.NuGetVersion);
                FileSystemHelper.DeleteFolder(hooksPath);
            }
        }
    }
}
