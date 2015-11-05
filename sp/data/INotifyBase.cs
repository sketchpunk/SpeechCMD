using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace sp.data {
	public class INotifyBase : System.ComponentModel.INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged( [CallerMemberName] string caller = ""){
			if ( PropertyChanged != null ) PropertyChanged( this, new PropertyChangedEventArgs( caller ) );
        }//func
	}//cls
}//ns