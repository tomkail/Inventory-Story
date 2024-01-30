using Ink.Runtime;
using UnityEngine;
using System.Linq;

// Bit of a grab bag of useful story related functions. 
public static class StoryUtils {
	public static int[] GetValuesFromInkListVariable(InkList listVar) {
		if (listVar == null) return new int[0];
		var ids = new int[listVar.Count];

		int i = 0;
		foreach (var listItem in listVar) {
			ids[i] = (listItem.Value);
			i++;
		}
		return ids;
	}

	public static string[] GetKeysFromInkList(InkList listVar) {
		if(listVar == null) return new string[0];
		var ids = new string[listVar.Count];

		int i = 0;
		foreach(var listItem in listVar) {
			ids[i] = (listItem.Key.itemName);
			i++;
		}
		return ids;
	}

    // The function evaluator from Pendragon.
    // Gets output from either the return value or the out value, so we don't really need to worry too much about how Jon's written things.
    // Takes a type, so you don't need to cast.
    // Provides some handy errors when things go wrong.
    public static T RunInkFunction<T>(this Story story, string inkFunctionName, params object[] args) {
		object rawOutput = null;
		T output = default(T);
		try {
			string textOutput = null;
			rawOutput = story.EvaluateFunction(inkFunctionName, out textOutput, args);
			if (rawOutput == null)
			{
				// Hi yes this is hacky but it's already been written to expect output like this
				return (T)(object)textOutput;
			}
			else
			{
				if (typeof(T) == typeof(bool) && rawOutput is int)
				{
					output = (T)(object)((int)rawOutput).ToBool();
				}
				else if (typeof(T) == typeof(bool) && rawOutput is float)
				{
					output = (T)(object)(((float)rawOutput) == 1f ? true : false);
				}
				else if (typeof(T) == typeof(int) && rawOutput is float)
				{
					output = (T)(object)Mathf.RoundToInt((float)rawOutput);
				}
				else
				{
					output = (T)rawOutput;
				}
			}
		}
		catch (System.Exception err)
		{
			var tTypeName = typeof(T).Name;
			var argsListStr = DebugX.ListAsString(args, (object o) => {
				if (o == null) return "NULL";
				else return o.ToString() + " (" + o.GetType().Name + ")";
			}, false);
			var errorsListStr = DebugX.ListAsString(story.currentErrors, null, false);
			if (rawOutput == null)
			{
				DebugX.LogError(story, "Ink function error at: " + inkFunctionName + "(<" + tTypeName + ">)" + "\nArgs:\n" + argsListStr + "\nInk Errors\n" + errorsListStr + "\n\nErr" + err);
			}
			else {
				var rawOutputString = rawOutput == null ? "NULL" : (rawOutput.ToString() + "(" + rawOutput.GetType() + ")");
				var outputString = output == null ? "NULL" : (output.ToString() + "(" + output.GetType() + ")");
				DebugX.LogError(story, "Ink function error at: " + inkFunctionName + "(<" + tTypeName + ">)" + "\nArgs:\n " + argsListStr + "\n\nRawOutput " + rawOutputString + "\nOutput " + outputString + "\nInk Errors:\n" + errorsListStr + "\nErr" + err);
			}
		}
		return output;
	}

}