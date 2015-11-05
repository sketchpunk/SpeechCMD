using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Threading;
using System.Linq;

namespace sp{
	public class SpeechEngine{
		#region Singleton functionality
		private static SpeechEngine gSpeechReg = null;
		public static SpeechEngine Instance(){
			if(gSpeechReg == null) gSpeechReg = new SpeechEngine();
			return gSpeechReg;
		}//func

		public static void End() {
			if(gSpeechReg != null) {
				gSpeechReg.Stop();
				gSpeechReg.Dispose();
				gSpeechReg = null;
			}//if
		}//func
		#endregion

		#region Prop,Construct
			private SpeechRecognitionEngine mEngine;
			private Dictionary<string,Tuple<string,VoiceCommand>> mGrammarList;
			private Dictionary<string,Profile> mProfileList;

			private bool mIsActive = false;
			private float mConfidenceMin = 0.9f;

			public bool IsActive{ get {return mIsActive;} }
			public int GrammarCount { get { return mGrammarList.Count; } }

			public SpeechEngine(){
				mProfileList = new Dictionary<string,Profile>();
				mGrammarList = new Dictionary<string,Tuple<string,VoiceCommand>>();

				mEngine = new SpeechRecognitionEngine();
				mEngine.SpeechRecognized += onSpeechRecognized;
				mEngine.SpeechRecognitionRejected += onSpeechRecognitionRejected;
				//mEngine.AudioLevelUpdated += onAudioLevelUpdated;
				mEngine.MaxAlternates = 3;
			}//func
		#endregion

		#region Object Methods
			public bool Start(){
				if(mIsActive) return true;

				try{ 
					mEngine.SetInputToDefaultAudioDevice(); // set the input of the speech recognizer to the default audio device
					mEngine.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous
					mIsActive = true;
				}catch(InvalidOperationException ex){
					Console.WriteLine(ex.Message);
					return false;
				}//func

				return true;
			}//func

			public void Stop(){
				if(mIsActive){ mEngine.RecognizeAsyncCancel(); mIsActive = false; }
			}//func

			public void Dispose() {
				Stop();

				//Disconnect Events
				mEngine.SpeechRecognized -= onSpeechRecognized;
				mEngine.SpeechRecognitionRejected -= onSpeechRecognitionRejected;
				//mEngine.AudioLevelUpdated -= onAudioLevelUpdated;

				//Dispose interal objects
				mEngine.Dispose();
			}//func
		#endregion

		#region Grammar Methods
			public void LoadGrammar(Grammar grammar) {
				mEngine.RequestRecognizerUpdate();
				mEngine.LoadGrammarAsync(grammar);
			}//func

			public bool LoadProfile(string profileName){
				var profile = Profile.Load(profileName);
				var choices = new Choices();

				foreach(VoiceCommand cmd in profile.Commands){
					foreach(string phrase in cmd.Grammar){
						choices.Add(phrase);
						mGrammarList.Add(phrase,new Tuple<string,VoiceCommand>(profile.Name,cmd));
						Console.WriteLine(phrase);
					}//for
				}//for
			
				var grammar = new Grammar(new GrammarBuilder(choices)){ Name = profile.Name };
				mEngine.RequestRecognizerUpdate();
				mEngine.LoadGrammarAsync(grammar);

				mProfileList.Add(profileName,profile);
				return false;
			}//func

			public bool UnloadProfile(string profileName){
				//.......................................
				//Remove the references to each grammar and command that was inputted into the engine.
				for(int i = mGrammarList.Count - 1;i >= 0;i--){
					var itm = mGrammarList.ElementAt(i);
					if(itm.Value.Item1.Equals(profileName)) mGrammarList.Remove(itm.Key);
				}//for

				//.......................................
				//Remove the profile data
				if(mProfileList.ContainsKey(profileName)) mProfileList.Remove(profileName);

				//.......................................
				//Remove Grammer From the engine
				foreach(Grammar gr in mEngine.Grammars){
					if(gr.Name.Equals(profileName)){
						mEngine.RequestRecognizerUpdate();
						mEngine.UnloadGrammar(gr);
						break;
					}//if
				}//for

				return true;
			}//func

			public void UnloadAllGrammars(){ mEngine.UnloadAllGrammars(); }//func
			
			public void UnloadGramar(String grammerName){
				foreach(Grammar  gr in mEngine.Grammars){
					if(gr.Name.Equals(grammerName)){ mEngine.UnloadGrammar(gr); break; }
				}//for
			}//func
		#endregion

		#region Speech Handled Events
			private void onSpeechRecognized(Object sender, SpeechRecognizedEventArgs e){
				//Only execute a command if the confidance is at a bar minimum
				if(e.Result.Confidence >= mConfidenceMin){
					if(mGrammarList.ContainsKey(e.Result.Text)) mGrammarList[e.Result.Text].Item2.Execute();
				}//if

				Console.WriteLine(e.Result.Text);
				Console.WriteLine(e.Result.Confidence);
				//outputSearchRecReport(e);
			}//func

			private void onSpeechRecognitionRejected(Object sender, SpeechRecognitionRejectedEventArgs e){
				if(e.Result.Alternates.Count == 0){ Console.WriteLine("Speech rejected. No candidate phrases found."); return; }//if

				Console.WriteLine("Speech input was rejected.");
				foreach(RecognizedPhrase phrase in e.Result.Alternates){
					Console.WriteLine("  Rejected phrase: " + phrase.Text);
					Console.WriteLine("  Confidence score: " + phrase.Confidence);
					if(phrase.Grammar != null) Console.WriteLine("  Grammar name:  " + phrase.Grammar.Name);
				}//if
			}//func

			private void onAudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e){
				 Console.WriteLine(e.AudioLevel);
			}//func
		#endregion

		#region Misc
			private void outputSearchRecReport(SpeechRecognizedEventArgs e) {
				//https://msdn.microsoft.com/en-us/library/microsoft.speech.recognition.speechrecognizedeventargs
				Console.WriteLine("Recognition result summary:");
				Console.WriteLine(" Recognized phrase: {0}\n Confidence score {1}\n Grammar used: {2}\n",e.Result.Text, e.Result.Confidence, e.Result.Grammar.Name);

				// Display the semantic values in the recognition result.
				Console.WriteLine("  Semantic results:");
				foreach (KeyValuePair<String, SemanticValue> child in e.Result.Semantics){
					Console.WriteLine("    The {0} city is {1}",child.Key, child.Value.Value ?? "null");
				}//for
				Console.WriteLine();

				// Display information about the words in the recognition result.
				Console.WriteLine("  Word summary: ");
				foreach(RecognizedWordUnit word in e.Result.Words){
					Console.WriteLine("    Lexical form ({1}) Pronunciation ({0}) Display form ({2})",word.Pronunciation, word.LexicalForm, word.DisplayAttributes);
				}//for

				// Display information about the audio in the recognition result.
				Console.WriteLine("  Input audio summary:\n" +
					"    Candidate Phrase at:       {0} mSec\n" +
					"    Phrase Length:             {1} mSec\n" +
					"    Input State Time:          {2}\n" +
					"    Input Format:              {3}\n",
					e.Result.Audio.AudioPosition,
					e.Result.Audio.Duration,
					e.Result.Audio.StartTime,
					e.Result.Audio.Format.EncodingFormat);

				// Display information about the alternate recognitions in the recognition result.
				Console.WriteLine("  Alternate phrase collection:");
				foreach(RecognizedPhrase phrase in e.Result.Alternates){
					Console.WriteLine("    Phrase: " + phrase.Text);
					Console.WriteLine("    Confidence score: " + phrase.Confidence);
				}//for

				//Display information about text that was replaced during normalization.
				if(e.Result.ReplacementWordUnits.Count != 0){
					Console.WriteLine("  Replacement text:\n");
					
					foreach(ReplacementText rep in e.Result.ReplacementWordUnits){
						Console.WriteLine("      At index {0} for {1} words. Text: {2}\n",rep.FirstWordIndex, rep.CountOfWords, rep.Text);
					}//for
				}else{
					Console.WriteLine(); 
					Console.WriteLine("No text was replaced");
				}//if

			}//func
		#endregion
	}//cls
}//ns
