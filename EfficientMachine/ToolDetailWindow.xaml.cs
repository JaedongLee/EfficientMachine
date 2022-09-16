using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using EfficientMachine.Entity;

namespace EfficientMachine
{
    /// <summary>
    ///     Interaction logic for ToolDetailWindow.xaml
    /// </summary>
    public partial class ToolDetailWindow
    {
        public ToolDetailWindow()
        {
            InitializeComponent();
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dataContext = (Dictionary<string, object>)DataContext;
            var tool = (Tool)dataContext["tool"];
            var programPath = (string)dataContext["programPath"];
            var releaseType = tool.ReleaseType;
            var mainProgramLocation = tool.MainProgramLocation;
            if (ReleaseType.Installer.ToString().Equals(releaseType) ||
                ReleaseType.Portable.ToString().Equals(releaseType))
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = $"{programPath}\\{mainProgramLocation}";
                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception)
                {
                    Console.WriteLine(startInfo.FileName);
                    throw;
                }
            }
            else
            {
                Process.Start(programPath);
            }
        }
    }
}