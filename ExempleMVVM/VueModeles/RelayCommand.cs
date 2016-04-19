namespace ExempleMVVM.VueModeles
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Permet de créer une commande de type ICommand en ne spécifiant que les fonctions "Execute" et
    /// "CanExecute". On peut envoyer un paramètre à la commande.
    /// </summary>
    /// <typeparam name="T">Type du paramètre de la commande.</typeparam>
    internal class RelayCommand<T> : ICommand
    {
        #region Private Fields

        /// <summary>
        /// Contient la fonction permettant de savoir si on peut exécuter la commande ou non
        /// </summary>
        private readonly Predicate<T> canExecute = null;

        /// <summary>
        /// Contient la méthode permettant d'exécuter la commande
        /// </summary>
        private readonly Action<T> execute = null;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">
        /// Délègue à execute quand la méthode Execute est appelée sur la commande. Peut être null
        /// pour spécifier seulement la méthode CanExecute.
        /// </param>
        /// <remarks><seealso cref="CanExecute"/> va toujours être vrai.</remarks>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="RelayCommand{T}"/>.
        /// </summary>
        /// <param name="execute">La logique d'exécution.</param>
        /// <param name="canExecute">La logique du statut de l'exécution</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion Public Constructors

        #region Public Events

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this object
        /// can be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute((T)parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this object
        /// can be set to <see langword="null"/>.
        /// </param>
        public void Execute(object parameter)
        {
            this.execute((T)parameter);
        }

        #endregion Public Methods
    }
}