// Usage example:

// using System.Collections.Generic;
// using System.Text.RegularExpressions;

// public enum AudioCategory {
// 	Undefined,
// 	Music,
// 	Narration,
// }
// [System.Serializable]
// public class AudioInstruction : ScriptContent {
// 	static Regex _parserRegex;
// 	static Regex parserRegex {
// 		get {
// 			if(_parserRegex == null) _parserRegex = new Regex(InkParserUtility.BuildInstructionPrefixRegex("AUDIO"), RegexOptions.IgnoreCase);
// 			return _parserRegex;
// 		}
// 	}
// 	public static AudioInstruction TryParse (string rawContent, List<string> tags) {
// 		var match = parserRegex.Match (rawContent);
// 		if(!match.Success) return null;
//         var arguments = InkParserUtility.ParseArguments(match.Groups[1].Value);
//         var instruction = new AudioInstruction();
// 		instruction.ParseDefaultArguments(arguments);
//         arguments.TryGetValue("path", ref instruction.audioPath);
//         arguments.TryGetValue("loop", ref instruction.loop);
//         arguments.TryGetValue("audioType", ref instruction.audioType);
//         arguments.TryGetValue("stopOthersOfType", ref instruction.stopOthersOfType);

// 		var cssArguments = InkParserUtility.ParseCSSStyleArguments(tags);
// 		instruction.ParseDefaultCSSArguments(cssArguments);
	
// 		arguments.TryGetValue("delay", ref instruction.delay);
        
// 		string modeStr = null;
//         if(!arguments.TryGetValue("mode", ref modeStr) || !System.Enum.TryParse(modeStr, true, out instruction.mode))
// 			instruction.mode = MediaPlayInstruction.Play;

//         if(instruction.mode == MediaPlayInstruction.Play && instruction.audioPath == null) return null;

// 		return instruction;
// 	}

//     public AudioInstruction () {}
//     public AudioInstruction (AudioInstruction toClone) : base(toClone) {
//         this.audioPath = toClone.audioPath;
//         this.loop = toClone.loop;
//         this.mode = toClone.mode;
//         this.audioType = toClone.audioType;
//         this.stopOthersOfType = toClone.stopOthersOfType;
//     }
    
//     public string audioPath;
//     public bool loop = true;
// 	public MediaPlayInstruction mode;
// 	public AudioCategory audioType;
// 	public bool stopOthersOfType;
// }
// public enum MediaPlayInstruction {
// 	Play,
// 	Pause,
// 	Stop,
// }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

public static class InkParserUtility {
	// Standard instruction prefix >>>
	public const string instructionPrefixRegexString = @"^\s*>{2,}\s*";
	static Regex _instructionPrefixRegex;
	public static Regex instructionPrefixRegex {
		get {
			if(_instructionPrefixRegex == null) _instructionPrefixRegex = new Regex(instructionPrefixRegexString, RegexOptions.IgnoreCase);
			return _instructionPrefixRegex;
		}
	}

	// Parses ink instructions in the form: >>> MY INSTRUCTION
	// Arguments may be added: >>> MY INSTRUCTION (param1=blah) (param2=2)
	public static bool IsInstruction (string rawContent) {
		return instructionPrefixRegex.Match (rawContent).Success;
	}
	
	public static string BuildInstructionPrefixRegex (string instructionName, bool disallowAdditionalTextBetweenNameAndArguments = true) {
		// This version prevents any additional text being present between the instruction name and any arguments.
		// This ensures instruction names are exact, which can otherwise be a problem if one instruction name is a subset of another (e.g. "LOG" and "LOG ERROR". In this case, "LOG" would match both instructions).
		if(disallowAdditionalTextBetweenNameAndArguments) {
			return $@"{instructionPrefixRegexString}(?:{instructionName})\s*(?:(?=\()((?:.|\n)*)|$)";
		}
		// In this version, the instruction name may be followed by any text, up to the start of the arguments.
		else {
			return $@"{instructionPrefixRegexString}(?:{instructionName})\b((?:.|\n)*)";
		}
	}


	// Parses all arguments in the form "(thing=2) (otherthing=blah)"
	// Type may be specified "(string thing=2) (MyEnumType otherthing=blah)"
	// Value is optional, so you can declare bools just by writing the variable name "(enableThing)" instead of "(enableThing=true)"
	// We also allow : to be used instead of =, for CSS style syntax
	// Must use brackets!
	static Regex _argumentAndValueParser;
	public static Regex argumentAndValueParser {
		get {
			// if(_argumentAndValueParser == null) _argumentAndValueParser = new Regex(@"\(\s*(.*?)\s*[:=]\s*(.*?\s*)\)");
			if(_argumentAndValueParser == null) _argumentAndValueParser = new Regex(@"\((\w*?)\s*(\w*?)\s*(?:[:=]\s*((?:.|\n)*?\s*))?\)");
			return _argumentAndValueParser;
		}
	}

	[Serializable]
	public class ParsedArgument {
		// Type is optional and may be null.
		public string variableStringType;
		public Type type;

		public string variableName;
		public string variableStringValue;
		public bool variableDefined => !string.IsNullOrEmpty(variableStringValue);
		static Dictionary<string, Type> primitiveTypes = new Dictionary<string, Type> {
			{"object", typeof(object)},
			{"bool", typeof(bool)},
			{"int", typeof(int)},
			{"float", typeof(float)},
			{"char", typeof(char)},
			{"string", typeof(string)},
		};

		public ParsedArgument (string variableStringType, string variableName, string variableStringValue) {
			this.variableStringType = variableStringType;
			if(!string.IsNullOrWhiteSpace(this.variableStringType)) {
				if(!primitiveTypes.TryGetValue(this.variableStringType, out type)) {
					type = Type.GetType(this.variableStringType, false);
				}
			}
			this.variableName = variableName;
			this.variableStringValue = variableStringValue;
		}

		public override string ToString () {
			return string.Format ("[ParsedArgument: type={0}, variableStringType={1}, variableName={2}, variableStringValue={3}]", type, variableStringType, variableName, variableStringValue);
			
		}
	}

	public class ParsedArgumentCollection : Dictionary<string, ParsedArgument> {
		public string rawText;
		public string textBeforeArguments;

		public ParsedArgumentCollection () : base (StringComparer.OrdinalIgnoreCase) {}

		// Gets the value with the given key. Case is ignored.
		public bool TryGetValue<T> (string key, ref T val) {
			ParsedArgument parsedArgument = null;
			if(!base.TryGetValue(key, out parsedArgument)) {
				return false;
			}
			var type = typeof(T);
			type = Nullable.GetUnderlyingType(type) ?? type;

			if(parsedArgument.type != null) {
				if(!type.IsAssignableFrom(parsedArgument.type)) {
					Debug.LogError("Expected variable type "+type+" from key "+key+" is not assignable from ink specified type "+parsedArgument.type);
					return false;
				} else {
					type = parsedArgument.type;
				}
			}

			// If the type is vague, try to resolve it manually.
			var isObjectType = type == typeof(object);
			if (isObjectType || type == typeof(bool)) {
				// If no value is defined we return true.
				if(!parsedArgument.variableDefined) {
					val = (T)(object)true;
					return true;
				} else if(bool.TryParse(parsedArgument.variableStringValue, out var boolVal)) {
					val = (T)(object)boolVal;
					return true;
				}
			}
			if (isObjectType || type == typeof(float)) {
				if(float.TryParse(parsedArgument.variableStringValue, out var floatVal)) {
					val = (T)(object)floatVal;
					return true;
				}
			}
			if (isObjectType || type == typeof(int)) {
				if(int.TryParse(parsedArgument.variableStringValue, out var intVal)) {
					val = (T)(object)intVal;
					return true;
				}
			}
			if (isObjectType || type == typeof(Color)) {
				var htmlString = parsedArgument.variableStringValue.StartsWith("#") || parsedArgument.variableStringValue.StartsWith("\\#") ? parsedArgument.variableStringValue : '#'+parsedArgument.variableStringValue;
				if(ColorUtility.TryParseHtmlString(htmlString, out var colorVal)) {
					val = (T)(object)colorVal;
					return true;
				}
			}
			if (isObjectType || type == typeof(Rect)) {
				if(TryParseRect(parsedArgument.variableStringValue, out var rectVal)) {
					val = (T)(object)rectVal;
					return true;
				}
			}
			if (isObjectType || type == typeof(Vector2)) {
				if(TryParseVector2(parsedArgument.variableStringValue, out var vector2Val)) {
					val = (T)(object)vector2Val;
					return true;
				}
			}
			if (type.IsEnum) {
				if(TryParseEnum(type, parsedArgument.variableStringValue, true, out var objVal)) {
					val = (T)objVal;
					return true;
				}
			}
			if (isObjectType || type == typeof(string)) {
				val = (T)(object)parsedArgument.variableStringValue;
				return true;
			}
			return false;
		}

		public bool TryGetRequiredValue<T> (string key, ref T val, [CallerFilePath] string callingFilePath = "") {
			if(!TryGetValue(key, ref val)) {
				Debug.LogWarning("Parsed ink content type \""+callingFilePath+"\" found but required argument \""+key+"\" missing!");
				return false;
			}
			return true;
		}
	}

	static MethodInfo _enumTryParse;
	static MethodInfo enumTryParse {
		get {
			if(_enumTryParse == null) {
				_enumTryParse = typeof(Enum)
					.GetMethods(BindingFlags.Public | BindingFlags.Static)
					.First(m => m.Name == "TryParse" && m.GetParameters().Length == 3);
			}
			return _enumTryParse;
		}
	}

	public static bool TryParseEnum(Type enumType, string value, bool ignoreCase, out object enumValue) {
		// This method seems faster? I've not tested it but we should!
		// if (!Enum.IsDefined(enumType, value)) {
		// 	enumValue = null;
		// 	return false;
		// } else {
		// 	enumValue = Enum.Parse(enumType, value, ignoreCase);
		// 	return true;
		// }

		MethodInfo genericEnumTryParse = enumTryParse.MakeGenericMethod(enumType);

		object[] args = { value, ignoreCase, Enum.ToObject(enumType, 0) };
		bool success = (bool)genericEnumTryParse.Invoke(null, args);
		enumValue = args[2];

		return success;
	}



	public const string floatParseString = @"([+-]?(?:[0-9]*[.])?[0-9]+)";
	
	public const string vector2ParseString = @"(?:x|min)="+floatParseString+@",\s*(?:y|max)="+floatParseString;
	static Regex _vector2Parser;
	public static Regex vector2Parser {
		get {
			if(_vector2Parser == null) _vector2Parser = new Regex(vector2ParseString, RegexOptions.IgnoreCase);
			return _vector2Parser;
		}
	}
	// In the form:
	// X=54.1,Y=15.323
	// x and y can be written as Min or Max
	public static bool TryParseVector2(string value, out Vector2 vector2) {
		vector2 = Vector2.zero;
		var match = vector2Parser.Match (value);
		if(!match.Success) return false;
		float x,y;
		if(!float.TryParse(match.Groups[1].Value, out x)) return false;
		if(!float.TryParse(match.Groups[2].Value, out y)) return false;
		vector2 = new Vector2(x,y);
		return true;
	}

	public const string rectParseString = @"x="+floatParseString+@",\s*y="+floatParseString+@",\s*(?:w|width)="+floatParseString+@",(?:h|height)="+floatParseString;
	static Regex _rectParser;
	public static Regex rectParser {
		get {
			if(_rectParser == null) _rectParser = new Regex(rectParseString, RegexOptions.IgnoreCase);
			return _rectParser;
		}
	}
	// In the form:
	// X=54.1,Y=15.323,Width=723,Height=555
	// Width and Height can be written as w or h
	public static bool TryParseRect(string value, out Rect rect) {
		rect = Rect.zero;
		var match = rectParser.Match (value);
		if(!match.Success) return false;
		float x,y,width,height;
		if(!float.TryParse(match.Groups[1].Value, out x)) return false;
		if(!float.TryParse(match.Groups[2].Value, out y)) return false;
		if(!float.TryParse(match.Groups[3].Value, out width)) return false;
		if(!float.TryParse(match.Groups[4].Value, out height)) return false;
		rect = new Rect(x,y,width,height);
		return true;
	}
	
	// In the form:
	// (color1=\#fafa6e) (time1=0) (color2=\#64c987) (time2=0.33) (color3=\#00898a) (time3=0.66) (color4=\#2a4858) (time4=1)
	// We might want to consider moving to something more like CSS gradients, which are written like: linear-gradient(90deg, #020024 0%, #090979 35%, #00d4ff 100%);
	// We might make this: (gradient=linear, 90deg, \#020024 0%, \#090979 35%, \#00d4ff 100%))
	public static bool TryParseGradient(ParsedArgumentCollection arguments, out Gradient gradient) {
		static bool TryAddColor (ParsedArgumentCollection arguments, int index, ref List<GradientColorKey> colorKeys, ref List<GradientAlphaKey> alphaKeys) {
			Color color = Color.black;
			float time = 0f;

			if(!arguments.TryGetValue("color"+(index+1), ref color)) return false;
			if(!arguments.TryGetValue("time"+(index+1), ref time)) return false;

			colorKeys.Add(new GradientColorKey(color, time));
			alphaKeys.Add(new GradientAlphaKey(color.a, time));
			return true;
		}
		
		gradient = null;
		
		List<GradientColorKey> colorKeys = new List<GradientColorKey>();
		List<GradientAlphaKey> alphaKeys = new List<GradientAlphaKey>();
		for(int i = 0; i < 4; i++)
			if(!TryAddColor(arguments, i, ref colorKeys, ref alphaKeys))
				break;
		if(colorKeys.Count > 0) {
			gradient = new Gradient();
			gradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
			return true;
		}

		return false;
	}

	public static ParsedArgumentCollection ParseArguments (string text) {
		ParsedArgumentCollection arguments = new ParsedArgumentCollection();
		arguments.textBeforeArguments = arguments.rawText = text;
		int firstIndex = arguments.rawText.Length;
		foreach(Match kvp in argumentAndValueParser.Matches(text)) {
			firstIndex = Mathf.Min(firstIndex, kvp.Index);
			var variableType = kvp.Groups[1].Value;
			var variableName = kvp.Groups[2].Value;
			var variableValue = Regex.Unescape(kvp.Groups[3].Value);
			if(arguments.ContainsKey(variableName)) {
				Debug.LogWarning("Tried to add key "+variableName+" but key already exists! Text was "+text);
			} else {
				arguments.Add(variableName, new ParsedArgument(variableType, variableName, variableValue));
			}
		}
		if(firstIndex != arguments.textBeforeArguments.Length)
			arguments.textBeforeArguments = arguments.textBeforeArguments.Substring(0, firstIndex).Trim();
		return arguments;
	}
	



    // Parses CSS style arguments in the format #size:10
    // = can be used instead of :
	public static ParsedArgumentCollection ParseCSSStyleArguments (List<string> stringArguments) {
		ParsedArgumentCollection arguments = new ParsedArgumentCollection();
		if(stringArguments != null) {
			foreach(var stringArgument in stringArguments) {
				var colonIndex = stringArgument.IndexOf(':');
				if(colonIndex == -1) colonIndex = stringArgument.IndexOf('=');
				if(colonIndex != -1 && colonIndex < stringArgument.Length-1) {
					var key = stringArgument.Substring(0,colonIndex).Trim();
					var val = Regex.Unescape(stringArgument.Substring(colonIndex+1,(stringArgument.Length-1)-colonIndex).Trim());
					if(arguments.ContainsKey(key)) {
                        Debug.LogWarning("CSS Style with key "+key+" already exists! Full list of arguments: \n"+DebugX.ListAsString(stringArguments));
                    } else {
					    arguments.Add(key, new ParsedArgument(null, key, val));
                    }
				} else {
					arguments.Add(stringArgument, new ParsedArgument(null, stringArgument, null));
				}
			}
		}
		return arguments;
	}





	static Regex _tagRegex;
	// Simple single HTML style tag parser
	public static bool ParseTag (string content, out string contentWithoutTags, out string tagID) {
		contentWithoutTags = content;
		tagID = null;
		if( _tagRegex == null ) _tagRegex = new Regex(@"<(\w+)>(.+?)<\/\w+>", RegexOptions.IgnoreCase);
		var tagMatch = _tagRegex.Match (content);
		if( tagMatch.Success ) {
			contentWithoutTags = tagMatch.Groups[2].Value;
			tagID = tagMatch.Groups[1].Value;
			return true;
		} else return false;
	}

	static char[] lineSplitCharacters = {'\n'};
	// Splits text into lines. Useful for strings returned via a function. 
	public static string[] SplitTextIntoLines (string content) {
		content = content.Trim();
		var lines = content.Split(lineSplitCharacters, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < lines.Length; i++) {
			lines[i] = lines[i].Trim();
		}
		return lines;
	}
}