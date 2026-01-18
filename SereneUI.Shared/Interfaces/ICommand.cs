using System;

namespace SereneUI.Shared.Interfaces;

/// <summary>
/// Interface for ui commands. 
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Method stub for the can execute method.
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="args">Arguments object</param>
    /// <returns>true if the command can be executed, false otherwise.</returns>
    bool CanExecute(object? sender, object? args = null);
    
    /// <summary>
    /// Executes the command. 
    /// </summary>
    /// <param name="sender">Sender object.</param>
    /// <param name="args">Arguments object</param>
    void Execute(object? sender, object? args = null);
    
    /// <summary>
    /// Event can be triggered if can execute might have changed. 
    /// </summary>
    event EventHandler CanExecuteChanged;
}
