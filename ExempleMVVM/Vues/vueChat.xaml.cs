﻿namespace ExempleMVVM.Vues
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using ExempleMVVM.VueModeles;

    /// <summary>
    /// Logique d'interaction pour vueChat.xaml
    /// </summary>
    public partial class vueChat : UserControl
    {
        #region Public Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="vueChat"/>
        /// </summary>
        public vueChat()
        {
            this.InitializeComponent();
        }

        #endregion Public Constructors

        #region Private Methods

        /// <summary>
        /// Permet de gérer l'appuie du DoubleClick sur un nom d'utilisateur pour ouvrir une
        /// conversation avec cet utilisateur.
        /// </summary>
        /// <param name="sender">Celui qui a appelé l'évènement</param>
        /// <param name="e">Contient les informations sur le clique effectué</param>
        private void LvUtilisateursConnectesItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            vmChat vm = DataContext as vmChat;
            if (vm.OuvrirConversation.CanExecute(lvUtilisateursConnectes.SelectedItem))
            {
                vm.OuvrirConversation.Execute(lvUtilisateursConnectes.SelectedItem);
            }
        }

        /// <summary>
        /// Permet de gérer l'appuie de la touche "Enter" sur le TextBox de message à envoyer pour
        /// appeler la commande d'envoie de message
        /// </summary>
        /// <param name="sender">Celui qui a appelé l'évènement</param>
        /// <param name="e">Contient les informations sur la touche appuyée</param>
        private void TxtMessageAEnvoyer_KeyDown(object sender, KeyEventArgs e)
        {
            vmChat vm = DataContext as vmChat;
            if (e.Key == Key.Enter && vm.EnvoyerMessage.CanExecute(null))
            {
                vm.EnvoyerMessage.Execute(null);
            }
        }

        #endregion Private Methods
    }
}