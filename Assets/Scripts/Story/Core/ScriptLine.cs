using System;
using System.Text;
using System.Reflection;

/// <summary>
/// A parsed script line. 
/// </summary>
[System.Serializable]
public class ScriptLine {
	// An optional, assumed "unique" id for the line.
	public string id;
	public string stylesheetOverride = null;

	public ScriptLine () {}
	public ScriptLine (ScriptContent toClone) {
        this.id = toClone.id;
        this.stylesheetOverride = toClone.stylesheetOverride;
	}
	
	protected virtual void ParseDefaultArguments (InkParserUtility.ParsedArgumentCollection arguments) {
		arguments.TryGetValue("id", ref id);
        arguments.TryGetValue("stylesheet", ref stylesheetOverride);
	}

	protected virtual void ParseDefaultCSSArguments (InkParserUtility.ParsedArgumentCollection cssArguments) {
        cssArguments.TryGetValue("stylesheet", ref stylesheetOverride);
	}

	public override string ToString () {
		StringBuilder s = new StringBuilder("["+this.GetType()+"]\n");
		FieldInfo[] properties = GetType().GetFields();
		foreach (FieldInfo property in properties) {
			var value = property.GetValue(this);
			s.Append(" ");
			s.Append(property.Name);
			s.Append(":");
			s.Append(value == null ? "NULL" : value.ToString());
		}
		return s.ToString();
	}

	public class ParserScriptInfo {
		public Type parserType;
		public MethodInfo parseMethod;
		public int priority;

		public ParserScriptInfo (Type parserType) {
			this.parserType = parserType;
			PropertyInfo sortOrderPropertyInfo = parserType.GetProperty("parserSortPriority", BindingFlags.Public | BindingFlags.Static);
			if(sortOrderPropertyInfo != null) this.priority = (int)sortOrderPropertyInfo.GetValue(null, null);
			parseMethod = parserType.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static);
			if(parseMethod == null) UnityEngine.Debug.LogWarning("No TryParse method found on Parser Script "+parserType);
		}
	}
}