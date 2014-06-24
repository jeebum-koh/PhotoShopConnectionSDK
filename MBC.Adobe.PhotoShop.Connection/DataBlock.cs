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
using System.Text;

namespace MBC.Adobe.PhotoShop.Connection
{
    /// <summary>
    /// values for content type
    /// </summary>
    public enum ContentType : int
    {
        /// <summary>
        /// not supported illegal content type
        /// </summary>
        ILLEGAL     = 0,
        /// <summary>
        /// error string type, PS generated
        /// </summary>
        ERRORSTRING = 1,
        /// <summary>
        /// javascript content type
        /// </summary>
        JAVASCRIPT  = 2,
        /// <summary>
        /// image content type
        /// </summary>
        IMAGE       = 3,
        /// <summary>
        /// profile content type, not implemented on PhotoShop side
        /// </summary>
        PROFILE     = 4,
        /// <summary>
        /// data content type
        /// </summary>
	    DATA        = 5,
    }

    /// <summary>
    /// Photoshop communication protocol definition 
    /// +-------------+---------+----------------------+
    /// | Unencrypted | 4 bytes | Length of messages   |   
    /// |             | 4 bytes | Communication Status |
    /// +-------------+---------+----------------------+
    /// | Encrypted   | 4 bytes | Protocol Version     |
    /// |             | 4 bytes | Transaction ID       |
    /// |             | 4 bytes | Content Type         |
    /// |             | n bytes | Content              |
    /// +-------------+---------+----------------------+
    /// ** NOTE : 
    /// Byte-order should be Big-Endian, for every 4 bytes integer value.
    /// ConnectionSDK Java sample doesn't explicitly express this, 
    /// but Java DataOutputStream assumes Big-Endian and
    /// constructing Protocol Version, Transaction ID, Content Type shows
    /// explicitly byte-order change in Java Sample.
    /// 
    /// This structure covers Encrypted part of protocol
    /// </summary>
    public struct DataBlock
    {
        /// <summary>
        /// Protocol Version
        /// </summary>
        public int ProtocolVersion;

        /// <summary>
        /// Transaction ID
        /// </summary>
        public int TransactionID;

        /// <summary>
        /// Content Type
        /// </summary>
        public ContentType ContentType;

        /// <summary>
        /// Raw byte array for Content
        /// </summary>
        public byte[] Content;

        /// <summary>
        /// create new <see cref="DataBlock"/> from given encrypted byte array
        /// </summary>
        /// <param name="decryptor">
        /// used for descryption of given encrypted byte array</param>
        /// <param name="encryptedBytes">
        /// encrypted byte array to decrypt</param>
        /// <returns>
        /// new <see cref="DataBlock"/> from given encrypted byte array
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// thrown when given "decryptor" parameter is null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// thrown when given "encryptedBytes" parameter is null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// thrown when given "encryptedBytes" parameter is shorter than 
        /// <seealso cref="PhotoShopConstants.PROTOCOL_LENGTH"/>
        /// </exception>
        public static DataBlock CreateNonErrorDataBlock(
            EncryptDecrypt decryptor,
            byte[] encryptedBytes)
        {
            // check for required conditions
            if (null == decryptor)
                throw new ArgumentNullException("decryptor");
            if (null == encryptedBytes)
                throw new ArgumentNullException("encryptedBytes");
            if (encryptedBytes.Length < PhotoShopConstants.PROTOCOL_LENGTH)
                throw new ArgumentOutOfRangeException("encryptedBytes");

            using (var msStream = new MemoryStream(decryptor.Decrypt(encryptedBytes)))
            {
                var result = new DataBlock();
                using (var brReader = new BinaryReader(msStream))
                {
                    // protocol version, transaction id, content type are passed
                    // with Big-Endian byte order
                    // so need to convert to host byte order, ie little-endian
                    result.ProtocolVersion = 
                        IPAddress.NetworkToHostOrder(
                            brReader.ReadInt32());
                    result.TransactionID = 
                        IPAddress.NetworkToHostOrder(
                            brReader.ReadInt32());
                    var contentTypeValue = 
                        IPAddress.NetworkToHostOrder(
                            brReader.ReadInt32());
                    switch (contentTypeValue)
                    {
                        case (int)ContentType.ERRORSTRING:
                        case (int)ContentType.JAVASCRIPT:
                        case (int)ContentType.IMAGE:
                        case (int)ContentType.PROFILE:
                        case (int)ContentType.DATA:
                            result.ContentType = (ContentType)contentTypeValue;
                            break;
                        default:
                            break;
                    }
                    var toRead = 
                        (int)(msStream.Length - PhotoShopConstants.PROTOCOL_LENGTH);
                    result.Content =
                        (0 == toRead) ?
                        null :
                        brReader.ReadBytes(toRead);

                    return result;
                }
            }
        }

        /// <summary>
        /// create new <see cref="DataBlock"/> from given error byte array, 
        /// ie plain byte array
        /// </summary>
        /// <param name="errorBytes">byte array containing raw data.</param>
        /// <returns>
        /// new <see cref="DataBlock"/> from given error byte array
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// thrown when given "errorBytes" parameter is null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// thrown when given "errorBytes" parameter is shorter than 
        /// <seealso cref="PhotoShopConstants.PROTOCOL_LENGTH"/>
        /// </exception>
        public static DataBlock CreateErrorDataBlock(
            byte[] errorBytes)
        {
            // check for required conditions
            if (null == errorBytes)
                throw new ArgumentNullException("errorBytes");
            if (errorBytes.Length < PhotoShopConstants.PROTOCOL_LENGTH)
                throw new ArgumentOutOfRangeException("errorBytes");

            using (var msStream = new MemoryStream(errorBytes))
            {
                var result = new DataBlock();
                using (var brReader = new BinaryReader(msStream))
                {
                    // never mind protocol version, transaction id, content type.
                    result.ProtocolVersion = 
                        IPAddress.NetworkToHostOrder(
                            brReader.ReadInt32());
                    result.TransactionID = 
                        IPAddress.NetworkToHostOrder(
                            brReader.ReadInt32());
                    // meaningless operation, just move stream pointer forward.
                    var contentTypeValue = 
                        IPAddress.NetworkToHostOrder(
                            brReader.ReadInt32());
                    
                    // context assumes that comming content is error string type.
                    result.ContentType = ContentType.ERRORSTRING;

                    var toRead = 
                        (int)(msStream.Length - PhotoShopConstants.PROTOCOL_LENGTH);
                    result.Content = 
                        (0 == toRead) ?
                        null :
                        brReader.ReadBytes(toRead);

                    return result;
                }
            }
        }

        /// <summary>
        /// generate byte array from current object's fields's values.
        /// </summary>
        /// <returns>
        /// byte array representation of current object, not encrypted.
        /// </returns>
        /// <exception cref="InvalidDataException">
        /// thrown when <see cref="ContentType"/> is 
        /// <see cref="MBC.Adobe.PhotoShop.Connection.ContentType.ILLEGAL"/>
        /// </exception>
        /// <exception cref="InvalidDataException">
        /// thrown when <see cref="Content"/> is null or shorter than 
        /// <seealso cref="PhotoShopConstants.PROTOCOL_LENGTH"/>
        /// </exception>
        public byte[] GetPlainBuffer()
        {
            // check for required conditions
            if (ContentType.ILLEGAL == ContentType)
                throw 
                    new InvalidDataException(
                        "contentType - Not allowed for ILLEGAL_TYPE");
            
            if (null == Content ||
                Content.Length < 1)
                throw 
                    new InvalidDataException(
                        "content - Not allowed for null content");

            using (var msBuffer = new MemoryStream())
            {
                using(var bwWriter = new BinaryWriter(msBuffer))
	            {
                    // Protocol Version 
                    bwWriter.Write(
                        IPAddress.HostToNetworkOrder(
                            ProtocolVersion));

                    // Transaction ID
                    bwWriter.Write(
                        IPAddress.HostToNetworkOrder(
                            TransactionID));

                    // Content Type
                    bwWriter.Write(
                        IPAddress.HostToNetworkOrder(
                            (int)ContentType));

                    // Content
                    bwWriter.Write(Content, 0, Content.Length);

                    // Line-Feed to terminate if content type is javascript
                    if (ContentType.JAVASCRIPT == this.ContentType)
                        bwWriter.Write((byte)0x0a);

                    return msBuffer.ToArray();
	            }
            }
        }

        /// <summary>
        /// generate encrypted byte array from current object's fields's values.
        /// </summary>
        /// <param name="encryptor">used for enscryption</param>
        /// <returns>
        /// encrypted byte array representation of current object
        /// </returns>
        public byte[] GetEncryptedBuffer(
            EncryptDecrypt encryptor)
        {
            return encryptor.Encrypt(GetPlainBuffer());
        }

        /// <summary>
        /// generate string representation of 
        /// current object's <see cref="Content"/>
        /// </summary>
        /// <returns></returns>
        public string GetContentString()
        {
            if (null == Content ||
                0 == Content.Length)
                return string.Empty;

            switch (ContentType)
            {
                case ContentType.ERRORSTRING:
                case ContentType.JAVASCRIPT:
                    return Encoding.UTF8.GetString(Content);
                case ContentType.IMAGE:
                    return "IMAGE";
                case ContentType.PROFILE:
                    return "PROFILE";
                case ContentType.DATA:
                    return "DATA";
                case ContentType.ILLEGAL:
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// override to <seealso cref="Object.ToString"/>
        /// </summary>
        /// <returns>string representation of current object</returns>
        public override string ToString()
        {
            return
                ToString(true);
        }

        /// <summary>
        /// ToString variant, user can control to display byte representation or not.
        /// </summary>
        /// <param name="includeByteRepresentation">
        /// true, to include byte representation, false, not
        /// </param>
        /// <returns>
        /// string representation of current object
        /// </returns>
        public string ToString(
            bool includeByteRepresentation)
        {
            return
                string.Format(
                    "ProtocolVersion : {0} " + Environment.NewLine +
                    "TransactionID : {1} " + Environment.NewLine +
                    "ContentType : {2} " + Environment.NewLine +
                    "Content : {3}",
                    this.ProtocolVersion,
                    this.TransactionID,
                    this.ContentType,
                    (null == this.Content || this.Content.Length < 1) ?
                        "NULL" :
                        (includeByteRepresentation) ?
                            Environment.NewLine + 
                                this.Content.GetHexaDisplayString("    ") :
                            this.Content.Length.ToString() + " BYTES");

        }
    }
}
