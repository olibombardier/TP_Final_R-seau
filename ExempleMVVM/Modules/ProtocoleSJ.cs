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
using System.Net.NetworkInformation;

namespace ExempleMVVM.Modules
{

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
        /// Met ton adresse IP dans une liste
        /// </summary>
        private static List<IPAddress> mesAdresse = new List<IPAddress>();

        /// <summary>
        /// Met un bool à True ou False dépendant du fait ou l'application écoute ou non
        /// </summary>
        private static bool enEcoute;

        /// <summary>
        /// Fait une liste d'utilisateurs afin de pouvoir rafraichir la liste
        /// </summary>
        private static List<Utilisateur> utilisateurTemp = new List<Utilisateur>();

        private static List<Utilisateur> listeASupprimer = new List<Utilisateur>();

        private static List<Utilisateur> utilisateurASupprimer = new List<Utilisateur>();

        private static List<Utilisateur> utilisateurAAjouter = new List<Utilisateur>();

        private static List<Utilisateur> nouvelUtilisateur = new List<Utilisateur>();

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


            // Trouver nos propre adresse IP
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation information in ni.GetIPProperties().UnicastAddresses)
                {
                    if (information.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        mesAdresse.Add(information.Address);
                    }
                }
            }

            profilApplication = profil;
            profil.ConnexionEnCours = true;

            Recevoir(conversationGlobale);
            EnvoyerDiscovery();

            await Task.Delay(5000);
            
            foreach(Utilisateur utilisateur in utilisateurTemp)
            {
                profilApplication.UtilisateursConnectes.Add(utilisateur);
            }

            utilisateurTemp.Clear();

            if (!profil.UtilisateursConnectes.Any(u => u.Nom == profil.UtilisateurLocal.Nom))
            {
                profil.Connecte = true;
            }
            else
            {
                enEcoute = false;
                conversationGlobale.Socket.Close();
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
            await Task.Factory.StartNew(() =>
            {
                EnvoyerDiscovery();
                Task.Delay(5000);
                nouvelUtilisateur.Clear();

                foreach (Utilisateur vieuxUtilisateur in profilApplication.UtilisateursConnectes)
                {
                    Utilisateur utilisateur = utilisateurTemp.Find((nouvelUtilisateur) =>
                    vieuxUtilisateur.Nom == nouvelUtilisateur.Nom && vieuxUtilisateur.IP == nouvelUtilisateur.IP);
                    if (utilisateur == null)
                    {
                        listeASupprimer.Add(vieuxUtilisateur);
                    }
                    else
                    {
                        utilisateurTemp.Remove(utilisateur);
                    }
                }

                foreach(Utilisateur utilisateurASupprimer in listeASupprimer)
                {
                    profilApplication.UtilisateursConnectes.Remove(utilisateurASupprimer);
                }

                foreach(Utilisateur utilisateurAAjouter in nouvelUtilisateur)
                {
                    profilApplication.UtilisateursConnectes.Add(utilisateurAAjouter);
                }

            });
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
        public static void RecevoirMessage(Conversation conversation, string message, EndPoint endPoint)
        {
            Utilisateur envoyeur = TrouverUtilisateurSelonEndPoint(endPoint);
            IPAddress adresse = ((IPEndPoint)endPoint).Address;
            if(!EstMonAdresse(adresse))
            {
                LigneConversation ligne = new LigneConversation();
                ligne.Message = message;
                ligne.Utilisateur = envoyeur;
                conversation.Lignes.Add(ligne);

            };
        }

        /// <summary>
        /// Retourne l'utilisateur à qui apartient l'endpoint
        /// </summary>
        /// <param name="endPoint">EndPoint</param>
        /// <returns>L'utilisateur, null s'il n'est pas trouvé</returns>
        public static Utilisateur TrouverUtilisateurSelonEndPoint(EndPoint endPoint)
        {
            string ip = ((IPEndPoint) endPoint).Address.ToString();
            Utilisateur resultat = null;

            if (profilApplication.UtilisateursConnectes.Count > 0)
            {
                IEnumerable<Utilisateur> utilisateursTrouve = profilApplication.UtilisateursConnectes.Where(u => u.IP == ip);
                if (utilisateursTrouve.Count() > 0)
                {
                    resultat = utilisateursTrouve.First();
                }
            }

            return resultat;
        }

        /// <summary>
        /// Permet de recevoir l'adresse IP ainsi que le Nom de l'utilisateur.
        /// </summary>
        public static async void Recevoir(Conversation conversation)
        {
            enEcoute = true;
            while (enEcoute)
            {
                byte[] data = new byte[1024];

                int byteRead = 0;
                EndPoint otherEndPoint = new IPEndPoint(IPAddress.Any, 0);

                await Task.Factory.StartNew(() =>
                {
                    try{
                        if (conversation.EstPrivee)
                        {
                            byteRead = conversation.Socket.Receive(data);
                        }
                        else
                        {
                            byteRead = conversation.Socket.ReceiveFrom(data, ref otherEndPoint);
                        }
                    }
                    catch(Exception)
                    {
                        profilApplication.Connecte = false;
                        enEcoute = false;
                    }
                });
                string message = Encoding.Unicode.GetString(data ,0 ,byteRead);

                if(byteRead > 0)
                {
                    if(message.Substring(0, 3) == "TPR")
                    {
                        switch (message[3])
                        {
                            case 'D':
                                EnvoyerIdentification(conversation, otherEndPoint);
                                break;
                            case 'I':
                                if (conversation.EstGlobale)
                                {
                                    RecevoirIdentification((IPEndPoint)otherEndPoint, message);
                                }
                                break;
                            case 'M':
                                RecevoirMessage(conversation, message.Substring(4), otherEndPoint);
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
        }

        /// <summary>
        /// Reçois l'idetification d'un utilisateur
        /// </summary>
        /// <param name="endpoint"></param>
        public static void RecevoirIdentification(IPEndPoint endpoint, string message)
        {
            string nom = message.Substring(4);
            IPAddress adresse = endpoint.Address;

            if (!(profilApplication.UtilisateursConnectes.Any(u => u.IP == adresse.ToString()) ||
                EstMonAdresse(adresse)))
            {
                utilisateurTemp.Add(new Utilisateur()
                {
                    Nom = nom,
                    IP = adresse.ToString()
                });
            }
        }

        /// <summary>
        /// Retourne vrai si l'adresse passée en argument est la notre
        /// </summary>
        /// <param name="adresse">Adresse à vérifier</param>
        public static bool EstMonAdresse(IPAddress adresse)
        {
            return mesAdresse.Any(a => a.Equals(adresse));
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
                    // Envoyer juste au bonne personnes
                    foreach (Utilisateur utilisateur in profilApplication.UtilisateursConnectes)
                    {
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(utilisateur.IP),port);
                        conversation.Socket.SendTo(data, endPoint);
                    }
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

        /// <summary>
        /// Envois un message en UDP au endPoint spécifié. À utiliser pour envoyer un
        /// message à quelqu'un qui n'est pas dans la liste d'utilisateur.
        /// </summary>
        /// <param name="endPoint">Destination</param>
        /// <param name="socket">Socket par lequel envoyer le message</param>
        /// <param name="message">Message à envoyer</param>
        public static async void Envoyer(EndPoint endPoint, Socket socket, string message)
        {
            await Task.Factory.StartNew(() =>
            {
                socket.SendTo(Encoding.Unicode.GetBytes("TPR" + message), endPoint);
            });
        }

        /// <summary>
        /// Envoie un message en broadcast
        /// </summary>
        /// <param name="conversation">Conversation ayant un socket UDP</param>
        /// <param name="message">Message à envoyer</param>
        public static void EnvoyerBroadcast(Conversation conversation, string message)
        {
            byte[] data = Encoding.Unicode.GetBytes("TPR" + message);
            foreach (var broadcast in ObtenirAdressesBroadcast())
            {
                conversation.Socket.SendTo(data, 0, data.Length, SocketFlags.None, new IPEndPoint(broadcast, 50000));
            }
        }

        /// <summary>
        /// Permet d'obtenir la liste des adresses Broadcast disponibles. La fonction élimine les adresses des cartes Loopback et des cartes qui ne sont pas branchées.
        /// </summary>
        /// <returns>La liste des adresses Broadcast disponibles. La fonction élimine les adresses des cartes Loopback et des cartes qui ne sont pas branchées.</returns>
        private static HashSet<IPAddress> ObtenirAdressesBroadcast()
        {
            HashSet<IPAddress> broadcasts = new HashSet<IPAddress>();
            foreach (var i in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (i.OperationalStatus == OperationalStatus.Up && i.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    foreach (var ua in i.GetIPProperties().UnicastAddresses)
                    {
                        if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            IPAddress broadcast = new IPAddress(BitConverter.ToUInt32(ua.Address.GetAddressBytes(), 0) | (BitConverter.ToUInt32(ua.IPv4Mask.GetAddressBytes(), 0) ^ BitConverter.ToUInt32(IPAddress.Broadcast.GetAddressBytes(), 0)));
                            broadcasts.Add(broadcast);
                        }
                    }
                }
            }
            return broadcasts;
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
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    cryptoStream.Write(data, 0, data.Length);
                }
            }
            return resultat;
        }

        /// <summary>
        /// Envois un message de discovery en broadcast
        /// </summary>
        public static async void EnvoyerDiscovery()
        {
            EnvoyerBroadcast(conversationGlobale,"D");
        }

        /// <summary>
        /// Envois le nom de l'utilisateur à la conversation fournie
        /// </summary>
        /// <param name="conversation"></param>
        public static async void EnvoyerIdentification(Conversation conversation, EndPoint endpoint)
        {
            Envoyer(endpoint, conversation.Socket, "I" + profilApplication.Nom);
        }

        /// <summary>
        /// Indique à l'autre utilisateur qu'on a ouvert un port et on attend un conversation privée
        /// </summary>
        /// <param name="conversationGlobale">Conversation globale</param>
        public static async void EnvoyerDemendeConversationPrivee(Conversation conversationGlobale)
        {
            byte[] cle = new byte[128];
            for (int i = 0; i < 128; i++)
            {
                
            }
        }

        #endregion Envois

        #endregion Public Methods
    }
}
