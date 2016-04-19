namespace ExempleMVVM.VueModeles
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;
    using ExempleMVVM.Modeles;
    using ExempleMVVM.Modules;

    /// <summary>
    /// Vue-modèle pour la vue vueConnexion
    /// </summary>
    internal class vmConnexion : ViewModelBase
    {
        #region Private Fields

        /// <summary>
        /// Variable privée de la propriété <see cref="ConnecterUtilisateur"/>
        /// </summary>
        private ICommand connecterUtilisateur;

        /// <summary>
        /// Profil de l'utilsateur à faire suivre à travers les vues-modèles de l'application pour
        /// avoir l'état de l'application
        /// </summary>
        private Profil profil;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="vmConnexion"/>
        /// </summary>
        /// <param name="profil">
        /// profil de l'utilsateur à faire suivre à travers les vues-modèles de l'application pour
        /// avoir l'état de l'application
        /// </param>
        public vmConnexion(Profil profil)
        {
            this.profil = profil;

            this.profil.PropertyChanged += this.Profil_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Obtient la commande permettant de connecter l'utilisateur à l'application. Théoriquement,
        /// vous devriez mettre le code permettant de voir si une autre utilisateur possède le même
        /// nom d'utilisateur.
        /// </summary>
        public ICommand ConnecterUtilisateur
        {
            get
            {
                if (this.connecterUtilisateur == null)
                {
                    this.connecterUtilisateur = new RelayCommand<object>(
                    (obj) =>
                    {
                        ProtocoleSJ.Connexion(profil);
                    },
                    (obj) =>
                    {
                        return !string.IsNullOrWhiteSpace(NomUtilisateur);
                    });
                }

                return this.connecterUtilisateur;
            }
        }

        /// <summary>
        /// Obtient ou définit le nom de l'utilisateur local pour se connecter
        /// </summary>
        public string NomUtilisateur
        {
            get
            {
                return this.profil.Nom;
            }

            set
            {
                if (this.profil.Nom != value)
                {
                    this.ValidationErrors.Clear();
                    this.profil.Nom = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Permet de vérifier le changement de propriété du profil. Si profil.ConnexionEnCours devient à faux et que profil.Connecte est toujours à faux, c'est qu'il y a eu
        /// un problème de connexion. Pour l'instant, le seul problème géré est un nom d'utilisateur déjà utilisé.
        /// </summary>
        /// <param name="sender">Envoyeur de l'évènement</param>
        /// <param name="e">Contient le nom de la propriété qui a changée</param>
        private void Profil_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConnexionEnCours")
            {
                if (!this.profil.ConnexionEnCours && !this.profil.Connecte)
                {
                    this.ValidationErrors.Clear();
                    this.ValidationErrors.Add("NomUtilisateur", new List<string>());
                    this.ValidationErrors["NomUtilisateur"].Add("Nom d'utilisateur déjà utilisé!");
                    this.NotifyErrorsChanged("NomUtilisateur");
                }
            }
        }

        #endregion Private Methods
    }
}