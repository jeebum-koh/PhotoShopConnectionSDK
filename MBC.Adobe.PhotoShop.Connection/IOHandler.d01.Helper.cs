/* MBC, the number one terrestrial broadcast station in Korea (Republic of) ***
 * 
 * History
 * ----------------------------------------------------------------------------
 *   2014.06.24, JeeBum Koh, Initial release
 *   
 * 
 ******************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MBC.Adobe.PhotoShop.Connection
{
    #region MISC data types
    /// <summary>
    /// Status of Communication with PhotoShop
    /// </summary>
    public enum CommunicationStatus
    {
        /// <summary>
        /// no error
        /// </summary>
        OK,
        /// <summary>
        /// unexpected exception was raised from sending and receiving data
        /// </summary>
        ERROR_COMMUNICATION,
        /// <summary>
        /// PhotoShop explicitly reports error
        /// </summary>
        ERROR_PS_REPORT,
        /// <summary>
        /// no data is received from PhotoShop
        /// </summary>
        NO_DATA,
    }

    /// <summary>
    /// encapsulation of Communication with PhotoShop
    /// </summary>
    public struct PhotoShopResponse
    {
        /// <summary>
        /// Status of Communication with PhotoShop
        /// </summary>
        public CommunicationStatus Status;
        /// <summary>
        /// contains decrypted PhotoShop response 
        /// </summary>
        public DataBlock ResponseBlock;

        /// <summary>
        /// Error string received from PhotoShop
        /// </summary>
        public string ErrorString
        {
            get
            {
                if (ContentType.ERRORSTRING != ResponseBlock.ContentType)
                    return string.Empty;

                var strBytes = ResponseBlock.Content;

                if (null == strBytes ||
                    strBytes.Length < 1)
                    return string.Empty;

                return Encoding.UTF8.GetString(strBytes);
            }
        }

        /// <summary>
        /// Return string received from PhotoShop
        /// </summary>
        public string ReturnString
        {
            get
            {
                if (ContentType.JAVASCRIPT != ResponseBlock.ContentType)
                    return string.Empty;

                var strBytes = ResponseBlock.Content;

                if (null == strBytes ||
                    strBytes.Length < 1)
                    return string.Empty;

                return Encoding.UTF8.GetString(strBytes);
            }
        }

        /// <summary>
        /// Canonical override to <seealso cref="Object.ToString"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                string.Format(
                    "Status : {0}" + Environment.NewLine +
                    "Recv Data : {1}",
                    Status,
                    (string.IsNullOrEmpty(ErrorString)) ?
                        ((string.IsNullOrEmpty(ReturnString)) ?
                            Environment.NewLine + ResponseBlock.ToString() :
                            ReturnString) :
                        ErrorString);
        }
    }
    #endregion MISC data types

    partial class IOHandler
    {
        /// <summary>
        /// writes as run log
        /// </summary>
        /// <param name="runLog">log message</param>
        /// <param name="byteArray">
        /// byte array to display, if any
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        private void writeAsRunLog(
            string runLog,
            byte[] byteArray = null)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            var logger = AsRunLogger;
            if (null == logger)
                return;

            lock (logger)
            {
                logger.WriteLine(runLog);

                if (null == byteArray ||
                    byteArray.Length < 1)
                    return;

                if (true == ShowByteDisplayInAsRunLog)
                    logger.WriteLine(
                        byteArray.GetHexaDisplayString());

                logger.WriteLine();
            }
        }

        /// <summary>
        /// writes as run log
        /// </summary>
        /// <param name="runLog">log message</param>
        /// <param name="toPrint">log data if any</param>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        private void writeAsRunLog(
            string runLog,
            DataBlock toPrint)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            var logger = AsRunLogger;
            if (null == logger)
                return;

            lock (logger)
            {
                logger.WriteLine(runLog);

                logger.WriteLine(
                    toPrint.ToString(
                        ShowByteDisplayInAsRunLog));

                logger.WriteLine();
            }
        }

        /// <summary>
        /// inner operation for sending data to PS.
        /// In this context, locking is handled upward(in calling context),
        /// so we don't care about synchronization here.
        /// </summary>
        /// <param name="toSend">data to be sent to PS</param>
        /// <returns>
        /// true, when sending operation succeeded, so we can proceed forward.
        /// else communication error arised, so we should stop proceeding.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoking on disposed object
        /// </exception>
        private bool sendDataBlock(
            DataBlock toSend)
        {
            // 01. check if disposed
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            // 02. prepare data to send
            var plainBytes = toSend.GetPlainBuffer();
            writeAsRunLog(
                "Bytes to encrypt: " + plainBytes.Length,
                plainBytes);

            var encryptedBytes = toSend.GetEncryptedBuffer(_encryptDecrypt);
            writeAsRunLog(
                "Bytes encrypted: " + encryptedBytes.Length,
                encryptedBytes);

            try
            {
                // 03. send data to PS
                using (
                    var bwWriter =
                        new BinaryWriter(
                            _netStream,
                            Encoding.UTF8,
                            true))
                {
                    // send length value in network order
                    var lengthValue =
                        IPAddress.HostToNetworkOrder(
                            (int)PhotoShopConstants.COMM_LENGTH +
                                encryptedBytes.Length);
                    bwWriter.Write((int)lengthValue);

                    // send status value in network order
                    var statusValue =
                        IPAddress.HostToNetworkOrder(
                            (int)PhotoShopConstants.NO_COMM_ERROR);
                    bwWriter.Write((int)statusValue);

                    // send encrypted data
                    bwWriter.Write(
                        encryptedBytes,
                        0,
                        encryptedBytes.Length);

                    bwWriter.Flush();
                }

                return true;
            }
            catch (Exception e)
            {
                var strOut =
                    string.Format(
                        "{0} raised during data sending to PS \n" +
                            "with message \"{1}\"",
                        e.GetType().Name,
                        e.Message);
                writeAsRunLog(strOut);

                return false;
            }
        }

        /// <summary>
        /// get data from PS
        /// In this context, locking is handled upward(in calling context),
        /// so we don't care about synchronization here.
        /// Also in this context, checking availability of receving data 
        /// is handled upward(in calling context).
        /// </summary>
        /// <returns>
        /// data received from PS or error indicator
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoking on disposed object
        /// </exception>
        private PhotoShopResponse receivePhotoResponse()
        {
            // 01. check if disposed
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            try
            {
                // 02. receive data from PS
                using (
                    var brReader =
                        new BinaryReader(
                            _netStream,
                            Encoding.UTF8,
                            true))
                {
                    // receive data-length from PS, 
                    // careful about byte order, it's Network_order!!
                    var inLength =
                        IPAddress.NetworkToHostOrder(
                            brReader.ReadInt32());
                    writeAsRunLog("Reading length: " + inLength);

                    // receive communication status from PS,
                    // careful about byte order, it's Network_order!!
                    var inCommStatus =
                        IPAddress.NetworkToHostOrder(
                            brReader.ReadInt32());
                    writeAsRunLog("Comm Status : " + inCommStatus);

                    // receive response data from PS
                    var replyBytes =
                        brReader.ReadBytes(
                            inLength - PhotoShopConstants.COMM_LENGTH);

                    return
                        constructPhotoShopResponse(inCommStatus, replyBytes);
                }
            }
            catch (Exception e)
            {
                var strOut =
                    string.Format(
                        "{0} raised during data receiving from PS \n" +
                            "with message \"{1}\"",
                        e.GetType().Name,
                        e.Message);
                writeAsRunLog(strOut);

                return
                    new PhotoShopResponse()
                    {
                        Status = CommunicationStatus.ERROR_COMMUNICATION
                    };
            }
        }

        /// <summary>
        /// construct new <see cref="PhotoShopResponse"/> from given parameters.
        /// </summary>
        /// <param name="commStatus">
        /// communication status returned from PhotoShop
        /// </param>
        /// <param name="replyBytes">
        /// byte array returned from PhotoShop
        /// </param>
        /// <returns>
        /// new <see cref="PhotoShopResponse"/> from given parameters.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// thrown when <paramref name="replyBytes"/> is null
        /// </exception>
        private PhotoShopResponse constructPhotoShopResponse(
            int commStatus,
            byte[] replyBytes)
        {
            if (null == replyBytes)
                throw
                    new ArgumentNullException("replyBytes");

            try
            {
                switch (commStatus)
                {
                    // when PS reports no-error,
                    case PhotoShopConstants.NO_COMM_ERROR:
                        writeAsRunLog(
                            "Read this encrypted message : " +
                                replyBytes.Length,
                            replyBytes);

                        var recvDataBlock =
                            DataBlock.CreateNonErrorDataBlock(
                                _encryptDecrypt,
                                replyBytes);
                        writeAsRunLog(
                            "Decrypted Message : ",
                            recvDataBlock);

                        var resultOK =
                            new PhotoShopResponse()
                            {
                                Status = CommunicationStatus.OK,
                                ResponseBlock = recvDataBlock,
                            };
                        writeAsRunLog(
                            "PS Response" + Environment.NewLine +
                                resultOK.ToString());

                        return resultOK;
                    // when PS reports error of its own
                    default:
                        writeAsRunLog(
                            "Read this error message : " +
                                replyBytes.Length,
                            replyBytes);

                        var errorBlock =
                            DataBlock.CreateErrorDataBlock(replyBytes);
                        var resultError =
                            new PhotoShopResponse()
                            {
                                Status = CommunicationStatus.ERROR_PS_REPORT,
                                ResponseBlock = errorBlock
                            };
                        writeAsRunLog(
                            "PS Response" + Environment.NewLine +
                                resultError.ToString());

                        return resultError;
                }
            }
            catch
            {
                return
                    new PhotoShopResponse()
                    {
                        Status = CommunicationStatus.ERROR_COMMUNICATION
                    };
            }
        }

        /// <summary>
        /// get desired <see cref="PhotoShopResponse"/> with given transaction id.
        /// based on blockIO parameter, it operates only peeking or holding.
        /// </summary>
        /// <param name="desiredTransactionID">
        /// transaction id which context waits.
        /// </param>
        /// <returns>
        /// <see cref="PhotoShopResponse"/> with desired transaction id.
        /// calling thread is blocked until desired transaction id is received
        /// if there's no such response, 
        /// <see cref="PhotoShopResponse"/> with <see cref="CommunicationStatus.NO_DATA"/>
        /// is returned.
        /// </returns>
        private PhotoShopResponse getPhotoShopResponseUntil(
            int desiredTransactionID)
        {
            lock (_netStream)
            {
                var responses = new List<PhotoShopResponse>();

                try
                {
                    while (true)
                    {
                        // check whether data is available in NetStream
                        if (false == DataAvailable)
                        {
                            // do nothing just waiting reply...
                        }
                        else
                        {
                            // receive packet from PhotoShop
                            var received = receivePhotoResponse();

                            // check whether received is invalid data
                            if (CommunicationStatus.ERROR_COMMUNICATION == received.Status)
                                return received;

                            // if given data is what we are waiting for?
                            if (desiredTransactionID == received.ResponseBlock.TransactionID)
                            {
                                // if there're PhotoShopResponses stacked up, 
                                // invoke that in another thread.
                                if (responses.Count > 0)
                                {
                                    ThreadPool.QueueUserWorkItem(
                                        (obj) =>
                                        {
                                            Thread.Sleep(500);
                                            foreach (var item in responses)
                                            {
                                                processNotification(item);
                                            }
                                        });
                                }

                                return received;
                            }

                            // otherwise, we do processing "received" based on 
                            // the assumtion that PhotoShop sent us event-fired notification.
                            // To avoid race condition in thread synchronization, 
                            // stack up all responses and process in another thread.
                            responses.Add(received);
                        }

                        // waiting a while
                        Thread.Sleep(10);
                    }
                }
                catch
                {
                    return
                        new PhotoShopResponse()
                        {
                            Status = CommunicationStatus.ERROR_COMMUNICATION
                        };
                }
            }
        }

        /// <summary>
        /// base function for communication with PhotoShop
        /// </summary>
        /// <param name="toSend">
        /// <see cref="DataBlock"/> containing data to pass to PhotoShop</param>
        /// <returns>PhotoShop Response w.r.t <paramref name="toSend"/></returns>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoking on disposed object
        /// </exception>
        private PhotoShopResponse SendAndReceive(
            DataBlock toSend)
        {
            // 01. check if object is disposed
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            var commError =
                new PhotoShopResponse()
                {
                    Status = CommunicationStatus.ERROR_COMMUNICATION
                };

            lock (_netStream)
            {
                // 02. send data to PS
                if (false == sendDataBlock(toSend))
                    return commError;

                // 03. receive data from PS for dedicated transaction ID
                return
                    getPhotoShopResponseUntil(toSend.TransactionID);
            }
        }

        /// <summary>
        /// check if underlying socket has data to read.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoking on disposed object
        /// </exception>
        private bool DataAvailable
        {
            get
            {
                // 01. check if object is disposed
                if (IsDisposed)
                    throw
                        new ObjectDisposedException("IOHandler");

                return _netStream.DataAvailable;
            }
        }
    }
}
