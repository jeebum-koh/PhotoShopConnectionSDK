/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace MBC.Adobe.PhotoShop.Connection
{
    partial class IOHandler : PhotoShopAppAction
    {
        /// <summary>
        /// get/set ForegroundColor of PhotoShop Connected.
        /// if invoken on disposed object, ObjectDisposedException is raised.
        /// when get, Color.Empty is returned when any error happens.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        Color PhotoShopAppAction.ForegroundColor
        {
            get
            {
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");

                if (false == IsPhotoShopWorking)
                    return Color.Empty;

                var result = 
                    ProcessJavaScript(
                        JavascriptSnippet.PhotoShopApp.GET_FOREGROUND_COLOR);

                if (CommunicationStatus.OK != result.Status)
                    return Color.Empty;

                var strColor = result.ReturnString;
                if (string.IsNullOrEmpty(strColor))
                    return Color.Empty;
                if (6 != strColor.Length)
                    return Color.Empty;

                try
                {
                    var red   = int.Parse(strColor.Substring(0, 2), NumberStyles.HexNumber);
                    var green = int.Parse(strColor.Substring(2, 2), NumberStyles.HexNumber);
                    var blue  = int.Parse(strColor.Substring(4, 2), NumberStyles.HexNumber);
                    return Color.FromArgb(red, green, blue);
                }
                catch
                {
                    return Color.Empty;
                }
            }
            set
            {
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");
                /*
                if (false == IsPhotoShopWorking)
                    throw new InvalidOperationException("PhotoShop is not working.");
                 */

                var javascript =
                    string.Format(
                        JavascriptSnippet.PhotoShopApp.SET_FOREGROUND_COLOR,
                        value.R,
                        value.G,
                        value.B);
                var result = ProcessJavaScript(javascript);

                // never mind about whether operation is well-done, or rejected from PhotoShop,
                // 'cause this one is implemented as property,
                // raising exception when given action is not success is of no use
            }
        }

        /// <summary>
        /// get/set BackgroundColor of PhotoShop Connected.
        /// if invoken on disposed object, ObjectDisposedException is raised.
        /// when get, Color.Empty is returned when any error happens.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        Color PhotoShopAppAction.BackgroundColor
        {
            get
            {
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");

                if (false == IsPhotoShopWorking)
                    return Color.Empty;

                var result =
                    ProcessJavaScript(
                        JavascriptSnippet.PhotoShopApp.GET_BACKGROUND_COLOR);

                if (CommunicationStatus.OK != result.Status)
                    return Color.Empty;

                var strColor = result.ReturnString;
                if (string.IsNullOrEmpty(strColor))
                    return Color.Empty;
                if (6 != strColor.Length)
                    return Color.Empty;

                try
                {
                    var red   = int.Parse(strColor.Substring(0, 2), NumberStyles.HexNumber);
                    var green = int.Parse(strColor.Substring(2, 2), NumberStyles.HexNumber);
                    var blue  = int.Parse(strColor.Substring(4, 2), NumberStyles.HexNumber);
                    return Color.FromArgb(red, green, blue);
                }
                catch
                {
                    return Color.Empty;
                }
            }
            set
            {
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");
                /*
                if (false == IsPhotoShopWorking)
                    throw new InvalidOperationException("PhotoShop is not working.");
                 */

                var javascript =
                    string.Format(
                        JavascriptSnippet.PhotoShopApp.SET_BACKGROUND_COLOR,
                        value.R,
                        value.G,
                        value.B);
                var result = ProcessJavaScript(javascript);

                // never mind about whether operation is well-done, or rejected from PhotoShop,
                // 'cause this one is implemented as property,
                // raising exception when given action is not success is of no use
            }
        }

        /// <summary>
        /// get all documents's names opened in PhotoShop.
        /// null, when PhotoShop is not working, there's no document opened, etc
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        string[] PhotoShopAppAction.AllDocumentNames 
        { 
            get
            {
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");

                if (false == IsPhotoShopWorking)
                    return null;

                var result =
                    ProcessJavaScript(
                        JavascriptSnippet.PhotoShopApp.GET_ALL_DOCUMENT_NAMES);

                if (CommunicationStatus.OK != result.Status)
                    return null;
                var docNames = result.ReturnString;
                if (string.IsNullOrEmpty(docNames))
                    return null;
            
                return
                    docNames.Split(
                        new string[] { "\r", "\n" },
                        StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// get active document's name opened in PhotoShop.
        /// string.Empty, when PhotoShop is not working, there's no document opened, etc
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        string PhotoShopAppAction.ActiveDocumentName
        {
            get
            {
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");

                if (false == IsPhotoShopWorking)
                    return string.Empty;

                var result =
                    ProcessJavaScript(
                        JavascriptSnippet.PhotoShopApp.GET_ACTIVE_DOCUMENT_NAME);

                if (CommunicationStatus.OK != result.Status)
                    return string.Empty;

                return result.ReturnString;
            }
        }

        /// <summary>
        /// get active document's id opened in PhotoShop.
        /// -1, when PhotoShop is not working, there's no document opened, etc
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        int PhotoShopAppAction.ActiveDocumentID
        {
            get 
            { 
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");

                if (false == IsPhotoShopWorking)
                    return -1;

                var result =
                    ProcessJavaScript(
                        JavascriptSnippet.PhotoShopApp.GET_ACTIVE_DOCUMENT_ID);

                if (CommunicationStatus.OK != result.Status)
                    return -1;

                var strID = result.ReturnString;
                if (string.IsNullOrEmpty(strID))
                    return -1;

                int resultID = -1;
                var canConvert =
                    int.TryParse(strID, out resultID);

                return
                    (canConvert) ?
                    resultID :
                    -1;
            }
        }

        /// <summary>
        /// get/set active tool id of PhotoShop connected
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        string PhotoShopAppAction.ActiveToolID
        {
            get
            {
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");

                if (false == IsPhotoShopWorking)
                    return string.Empty;

                var result =
                    ProcessJavaScript(
                        JavascriptSnippet.PhotoShopApp.GET_ACTIVE_TOOL_ID);

                if (CommunicationStatus.OK != result.Status)
                    return string.Empty;

                return result.ReturnString;
            }
            set
            {
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");
                /*
                if (false == IsPhotoShopWorking)
                    throw new InvalidOperationException("PhotoShop is not working.");
                 */

                var javascript =
                    string.Format(
                        JavascriptSnippet.PhotoShopApp.SET_ACTIVE_TOOL_ID,
                        value);
                var result = ProcessJavaScript(javascript);

                // never mind about whether operation is well-done, or rejected from PhotoShop,
                // 'cause this one is implemented as property,
                // raising exception when given action is not success is of no use
            }
        }

        /// <summary>
        /// instruct PhotoShop to open given file
        /// </summary>
        /// <param name="fileName">
        /// file path which PhotoShop can understand.
        /// </param>
        /// <returns>
        /// true, if sucess, otherwise false
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// thrown when given <paramref name="fileName"/> is null or empty
        /// </exception>
        bool PhotoShopAppAction.OpenFile(
            string fileName)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            if (false == IsPhotoShopWorking)
                return false;

            if (string.IsNullOrEmpty(fileName))
                throw
                    new ArgumentNullException("psFilePath");

            var javascript =
                string.Format(
                    JavascriptSnippet.PhotoShopApp.OPEN_FILE,
                    fileName);

            var result = ProcessJavaScript(javascript);

            // in success, PhotoShop returns in "[Document {FileName}]" format.
            // in error(if given file path is wrong or something), 
            // PhotoShop returns NULL string
            if (string.IsNullOrEmpty(result.ReturnString))
                return false;

            var sentFileName =
                fileName.Substring(
                    fileName.LastIndexOf('/') + 1);
            var receivedFileName = // "[Document {FileName}]
                result.ReturnString
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Replace("Document ", string.Empty);

            return
                sentFileName == receivedFileName;
        }

        /// <summary>
        /// save current active document as given path
        /// </summary>
        /// <param name="fileName"> file path which PhotoShop can understand.</param>
        /// <returns>true, if sucess, otherwise false</returns>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// thrown when given <paramref name="fileName"/> is null or empty
        /// </exception>
        bool PhotoShopAppAction.SaveActiveDocumentAs(
            string fileName)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            if (false == IsPhotoShopWorking)
                return false;

            if (string.IsNullOrEmpty(fileName))
                throw
                    new ArgumentNullException("psFilePath");

            // is there active document?
            int docLength = -1;
            var docLengthResult =
                ProcessJavaScript(JavascriptSnippet.PhotoShopApp.GET_DOCUMENTS_LENGTH);
            if (docLengthResult.Status != CommunicationStatus.OK ||
                false == int.TryParse(docLengthResult.ReturnString, out docLength) ||
                1 > docLength)
            {
                return false;
            }

            // save as...
            var javascript =
                string.Format(
                    JavascriptSnippet.PhotoShopApp.SAVE_AS_ACTIVE_DOCUMENT,
                    fileName);

            var result = ProcessJavaScript(javascript);

            // if success, PhotoShop returns in "undefined" literal string.
            // in error(if given file path is wrong or something), 
            // PhotoShop returns NULL string
            if (string.IsNullOrEmpty(result.ReturnString))
                return false;

            return
                @"undefined" == result.ReturnString.ToLower();
        }

        /// <summary>
        /// send image to PS as JPEG format.
        /// if given file has image format other than JPEG,
        /// it's converted to JPEG and then transmitted to PS.
        /// (I gave up with generating PIXMAP you PS...)
        /// if given file is not of graphic format, do nothing.
        /// </summary>
        /// <param name="remoteImageFileName">graphic file to send to PS</param>
        /// <returns>true, successfully sent to PS, otherwise false</returns>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// thrown when <paramref name="remoteImageFileName"/> is null or empty
        /// </exception>
        /// <exception cref="ArgumentException">
        /// thrown when given file doesn't exist
        /// </exception>
        /// <exception cref="ArgumentException">
        /// thrown when given file is not of graphic format
        /// </exception>
        bool PhotoShopAppAction.LoadRemoteImage(
            string remoteImageFileName)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            if (false == IsPhotoShopWorking)
                return false;

            if (string.IsNullOrEmpty(remoteImageFileName))
                throw
                    new ArgumentNullException("imageFileName");

            if (false == File.Exists(remoteImageFileName))
                throw
                    new ArgumentException(
                        "File should exist.",
                        "imageFileName");

            byte[] toSend = null;
            using (var memStream = new MemoryStream())
            {
                using (var bwWriter = new BinaryWriter(memStream))
                {
                    try
                    {
                        using (var imgInfo = Image.FromFile(remoteImageFileName))
                        {
                            // in connection sdk document, images section says:
                            // "Embed an unsigned char of 1 for JPEG or 2 for Pixmap...."
                            bwWriter.Write((byte)1);
                            imgInfo.Save(memStream, ImageFormat.Jpeg);
                            bwWriter.Flush();

                            toSend = memStream.ToArray();
                        }
                    }
                    catch (Exception)
                    {
                        throw
                            new ArgumentException(
                                "File should be of graphic format.",
                                "fileName");
                    }
                }
            }

            try
            {
                var result =
                    SendAndReceive(
                        new DataBlock()
                        {
                            ProtocolVersion = PhotoShopConstants.PROTOCOL_VERSION,
                            // don't forget increment TransactionID!!!
                            TransactionID = this.TransactionID++,
                            ContentType = ContentType.IMAGE,
                            Content = toSend
                        });

                return CommunicationStatus.OK == result.Status;
            }
            catch (Exception)
            {
                return false;
            }
        }

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
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// thrown when given file doesn't exist, or when file-length is zero
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="remoteFileName"/> is null or empty
        /// </exception>
        string PhotoShopAppAction.LoadRemoteData(
            string remoteFileName)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            if (false == IsPhotoShopWorking)
                return string.Empty;

            if (string.IsNullOrEmpty(remoteFileName))
                throw
                    new ArgumentNullException("fileName");

            if (false == File.Exists(remoteFileName))
                throw
                    new ArgumentException(
                        "given file should exist",
                        "fileName");

            if (new FileInfo(remoteFileName).Length < 1)
                throw
                    new ArgumentException(
                        "file size should be greater than zero",
                        "fileName");

            using (var fileStream = new FileStream(remoteFileName, FileMode.Open))
            {
                return (this as PhotoShopAppAction).LoadRemoteData(fileStream);
            }
        }

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
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// thrown when <paramref name="remoteStream"/> is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// thrown when <paramref name="remoteStream"/> doesn't provide reading
        /// </exception>
        string PhotoShopAppAction.LoadRemoteData(
            Stream remoteStream)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            if (false == IsPhotoShopWorking)
                return string.Empty;

            if (null == remoteStream)
                throw
                    new ArgumentNullException("dataStream");

            if (false == remoteStream.CanRead)
                throw
                    new ArgumentException(
                        "given stream should support read",
                        "dataStream");

            int count = (int)remoteStream.Length;
            remoteStream.Seek(0, SeekOrigin.Begin);
            using (var brReader = new BinaryReader(remoteStream, Encoding.UTF8, true))
            {
                var result =
                    SendAndReceive(
                        new DataBlock()
                        {
                            ProtocolVersion = PhotoShopConstants.PROTOCOL_VERSION,
                            // don't forget increment TransactionID!!!
                            TransactionID = this.TransactionID++,
                            ContentType = ContentType.DATA,
                            Content = brReader.ReadBytes(count)
                        });
                return
                    (CommunicationStatus.OK == result.Status &&
                    false == string.IsNullOrEmpty(result.ReturnString)) ?
                        result.ReturnString :
                        string.Empty;
            }
        }

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
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
         Image PhotoShopAppAction.GetCurrentThumbnail(
            int width, 
            int height)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            if (false == IsPhotoShopWorking)
                return null;

            // is there active document?
            int docLength = -1;
            var docLengthResult =
                ProcessJavaScript(JavascriptSnippet.PhotoShopApp.GET_DOCUMENTS_LENGTH);
            if (docLengthResult.Status != CommunicationStatus.OK ||
                false == int.TryParse(docLengthResult.ReturnString, out docLength) ||
                1 > docLength)
            {
                return null;
            }

            // what's active document's id?
            int docID = -1;
            var docIDResult =
                ProcessJavaScript(JavascriptSnippet.PhotoShopApp.GET_ACTIVE_DOCUMENT_ID);
            if (docLengthResult.Status != CommunicationStatus.OK ||
                false == int.TryParse(docIDResult.ReturnString, out docID) ||
                1 > docID)
            {
                return null;
            }

            // execute javascript "send me image!" .
            var javascript =
                string.Format(
                    JavascriptSnippet.PhotoShopApp.GET_THUMBNAIL_IMAGE,
                    docID,
                    width,
                    height);
            var commResult =
                ProcessJavaScript(javascript);

            // Funny enough, when requesting image, 
            // another data block is sent from PS with same transaction id.
            // It seems that data block is sent from PS in following order.
            // 01. request thumbnail from client(like this) with javascripts(like above).
            // 02. request javascript ends with executeAction(...) which return type is ActionDescriptor.
            // 03. PhotoShop responding Image Byte array with Transaction ID
            // 04. PhotoShop responding final result of passed javascript, ie. responding ActionDescriptor.
            // in this case, the 04-th responding block is of no use. 
            // so, we neglect the final blcok of ActionDescriptor.
            // Since receiving message block(ActionDescriptor) is handled in event handing,
            // this one would be discarded.

            // return final image
            if (commResult.Status == CommunicationStatus.OK)
                return commResult.ResponseBlock.Content.DecodeArray();
            else
                return null;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="remoteFileName"></param>
        /// <returns></returns>
        bool PhotoShopAppAction.SaveActiveDocumentToRemote(
            string remoteFileName)
        {
            throw new NotImplementedException();
        }
    }
}
