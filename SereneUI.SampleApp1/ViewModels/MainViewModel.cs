using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SereneUI.Base;
using SereneUI.Shared.Enums;
using SereneUI.Shared.Interfaces;

namespace SereneUI.SampleApp1.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int count = 0;

    public string Title
    {
        get;
        set => SetProperty(ref field, value);
    } = "SereneUI Sample App";
    
    public string Exit
    {
        get;
        set => SetProperty(ref field, value);
    } = "Exit Game";
    
    public string TestButtonText
    {
        get;
        set => SetProperty(ref field, value);
    } = "click me";

    public string LastDragEvent
    {
        get;
        set => SetProperty(ref field, value);
    } = "none";

    public bool RequestExit
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public HorizontalAlignment CurrentHAlignment { get; set => SetProperty(ref field, value); } = HorizontalAlignment.Right;

    public RelayCommand ClickCommand { get; set; }
    public RelayCommand ExitClickCommand { get; set; }
    public RelayCommand MouseDownCommand { get; set; }
    public RelayCommand MouseUpCommand { get; set; }
    public RelayCommand MouseLeaveCommand { get; set; }
    
    public Color ClearColor { get; set; } = Color.Aquamarine;

    private Random _random = new Random();

    public MainViewModel()
    {
        ClickCommand = new RelayCommand((sender, args) =>
        {
            TestButtonText = $"count: {count++}";
            ClearColor = new Color(_random.Next(0, 255), _random.Next(0, 255), _random.Next(0, 255));
        });
        
        ExitClickCommand = new RelayCommand((sender, args) =>
        {
            RequestExit = true;
        });

        MouseDownCommand = new RelayCommand((sender, args) =>
        {
            LastDragEvent = nameof(MouseDownCommand);
        });
        
        MouseUpCommand = new RelayCommand((sender, args) =>
        {
            LastDragEvent = nameof(MouseUpCommand);
        });
        
        MouseLeaveCommand = new RelayCommand((sender, args) =>
        {
            LastDragEvent = nameof(MouseLeaveCommand);
        });
    }
}