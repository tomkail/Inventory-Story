using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class VersionLabelView : MonoBehaviour {
    TMP_Text text => GetComponent<TMP_Text>();
    void OnEnable () {
        text.text = UnityX.Versioning.CurrentVersionSO.Instance.version.ToString();
    }
}