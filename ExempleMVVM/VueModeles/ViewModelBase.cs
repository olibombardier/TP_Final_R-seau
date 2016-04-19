namespace ExempleMVVM.VueModeles
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Classe mère des vues-modèles du système.
    /// </summary>
    internal class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region Protected Fields

        /// <summary>
        /// Dictionnaire contenant les erreurs de validation. La clé est égal au nom de la propriété
        /// contenant des erreurs en string. La valeur est égal à une liste de string des erreurs.
        /// </summary>
        protected readonly Dictionary<string, ICollection<string>> ValidationErrors = new Dictionary<string, ICollection<string>>();

        #endregion Protected Fields

        #region Public Events

        /// <summary>
        /// Évènement avertissant qu'il y a des erreurs de validation pour une propriété.
        /// </summary>
        public event System.EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Évènement permettant d'avertir les observateurs du changement d'une propriété
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// Obtient une valeur indiquant si la liste ValidationErrors contient des erreurs.
        /// </summary>
        public bool HasErrors
        {
            get { return this.ValidationErrors.Count > 0; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Permet d'obtenir la liste des erreurs d'une propriété.
        /// </summary>
        /// <param name="nomPropriete">Nom de la propriété en string</param>
        /// <returns>Liste des erreurs d'une propriété</returns>
        public System.Collections.IEnumerable GetErrors(string nomPropriete)
        {
            if (string.IsNullOrEmpty(nomPropriete)
            || !this.ValidationErrors.ContainsKey(nomPropriete))
            {
                return null;
            }

            return this.ValidationErrors[nomPropriete];
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Méthode permettant d'appeler ErrorsChanged sans avoir à spécifier ses paramètres
        /// </summary>
        /// <param name="nomPropriete">Nom de la propriété qui a des erreurs de validation</param>
        protected void NotifyErrorsChanged([CallerMemberName] string nomPropriete = "")
        {
            if (this.ErrorsChanged != null)
            {
                this.ErrorsChanged(this, new DataErrorsChangedEventArgs(nomPropriete));
            }
        }

        /// <summary>
        /// Méthode permettant d'appeler PropertyChanged sans avoir à spécifier ses paramètres
        /// </summary>
        /// <param name="nomPropriete">Nom de la propriété qui a changée</param>
        protected void NotifyPropertyChanged([CallerMemberName] string nomPropriete = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }

        #endregion Protected Methods
    }
}