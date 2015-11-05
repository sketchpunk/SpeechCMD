using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SpeechCMD{
	public partial class App : Application{
		// Application has loaded
		public void App_Startup(object sender, StartupEventArgs e){
			string rootPath = sp.io.FileSystem.RootPath(),
				profilePath = rootPath + "profiles",
				audioPath = rootPath + "audio",
				scriptPath = rootPath + "scripts";

			if(!System.IO.Directory.Exists(profilePath)) sp.io.FileSystem.MkDir(profilePath);
			if(!System.IO.Directory.Exists(audioPath)) sp.io.FileSystem.MkDir(audioPath);
			if(!System.IO.Directory.Exists(scriptPath)) sp.io.FileSystem.MkDir(scriptPath);
		}//func
	}//cls
}//ns
