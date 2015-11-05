using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;

namespace sp{
	public class Profile{
		#region Prop, Construct 
			public string Name { get; set; }
			public List<VoiceCommand> Commands = new List<VoiceCommand>();
		
			public Profile(){ }
			public Profile(string name){ Name = name; }
		#endregion

		#region Data Methods
			public static Profile Load(string name){
				try{ 
					string path = GetProfilePath(name);
					if(!System.IO.File.Exists(path)) return null;

					using (StreamReader file = File.OpenText(path)){
						JsonSerializer serializer = new JsonSerializer();
						Profile rtn = (Profile)serializer.Deserialize(file, typeof(Profile));
						return rtn;
					}//using
				}catch(Exception ex){
					Console.WriteLine(ex.Message);
				}//try

				return null;
			}//func

			public bool Save(){
				try{ 
					string path = GetProfilePath(this.Name);

					using(FileStream fs = File.Open(path, FileMode.OpenOrCreate))
					using(StreamWriter sw = new StreamWriter(fs))
					using(JsonWriter jw = new JsonTextWriter(sw)){
						jw.Formatting = Formatting.Indented;
						JsonSerializer serializer = new JsonSerializer();
						serializer.Serialize(jw, this);
					}//using
					return true;
				}catch(Exception ex){
					Console.WriteLine(ex.Message);
				}//try

				return false;
			}//func
		#endregion

		#region Action Methods
			public VoiceCommand AddCommand(){
				var cmd = new VoiceCommand("New Command");
				Commands.Add(cmd);
				return cmd;
			}//func

			public bool UpdateCommand(){
				return false;
			}//func

			public bool RemoveCommand(){
				return false;
			}//func
		#endregion

		#region Static Functions
			public static bool Remove(string name){ return sp.io.FileSystem.RmFile(GetProfilePath(name)); }//func
		
			public static string GetProfilePath(string name){ 
				return (String.IsNullOrEmpty(name))? sp.io.FileSystem.RootPath("profiles\\") : sp.io.FileSystem.RootPath("profiles\\" +name + ".txt"); 
			}//func

			public static Task<string[]> GetProfileListAsync(){ return Task.Run(()=>GetProfileList()); }//func
			public static string[] GetProfileList(){
				var list = sp.io.FileSystem.EnumFiles(GetProfilePath(null),".txt");
				string[] ary = list.ToArray<string>();

				for(int i = 0;i < ary.Length;i++) ary[i] = Path.GetFileNameWithoutExtension(ary[i]);

				return ary;
			}//func
		#endregion
	}//cls
}//func
