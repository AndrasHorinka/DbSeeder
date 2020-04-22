using System;
using System.Windows.Input;

namespace DbSeeder.WPF.Services
{
    /// <summary>
    /// A basic command that runs an Action
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private members

        /// <summary>
        /// The Action to run
        /// </summary>
        private Action _Action;

        #endregion

        #region Public Events 

        /// <summary>
        /// The event thats fired when <see cref=CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="action"></param>
        public RelayCommand(Action action)
        {
            _Action = action;
        }

        #endregion

        #region Command methods

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Execute the action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _Action();
        }

        #endregion
    }
}
