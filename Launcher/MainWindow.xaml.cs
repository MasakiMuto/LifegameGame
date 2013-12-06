using System;
using System.Collections.Generic;
using System.Linq;
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
using LifegameGame;

namespace Launcher
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			var name = System.Net.Dns.GetHostName();
			var host = System.Net.Dns.GetHostEntry(name);
			this.MyIPLabel.Content = host.AddressList.Last().ToString();
			this.IPAddress.Text = host.AddressList.Last().ToString();
			this.PortNumber.Text = (2001).ToString();
			Program.SetTrace();
		}

		[STAThread]
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var arg = new LaunchArgment()
			{
				IsOnline = this.IsOnline.IsChecked.Value,
			};
			if (arg.IsOnline)
			{
				arg.Connection = new ConnectionInfo()
				{
					IsHost = this.IsHost.IsChecked.Value,
					Port = Int32.Parse(this.PortNumber.Text),
					TargetIP = System.Net.IPAddress.Parse(this.IPAddress.Text)
				};
			}
			using (var game = new LifegameGame.Game1(arg))
			{
				game.Run();
			}
		}
	}
}
