﻿using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace LigerShark.Farticus
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidFarticusPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideOptionPage(typeof(FartOptions), "Farticus", "General", 101, 101, true, new[] { "The original Visual Studio fart app" })]
    public sealed class FarticusPackage : ExtensionPointPackage
    {
        private DTE2 _dte;
        private BuildEvents _events;

        private readonly List<string> _messages = new List<string>()
        {
            "Yes! This is my favorite type of fart",
            "Excellent. Don't hold back!",
            "Great job! It smells like roses in here now",
            "Sometimes you just have to let 'em rip",
            "Ahhh, that felt right",
            "Ooops, I'm not sure about that one",
            "Wow, someone has been practicing",
            "You might want to be a little careful",
            "What a stinker. The paint is coming off the walls"
        };

        protected override void Initialize()
        {
            base.Initialize();

            _dte = GetService(typeof(DTE)) as DTE2;
            _events = _dte.Events.BuildEvents;
            _events.OnBuildProjConfigDone += OnBuildDone;
            
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
        
            if (null != mcs)
            {
                CommandID fartCmd = new CommandID(GuidList.guidFarticusCmdSet, (int)PkgCmdIDList.cmdidRandomFart);
                MenuCommand menuItem = new MenuCommand(OnFartButtonClick, fartCmd);
                mcs.AddCommand(menuItem);
            }
        }

        private void OnBuildDone(string Project, string ProjectConfig, string Platform, string SolutionConfig, bool Success)
        {
            bool hasWarnings = _dte.ToolWindows.ErrorList.ErrorItems.Count > 0;
            FartOptions options = (FartOptions)GetDialogPage(typeof(FartOptions));

            if (!Success )
            {
                FartPlayer.PlayErrorFart(options);
            }
            else if (Success && hasWarnings)
            {
                FartPlayer.PlayWarningFart(options);
            }
        }

        private void OnFartButtonClick(object sender, EventArgs e)
        {
            FartPlayer.PlayRandomFart();

            Random rn = new Random(DateTime.Now.Millisecond);
            int index = rn.Next(0, _messages.Count);

            _dte.StatusBar.Text = _messages[index];
        }
    }
}