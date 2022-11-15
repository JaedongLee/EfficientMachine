using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using EfficientMachine.Entity;
using EfficientMachine.Enumeration;

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
            if (ReleaseTypeEnum.Installer.ToString().Equals(releaseType) ||
                ReleaseTypeEnum.Portable.ToString().Equals(releaseType))
            {
                var startInfo = new ProcessStartInfo();
                var fileName = tool.FileName;
                var fileExtensionName = tool.FileExtensionName;
                if (fileExtensionName == FileExtensionNameEnum.exe.ToString())
                    startInfo.FileName = $"{programPath}\\{fileName}.{fileExtensionName}";
                else if (fileExtensionName == FileExtensionNameEnum.zip.ToString())
                    startInfo.FileName = $"{programPath}\\{fileName}\\{mainProgramLocation}";
                else
                    throw new Exception($"没有对应的文件后缀: {fileExtensionName}. 工具名: {tool.Name}");
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