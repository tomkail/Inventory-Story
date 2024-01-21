using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Ink.InstructionsSystem.Editor {
    /// <summary>
    /// Creates a link.xml file in the folder this file is in that ensures all the instruction scripts are included
    /// This is important because instruction scripts are loaded via reflection and would be lost if code stripping was enabled in Build Settings. 
    /// </summary>
    public class InkParserGenerateLinkXml : IPreprocessBuildWithReport {
        public int callbackOrder { get { return 0; } }
        const string k_LinkXml = "link.xml";
        
        public void OnPreprocessBuild(BuildReport report) {
            var linker = LinkXmlGenerator.CreateDefault();
            linker.AddTypes(Assembly.GetAssembly(typeof(ScriptLine)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(ScriptLine))));
            var g = AssetDatabase.FindAssets ( $"t:Script {nameof(InkParserGenerateLinkXml)}" );
            string fullPath = AssetDatabase.GUIDToAssetPath ( g [ 0 ] );
            fullPath = Path.Combine(Path.GetDirectoryName(fullPath), k_LinkXml);
            linker.Save(fullPath);
        }
    }
}