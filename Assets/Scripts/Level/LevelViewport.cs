using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SLayout))]
public class LevelViewport : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>(); 
    public Level Level => GetComponent<Level>();
    public Image background;
}