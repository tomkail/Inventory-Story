using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum AudioCategory {
	Undefined,
	Music,
	Narration,
}
[System.Serializable]
public class AudioInstruction : ScriptContent {
	static Regex _parserRegex;
	static Regex parserRegex {
		get {
			if(_parserRegex == null) _parserRegex = new Regex(InkParserUtility.BuildInstructionPrefixRegex("AUDIO"), RegexOptions.IgnoreCase);
			return _parserRegex;
		}
	}
	public static AudioInstruction TryParse (string rawContent, List<string> tags) {
		var match = parserRegex.Match (rawContent);
		if(!match.Success) return null;
        var arguments = InkParserUtility.ParseArguments(match.Groups[1].Value);
        var instruction = new AudioInstruction();
		instruction.ParseDefaultArguments(arguments);
        arguments.TryGetValue("path", ref instruction.audioPath);
        arguments.TryGetValue("loop", ref instruction.loop);
        arguments.TryGetValue("audioType", ref instruction.audioType);
        arguments.TryGetValue("stopOthersOfType", ref instruction.stopOthersOfType);

		var cssArguments = InkParserUtility.ParseCSSStyleArguments(tags);
		instruction.ParseDefaultCSSArguments(cssArguments);
	
		arguments.TryGetValue("delay", ref instruction.delay);
        
		string modeStr = null;
        if(!arguments.TryGetValue("mode", ref modeStr) || !System.Enum.TryParse(modeStr, true, out instruction.mode))
			instruction.mode = MediaPlayInstruction.Play;

        if(instruction.mode == MediaPlayInstruction.Play && instruction.audioPath == null) return null;

		return instruction;
	}

    public AudioInstruction () {}
    public AudioInstruction (AudioInstruction toClone) : base(toClone) {
        this.audioPath = toClone.audioPath;
        this.loop = toClone.loop;
        this.mode = toClone.mode;
        this.audioType = toClone.audioType;
        this.stopOthersOfType = toClone.stopOthersOfType;
    }
    
    public string audioPath;
    public bool loop = true;
	public MediaPlayInstruction mode;
	public AudioCategory audioType;
	public bool stopOthersOfType;
}
public enum MediaPlayInstruction {
	Play,
	Pause,
	Stop,
}