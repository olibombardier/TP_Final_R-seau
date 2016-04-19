namespace ExempleMVVM.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ExempleMVVM.Modeles;

    /// <summary>
    /// Protocole utilisant UDP 50000 permettant de clavarder avec plusieurs utilisateurs sans avoir
    /// un serveur centralisé. De plus, on peut initialiser une conversation crypter en AES128
    /// utilisant le TCP (port aléatoire).
    /// </summary>
    public class ProtocoleSJ
    {
        #region Public Methods

        /// <summary>
        /// Représente le chat global
        /// </summary>
        public static Conversation conversationGlobal;

        /// <summary>
        /// Port utilisé pour le chat global
        /// </summary>
        public const int port = 50000;

        /// <summary>
        /// Permet de vérifier si le nom d'utilisateur d'un profil est déjà utilisé sur le réseau et
        /// de démarre l'écoute sur le port 50000 UDP pour répondre au demande des autres
        /// utilisateurs. Durant le processus de connexion, profil.ConnexionEnCours est égal à vrai.
        /// Si le nom d'utilisateur est utilisé, on ferme l'écoute sur le port 50000 UDP. Sinon,
        /// profil.Connecte est égal à vrai.
        /// </summary>
        /// <param name="profil">Profil utilisé dans l'application pour avoir l'état de l'application</param>
        public static async void Connexion(Profil profil)
        {
            conversationGlobal.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            conversationGlobal.Socket.EnableBroadcast = true;
            conversationGlobal.Socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        /// <summary>
        /// Méthode permettant de fermer toutes les connexions en cours (UDP et TCP)
        /// </summary>
        public static void Deconnexion()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Permet d'ouvrir un port TCP et d'envoyer par UDP une demande de connexion en privée à l'utilisateur distant
        /// </summary>
        /// <param name="nouvelleConversation">Conversation privée contenant l'utilisateur distant</param>
        public static void DemarrerConversationPrivee(Conversation nouvelleConversation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Méthode permettant d'envoyer un message sur la conversation en cours
        /// </summary>
        /// <param name="conversationEnCours">
        /// Conversation présentement sélectionnée pour envoyer le message
        /// </param>
        /// <param name="messageAEnvoyer">Message à envoyer à tous les utilisateurs</param>
        public static void EnvoyerMessage(Conversation conversationEnCours, string messageAEnvoyer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Méthode permettant d'envoyer un message en "broadcast" pour sonder tous les utilisateurs
        /// utilisant l'application sur le réseau. Par la suite, on peut rafraîchir la liste profilUtilisateursConnectes.
        /// </summary>
        public static async void RafraichirListeUtilisateursConnectes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Permet de fermer correctement une conversation privée
        /// </summary>
        /// <param name="conversation">Conversation à fermer</param>
        public static void TerminerConversationPrivee(Conversation conversation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Envois un message en UDP
        /// </summary>
        /// <param name="ipDestinataire"></param>
        /// <param name="message"></param>
        public static async void Envoyer(string ipDestinataire, string message)
        {
            byte[] data = Encoding.Unicode.GetBytes("TPR" + message);

            await Task.Factory.StartNew(() =>
            {
                conversationGlobal.Socket.Send(data);
            });
        }

        #endregion Public Methods
    }
}