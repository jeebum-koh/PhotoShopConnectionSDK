/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

namespace MBC.Adobe.PhotoShop.Connection
{
    /// <summary>
    /// collection of photoshop constants
    /// </summary>
    public static class PhotoShopConstants
    {
        /// <summary>
        /// salt value used in PBKDF2 key generation. 
        /// this must match the value used in Photoshop, DO NOT CHANGE
        /// </summary>
        public const string SALT = "Adobe Photoshop";

        /// <summary>
        /// iteration count value used in PBKDF2 key generation. 
        /// this must match the value used in Photoshop, DO NOT CHANGE
        /// </summary>
        public const int ITERATION_COUNT = 1000;

        /// <summary>
        /// key length(bytes) value used in PBKDF2 key generation. 
        /// this must match the value used in Photoshop, DO NOT CHANGE
        /// </summary>
        public const int KEY_LENGTH = 24;

        /// <summary>
        /// Communication Status Def, NON-Error = 0, otherwise error.
        /// </summary>
        public const int NO_COMM_ERROR = 0;

        /// <summary>
        /// current protocol version
        /// </summary>
        public const int PROTOCOL_VERSION = 1;

        /// <summary>
        /// length of the header not including 
        /// the actual length byte or the communication status
        /// </summary>
        public const int PROTOCOL_LENGTH = 4 + 4 + 4;
        /// <summary>
        /// length of communication status field
        /// </summary>
        public const int COMM_LENGTH = 4;

        /// <summary>
        /// predefined communication port for PhotoShop communication
        /// </summary>
        public const int COMMUNICATION_PORT = 49494;
    }

    /// <summary>
    /// collection of constants used in this library
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// default local port for constructing SOCKET.
        /// </summary>
        public const int LOCAL_PORT_DEFAULT = 59595;

        /// <summary>
        /// default image width for PS communication
        /// </summary>
        public const int IMG_WIDTH = 1920;

        /// <summary>
        /// default image height for PS communication
        /// </summary>
        public const int IMG_HEIGHT = 1080;

        /// <summary>
        /// we're planning to use initial transaction id like this.
        /// </summary>
        public const int INITIAL_TRANSACTION_ID = 100;
    }
}
