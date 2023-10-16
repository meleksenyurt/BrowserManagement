using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BrowserCloseForSpecficTabOrApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			List<IntPtr> ChromeWindows = WindowsFinder("Chrome_WidgetWin_1", "chrome");//ClassName Alternatives for Chrome Chrome_WidgetWin_0,Chrome_WidgetWin_1
			List<string> nameList = new List<string>();
			foreach (IntPtr windowHandle in ChromeWindows)
			{
				int length = GetWindowTextLength(windowHandle);
				StringBuilder sb = new StringBuilder(length + 1);
				GetWindowText(windowHandle, sb, sb.Capacity);
				nameList.Add(sb.ToString());
				if (sb.ToString() == "GitHub")
				{
					CloseWindow(windowHandle);
				}
			}
		}
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

		private const UInt32 WM_CLOSE = 0x0010;

		void CloseWindow(IntPtr hwnd)
		{
			SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
		}
		[DllImport("user32.dll")]
		private static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("User32", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr windowHandle, StringBuilder stringBuilder, int nMaxCount);

		[DllImport("user32.dll", EntryPoint = "GetWindowTextLength", SetLastError = true)]
		internal static extern int GetWindowTextLength(IntPtr hwnd);

		private static List<IntPtr> windowList;
		private static string _className;
		private static StringBuilder apiResult = new StringBuilder(256); //256 Is max class name length.
		private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

		static List<string> nameAlternatives = new List<string>();
		private static List<IntPtr> WindowsFinder(string className, string process)
		{
			_className = className;
			windowList = new List<IntPtr>();

			Process[] chromeList = Process.GetProcessesByName(process);

			if (chromeList.Length > 0)
			{
				foreach (Process chrome in chromeList)
				{
					if (chrome.MainWindowHandle != IntPtr.Zero)
					{
						foreach (ProcessThread thread in chrome.Threads)
						{
							EnumThreadWindows((uint)thread.Id, new EnumThreadDelegate(EnumThreadCallback), IntPtr.Zero);
						}
					}
				}
			}

			return windowList;
		}

		static bool EnumThreadCallback(IntPtr hWnd, IntPtr lParam)
		{
			nameAlternatives.Add(apiResult.ToString());
			if (GetClassName(hWnd, apiResult, apiResult.Capacity) != 0)
			{
				//if (string.CompareOrdinal(apiResult.ToString(), _className) == 0)
				//{
				windowList.Add(hWnd);
				//}
			}
			return true;
		}

	}
}