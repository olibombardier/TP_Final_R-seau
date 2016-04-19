namespace ExempleMVVM
{
    using System.Windows;
    using ExempleMVVM.VueModeles;

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Public Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Gère la fermeture de l'application pour fermer correctement l'ensemble des connexions
        /// </summary>
        /// <param name="sender">Celui qui a applelé l'évènement</param>
        /// <param name="e">Permet d'annuler la fermeture de l'application</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vmMainWindow vm = this.DataContext as vmMainWindow;
            if (vm.FermerApplication.CanExecute(null))
            {
                vm.FermerApplication.Execute(null);
            }
        }

        #endregion Private Methods
    }
}