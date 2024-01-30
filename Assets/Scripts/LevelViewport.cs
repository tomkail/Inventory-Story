using UnityEngine;

[RequireComponent(typeof(SLayout))]
public class LevelViewport : MonoBehaviour {
    public SLayout layout => GetComponent<SLayout>(); 
    public LevelController levelController => GetComponent<LevelController>(); 
}