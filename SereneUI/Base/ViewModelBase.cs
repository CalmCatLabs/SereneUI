using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SereneUI.Base;

/// <summary>
/// Base class for view models. 
/// </summary>
public class ObservableObject : INotifyPropertyChanged
{
    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
    
    /// <summary>
    /// Raises PropertyChanged for a property name.
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
    /// <summary>
    /// Sets a backing field and notifies if the value actually changed.
    /// </summary>
    protected bool SetProperty<T>(
        ref T field,
        T value,
        Action? onChanged = null,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class ViewModelBase : ObservableObject
{
}