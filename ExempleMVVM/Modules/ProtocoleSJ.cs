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
        public static Conversation conversationGlobale;

        /// <summary>
        /// Profil utilisé par l'application
        /// </summary>
        public static Profil profilApplication;

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
            conversationGlobale = profil.Conversations.Where(c => c.EstGlobale).First();

            //Si la converstion globale n'existe pas, elle est crée

            if (conversationGlobale == null)
            {
                conversationGlobale = new Conversation();
                conversationGlobale.EstGlobale = true;

                profil.Conversations.Add(conversationGlobale);
            }

            conversationGlobale.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            conversationGlobale.Socket.EnableBroadcast = true;
            conversationGlobale.Socket.Bind(new IPEndPoint(IPAddress.Any, port));

            profilApplication = profil;
            profil.ConnexionEnCours = true;

            Recevoir(conversationGlobale);
            EnvoyerDiscovery();

            await Task.Delay(5000);

            if (!profil.UtilisateursConnectes.Any(u => u.Nom == profil.UtilisateurLocal.Nom))
            {
                profil.Connecte = true;
            }

            profil.ConnexionEnCours = false;
        }
        
        /// <summary>
        /// Méthode permettant de fermer toutes les connexions en cours (UDP et TCP)
        /// </summary>
        public static void Deconnexion()
        {
            foreach (Conversation c in profilApplication.Conversations)
            {
                if (c.EstPrivee)
                {
                    TerminerConversationPrivee(c);
                }
                else
                {
                    c.Socket.Close();
                }
            }
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
            Envoyer(conversationEnCours, "M" + messageAEnvoyer);
        }

        /// <summary>
        /// Méthode permettant d'envoyer un message en "broadcast" pour sonder tous les utilisateurs
        /// utilisant l'application sur le réseau. Par la suite, on peut rafraîchir la liste profilUtilisateursConnectes.
        /// </summary>
        public static async void RafraichirListeUtilisateursConnectes()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Permet de fermer correctement une conversation privée
        /// </summary>
        /// <param name="conversation">Conversation à fermer</param>
        public static void TerminerConversationPrivee(Conversation conversation)
        {
            throw new NotImplementedException();
        }


        #region reception
        /// <summary>
        /// Permet de recevoir un message en mode global
        /// </summary>
        public static void RecevoirMessage()
        {
            
        }

        /// <summary>
        /// Permet de recevoir l'adresse IP ainsi que le Nom de l'utilisateur.
        /// </summary>
        public static async void Recevoir(Conversation conversation)
        {
            bool Connecte = false;

            while (!Connecte)
            {
                byte[] data = new byte[1024];

                int byteRead = 0;

                await Task.Factory.StartNew(() =>
                {
                   byteRead = conversationGlobale.Socket.Receive(data);
                });
                string message = Encoding.Unicode.GetString(data ,0 ,byteRead);

                if(message.Substring(0, 3) == "TPR")
                {
                    switch (message[3])
                    {
                        case 'D':
                            EnvoyerIdentification(conversation);
                            break;
                        case 'I':
                            System.Diagnostics.Debug.WriteLine("Identification");
                            break;
                        case 'M':
                            System.Diagnostics.Debug.WriteLine("Message");
                            break;
                        case 'P':
                            System.Diagnostics.Debug.WriteLine("Privé");
                            break;
                        case 'Q':
                            System.Diagnostics.Debug.WriteLine("Quitter");
                            break;
                    }
                }
            }
        }

        #endregion reception

        #region Envois

        /// <summary>
        /// Envois un message en UDP ou TCP selon la conversation et l'encrypte
        /// si nécessaire
        /// </summary>
        /// <param name="ipDestinataire"></param>
        /// <param name="conversation">Conversation à laquelle envoyer le message</param>
        /// <param name="message">Message à envoyer</param>
        public static async void Envoyer(Conversation conversation, string message)
        {
            string messageComplet = "TPR" + message;

            if (conversation.EstGlobale)
            {
                byte[] data = Encoding.Unicode.GetBytes(messageComplet);
                await Task.Factory.StartNew(() =>
                {
                    conversation.Socket.SendTo(data, new IPEndPoint(IPAddress.Broadcast, port));
                });
            }
            else // Conversation privée
            {
                byte[] data = Encrypter(messageComplet, conversation.Key);
                await Task.Factory.StartNew(() =>
                {
                    conversation.Socket.Send(data);
                });
            }
        }

        public static byte[] Encrypter(string message, byte[] cle)
        {
            byte[] resultat = new byte[1024];
            Aes aes = Aes.Create();

            if (cle.Length != 128)
            {
                throw new ArgumentException("La clé doit être de 128 bits");
            }

            aes.Key = cle;

            ICryptoTransform encrypteur = aes.CreateEncryptor();
            using (MemoryStream memoryStream = new MemoryStream(resultat))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encrypteur, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(cryptoStream))
                    {
                        writer.Write(message);
                    }
                }
            }
            return resultat;
        }

        /// <summary>
        /// Envois un message de discovery en broadcast
        /// </summary>
        public static async void EnvoyerDiscovery()
        {
            Envoyer(conversationGlobale,"D");
        }

        /// <summary>
        /// Envois le nom de l'utilisateur à la conversation fournie
        /// </summary>
        /// <param name="conversation"></param>
        public static async void EnvoyerIdentification(Conversation conversation)
        {
            Envoyer(conversation, "I" + profilApplication.Nom);
        }

        #endregion Envois

        #endregion Public Methods
    }
}