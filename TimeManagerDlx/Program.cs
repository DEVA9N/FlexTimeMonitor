using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace TimeManagerDlx
{
	static class Program
	{
		/// <summary>
		/// Der Haupteinstiegspunkt für die Anwendung.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Prevent multiple instances
			bool isSingleInstance = false;
			Mutex singleInstanceMutex = new Mutex(true, "TimeManagerDlx", out isSingleInstance);

			if (isSingleInstance)
			{
				Application.Run(new MainWindow());
			}
			else
			{
				MessageBox.Show(null, "Another instance is already running.", "TimeManagerDlx");
			}

		}
	}
}
