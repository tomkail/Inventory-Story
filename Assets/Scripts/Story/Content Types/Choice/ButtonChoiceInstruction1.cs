using System;
using UnityEngine;
using System.Text.RegularExpressions;
using Ink.Runtime;

[Serializable]
public class CombineItemsChoiceInstruction : ScriptChoice {
	public static int parserSortPriority => 1;
	
	static Regex _argumentAndValueParser;
	public static Regex argumentAndValueParser {
		get {
			// if(_argumentAndValueParser == null) _argumentAndValueParser = new Regex(@"\(\s*(.*?)\s*[:=]\s*(.*?\s*)\)");
			if(_argumentAndValueParser == null) _argumentAndValueParser = new Regex(@"(\w+?)\s*-\s*(\w+)");
			return _argumentAndValueParser;
		}
	}
	
	public static CombineItemsChoiceInstruction TryParse (Choice choice) {
		if(string.IsNullOrWhiteSpace(choice.text)) return null;

		var matches = argumentAndValueParser.Matches(choice.text);
		if (matches.Count == 0) return null;
		
		var instruction = new CombineItemsChoiceInstruction(choice);
		var match = matches.First();
		instruction.itemAKey = match.Groups[1].Value;
		instruction.itemBKey = match.Groups[2].Value;
        return instruction;
	}

	public string itemAKey;
	public string itemBKey;

	public bool MatchesItems(InkListItem itemA, InkListItem itemB) {
		return 
			(itemA.itemName.Equals(itemAKey, StringComparison.OrdinalIgnoreCase) && itemB.itemName.Equals(itemBKey, StringComparison.OrdinalIgnoreCase)) || 
			(itemA.itemName.Equals(itemBKey, StringComparison.OrdinalIgnoreCase) && itemB.itemName.Equals(itemAKey, StringComparison.OrdinalIgnoreCase));
	}
	public CombineItemsChoiceInstruction (Choice storyChoice) : base(storyChoice) {}
}