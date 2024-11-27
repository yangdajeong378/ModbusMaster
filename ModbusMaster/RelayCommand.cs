using System.Windows.Input;

//제네릭 매개변수를 받는 릴레이 커맨드
public class RelayCommand<T> : ICommand
{
    private readonly Action<T>? _execute;
    private readonly Predicate<T>? _canExecute;
    public RelayCommand(Action<T>? execute, Predicate<T>? canExecute = null)
    {
        this._execute = execute;
        this._canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke((T)parameter!) ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute?.Invoke((T)parameter!);
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}






//매개변수를 받지 않는 릴레이 커맨드

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public void Execute(object? parameter)
    {
        _execute();
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}
