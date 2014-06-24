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
using System.Text;
using System.Threading;

namespace MBC.Adobe.PhotoShop.Connection
{
    /// <summary>
    /// enumeration for change notification PhotoShop possibly sends
    /// </summary>
    public enum PhotoShopNotification
    {
        /// <summary>
        /// default value for PhotoShopNotification.
        /// PhotoShop doesn't send this.
        /// </summary>
        INVALID_NOTIFICATION,
        /// <summary>
        /// fore-ground color changed
        /// </summary>
        foregroundColorChanged,
        /// <summary>
        /// back-ground color changed
        /// </summary>
        backgroundColorChanged,
        /// <summary>
        /// tool changed
        /// </summary>
        toolChanged,
        /// <summary>
        /// document closed
        /// </summary>
        closedDocument,
        /// <summary>
        /// new document view created
        /// </summary>
        newDocumentViewCreated,
        /// <summary>
        /// current document changed
        /// </summary>
        currentDocumentChanged,
        /// <summary>
        /// active view changed
        /// </summary>
        activeViewChanged,
        /// <summary>
        /// document names changed
        /// </summary>
        documentNamesChanged,
        /// <summary>
        /// color settings changed
        /// </summary>
        colorSettingsChanged,
        /// <summary>
        /// keyboard shortcuts changed
        /// </summary>
        keyboardShortcutsChanged,
        /// <summary>
        /// preferences changed
        /// </summary>
        preferencesChanged,
        /// <summary>
        /// quick mask state changed
        /// </summary>
        quickMaskStateChanged,
        /// <summary>
        /// screen mode changed
        /// </summary>
        screenModeChanged,
        /// <summary>
        /// gaussianBlur
        /// </summary>
        gaussianBlur,
    }

    partial class IOHandler
    {
        /// <summary>
        /// queue <see cref="checkNotification"/> into ThreadPool.
        /// If disposed, no more queue up.
        /// </summary>
        private void queueCheckNotification()
        {
            if (IsDisposed)
                return;

            ThreadPool.QueueUserWorkItem(checkNotification);
        }

        /// <summary>
        /// check communication socket to see there's notification pending 
        /// </summary>
        /// <param name="obj">NO USE</param>
        private void checkNotification(
            object obj)
        {
            if (IsDisposed)
                return;

            lock (_netStream)
            {
                try
                {
                    while (true)
                    {
                        // check whether data is available in NetStream
                        if (false == DataAvailable)
                            break;

                        // receive packet from PhotoShop
                        var received = receivePhotoResponse();

                        // check whether received is invalid data
                        if (CommunicationStatus.ERROR_COMMUNICATION == received.Status)
                            break;

                        try
                        {
                            processNotification(received);
                        }
                        catch
                        {
                            // do nothing
                        }

                        // waiting a while
                        Thread.Sleep(10);
                    }
                }
                catch
                {
                    // if error is happened, let's assume it's from communication-level error,
                    // which we cannot recover unless new communication channel is made.
                    return;
                }
            }

            Thread.Sleep(100);

            queueCheckNotification();
        }

        /// <summary>
        /// change notification from PhotoShop is sent with transaction id,
        /// which is the transaction id when subscribing given event.
        /// Let's say, we subscribe foregroundColorChanged with transaction id 123,
        /// and toolChanged with transaction id 567.
        /// Then PhotoShop sends us foregroundColorChanged with transaction id 123,
        /// and toolChanged with transaction id 567 respectively.
        /// </summary>
        /// <param name="response">
        /// <see cref="PhotoShopResponse"/> to handle.
        /// </param>
        private void processNotification(
            PhotoShopResponse response)
        {
            if (CommunicationStatus.OK != response.Status)
                return;
            var isEventNotification =
                (1 + _notificationSubscribedTransactionID) >
                response.ResponseBlock.TransactionID;
            if (false == isEventNotification)
                return;

            var actionToInvoke = PhotoShopNotificationProc;
            if (null == actionToInvoke)
                return;

            var strNotification =
                response.ReturnString
                .Split(
                    new string[] { "\r", "\n" },
                    StringSplitOptions.RemoveEmptyEntries);
            if (null == strNotification ||
                strNotification.Length < 1)
                return;

            var psEvent = PhotoShopNotification.INVALID_NOTIFICATION;
            var isValidPhotoShopEvent =
                Enum.TryParse<PhotoShopNotification>(strNotification[0], out psEvent);
            //if (false == isValidPhotoShopEvent)
            //    return;
            var extraData =
                (strNotification.Length < 2) ?
                string.Empty :
                strNotification[1];

            try
            {
                switch (psEvent)
                {
                    case PhotoShopNotification.foregroundColorChanged:
                    case PhotoShopNotification.backgroundColorChanged:
                    case PhotoShopNotification.toolChanged:
                    case PhotoShopNotification.closedDocument:
                    case PhotoShopNotification.newDocumentViewCreated:
                    case PhotoShopNotification.currentDocumentChanged:
                    case PhotoShopNotification.activeViewChanged:
                    case PhotoShopNotification.documentNamesChanged:
                    case PhotoShopNotification.colorSettingsChanged:
                    case PhotoShopNotification.keyboardShortcutsChanged:
                    case PhotoShopNotification.preferencesChanged:
                    case PhotoShopNotification.quickMaskStateChanged:
                    case PhotoShopNotification.screenModeChanged:
                    case PhotoShopNotification.gaussianBlur:
                        actionToInvoke(psEvent, extraData);
                        break;
                    default:
                        actionToInvoke(PhotoShopNotification.INVALID_NOTIFICATION, response.ReturnString);
                        break;
                }
            }
            catch
            {
                // do nothing.
            }
        }

        /// <summary>
        /// generate event subscription javascript.
        /// </summary>
        /// <param name="eventSet">
        /// collection of event-type to subscribe.
        /// </param>
        /// <returns>
        /// newly generated javascript snippet.
        /// </returns>
        private string generateEventSubscriptionJavascript(
            HashSet<PhotoShopNotification> eventSet)
        {
            if (null == eventSet ||
                eventSet.Count < 1)
                throw
                    new ArgumentNullException("eventSet");

            StringBuilder build = new StringBuilder();
            build.AppendLine(JavascriptSnippet.PSEvent.SUBSCRIBE_EVENT_HEADER);
            foreach (var item in eventSet)
            {
                build.AppendLine(
                    string.Format(
                        JavascriptSnippet.PSEvent.SUBSCRIBE_EVENT_BODY,
                        item.ToString() + "_desc",
                        item.ToString()));
            }
            build.AppendLine(JavascriptSnippet.PSEvent.SUBSCRIBE_EVENT_FOOTER);

            return build.ToString();
        }

        /// <summary>
        /// variable indicating whether event subscription and 
        /// queueing up <see cref="queueCheckNotification"/> was invoked.
        /// </summary>
        private bool _notificationSubscribed = false;

        /// <summary>
        /// records the transaction id which passed to PhotoShop when notification subscribing.
        /// </summary>
        private int _notificationSubscribedTransactionID = -1;

        /// <summary>
        /// subscribe change notification to PhotoShop
        /// </summary>
        /// <param name="eventSet">
        /// collection of event-type to subscribe.
        /// </param>
        /// <returns>
        /// true, if subscription was successful.
        /// false otherwise
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        public bool subscribeEvent(
            HashSet<PhotoShopNotification> eventSet)
        {
            if (IsDisposed)
                throw
                    new ObjectDisposedException("IOHandler");

            if (false == IsPhotoShopWorking)
                return false;

            var javascript = generateEventSubscriptionJavascript(eventSet);
            var result = ProcessJavaScript(javascript);

            var isSuccess =
                CommunicationStatus.OK == result.Status ||
                result.ReturnString == JavascriptSnippet.PSEvent.SUBSCRIBE_EVENT_SUCCESS;
            if (isSuccess)
            {
                if (false == _notificationSubscribed)
                    queueCheckNotification();
                _notificationSubscribed = true;
                _notificationSubscribedTransactionID = result.ResponseBlock.TransactionID;
            }
            return isSuccess;
        }

        /// <summary>
        /// subscribe change notification to PhotoShop
        /// </summary>
        /// <param name="evt">event-type to subscribe.</param>
        /// <returns>
        /// true, if subscription was successful.
        /// false otherwise
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// thrown when invoked on disposed object.
        /// </exception>
        public bool subscribeEvent(
            PhotoShopNotification evt)
        {
            return 
                subscribeEvent(
                    new HashSet<PhotoShopNotification>() 
                    { 
                        evt 
                    });
        }
    }
}
