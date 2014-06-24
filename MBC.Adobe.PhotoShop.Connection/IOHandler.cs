/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MBC.Adobe.PhotoShop.Connection
{
    /// <summary>
    /// Basic class handling Network Socket IO from/to PhotoShop
    /// </summary>
    public partial class IOHandler : IDisposable
    {
        /// <summary>
        /// Network IO Stream (TCP) for communication with PhotoShop.
        /// Also this object is used for locking 
        /// between command sending operations to PhotoShop 
        /// </summary>
        NetworkStream _netStream = null;

        /// <summary>
        /// <see cref="EncryptDecrypt"/> for encryption/decryption 
        /// required from PhotoShop
        /// </summary>
        EncryptDecrypt _encryptDecrypt = null;

        /// <summary>
        /// Transaction ID, when communication is made, 
        /// this value is automatically incremented.
        /// </summary>
        public int TransactionID { get; protected set; }

        /// <summary>
        /// used for As Run Log printing.
        /// Actually, 
        /// <seealso cref="TextWriter.WriteLine(string)"/> is the only function used.
        /// </summary>
        public TextWriter AsRunLogger { get; set; }

        /// <summary>
        /// When as run log is written, including byte representation in it.
        /// </summary>
        public bool ShowByteDisplayInAsRunLog { get; set; }

        /// <summary>
        /// Action to invoke when changed notification from PhotoShop received.
        /// NOTE: to use this, always mind about Thread Synchronization.
        /// Since .Net uses single ui thread to handle ui operation scheme,
        /// not-careful-assignment to this field would be result in cross thread violation.
        /// <code>
        /// var context = SynchronizationContext.Current;
        /// _cmdHandler.PhotoShopNotificationProc = 
        ///     (evtEnum, message) =>
        ///     {
        ///         context.Send(
        ///             (obj) =>
        ///             {
        ///                 switch (evtEnum)
        ///                 {
        ///                     case PhotoShopNotification.INVALID_NOTIFICATION:
        ///                         break;
        ///                     case PhotoShopNotification.foregroundColorChanged:
        ///                         pbForeground.BackColor = message.convertToColor();
        ///                         break;
        ///                     default:
        ///                         break;
        ///                 }
        ///             },
        ///             NULL);
        ///      };
        /// </code>
        /// </summary>
        public Action<PhotoShopNotification, string> PhotoShopNotificationProc { get; set; }

        /// <summary>
        /// gets explicit interface implementation from this object
        /// </summary>
        public PhotoShopAppAction App
        {
            get
            {
                return (PhotoShopAppAction)this;
            }
        }

        /// <summary>
        /// direct instanciation of this default constructor is prohibited.
        /// </summary>
        private IOHandler() 
        {
            TransactionID = Constants.INITIAL_TRANSACTION_ID;
            AsRunLogger = null;
            ShowByteDisplayInAsRunLog = false;

            PhotoShopNotificationProc = null;
        }

        /// <summary>
        /// constructor with params.
        /// actual object should be passed.
        /// </summary>
        /// <param name="encryptdecrypt">
        /// used for encrypt/decrypt. 
        /// should be valid object.
        /// </param>
        /// <param name="netStream">
        /// used for network communication.
        /// should be valid object AND connected to host.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// thrown when <paramref name="encryptdecrypt"/> is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// thrown when <paramref name="netStream"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// thrown when <paramref name="netStream"/> is not connected
        /// </exception>
        protected IOHandler(
            EncryptDecrypt encryptdecrypt,
            NetworkStream netStream)
            : this()
        {
            if (null == encryptdecrypt)
                throw
                    new ArgumentNullException("encryptdecrypt");

            if (null == netStream)
                throw
                    new ArgumentNullException("netStream");

            if (false == netStream.CanRead ||
                false == netStream.CanWrite)
                throw
                    new ArgumentException(
                        "should be open and connected to PhotoShop",
                        "netStream");

            this._encryptDecrypt = encryptdecrypt;
            this._netStream = netStream;

            queueCheckNotification();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ IOHandler()
        {
            Dispose(false);
        }

        /// <summary>
        /// Default implementation of <seealso cref="IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Canonical implementation of IDisposable pattern.
        /// </summary>
        /// <param name="disposing">
        /// indicator whether this function is invoked 
        /// from normal <seealso cref="IDisposable.Dispose"/> context 
        /// or destructor context</param>
        protected virtual void Dispose(
            bool disposing)
        {
            // in this implementation, disposing parameter doesn't affect anything..
            if (null != _netStream)
            {
                _netStream.Close();
                _netStream = null;
            }
        }
        
        /// <summary>
        /// check if disposed
        /// </summary>
        public virtual bool IsDisposed
        {
            get
            {
                return null == _netStream;
            }
        }

        /// <summary>
        /// create new <see cref="IOHandler"/> object from given parameters.
        /// </summary>
        /// <param name="password">password used with encrypt/decrypt</param>
        /// <param name="hostName">communication target</param>
        /// <param name="writer">
        /// as run logger, 
        /// Actually, 
        /// <seealso cref="TextWriter.WriteLine(string)"/> is the only function used.
        /// </param>
        /// <param name="localPort">
        /// local host port, when establishing communication socket
        /// </param>
        /// <returns>
        /// newly created <see cref="IOHandler"/> object 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// thrown when <paramref name="password"/> is null or empty
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// thrown when password argument contains characters 
        /// except ascii visible characters ([\x20-\x7E]) 
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// thrown when <paramref name="hostName"/> is null or empty
        /// </exception>
        public static IOHandler CreateNew(
            string password,
            string hostName,
            TextWriter writer = null,
            int localPort = Constants.LOCAL_PORT_DEFAULT)
        {
            if (string.IsNullOrEmpty(password))
                throw
                    new ArgumentNullException("password");

            if (false == password.IsPhotoShopPBKDF2AcceptableString())
                throw
                    new ArgumentOutOfRangeException(
                        "password",
                        "password should be composed of " +
                            "ascii visible characters ([\x20-\x7E]).");

            if (string.IsNullOrEmpty(hostName))
                throw
                    new ArgumentNullException("hostName");

            var encryptDecrypt = new EncryptDecrypt(password);
            
            var tcpclient = 
                new TcpClient(
                    new IPEndPoint(
                        IPAddress.Parse("0.0.0.0"),
                        localPort));
            tcpclient.Connect(hostName, PhotoShopConstants.COMMUNICATION_PORT);
            var netStream = tcpclient.GetStream();

            return
                new IOHandler(encryptDecrypt, netStream)
                {
                    AsRunLogger = writer
                };
        }
    }
}
