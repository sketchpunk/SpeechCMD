using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using System.Threading;

namespace SpeechCMD{
	public class ProfileItem : sp.data.INotifyBase{
		public string Name { get; set; }
		private bool _IsLoaded = false;
		public bool IsLoaded { get{ return _IsLoaded; } set{ _IsLoaded = value ; RaisePropertyChanged(); }}
	}//struct

	public partial class MainWindow : Window {
		#region Init / Variables
			private ObservableCollection<ProfileItem> mProfileItems;
			private ContextMenu mProfileItemMenu;
			public MainWindow() { InitializeComponent(); }//func
		#endregion

		#region Window Events
			private void Window_Loaded(object sender, RoutedEventArgs e){
				//var oSpeech = sp.SpeechReg.Instance();
				//oSpeech.LoadProfile("notepad");
				//oSpeech.Start();

				mProfileItemMenu = new ContextMenu();
				MenuItem mnu = new MenuItem(){ Header = "Edit", Tag = "edit" };
				mnu.Click += lvProfiles_MenuClick;
				mProfileItemMenu.Items.Add(mnu);

				mnu = new MenuItem(){ Header = "Delete", Tag = "delete" };
				mnu.Click += lvProfiles_MenuClick;
				mProfileItemMenu.Items.Add(mnu);

				LoadDataAsync();
			}//func

			private void Window_Unloaded(object sender, RoutedEventArgs e) {
				sp.SpeechEngine.End();
			}//func

			private async void LoadDataAsync(){
				mProfileItems = new ObservableCollection<ProfileItem>();

				string[] ary = await sp.Profile.GetProfileListAsync();
				foreach(var itm in ary) mProfileItems.Add(new ProfileItem() { Name = itm, IsLoaded = false });

				lvProfiles.ItemsSource = mProfileItems;
			}//func
		#endregion

		#region Button Events
			private void Button_Click(object sender, RoutedEventArgs e){
				Button btn = sender as Button;

				Console.WriteLine(btn.Name);
				switch(btn.Name){
					//.................................................
					case "btnAdd":
						if(SpeechCMD.layouts.dlgProfile.Show(null)) {
							Console.WriteLine("New Profile Created");
						}//if
						break;
					//.................................................
					case "btnRemove":
						if(lvProfiles.SelectedItem != null){
							ProfileItem pItm = (ProfileItem) lvProfiles.SelectedItem;

							if(pItm.IsLoaded) sp.SpeechEngine.Instance().UnloadProfile(pItm.Name);
							mProfileItems.Remove(pItm);
						}//if
						break;
					//.................................................
					case "btnState":
						var engine = sp.SpeechEngine.Instance();
						if(engine.IsActive){
							engine.Stop();
							btn.Content = "Start Listening";
							Console.WriteLine("Stopping Engine");
						}else{
							if(engine.GrammarCount > 0) { 
								engine.Start();
								btn.Content = "Stop Listening";

								Console.WriteLine("Starting Engine");
							}else{
								MessageBox.Show("Need to have at least one profile active for listening to begin.","Warning",MessageBoxButton.OK,MessageBoxImage.Asterisk);
							}//if
						}//if
						break;
				}//switch
			}//func
		#endregion

		#region Profile Events
			private void lvProfiles_ItemDoubleClick(object sender, MouseButtonEventArgs e){
				if(e.ChangedButton == MouseButton.Left) { 
					ListViewItem item = sender as ListViewItem;
					var obj = (ProfileItem)item.Content;

					var speech = sp.SpeechEngine.Instance();
					if(obj.IsLoaded){
						speech.UnloadProfile(obj.Name);
						obj.IsLoaded = false;
					}else{
						speech.LoadProfile(obj.Name);
						obj.IsLoaded = true;
					}//if

					e.Handled = true;
				}//if
			}//func

			private void lvProfiles_ItemMouseUp(object sender, MouseButtonEventArgs e){
				if(e.ChangedButton == MouseButton.Right){ 
					ListViewItem item = sender as ListViewItem;
					var obj = (ProfileItem)item.Content;

					mProfileItemMenu.IsOpen = true;
					e.Handled = true;
				}//if
			}//func

			void lvProfiles_MenuClick(object sender, RoutedEventArgs e){
				if(lvProfiles.SelectedItem == null) return;

				MenuItem mi = (MenuItem) sender;
				ProfileItem pi = (ProfileItem)lvProfiles.SelectedItem;

				switch(mi.Tag.ToString()){
					case "edit": Console.WriteLine("Edit " + pi.Name); SpeechCMD.layouts.dlgProfile.Show(pi.Name); break;
					case "delete": Console.WriteLine("delete " + pi.Name); break;
					case "new": Console.WriteLine("new " + pi.Name); break;
				}//switch
			}//func
		
			private void MenuItem_Click(object sender, RoutedEventArgs e){
				Console.WriteLine(sender);
				Console.WriteLine(e.OriginalSource);
				Console.WriteLine(e.Source);

				Console.WriteLine(lvProfiles.SelectedItem);
			}//func
		#endregion
	}//cls
}//ns
