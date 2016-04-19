namespace ExempleMVVM.VueModeles
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using ExempleMVVM.Modeles;
    using ExempleMVVM.Modules;

    /// <summary>
    /// Vue-modèle pour la vue vueChat
    /// </summary>
    internal class vmChat : ViewModelBase
    {
        #region Private Fields

        /// <summary>
        /// Variable privée de la propriété <see cref="ConversationEnCours"/>
        /// </summary>
        private Conversation conversationEnCours;

        /// <summary>
        /// Variable privée de la propriété <see cref="EnvoyerMessage"/>
        /// </summary>
        private ICommand envoyerMessage;

        /// <summary>
        /// Variable privée de la propriété <see cref="FermerConversation"/>
        /// </summary>
        private ICommand fermerConversation;

        /// <summary>
        /// Variable privée de la propriété <see cref="MessageAEnvoyer"/>
        /// </summary>
        private string messageAEnvoyer;

        /// <summary>
        /// Variable privée de la propriété <see cref="OuvrirConversation"/>
        /// </summary>
        private ICommand ouvrirConversation;

        /// <summary>
        /// Profil de l'utilsateur à faire suivre à travers les vues-modèles de l'application pour
        /// avoir l'état de l'application
        /// </summary>
        private Profil profil;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="vmChat"/>
        /// </summary>
        /// <param name="profil">
        /// Profil de l'utilsateur à faire suivre à travers les vues-modèles de l'application pour
        /// avoir l'état de l'application
        /// </param>
        public vmChat(Profil profil)
        {
            this.profil = profil;

            this.profil.PropertyChanged += this.Profil_PropertyChanged;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Obtient ou définit la conversation présentement sélectionnée, à qui les messages sont envoyés
        /// </summary>
        public Conversation ConversationEnCours
        {
            get
            {
                if (this.conversationEnCours == null && this.profil.Conversations.Count > 0)
                {
                    this.conversationEnCours = this.profil.Conversations[0];
                }

                return this.conversationEnCours;
            }

            set
            {
                if (this.conversationEnCours != value)
                {
                    this.conversationEnCours = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la commande permettant d'envoyer le message se trouvant dans MessageAEnvoyer dans
        /// la conversation en cours.
        /// </summary>
        public ICommand EnvoyerMessage
        {
            get
            {
                if (this.envoyerMessage == null)
                {
                    this.envoyerMessage = new RelayCommand<object>(
                        (obj) =>
                        {
                            ConversationEnCours.Lignes.Add(new LigneConversation()
                            {
                                Utilisateur = profil.UtilisateurLocal,
                                Message = MessageAEnvoyer
                            });
                            ProtocoleSJ.EnvoyerMessage(ConversationEnCours, MessageAEnvoyer);
                            MessageAEnvoyer = string.Empty;
                        },
                        (obj) =>
                        {
                            return !string.IsNullOrWhiteSpace(messageAEnvoyer) && conversationEnCours.Connecte;
                        });
                }

                return this.envoyerMessage;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de fermer une conversation privée.
        /// </summary>
        public ICommand FermerConversation
        {
            get
            {
                if (this.fermerConversation == null)
                {
                    this.fermerConversation = new RelayCommand<Conversation>(
                        (conversation) =>
                        {
                            ConversationEnCours = profil.Conversations[0];
                            ProtocoleSJ.TerminerConversationPrivee(conversation);
                        },
                        (conversation) =>
                        {
                            return conversation != null && profil.Conversations.Count > 0 && conversation.EstPrivee;
                        });
                }

                return this.fermerConversation;
            }
        }

        /// <summary>
        /// Obtient la liste des conversations en cours incluant la conversation globale
        /// </summary>
        public ObservableCollection<Conversation> ListeConversations
        {
            get
            {
                return this.profil.Conversations;
            }
        }

        /// <summary>
        /// Obtient la liste des utilisateurs connectées qui utilise cette application sur le réseau présentement
        /// </summary>
        public ObservableCollection<Utilisateur> ListeUtilisateursConnectes
        {
            get
            {
                return this.profil.UtilisateursConnectes;
            }
        }

        /// <summary>
        /// Obtient ou définit le message a envoyé à la conversation en cours lors de l'appel de la
        /// commande EnvoyerMessage
        /// </summary>
        public string MessageAEnvoyer
        {
            get
            {
                return this.messageAEnvoyer;
            }

            set
            {
                if (this.messageAEnvoyer != value)
                {
                    this.messageAEnvoyer = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la commande permettant d'ouvrir une conversation entre l'utilisateur local et un
        /// utilisateur dans la liste des utilisateurs connectés.
        /// </summary>
        public ICommand OuvrirConversation
        {
            get
            {
                if (this.ouvrirConversation == null)
                {
                    this.ouvrirConversation = new RelayCommand<Utilisateur>(
                        (utilisateur) =>
                        {
                            List<Conversation> conversationsExistantes = profil.Conversations.Where((c) =>
                            {
                                return c.Utilisateur.Nom == utilisateur.Nom && c.Utilisateur.IP == utilisateur.IP;
                            }).ToList();
                            if (conversationsExistantes.Count == 0)
                            {
                                Conversation nouvelleConversation = new Conversation()
                                {
                                    Connecte = false,
                                    EstGlobale = false,
                                    Utilisateur = utilisateur
                                };
                                profil.Conversations.Add(nouvelleConversation);
                                ProtocoleSJ.DemarrerConversationPrivee(nouvelleConversation);
                                conversationsExistantes.Add(nouvelleConversation);
                            }

                            ConversationEnCours = conversationsExistantes[0];
                        },
                        (utilisateur) =>
                        {
                            return utilisateur != null;
                        });
                }

                return this.ouvrirConversation;
            }
        }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Permet de démarrer le processus de récupération des utilisateurs
        /// </summary>
        /// <param name="sender">Celui qui a appelé l'évènement</param>
        /// <param name="e">Contient le nom de la propriété qui a changée</param>
        private void Profil_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Connecte":
                    if (this.profil.Connecte)
                    {
                        ProtocoleSJ.RafraichirListeUtilisateursConnectes();
                    }

                    break;

                default:
                    break;
            }
        }

        #endregion Private Methods
    }
}