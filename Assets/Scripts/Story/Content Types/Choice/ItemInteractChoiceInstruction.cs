using System;
using Ink.Runtime;

[Serializable]
public class ItemInteractChoiceInstruction : ScriptChoice {
    public static int parserSortPriority => 0;
	
    public static ItemInteractChoiceInstruction TryParse (Choice choice) {
        if(string.IsNullOrWhiteSpace(choice.text)) return null;

        var instruction = new ItemInteractChoiceInstruction(choice);
        instruction.itemKey = choice.text.Trim();
        return instruction;
    }

    public string itemKey;

    public bool MatchesItem(InkListItem item) {
        return item.itemName.Equals(itemKey, StringComparison.OrdinalIgnoreCase);
    }
    public ItemInteractChoiceInstruction (Choice storyChoice) : base(storyChoice) {}
}