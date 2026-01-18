using System;

namespace SereneUI.Shared.Interfaces;

public interface ICommand
{
    bool CanExecute(object? sender, object? args = null);
    void Execute(object? sender, object? args = null);
    event EventHandler CanExecuteChanged;
}
