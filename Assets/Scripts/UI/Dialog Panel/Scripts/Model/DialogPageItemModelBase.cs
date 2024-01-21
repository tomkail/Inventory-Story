[System.Serializable]
public class DialogPageItemModelBase {
    public System.Action onChange;
    public void TriggerOnChange() {
        onChange?.Invoke();
    }
}
