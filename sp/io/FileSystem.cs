using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace sp.io{
	public abstract class FileSystem{
		#region Location
			public static string RootPath(string relativePath){ return RootPath() + relativePath; }
			public static string RootPath(){
				string rtn = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
				if(!rtn.EndsWith("\\")) rtn += "\\";

				return rtn;
			}//func
		#endregion

		#region Directory Operations
			public static Boolean MkDir(string path){
				try{
					Directory.CreateDirectory(path);
				}catch(Exception e){ return false; }
				return true;
			}//func
			
			public static Boolean RmDir(string path){
				try{
					Directory.Delete(path,true);
				}catch(Exception e){ return false; }
				return true;
			}//func
			
			public static Boolean MoveDir(string oldPath,string newPath){
				DirectoryInfo di = new DirectoryInfo(oldPath);
				if(di.Exists) di.MoveTo(newPath);
				
				return true;
			}//func
			
			public static void CopyDir(string from, string to){
				//new Microsoft.VisualBasic.MyServices.MyServerComputer().FileSystem.CopyFile(from,to,false);
			}//func

			public static string[] GetDirs(String path){
				return Directory.GetDirectories(path);
			}//func
			
			public static int FileCount(String path){
				return Directory.GetFiles(path).Length;
			}//func
			
			public static DirectoryInfo[] GetFullDirs(string path){
				return new DirectoryInfo(path).GetDirectories();	
			}//func
		#endregion

		#region File Operations
			public static IEnumerable<string> EnumFiles(string path, params string[] exts){
				//return exts.SelectMany(x => Directory.EnumerateFiles(path, x)); //Might be faster but only in large folders and small ext list.
				return Directory.EnumerateFiles(path,"*.*",SearchOption.TopDirectoryOnly)
					.Where(file => exts.Any(ext => file.EndsWith(ext,StringComparison.OrdinalIgnoreCase)));
			}//func

			public static string[] GetEntries(String path){ return Directory.GetFileSystemEntries(path); }//func
			
			public static string[] GetFiles(String path){ return Directory.GetFiles(path);}//func
			
			public static string[] GetFilesByPattern(String path,string pat){ return Directory.GetFiles(path,pat); }//func
			
			public static FileInfo[] GetFullFiles(string path){ return new DirectoryInfo(path).GetFiles(); }//func

			public static string GetFilename(string path){ return Path.GetFileNameWithoutExtension(path); }//finc

			public static Boolean RmFile(string[] path){
				Boolean rtn = true;
			
				for(int i = 0; i < path.Length; i++){
					if( !RmFile(path[i]) ) rtn = false;
				}//for

				return rtn;
			}//func
			
			public static Boolean RmFile(string path){
				try{
					File.Delete(path);
				}catch(Exception e){ return false; }
				return true;
			}//func
			
			public static Boolean MoveFile(string oldPath,string newPath){
				FileInfo fi = new FileInfo(oldPath);
				try{
					if(fi.Exists) fi.MoveTo(newPath);
					return true;
				}catch(Exception e){
					//Logger.Exception("movefile",e,oldPath + " to " + newPath);
				}//try
				return false;	
			}//func
			
			public static Boolean CopyFile(String oldPath,string newPath, bool overwrite){
				try{
					File.Copy(oldPath,newPath,overwrite);
					return true;
				}catch(Exception e){
					//Logger.Exception("copyfile",e,oldPath + ":"+ newPath);
				}//try
				return false;
			}//func

			public static string QuickFileRead(string path){
				if(! File.Exists(path)) return "";
				return File.ReadAllText(path);
			}//func

			public static Boolean QuickFileWrite(string path, string txt){
				StreamWriter sw = null;
				Boolean stat = true;
				try{
					sw = new StreamWriter(path);
					sw.Write(txt);
				}catch(Exception e){
					stat = false;
				}finally{
					if(sw != null){ sw.Close(); sw = null; }//if
				}//try
				
				return stat;
			}//func

			public static Boolean QuickFileAppend(string path, string txt){
				StreamWriter sw = null;
				Boolean stat = true;
				try{
					sw = new StreamWriter(path,true);
					sw.Write(txt);
				}catch(Exception e){
					stat = false;
				}finally{
					if(sw != null){ sw.Close(); sw = null; }//if
				}//try
				return stat;
			}//func
			
			public static string FileToBase64(string path){
				byte[] buf;
				long bLen = 0;
				
				try{
					using(FileStream f = new FileStream(path,FileMode.Open,FileAccess.Read)){
						buf = new Byte[f.Length];
						bLen = f.Read(buf,0,(int)f.Length);
					}//func
				}catch (System.Exception exp){
					return exp.Message;
				}//try
				
				if(bLen > 0) return System.Convert.ToBase64String(buf,0,buf.Length);
				return "error";
			}//func
		#endregion
	}//cls
}//cls
