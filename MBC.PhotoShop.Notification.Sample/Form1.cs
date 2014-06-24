using MBC.Adobe.PhotoShop.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MBC.PhotoShop.Notification.Sample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        IOHandler _handler = null;

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (null != _handler)
                return;

            _handler = IOHandler.CreateNew(txtPassword.Text, txtIPAddress.Text);

            _handler.subscribeEvent(
                new HashSet<PhotoShopNotification>()
                {
                    PhotoShopNotification.foregroundColorChanged,
                    PhotoShopNotification.backgroundColorChanged,
                    PhotoShopNotification.toolChanged,
                });

            var context = SynchronizationContext.Current;
            _handler.PhotoShopNotificationProc = 
                (evtEnum, message) =>
                {
                    context.Send(
                        (obj) =>
                        {
                            switch (evtEnum)
                            {
                                case PhotoShopNotification.foregroundColorChanged:
                                    pbForegroundColor.BackColor = message.convertToColor();
                                    break;
                                case PhotoShopNotification.backgroundColorChanged:
                                    pbBackgroundColor.BackColor = message.convertToColor();
                                    break;
                                case PhotoShopNotification.toolChanged:
                                    txtActiveToolName.Text = PhotoShopTools.getToolName(message);
                                    break;
                                default:
                                    break;
                            }
                        },
                        null);
                };

            var app = _handler.App;
            pbForegroundColor.BackColor = app.ForegroundColor;
            pbBackgroundColor.BackColor = app.BackgroundColor;
            txtActiveToolName.Text = PhotoShopTools.getToolName(app.ActiveToolID);
        }
    }
}
