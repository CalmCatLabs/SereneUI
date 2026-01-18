using System;
using Serene.Common.Interfaces;
using SereneUI.Shared.Interfaces;

namespace SereneUI.Base;

public class RelayCommand<TArgs>(Action<object?, TArgs?> execute, Func<object?, TArgs?, bool> canExecute)
    : ICommand<TArgs>
{
    private readonly Action<object?, TArgs?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly Func<object?, TArgs?, bool>? _canExecute = canExecute;
    
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? sender, TArgs? args)
        => _canExecute?.Invoke(sender, args) ?? true;

    public void Execute(object? sender, TArgs? args)
    {
        if (!CanExecute(sender, args)) return;
        _execute.Invoke(sender, args);
    }

    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public class RelayCommand(Action<object?, object?> execute, Func<object?, object?, bool>? canExecute = null)
    : ICommand
{
    private readonly Action<object?, object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? sender, object? args = null)
        => canExecute?.Invoke(sender, args) ?? true;

    public void Execute(object? sender, object? args = null)
    {
        if (!CanExecute(sender, args)) return;
        _execute.Invoke(sender, args);
    }

    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}