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
			var p1 = (player1Group.Content as StackPanel).Children.Cast<UIElement>();
			for (int i = 0; i < 2; i++)
			{
				var list = GetPlayerConfigGroup(i).OfType<ListBox>().First();
				foreach (var item in Enum.GetValues(typeof(PlayerType)).Cast<PlayerType>())
				{
					list.Items.Add(new ListBoxItem()
					{
						Content = item
					});
				}
				list.SelectedIndex = 0;
			}
			
		}

		IEnumerable<UIElement> GetPlayerConfigGroup(int index)
		{
			return ((index == 0 ? player1Group : player2Group).Content as StackPanel).Children.Cast<UIElement>();
		}

		[STAThread]
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			bool isOnline = bool.Parse((sender as Button).Tag as string);
			var arg = this.DataContext as LaunchArgment;
			if (arg.IsOnline)
			{
				arg.Connection = new ConnectionInfo()
				{
					IsHost = this.IsHost.IsChecked.Value,
					Port = Int32.Parse(this.PortNumber.Text),
					TargetIP = System.Net.IPAddress.Parse(this.IPAddress.Text)
				};
			}
			else
			{
				Func<int, PlayerType> getType = i => (PlayerType)((GetPlayerConfigGroup(i).OfType<ListBox>().First().SelectedItem as ListBoxItem).Content);
				//var o = getType(0);
				arg.Player1 = getType(0);
				arg.Player2 = getType(1);
			}
			using (var game = new LifegameGame.Game1(arg))
			{
				game.Run();
			}
		}
	}
}
