using System.Collections.Generic;
using UnityEngine;

public interface ISlottable {
    public IEnumerable<ISlot> GetEnterableSlots();
    public Vector2 draggableScreenPoint { get; }
    public Camera draggableScreenCamera { get; }
}