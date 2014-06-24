/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

using System;
using System.Text;

namespace MBC.Adobe.PhotoShop.Connection
{
    partial class IOHandler
    {
        /// <summary>
        /// send javascript code to PS and returns response from PS
        /// </summary>
        /// <param name="javaScript">javascript code snippet 
        /// to be passed to PS</param>
        /// <returns>response from PS</returns>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        public PhotoShopResponse ProcessJavaScript(
            string javaScript)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            return
                SendAndReceive(
                    new DataBlock()
                    {
                        ProtocolVersion = PhotoShopConstants.PROTOCOL_VERSION,
                        // don't forget increment TransactionID!!!
                        TransactionID = this.TransactionID++,
                        ContentType = ContentType.JAVASCRIPT,
                        Content = Encoding.UTF8.GetBytes(javaScript)
                    });
        }

        /// <summary>
        /// test connectivity to PhotoShop
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        public bool IsPhotoShopWorking
        {
            get
            {
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");

                var response = 
                    ProcessJavaScript(
                        JavascriptSnippet.PhotoShopApp.GET_VERSION);
                return
                    CommunicationStatus.ERROR_COMMUNICATION !=
                    response.Status;
            }
        }
    }
}
