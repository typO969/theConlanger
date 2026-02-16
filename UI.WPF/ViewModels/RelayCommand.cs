using System;
using System.Windows.Input;

namespace UI.WPF.ViewModels;
public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _action;
    private readonly Func<object?,bool>? _can;
    public RelayCommand(Action<object?> action, Func<object?,bool>? can=null)
    { _action=action; _can=can; }
    public bool CanExecute(object? p) => _can?.Invoke(p) ?? true;
    public void Execute(object? p) => _action(p);
    public event EventHandler? CanExecuteChanged { add{} remove{} }
}
