namespace ExempleMVVM.Vues
{
    using System.Collections.Specialized;
    using System.Windows.Controls;

    /// <summary>
    /// ListView ayant la possibilité de défiler automatique vers le bas lors de l'ajout de contenu
    /// </summary>
    public partial class AutoScrollingListView : ListView
    {
        #region Protected Methods

        /// <summary>
        /// Redéfinition de la méthode appelée lorsque la liste des items change. Ici, on s'assure d'associer la méthode ItemsCollectionChanged à l'évènement CollectionChanged
        /// de la nouvelle liste.
        /// </summary>
        /// <param name="oldValue">Ancienne liste d'items</param>
        /// <param name="newValue">Nouvelle liste d'items</param>
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (oldValue as INotifyCollectionChanged != null)
            {
                (oldValue as INotifyCollectionChanged).CollectionChanged -= this.ItemsCollectionChanged;
            }

            if (newValue as INotifyCollectionChanged == null)
            {
                return;
            }

            (newValue as INotifyCollectionChanged).CollectionChanged += this.ItemsCollectionChanged;
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Lorsque la liste change, on défile vers l'item le plus bas.
        /// </summary>
        /// <param name="sender">Celui qui a appelé l'évènement</param>
        /// <param name="e">Contient les changements effectués sur la liste</param>
        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Items.Count > 0)
            {
                this.ScrollIntoView(this.Items[this.Items.Count - 1]);
            }
        }

        #endregion Private Methods
    }
}