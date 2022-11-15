using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EfficientMachine.Entity;
using EfficientMachine.Enumeration;
using log4net;
using log4net.Config;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using SqlSugar;

namespace EfficientMachine
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindow));

        private static SqlSugarClient _dbClient;

        private static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;

        private IEnumerable<Tool> _tools;

        public MainWindow()
        {
            InitializeComponent();
            XmlConfigurator.Configure();
            var dbPath = Path.Combine(AppPath, @"Resources\Tools\Database\EfficientMachine.db");
            Log.Info("dpPath: " + dbPath);
            var connectString = $"Data Source={dbPath};";
            _dbClient = new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = connectString,
                DbType = DbType.Sqlite,
                LanguageType = LanguageType.English,
                IsAutoCloseConnection = true
            });
            _dbClient.Aop.OnLogExecuting = (sql, pars) => { Console.WriteLine(sql); };
            try
            {
                var tools = ListAllUsableTools();
                _tools = tools;
                BuildToolCards(tools);
            }
            catch (Exception e)
            {
                Log.Error("init error", e);
                throw;
            }
        }

        private void ListBox_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            var listBox = sender as ListBox;
            if (null == listBox) return;
            var selectedItems = listBox.SelectedItems.Cast<ListBoxItem>().ToList();
            List<Tool> tools;
            if (selectedItems.Count <= 0)
            {
                tools = ListAllUsableTools();
            }
            else
            {
                var selectedTags = selectedItems.AsEnumerable().Select(x => x.Content).ToList();
                tools = ListToolsByTagValues(selectedTags);
            }

            _tools = tools;
            BuildToolCards(tools);
        }

        private void BuildToolCards(IEnumerable<Tool> tools)
        {
            ToolsPanel.Children.Clear();
            foreach (var tool in tools)
            {
                var card = new Card
                {
                    Width = 169,
                    Height = 169,
                    Cursor = Cursors.Hand,
                    Margin = new Thickness(8, 8, 8, 8),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e6e6e6"))
                };
                card.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(ToolCard_OnMouseLeftButtonUp));
                var grid = new Grid();
                var rowDefinition1 = new RowDefinition
                {
                    Height = new GridLength(140)
                };
                var rowDefinition2 = new RowDefinition();
                grid.RowDefinitions.Add(rowDefinition1);
                grid.RowDefinitions.Add(rowDefinition2);
                var imageUri = ImageUtil.GetOrDefaultImageUri(tool.Name);
                var image = new Image
                {
                    Height = 140,
                    Source = new BitmapImage(imageUri),
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(1, 3, 1, 3)
                };
                grid.Children.Add(image);
                var textBlock = new TextBlock
                {
                    TextAlignment = TextAlignment.Center,
                    Text = tool.Name
                };
                textBlock.SetValue(Grid.RowProperty, 1);
                grid.Children.Add(textBlock);
                card.Content = grid;
                ToolsPanel.Children.Add(card);
            }
        }

        private static List<Tool> ListToolsByTagValues(IEnumerable<object> selectedTags)
        {
            return _dbClient.Queryable<Tool>()
                .Distinct()
                .LeftJoin<ToolTag>((t, tt) => t.Id == tt.ToolId)
                .Where((t, tt) => selectedTags.Contains(tt.Value) && t.Status == ToolStatusEnum.Usable.ToString())
                .ToList();
        }

        private static List<Tool> ListAllUsableTools()
        {
            var toolSourceTypes = new List<string>
            {
                ToolSourceTypeEnum.Github.ToString(),
                ToolSourceTypeEnum.IntellijPlatform.ToString()
            };
            return _dbClient.Queryable<Tool>().LeftJoin<ToolSource>((t, ts) => t.Id == ts.ToolId)
                .Where((t, ts) => t.Status == ToolStatusEnum.Usable.ToString() && toolSourceTypes.Contains(ts.Type))
                .ToList();
        }

        private void ToolCard_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var briefCard = (Card)sender;
            var textBlock = briefCard.FindChild<TextBlock>();
            if (textBlock == null) return;
            var toolName = textBlock.Text;
            var tool = _tools.AsEnumerable().First(x => toolName.Equals(x.Name));
            var programPath = Path.Combine(AppPath, $"Resources\\Tools\\Program\\{toolName}");
            var toolDetailWindow = new ToolDetailWindow();
            var dataContext = new Dictionary<string, object>
            {
                { "tool", tool },
                { "programPath", programPath }
            };
            toolDetailWindow.DataContext = dataContext;
            var imageUri = ImageUtil.GetOrDefaultImageUri(tool.Name);
            toolDetailWindow.ToolImage.Source = new BitmapImage(imageUri);
            toolDetailWindow.NameTextBlock.Text = tool.Name;
            toolDetailWindow.RuntimeEnvironmentTextBlock.Text = tool.RuntimeEnvironment;
            toolDetailWindow.ReleaseTypeTextBlock.Text = tool.ReleaseType;
            toolDetailWindow.DescriptionTextBlock.Text = tool.Description;
            toolDetailWindow.Show();
            if (ReleaseTypeEnum.Installer.ToString().Equals(tool.ReleaseType))
                toolDetailWindow.StartButton.Content = "安装";
            else if (ReleaseTypeEnum.Portable.ToString().Equals(tool.ReleaseType))
                toolDetailWindow.StartButton.Content = "运行";
            else
                toolDetailWindow.StartButton.Content = "目录";
        }
    }
}