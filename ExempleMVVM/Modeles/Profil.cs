namespace ExempleMVVM.Modeles
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Profil de l'utilisateur local.
    /// </summary>
    public class Profil : ModelBase
    {
        #region Private Fields

        /// <summary>
        /// Variable privée de la propriété <see cref="Connecte"/>
        /// </summary>
        private bool connecte = false;

        /// <summary>
        /// Variable privée de la propriété <see cref="ConnexionEnCours"/>
        /// </summary>
        private bool connexionEnCours = false;

        /// <summary>
        /// Variable privée de la propriété <see cref="Conversations"/>
        /// </summary>
        private ObservableCollection<Conversation> conversations;

        /// <summary>
        /// Variable privée de la propriété <see cref="Nom"/>
        /// </summary>
        private string nom = string.Empty;

        /// <summary>
        /// Variable privée de la propriété <see cref="UtilisateurLocal"/>
        /// </summary>
        private Utilisateur utilisateurLocal;

        /// <summary>
        /// Variable privée de la propriété <see cref="UtilisateurConnectes"/>
        /// </summary>
        private ObservableCollection<Utilisateur> utilisateursConnectes;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'utilisateur local a réussi à se connecter.
        /// </summary>
        public bool Connecte
        {
            get
            {
                return this.connecte;
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
        /// Obtient ou définit une valeur indiquant si le processus de connexion est en cours
        /// </summary>
        public bool ConnexionEnCours
        {
            get
            {
                return this.connexionEnCours;
            }

            set
            {
                if (this.connexionEnCours != value)
                {
                    this.connexionEnCours = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la liste des conversations en cours. Une conversation globale existe
        /// par défaut pour parler à tous les utilisateurs connectés. Les autres conversations seront
        /// de type privé.
        /// </summary>
        public ObservableCollection<Conversation> Conversations
        {
            get
            {
                if (this.conversations == null)
                {
                    this.conversations = new ObservableCollection<Conversation>();
                    this.conversations.Add(new Conversation()
                    {
                        Connecte = true,
                        EstGlobale = true,
                        Utilisateur = new Utilisateur()
                        {
                            Nom = "Global"
                        }
                    });
                }

                return this.conversations;
            }

            set
            {
                if (this.conversations != value)
                {
                    this.conversations = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le nom de l'utilisateur local.
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
                    this.utilisateurLocal = new Utilisateur()
                    {
                        Nom = this.Nom,
                        IP = "localhost"
                    };
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l'objet de type Utilisateur représentant l'utilisateur local. Utile
        /// pour ajouter une ligne dans une conversation envoyé par l'utilisateur local.
        /// </summary>
        public Utilisateur UtilisateurLocal
        {
            get
            {
                if (this.utilisateurLocal == null)
                {
                    this.utilisateurLocal = new Utilisateur()
                    {
                        Nom = this.Nom,
                        IP = "localhost"
                    };
                }

                return this.utilisateurLocal;
            }

            set
            {
                if (this.utilisateurLocal != value)
                {
                    this.utilisateurLocal = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la liste des utilisateurs connectées qui utilise présentement cette
        /// application sur le réseau.
        /// </summary>
        public ObservableCollection<Utilisateur> UtilisateursConnectes
        {
            get
            {
                if (this.utilisateursConnectes == null)
                {
                    this.utilisateursConnectes = new ObservableCollection<Utilisateur>();
                }

                return this.utilisateursConnectes;
            }

            set
            {
                if (this.utilisateursConnectes != value)
                {
                    this.utilisateursConnectes = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        #endregion Public Properties
    }
}