using UnityEditor;
using UnityEngine;

namespace BuildPipeline {
    [System.Serializable]
    public class BuildPipelineStep {
        public string name;
        public bool canBeDisabled = true;
        public bool enabled = true;
        public bool expandedInSettings = true;
        public BuildStageProgressTracker progressTracker = new BuildStageProgressTracker();

        public virtual void BeginRunPipeline() {
            progressTracker.Reset();
        }

        public virtual void DrawSettings() {
        }

        public virtual void DrawProgress() {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(name);
            BuildPipelineUtils.ProgressBar(progressTracker.label, progressTracker.progress);
            GUILayout.EndHorizontal();
        }
    }
}