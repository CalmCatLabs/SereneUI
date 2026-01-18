using System;

namespace SereneUI.Shared.Interfaces;

public interface ITypedCommand<in TArgs>
{
    bool CanExecute(object? sender, TArgs args);
    void Execute(object? sender, TArgs args);
    event EventHandler CanExecuteChanged;
}