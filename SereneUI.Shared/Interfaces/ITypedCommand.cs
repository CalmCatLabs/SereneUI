using System;

namespace SereneUI.Shared.Interfaces;

/// <summary>
/// A interface for typed commands. 
/// </summary>
/// <typeparam name="TArgs">Argument type for the command.</typeparam>
public interface ITypedCommand<in TArgs>
{
    /// <summary>
    /// Determines if the command can be executed.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="args">Argument for the command.</param>
    /// <returns>true when the command can be executed, otherwise false.</returns>
    bool CanExecute(object? sender, TArgs args);
    
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="sender">Sender Object.</param>
    /// <param name="args">Argument for the command.</param>
    void Execute(object? sender, TArgs args);
    
    /// <summary>
    /// Event that can be called, when can execute result might change.
    /// </summary>
    event EventHandler CanExecuteChanged;
}