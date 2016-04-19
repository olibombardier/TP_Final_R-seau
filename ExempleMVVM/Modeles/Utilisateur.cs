namespace ExempleMVVM.Modeles
{
    /// <summary>
    /// Utilisateur du système. Soit utilisateur local ou utilisateurs connectés utilisant cette application.
    /// </summary>
    public class Utilisateur : ModelBase
    {
        #region Private Fields

        /// <summary>
        /// Variable privée de la propriété <see cref="IP"/>
        /// </summary>
        private string ip;

        /// <summary>
        /// Variable privée de la propriété <see cref="Nom"/>
        /// </summary>
        private string nom;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Obtient ou définit l'adresse IP permettant d'envoyer un message à cet utilisateur.
        /// </summary>
        public string IP
        {
            get
            {
                return this.ip;
            }

            set
            {
                if (this.ip != value)
                {
                    this.ip = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le nom d'utilisateur
        /// </summary>
        public string Nom
        {
            get
            {
                return this.nom;
            }

            set
            {
                if (this.nom != value)
                {
                    this.nom = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        #endregion Public Properties
    }
}