using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace A9N.FlexTimeMonitor
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    internal class NotificationIcon
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private const int BalloonTimeOut = 3000;
        private readonly NotifyIcon _notificationIcon;
        private readonly string _applicationName;
        private readonly Action _restoreWindow;
        private readonly Func<String> _getBalloonText;

        public NotificationIcon(String applicationName, Icon applicationIcon, Action restoreWindow, Func<String> getBalloonText)
        {
            if (applicationIcon is null)
            {
                throw new ArgumentNullException(nameof(applicationIcon));
            }

            _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
            _restoreWindow = restoreWindow ?? throw new ArgumentNullException(nameof(restoreWindow));
            _getBalloonText = getBalloonText ?? throw new ArgumentNullException(nameof(getBalloonText));

            _notificationIcon = new NotifyIcon { Icon = applicationIcon, Text = applicationName, Visible = true };
            _notificationIcon.MouseClick += notificationIcon_MouseClick;
            _notificationIcon.MouseDoubleClick += notificationIcon_MouseDoubleClick;
        }

        private void notificationIcon_MouseClick(object sender, MouseEventArgs e)
        {
            var text = _getBalloonText();

            if (e.Button == MouseButtons.Right && !String.IsNullOrEmpty(text))
            {
                _notificationIcon.ShowBalloonTip(BalloonTimeOut, _applicationName, text, ToolTipIcon.Info);
            }
        }

        private void notificationIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            _restoreWindow();
        }
    }
}
