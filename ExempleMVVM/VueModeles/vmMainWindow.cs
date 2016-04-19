namespace ExempleMVVM.VueModeles
{
    using System.ComponentModel;
    using System.Windows.Input;
    using ExempleMVVM.Modeles;
    using ExempleMVVM.Modules;

    /// <summary>
    /// Vue-modèle pour la vue principale
    /// </summary>
    internal class vmMainWindow : ViewModelBase
    {
        #region Private Fields

        /// <summary>
        /// Variable privée de la propriété <see cref="CurrentViewModel"/>
        /// </summary>
        private ViewModelBase currentViewModel;

        /// <summary>
        /// Variable privée de la propriété <see cref="FermerApplication"/>
        /// </summary>
        private ICommand fermerApplication;

        /// <summary>
        /// Profil de l'utilsateur à faire suivre à travers les vues-modèles de l'application pour
        /// avoir l'état de l'application
        /// </summary>
        private Profil profil;

        /// <summary>
        /// Contient le vue-modèle associé à l'interface de clavardage
        /// </summary>
        private vmChat vmChat;

        /// <summary>
        /// Contient le vue-modèle associé à l'interface de demande de connexion
        /// </summary>
        private vmConnexion vmConnexion;

        /// <summary>
        /// Contient le vue-modèle associé à l'interface d'attente de connexion en cours
        /// </summary>
        private vmConnexionEnCours vmConnexionEnCours;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="vmMainWindow"/>. Initialise les
        /// vues-modèles de l'application et spécifie le premier vue-modèle à afficher
        /// </summary>
        public vmMainWindow()
        {
            this.profil = new Profil();
            this.vmConnexion = new vmConnexion(this.profil);
            this.vmChat = new vmChat(this.profil);
            this.vmConnexionEnCours = new vmConnexionEnCours();

            this.CurrentViewModel = this.vmConnexion;

            this.profil.PropertyChanged += this.Profil_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Obtient ou définit le vue-modèle présentement affiché dans la vue principale (les
        /// vues-modèles sont liés à leur vue dans le fichier App.xml)
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get
            {
                return this.currentViewModel;
            }

            set
            {
                if (this.currentViewModel != value)
                {
                    this.currentViewModel = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la commande permettant de bien fermer les connexions en cours avant de fermer l'application (UDP et TCP)
        /// </summary>
        public ICommand FermerApplication
        {
            get
            {
                if (this.fermerApplication == null)
                {
                    this.fermerApplication = new RelayCommand<object>(
                        (obj) =>
                        {
                            ProtocoleSJ.Deconnexion();
                        },
                        (obj) =>
                        {
                            return true;
                        });
                }

                return this.fermerApplication;
            }
        }

        /// <summary>
        /// Obtient le titre de l'application pour y mettre le nom de l'utilisateur
        /// </summary>
        public string Titre
        {
            get
            {
                if (this.profil == null || this.profil.Nom.Equals(string.Empty))
                {
                    return "ExempleMVVM";
                }
                else
                {
                    return "ExempleMVVM @ " + this.profil.Nom;
                }
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Permet de changer d'interface lorsque l'utilisateur est connecté
        /// </summary>
        /// <param name="sender">Celui qui a appelé l'évènement</param>
        /// <param name="e">Contient le nom de la propriété qui a changée</param>
        private void Profil_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Connecte")
            {
                if (this.profil.Connecte)
                {
                    this.CurrentViewModel = this.vmChat;
                }
                else
                {
                    this.CurrentViewModel = this.vmConnexion;
                }
            }
            else if (e.PropertyName == "ConnexionEnCours")
            {
                if (this.profil.ConnexionEnCours)
                {
                    this.CurrentViewModel = this.vmConnexionEnCours;
                }
                else if (!this.profil.Connecte)
                {
                    this.CurrentViewModel = this.vmConnexion;
                }
            }
            else if (e.PropertyName == "Nom")
            {
                this.NotifyPropertyChanged("Titre");
            }
        }

        #endregion Private Methods
    }
}