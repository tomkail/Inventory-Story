using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class VersionLabelView : MonoBehaviour {
    TMP_Text text => GetComponent<TMP_Text>();
    void OnEnable () {
        text.text = BuildInfo.Instance.ToString();
    }
}