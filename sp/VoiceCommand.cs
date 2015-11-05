using System;
using System.Collections.Generic;
using System.Speech.Synthesis;

namespace sp{
	public class VoiceCommand{
		public string Name;
		public string Synth;
		public string AudioPath;
		public List<string> Grammar;
		public List<VoiceAction> Actions;

		public VoiceCommand(string name){
			Name = name;
			Synth = "";
			AudioPath = "";
			Grammar = new List<string>();
			Actions = new List<VoiceAction>();
		}//func

		public void AddAction(string type, string data){ Actions.Add(new VoiceAction(type,data)); }//func
		public void RemoveAction(){}
		public void UpdateAction(){}
		public void MoveActionUp(){}
		public void MoveActionDown(){}

		public void Execute(){
			//.....................................
			//Process Any Actions
			if(Actions.Count > 0){
				foreach(VoiceAction act in Actions){
					switch(act.Type){
						case VoiceAction.KB: Keyboard.Process(act.Data); break;
						case VoiceAction.SCRIPT:break;
						case VoiceAction.CMD: System.Diagnostics.Process.Start(act.Data); break;
					}//switch
				}//for
			}//if

			//.....................................
			//Execute Synth if configured.
			if(!String.IsNullOrEmpty(Synth)) SpeechSynth.Say(Synth);
		}//func
	}//cls
}//ns
