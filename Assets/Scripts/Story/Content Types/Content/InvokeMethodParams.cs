using UnityEngine;

[System.Serializable]
public class InvokeMethodParams {
    public bool hasValues => !string.IsNullOrEmpty(className) && !string.IsNullOrEmpty(methodName);

    public string className;
	public string methodName;

	public bool param1Defined;
	public object param1;

	public bool param2Defined;
	public object param2;

	public bool param3Defined;
	public object param3;

	public static bool TryParse (InkParserUtility.ParsedArgumentCollection arguments, out InvokeMethodParams invokeMethodParams) {
        invokeMethodParams = new InvokeMethodParams();
        if(!arguments.TryGetValue("className", ref invokeMethodParams.className)) return false;
        if(!arguments.TryGetValue("methodName", ref invokeMethodParams.methodName)) return false;
        if(arguments.TryGetValue("param1", ref invokeMethodParams.param1)) invokeMethodParams.param1Defined = true;
        if(arguments.TryGetValue("param2", ref invokeMethodParams.param2)) invokeMethodParams.param2Defined = true;
        if(arguments.TryGetValue("param3", ref invokeMethodParams.param3)) invokeMethodParams.param3Defined = true;
		return invokeMethodParams.hasValues;
	}

    public void InvokeMethod () {
        var classType = System.Type.GetType(className);
        if(classType == null) {
            Debug.LogWarning("Couldnt find class for EventInstruction with name \""+className+"\"");
        } else {
            var method = classType.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if(method == null) {
                Debug.LogWarning("Couldnt find public static method in class "+className+" for EventInstruction with name "+methodName);
            } else {
                try {
                    if(param1Defined && param2Defined && param3Defined) method.Invoke(null, new object[] {param1, param2, param3});
                    else if(param1Defined && param2Defined) method.Invoke(null, new object[] {param1, param2});
                    else if(param1Defined) method.Invoke(null, new object[] {param1});
                    else method.Invoke(null, null);
                } catch (System.Exception e) {
                    Debug.LogError("Failed running method "+method+" on "+classType+". "+param1Defined+" "+param2Defined+" "+param3Defined+" \n"+e.ToString());
                }
            }
        }
    }
}