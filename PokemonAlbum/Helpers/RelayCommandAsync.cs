using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PokemonAlbum.Helpers
{
    public class RelayCommandAsync(Func<object?, Task> executeAction, Predicate<object?> canExecutePredicate) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private Func<object?, Task> executeAction = executeAction;
        private Predicate<object?> canExecutePredicate = canExecutePredicate;

        public bool CanExecute(object? parameter) => canExecutePredicate(parameter);
        public void Execute(object? parameter) => executeAction(parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
