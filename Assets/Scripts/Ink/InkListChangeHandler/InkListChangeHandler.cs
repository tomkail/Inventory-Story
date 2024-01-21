using Ink.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InkListChangeHandler {
    [UnityEngine.SerializeField]
    string _variableName;
    public string variableName => _variableName;
    [UnityEngine.SerializeField]
    bool observing;

    InkList _inkList;
    public InkList inkList => _inkList;

    List<InkListItem> prevListItems = new List<InkListItem>();
    [UnityEngine.SerializeField]
    List<InkListItem> _currentListItems = new List<InkListItem>();
    public IReadOnlyList<InkListItem> currentListItems => _currentListItems;
    List<InkListItem> itemsAdded = new List<InkListItem>();
    List<InkListItem> itemsRemoved = new List<InkListItem>();

    public delegate void OnChangeDelegate(IReadOnlyList<InkListItem> currentListItems, IReadOnlyList<InkListItem> itemsAdded, IReadOnlyList<InkListItem> itemsRemoved);
    public OnChangeDelegate OnChange;
    
    public InkListChangeHandler (string variableName) {
        this._variableName = variableName;
    }

    public void RefreshValue (Story story, bool silently) {
        if (!observing) return;
        OnInkVarChanged(variableName, story.variablesState[variableName], silently);
    }

    public void AddVariableObserver (Story story) {
        if(observing) {
            Debug.LogWarning("Tried observing story for variable with name "+_variableName+" but we're already observing. Please call RemoveVariableObserver first!");
            return;
        }

        try {
            story.ObserveVariable(variableName, OnInkVarChanged);
            observing = true;
        } catch (Exception e) {
            Debug.LogWarning(e);
        }
    }

    public void RemoveVariableObserver (Story story) {
        if(!observing) return;
        if(story != null) story.RemoveVariableObserver(OnInkVarChanged, _variableName);
        observing = false;
    }

    void OnInkVarChanged (string variableName, object newValue) {
        OnInkVarChanged(variableName, newValue, false);
    }
    void OnInkVarChanged (string variableName, object newValue, bool silently) {
        _inkList = (InkList)newValue;
        
        prevListItems.Clear();
        prevListItems.AddRange(_currentListItems);

        _currentListItems.Clear();
		foreach(var listItem in inkList)
			_currentListItems.Add(listItem.Key);
        
        if(IEnumerableX.GetChanges(prevListItems, _currentListItems, out itemsRemoved, out itemsAdded)) {
            if (!silently && OnChange != null) 
                OnChange(_currentListItems, itemsAdded, itemsRemoved);
        }
    }
}