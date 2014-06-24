/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

using System.Drawing;
using System.IO;

namespace MBC.Adobe.PhotoShop.Connection
{
    /// <summary>
    /// interface to action available in PhotoShop
    /// </summary>
    public interface PhotoShopAppAction
    {
        /// <summary>
        /// get/set ForegroundColor of PhotoShop Connected.
        /// if invoken on disposed object, ObjectDisposedException is raised.
        /// when get, Color.Empty is returned when any error happens.
        /// </summary>
        Color ForegroundColor { get; set; }

        /// <summary>
        /// get/set BackgroundColor of PhotoShop Connected.
        /// if invoken on disposed object, ObjectDisposedException is raised.
        /// when get, Color.Empty is returned when any error happens.
        /// </summary>
        Color BackgroundColor { get; set; }

        /// <summary>
        /// get all documents's names opened in PhotoShop.
        /// null, when PhotoShop is not working, there's no document opened, etc
        /// </summary>
        string[] AllDocumentNames { get; }

        /// <summary>
        /// get active document's name opened in PhotoShop.
        /// string.Empty, when PhotoShop is not working, there's no document opened, etc
        /// </summary>
        string ActiveDocumentName { get; }

        /// <summary>
        /// get active document's id opened in PhotoShop.
        /// -1, when PhotoShop is not working, there's no document opened, etc
        /// </summary>
        int ActiveDocumentID { get; }

        /// <summary>
        /// get/set active tool id in PhotoShop
        /// </summary>
        string ActiveToolID { get; set; }

        /// <summary>
        /// instruct PhotoShop to open given file
        /// </summary>
        /// <param name="fileName">file path which PhotoShop can understand.</param>
        /// <returns>true, if sucess, otherwise false</returns>
        bool OpenFile(
            string fileName);

        /// <summary>
        /// save current active document as given path
        /// </summary>
        /// <param name="fileName"> file path which PhotoShop can understand.</param>
        /// <returns>true, if sucess, otherwise false</returns>
        bool SaveActiveDocumentAs(
            string fileName);

        /// <summary>
        /// send image to PS as JPEG format.
        /// if given file has image format other than JPEG,
        /// it's converted to JPEG and then transmitted to PS.
        /// (I gave up with generating PIXMAP you PS...)
        /// if given file is not of graphic format, do nothing.
        /// </summary>
        /// <param name="remoteImageFileName">graphic file to send to PS</param>
        /// <returns>true, successfully sent to PS, otherwise false</returns>
        bool LoadRemoteImage(
            string remoteImageFileName);

        /// <summary>
        /// send binary data from existing file onto PhotoShop
        /// </summary>
        /// <param name="remoteFileName">
        /// file which should be passed onto PhotoShop.
        /// given file resides on client PC(like this) not on host pc of PhotoShop
        /// </param>
        /// <returns>
        /// if success, file path which PS returned. 
        /// This file path is in Windows-style file path on Windows client.
        /// otherwise empty string
        /// </returns>
        string LoadRemoteData(
            string remoteFileName);

        /// <summary>
        /// send binary data from given stream object onto PhotoShop
        /// </summary>
        /// <param name="remoteStream">
        /// stream containing binary data which should be passed onto PhotoShop
        /// </param>
        /// <returns>
        /// if success, file path which PS returned.
        /// This file path is in Windows-style file path.
        /// otherwise empty string
        /// </returns>
        string LoadRemoteData(
            Stream remoteStream);

        /// <summary>
        /// receive current active document's image from PS as JPEG format.
        /// if there's no active document, null image is returned.
        /// </summary>
        /// <param name="width">desired image width</param>
        /// <param name="height">desired image height</param>
        /// <returns>
        /// image object as jpeg format.
        /// if there's no image in PS, or error occurred, null reference is returned.
        /// </returns>
        Image GetCurrentThumbnail(
            int width = Constants.IMG_WIDTH,
            int height = Constants.IMG_HEIGHT);

        /// <summary>
        /// retrieve active document in PhotoShop onto client pc
        /// </summary>
        /// <param name="remoteFileName">
        /// file name to save PhotoShop's active document.
        /// should be valid windows path on client pc.
        /// </param>
        /// <returns>
        /// true, if successful in saving active document in PhotoShop to given file path on client pc.
        /// false, otherwise
        /// </returns>
        bool SaveActiveDocumentToRemote(
            string remoteFileName);
    }
}
