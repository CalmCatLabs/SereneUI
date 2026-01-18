using System;

using SereneUI.Shared.Interfaces;

namespace SereneUI.Base;

/// <summary>
/// Typed RelayCommand.
/// </summary>
/// <param name="execute">Action to execute</param>
/// <param name="canExecute">Func to check if execution is possible.</param>
/// <typeparam name="TArgs">Command argument type.</typeparam>
public class RelayCommand<TArgs>(Action<object?, TArgs?> execute, Func<object?, TArgs?, bool> canExecute)
    : ITypedCommand<TArgs>
{
    private readonly Action<object?, TArgs?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
    private readonly Func<object?, TArgs?, bool>? _canExecute = canExecute;
    
    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;
    
    /// <inheritdoc />
    public bool CanExecute(object? sender, TArgs? args)
        => _canExecute?.Invoke(sender, args) ?? true;
    
    /// <inheritdoc />
    public void Execute(object? sender, TArgs? args)
    {
        if (!CanExecute(sender, args)) return;
        _execute.Invoke(sender, args);
    }
    
    /// <summary>
    /// Raise a possibility that can execute result could change.
    /// </summary>
    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// RelayCommand.
/// </summary>
/// <param name="execute">Action to execute</param>
/// <param name="canExecute">Func to check if execution is possible.</param>
public class RelayCommand(Action<object?, object?> execute, Func<object?, object?, bool>? canExecute = null)
    : ICommand
{
    private readonly Action<object?, object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;
    
    /// <inheritdoc />
    public bool CanExecute(object? sender, object? args = null)
        => canExecute?.Invoke(sender, args) ?? true;
    
    /// <inheritdoc />
    public void Execute(object? sender, object? args = null)
    {
        if (!CanExecute(sender, args)) return;
        _execute.Invoke(sender, args);
    }
    
    /// <summary>
    /// Raise a possibility that can execute result could change.
    /// </summary>
    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}