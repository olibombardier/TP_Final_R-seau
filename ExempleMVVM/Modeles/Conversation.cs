namespace ExempleMVVM.Modeles
{
    using System.Collections.ObjectModel;
    using System.Net.Sockets;

    /// <summary>
    /// Permet de gérer une conversation. Une conversation peut être la conversation globale (avec
    /// tous les utilisateurs) ou une conversation privée (avec un utilisateur seulement).
    /// </summary>
    public class Conversation : ModelBase
    {
        #region Private Fields

        /// <summary>
        /// Variable privée de la propriété <see cref="Connecte"/>
        /// </summary>
        private bool connecte = false;

        /// <summary>
        /// Variable privée de la propriété <see cref="EstGlobale"/>
        /// </summary>
        private bool estGlobale;

        /// <summary>
        /// Variable privée de la propriété <see cref="IV"/>
        /// </summary>
        private byte[] iv;

        /// <summary>
        /// Variable privée de la propriété <see cref="Key"/>
        /// </summary>
        private byte[] key;

        /// <summary>
        /// Variable privée de la propriété <see cref="Lignes"/>
        /// </summary>
        private ObservableCollection<LigneConversation> lignes;

        /// <summary>
        /// Variable privée de la propriété <see cref="Socket"/>
        /// </summary>
        private Socket socket;

        /// <summary>
        /// Variable privée de la propriété <see cref="Utilisateur"/>
        /// </summary>
        private Utilisateur utilisateur;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la connexion est établie avec l'utilisateur
        /// distant (pour les conversations privée) (à vrai si conversation globale)
        /// </summary>
        public bool Connecte
        {
            get
            {
                return this.EstGlobale || this.connecte;
            }

            set
            {
                if (this.connecte != value)
                {
                    this.connecte = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la conversation est globale, c'est-à-dire, à
        /// tous les usagers du système
        /// </summary>
        public bool EstGlobale
        {
            get
            {
                return this.estGlobale;
            }

            set
            {
                if (this.estGlobale != value)
                {
                    this.estGlobale = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si la conversation est privée, c'est-à-dire, liée à un seul utilisateur.
        /// </summary>
        public bool EstPrivee
        {
            get
            {
                return !this.EstGlobale;
            }
        }

        /// <summary>
        /// Obtient ou définit la variable IV pour le cryptage en AES 128 pour les conversations privées
        /// </summary>
        public byte[] IV
        {
            get { return this.iv; }
            set { this.iv = value; }
        }

        /// <summary>
        /// Obtient ou définit la variable Key pour le cryptage en AES 128 pour les conversations privées
        /// </summary>
        public byte[] Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// Obtient ou définit la liste des lignes de la conversation contenant le nom d'utilisateur,
        /// l'adresse IP et le message.
        /// </summary>
        public ObservableCollection<LigneConversation> Lignes
        {
            get
            {
                if (this.lignes == null)
                {
                    this.lignes = new ObservableCollection<LigneConversation>();
                }

                return this.lignes;
            }

            set
            {
                if (this.lignes != value)
                {
                    this.lignes = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le Socket associé à la conversation
        /// </summary>
        public Socket Socket
        {
            get { return this.socket; }
            set { this.socket = value; }
        }

        /// <summary>
        /// Obtient ou définit l'utilisateur auquel la conversation est liée. Utile pour une
        /// conversation privée. Dans le cas d'une conversation globale, on peut mettre un
        /// utilisateur bidon pour avoir le nom de l'utilisateur de la conversation.
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