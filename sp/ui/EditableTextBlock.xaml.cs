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

namespace sp.ui{

	public partial class EditableTextBlock : UserControl {
		
		#region Var,Constructor
		private string mOrigText = "";

		public EditableTextBlock(){
			InitializeComponent();
			base.Focusable = true;
            base.FocusVisualStyle = null;
		}//func
		#endregion

		#region Properties
			public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), new PropertyMetadata(""));
			public string Text{
				get{ return (string)GetValue(TextProperty); }
				set{ SetValue(TextProperty,value); }
			}//prop

			public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(true));
			public bool IsEditable{
				get{ return (bool)GetValue(IsEditableProperty); }
				set{ SetValue(IsEditableProperty,value); }
			}//prop

			public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(EditableTextBlock), new PropertyMetadata(false));
			public bool IsInEditMode{
				get{ return (IsEditable)? (bool)GetValue(IsInEditModeProperty) : false; }
				set{
					if(IsEditable){
						if(value) mOrigText = Text;
						SetValue(IsInEditModeProperty,value);
					}//if
				}//set
			}//prop
		#endregion

		#region ControlEvents
			/*
			public static readonly RoutedEvent DoubleClickEvent = EventManager.RegisterRoutedEvent("DoubleClick",RoutingStrategy.Bubble,typeof(MouseButtonEventHandler),typeof(EditableTextBlock));
			public event MouseButtonEventHandler DoubleClick{
				add{ AddHandler(DoubleClickEvent,value); }
				remove{ RemoveHandler(DoubleClickEvent,value); }
			}//func
			*/

			protected override void OnMouseDoubleClick(MouseButtonEventArgs e){
				base.OnMouseDoubleClick(e);
				
				if(!IsInEditMode && IsEditable) IsInEditMode = true;
				
				//MouseButtonEventArgs newEventArgs = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton);
				//newEventArgs.RoutedEvent = DoubleClickEvent;
				//RaiseEvent(newEventArgs);
			}//func
		#endregion

		#region Textbox Events
			// Invoked when we enter edit mode.
			void TextBox_Loaded(object sender, RoutedEventArgs e){
				TextBox txt = sender as TextBox;
				txt.Focus(); //txt.SelectAll();

				txt.SelectionStart = txt.Text.Length; // add some logic if length is 0
				txt.SelectionLength = 0;
			}//func

			// Invoked when we exit edit mode.
			void TextBox_LostFocus(object sender, RoutedEventArgs e) { this.IsInEditMode = false; }

			// Invoked when the user edits the annotation.
			void TextBox_KeyDown(object sender, KeyEventArgs e){
				if (e.Key == Key.Enter){ this.IsInEditMode = false; e.Handled = true; }
				else if (e.Key == Key.Escape){
					this.IsInEditMode = false;
					Text = mOrigText;
					e.Handled = true;
				}//if
			}//func
        #endregion Event Handlers
	}//cls
}//ns
