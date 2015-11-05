using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
namespace sp{
	public abstract class SpeechSynth{
		private static SpeechSynthesizer mSynth = null;

		public static void Say(string txt){
			if(mSynth == null) mSynth = new SpeechSynthesizer(); //make this into a singleton class.
			mSynth.Speak(txt);
		}//func
	}//cls
}//ns
