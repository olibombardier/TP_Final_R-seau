namespace ExempleMVVM.Vues
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using ExempleMVVM.VueModeles;

    /// <summary>
    /// Logique d'interaction pour vueConnexion.xaml
    /// </summary>
    public partial class vueConnexion : UserControl
    {
        #region Public Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="vueConnexion"/>
        /// </summary>
        public vueConnexion()
        {
            this.InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Permet de gérer l'appuie de la touche "Enter" sur le textbox du nom d'utilisateur pour
        /// activer la commande de connexion
        /// </summary>
        /// <param name="sender">Celui qui a appelé l'évènement</param>
        /// <param name="e">Contient les informations sur la touche appuyée</param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            vmConnexion vm = (vmConnexion)DataContext;
            if (e.Key == Key.Enter)
            {
                if (vm.ConnecterUtilisateur.CanExecute(null))
                {
                    vm.ConnecterUtilisateur.Execute(null);
                }
            }
        }

        #endregion Private Methods
    }
}