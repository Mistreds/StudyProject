using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StudyProject
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие изменения свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// <see cref="CallerMemberNameAttribute"/> автоматически определяет имя вызываемой переменной.
        /// </summary>
        /// <param name="propertyName">Имя переменной.</param>
        ///
        ///
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class RelayCommand : ICommand
    {
        #region Private Members
        private Action mAction;
        #endregion

        #region Public events
        public event EventHandler CanExecuteChanged = (sender, e) => { };
        #endregion

        #region Conctructor
        public RelayCommand(Action action)
        {
            mAction = action;
        }
        #endregion

        #region Command methods
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            mAction();
        }
        #endregion
    }
    public class RelaysCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelaysCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}



    