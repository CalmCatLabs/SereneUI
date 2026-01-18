using System;
using SereneUI.Shared.Interfaces;

namespace SereneUI.Base;

public class RelayTypedCommand<TArgs> : ITypedCommand<TArgs>
{
    private readonly Action<object?, TArgs?> _execute;
    private readonly Func<object?, TArgs?, bool>? _canExecute;

    public event EventHandler? CanExecuteChanged;

    public RelayTypedCommand(Action<object?, TArgs?> execute, Func<object?, TArgs?, bool> canExecute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    
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