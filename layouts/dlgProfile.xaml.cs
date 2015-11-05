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
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace SpeechCMD.layouts{
	public class CommandItem : sp.data.INotifyBase{
		public string Name { get{return ObjRef.Name;} set{ObjRef.Name = value;} }
		public string Synth{ get{return ObjRef.Synth;} set{ObjRef.Synth = value;} }
		public string Grammar{ get; set; }
		public sp.VoiceCommand ObjRef { get; set; }
	}//struct

	public class ActionItem : sp.data.INotifyBase {
		public string Type { get{return ObjRef.Type;} set{ObjRef.Type = value;} }
		public string Data { get{return ObjRef.Data;} set{ObjRef.Data = value;} }
		public sp.VoiceAction ObjRef { get; set; }
		public ActionItem(sp.VoiceAction o) {  ObjRef = o; }
	}//struct

	public partial class dlgProfile : Window {
		#region Vars, Fields, Constructors
			private string mProfileName = null;
			private bool mHasSaved = false;
			private sp.Profile mProfile = null;
			private ObservableCollection<CommandItem> mCommandItems;
			private ObservableCollection<ActionItem> mActionItems;
			private ObservableCollection<Tuple<string,string>> mActionTypes = new ObservableCollection<Tuple<string,string>>();

			public bool HasSaved { get { return mHasSaved; } }

			public dlgProfile(){  InitializeComponent(); }
			public dlgProfile(string profileName){ InitializeComponent(); mProfileName = profileName; }
		#endregion

		#region Static Methods
		public static bool Show(string profileName){
				var dlg = new dlgProfile(profileName);
				dlg.ShowDialog();
				return dlg.HasSaved;
		}//func
		#endregion

		#region Window Events
			private void Window_Loaded(object sender, RoutedEventArgs e){
				mActionItems = new ObservableCollection<ActionItem>();
				mActionTypes.Add(new Tuple<string,string>("script","C# Script"));
				mActionTypes.Add(new Tuple<string,string>("cmd","Executable"));
				mActionTypes.Add(new Tuple<string,string>("kb","Keyboard"));
				this.DataContext = mActionTypes;

				lvActions.ItemsSource = mActionItems;

				mCommandItems = new ObservableCollection<CommandItem>();
				if(!String.IsNullOrEmpty(mProfileName)) { 
					mProfile = sp.Profile.Load(mProfileName);
					if(mProfile != null){
						tbProfileName.Text = mProfile.Name;

						foreach(var cmd in mProfile.Commands){
							mCommandItems.Add(new CommandItem(){ ObjRef=cmd, Synth=cmd.Synth, Grammar=String.Join(", ",cmd.Grammar) });
						}//for
					}//if
				}else{
					mProfile = new sp.Profile(); //Create a new Profile
				}//if

				lvCommands.ItemsSource = mCommandItems;
			}//func

			private void Button_Click(object sender, RoutedEventArgs e){
				switch((sender as Button).Name){
					//............................................................
					case "btnCancel": this.Close(); break;
					
					//............................................................
					case "btnAddCmd":
						var newCmd = new sp.VoiceCommand("New Command");
						mProfile.Commands.Add(newCmd);
						mCommandItems.Add(new CommandItem(){ ObjRef = newCmd });	

						var va = new sp.VoiceAction(){ Type = "kb", Data = "LCTRL x" };
						newCmd.Actions.Add(va);
					break;

					//............................................................
					case "btnRemoveCmd":
						if(lvCommands.SelectedItem != null) {
							CommandItem ci = (CommandItem) lvCommands.SelectedItem;
							mProfile.Commands.Remove(ci.ObjRef);	//Remove From Profile
							mCommandItems.Remove(ci);				//Remove from bound list
						}//if
					break;

					//............................................................
					case "btnAddAct":
						if(lvCommands.SelectedItem != null){ 
							var vaa = new sp.VoiceAction(){ Type = "kb", Data = "LCTRL x" };
							(lvCommands.SelectedItem as CommandItem).ObjRef.Actions.Add(vaa);
							mActionItems.Add(new ActionItem(vaa));	
						}//if
					break;

					//............................................................
					case "btnRemoveAct":
						if(lvActions.SelectedItem != null){
							CommandItem ci = (CommandItem) lvCommands.SelectedItem;
							ActionItem ai = (ActionItem) lvActions.SelectedItem;
							ci.ObjRef.Actions.Remove(ai.ObjRef);	//Remove From Command
							mActionItems.Remove(ai);				//Remove from bound list
						}//if
					break;

					//............................................................
					case "btnSave":
						string pName = tbProfileName.Text.Trim();
						if(String.IsNullOrEmpty(pName)) {
							MessageBox.Show("Must have a profile name","Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
							return;
						}//if

						//Convert delimited string into an string array for the profile.
						string[] ary;
						foreach(var cmd in mCommandItems){
							cmd.ObjRef.Grammar.Clear();
							ary = cmd.Grammar.ToLower().Split(',');
							foreach(string str in ary) cmd.ObjRef.Grammar.Add(str.Trim());
						}//for

						//Save everything else
						mProfile.Name = pName;
						mProfile.Save();
						mHasSaved = true;

						//If changing the profile name, then the profile will save to a new file
						//Because of that, we need to delete the old profile. TODO
						if(mProfileName != null && !mProfileName.Equals(pName)){
						}//if

						this.Close();
					break;
				}//switch
			}
		#endregion

		private void lvCommands_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if(e.AddedItems.Count > 0){
				CommandItem ci = e.AddedItems[0] as CommandItem;

				mActionItems.Clear();
				foreach(sp.VoiceAction act in ci.ObjRef.Actions)mActionItems.Add(new ActionItem(act));
			}//if
		}//func
	}//cls
}//ns
