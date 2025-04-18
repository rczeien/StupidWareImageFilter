namespace StupidWareGames.CrazyFilter;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia;
using System.Collections.Generic;
using System.Linq;
using StupidWareGames.CrazyFilter.Interfaces;
using StupidWareGames.CrazyFilter.Utils;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using OpenCvSharp;

public partial class MainWindow : Avalonia.Controls.Window
{
    private Dictionary<string, List<FilterAsset>> _filtersByPart = new();
    private DirectoryWatcher _watcher;
    private readonly Dictionary<string, string> _activeSelections = new();
    private int _currentCameraIndex = -1;
    private VideoCapture? _capture;
    private CancellationTokenSource? _cameraTokenSource;
    private WriteableBitmap? _cameraBitmap;


    public MainWindow()
    {
        InitializeComponent();
        _watcher = new DirectoryWatcher("Assets");
        _watcher.FiltersUpdated += () =>
        {
            LoadFilters();
            RenderFilterDropdowns();
        };
        _watcher.Start();
        LoadFilters();
        RenderFilterDropdowns();
        LoadCameraList();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadFilters()
    {
        var loader = new FilterLoader("Assets");
        var allFilters = loader.LoadAllFilters();

        _filtersByPart = allFilters
            .GroupBy(f => f.TargetPart.Name)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private void OnCameraSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox cb && cb.SelectedItem is string selectedCamera)
        {
            StartCamera(selectedCamera);
        }
    }

    private void StartCamera(string cameraName)
    {
        StopCamera(); // Stop previous camera stream

        if (string.IsNullOrWhiteSpace(cameraName)) return;

        var parts = cameraName.Split(' ');
        if (parts.Length == 2 && int.TryParse(parts[1], out var index))
        {
            _currentCameraIndex = index;
            _capture = new VideoCapture(index);
            _cameraTokenSource = new CancellationTokenSource();
            Task.Run(() => CameraLoop(_cameraTokenSource.Token));
        }
    }

    private void StopCamera()
    {
        _cameraTokenSource?.Cancel();
        _capture?.Release();
        _capture?.Dispose();
        _capture = null;
    }

    private static Avalonia.Media.Imaging.Bitmap BitmapFromMat(Mat mat)
    {
        using var ms = mat.ToMemoryStream(); // OpenCvSharp extension
        return new Avalonia.Media.Imaging.Bitmap(ms);
    }

    private async Task CameraLoop(CancellationToken token)
    {
        var imageControl = this.FindControl<Image>("ImageDisplay");

        while (!token.IsCancellationRequested && _capture != null && _capture.IsOpened())
        {
            using var mat = new Mat();
            if (!_capture.Read(mat) || mat.Empty())
                continue;

            var bitmap = BitmapFromMat(mat);

            // Update UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                imageControl.Source = bitmap;
            });


            await Task.Delay(33, token); // ~30 FPS
        }
    }

    private void LoadCameraList()
    {
        var cameras = CameraService.ListAvailableCameras();
        var selector = this.FindControl<ComboBox>("CameraSelector");

        selector.ItemsSource = cameras;

        if (cameras.Count > 0)
        {
            selector.SelectedIndex = 0;
            StartCamera(cameras[0]);
        }
    }

    private void RenderFilterDropdowns()
    {
        var panel = this.FindControl<StackPanel>("FilterDropdownPanel");
        if (panel is null) return;
        panel.Children.Clear();

        var ordered = _filtersByPart
            .Select(kvp => new
            {
                Part = kvp.Value.First().TargetPart,
                Filters = kvp.Value
            })
            .OrderBy(x => x.Part.SortOrder)
            .ThenBy(x => x.Part.Name);

        foreach (var entry in ordered)
        {
            var label = new TextBlock
            {
                Text = entry.Part.Name,
                Margin = new Thickness(0, 10, 0, 5)
            };

            var comboBox = new ComboBox
            {
                Width = 200,
                Margin = new Thickness(0, 0, 0, 10),
                Tag = entry.Part
            };

            var items = new List<FilterChoice>
            {
                new FilterChoice { DisplayName = "None", Value = "" }
            };

            items.AddRange(entry.Filters.Select(f => new FilterChoice
            {
                DisplayName = f.Name.SplitCamelCase(),
                Value = f.Name
            }));

            comboBox.ItemsSource = items;

            // Try to preserve previous selection or fall back to "None"
            string selectedValue = _activeSelections.TryGetValue(entry.Part.Name, out var val) ? val : "";
            comboBox.SelectedItem = items.FirstOrDefault(i => i.Value == selectedValue)
                                ?? items.First(); // fallback to "None"

            comboBox.SelectionChanged += OnFilterSelected;

            panel.Children.Add(label);
            panel.Children.Add(comboBox);
        }
    }

    private void OnFilterSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox cb && cb.SelectedItem is FilterChoice choice && cb.Tag is IBodyPart part)
        {
            if (string.IsNullOrEmpty(choice.Value))
            {
                _activeSelections[part.Name] = ""; // None
                System.Diagnostics.Debug.WriteLine($"Cleared filter for {part.Name}");
                return;
            }

            _activeSelections[part.Name] = choice.Value;

            var selected = _filtersByPart[part.Name].FirstOrDefault(f => f.Name == choice.Value);
            if (selected != null)
            {
                System.Diagnostics.Debug.WriteLine($"Selected {selected.Name} for {part.Name}");
                // TODO: apply or store filter
            }
        }
    }


    private class FilterChoice
    {
        public string DisplayName { get; set; } = "";
        public string Value { get; set; } = "";
        public override string ToString() => DisplayName;
    }
}
