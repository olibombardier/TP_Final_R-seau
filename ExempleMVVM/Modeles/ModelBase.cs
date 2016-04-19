namespace ExempleMVVM.Modeles
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Classe mère des modèles du système.
    /// </summary>
    public class ModelBase
    {
        #region Public Events

        /// <summary>
        /// Évènement permettant d'avertir les observateurs du changement d'une propriété
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Protected Methods

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