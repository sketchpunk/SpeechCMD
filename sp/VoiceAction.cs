using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace sp{
	public class VoiceAction{
		public const string KB = "kb";
		public const string SCRIPT = "script";
		public const string CMD = "cmd";

		public string Type { get; set; }
		public string Data { get; set; }

		public VoiceAction(){}
		public VoiceAction(string type, string data) { Type = type; Data = data; }

		/*
		public static bool Execute(VoiceAction act){
			switch(act.ActionType) {
				case VoiceAction.KB: break;
				case VoiceAction.SCRIPT: break;
				case VoiceAction.CMD: break;
			}//sw
			return false;
		}//func
		 */ 
	}//cls
}//ns
