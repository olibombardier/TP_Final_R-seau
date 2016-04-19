namespace ExempleMVVM.Modeles
{
    /// <summary>
    /// Spécifie une ligne dans une conversation. Il y a une ligne par message envoyé ou reçu.
    /// </summary>
    public class LigneConversation : ModelBase
    {
        #region Private Fields

        /// <summary>
        /// Variable privée de la propriété <see cref="Message"/>
        /// </summary>
        private string message;

        /// <summary>
        /// Variable privée de la propriété <see cref="Utilisateur"/>
        /// </summary>
        private Utilisateur utilisateur;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Obtient ou définit le message que l'utilisateur a envoyé
        /// </summary>
        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l'utilisateur qui a envoyé le message.
        /// </summary>
        public Utilisateur Utilisateur
        {
            get
            {
                return this.utilisateur;
            }

            set
            {
                if (this.utilisateur != value)
                {
                    this.utilisateur = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        #endregion Public Properties
    }
}