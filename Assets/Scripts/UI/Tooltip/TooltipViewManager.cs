using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Manages tooltips, including some helper functions
public class TooltipViewManager : MonoSingleton<TooltipViewManager> {
    [SerializeField]
    Prototype tooltipPrototype = null;
    [SerializeField]
    Prototype tooltipLabelContentPrototype = null;
    [SerializeField, Disable]
    List<TooltipView> _currentTooltips = new List<TooltipView>();
    public IEnumerable<TooltipView> currentTooltips => _currentTooltips;
    
    public bool AnyWithOwner (object owner) {
        for (int i = _currentTooltips.Count - 1; i >= 0; i--) {
            TooltipView tooltip = _currentTooltips[i];
            if(tooltip.owner == owner) {
                return true;
            }
        }
        return false;
    }
    public IEnumerable<TooltipView> FindAllWithOwner (object owner) {
        for (int i = _currentTooltips.Count - 1; i >= 0; i--) {
            TooltipView tooltip = _currentTooltips[i];
            if(tooltip.owner == owner) {
                yield return tooltip;
            }
        }
    }
    public void HideAllWithOwner (object owner) {
        for (int i = _currentTooltips.Count - 1; i >= 0; i--) {
            TooltipView tooltip = _currentTooltips[i];
            if(tooltip.owner == owner) {
                tooltip.Hide();
                _currentTooltips.RemoveAt(i);
            }
        }
    }
    public void HideAll () {
        for (int i = _currentTooltips.Count - 1; i >= 0; i--) {
            TooltipView tooltip = _currentTooltips[i];
            if (tooltip != null) tooltip.Hide();
            _currentTooltips.RemoveAt(i);
        }
    }
    public void Hide (TooltipView tooltip) {
        if(tooltip != null) {
            tooltip.Hide();
            _currentTooltips.Remove(tooltip);
        }
    }
    
    
    public TooltipView CreateAndShow (TooltipParams tooltipParams) {
        if(tooltipParams.containedLayout == null) return null;
        var newTooltip = tooltipPrototype.Instantiate<TooltipView>();
        newTooltip.transform.SetAsLastSibling();
        newTooltip.Init(tooltipParams);
        newTooltip.Show();
        _currentTooltips.Add(newTooltip);
        return newTooltip;
    }

    // Creates contained labels in the standard style to be passed into a tooltip.
    public TooltipLabelContent CreateTooltipLabelContent (params TooltipLine[] lines) {
        var tooltipLabelContent = tooltipLabelContentPrototype.Instantiate<TooltipLabelContent>();
        tooltipLabelContent.Init(lines);
        return tooltipLabelContent;
    }

    public TooltipParams.TooltipPositionParams GetPositionParamsFromScreenPoint (Vector2 screenPoint, TooltipParams.TooltipPositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        return GetPositionParamsFromScreenRect(new Rect(screenPoint.x, screenPoint.y, 0, 0), arrowDirection, extraDistance);
    }

    public TooltipParams.TooltipPositionParams GetPositionParamsFromScreenRect (Rect screenRect, TooltipParams.TooltipPositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        return GetPositionParamsFromScreenRect((RectTransform)transform, screenRect, arrowDirection, extraDistance);
    }

    public static TooltipParams.TooltipPositionParams GetPositionParamsFromScreenRect (RectTransform container, Rect screenRect, TooltipParams.TooltipPositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        var positionParams = new TooltipParams.TooltipPositionParams();
        positionParams.arrowDirection = arrowDirection;

        var canvas = container.GetComponentInParent<Canvas>().rootCanvas;
        var screenPoints = new Vector2[] {
            new Vector2(screenRect.x, screenRect.y),
            new Vector2(screenRect.x+screenRect.width, screenRect.y),
            new Vector2(screenRect.x+screenRect.width, screenRect.y+screenRect.height),
            new Vector2(screenRect.x, screenRect.y+screenRect.height),
        };
        for(int i = 0; i < corners3D.Length; i++)
            corners2D[i] =  (Vector2)ScreenPointToLocalPointInRectangle(canvas, container, screenPoints[i]);
        var tooltipCanvasSpaceRect = CreateEncapsulating(corners2D);
        
        if(arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Bottom) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.center.x, tooltipCanvasSpaceRect.yMax+extraDistance);
        } else if(arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Top) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.center.x, tooltipCanvasSpaceRect.yMin-extraDistance);
        } else if(arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Left) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.xMax+extraDistance, tooltipCanvasSpaceRect.center.y);
        } else if(arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Right) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.xMin-extraDistance, tooltipCanvasSpaceRect.center.y);
        }
        return positionParams;
    }

    public TooltipParams.TooltipPositionParams GetPositionParamsFromTarget (RectTransform target, TooltipParams.TooltipPositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        return GetPositionParamsFromTarget((RectTransform)transform, target, arrowDirection, extraDistance);
    }

    public static TooltipParams.TooltipPositionParams GetPositionParamsFromTarget (RectTransform container, RectTransform target, TooltipParams.TooltipPositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        var positionParams = new TooltipParams.TooltipPositionParams();
        positionParams.arrowDirection = arrowDirection;

        var targetCanvas = target.GetComponentInParent<Canvas>().rootCanvas;
        var canvas = container.GetComponentInParent<Canvas>().rootCanvas;
        Rect tooltipCanvasSpaceRect = Rect.zero;
        target.GetWorldCorners(corners3D);
        if(canvas == targetCanvas) {
            for(int i = 0; i < corners3D.Length; i++)
                corners2D[i] = (Vector2)container.InverseTransformPoint(corners3D[i]);
        } else {
            for(int i = 0; i < corners3D.Length; i++)
                corners2D[i] =  (Vector2)ScreenPointToLocalPointInRectangle(canvas, container, (Vector2)RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, corners3D[i]));
        }
        tooltipCanvasSpaceRect = CreateEncapsulating(corners2D);
        
        if(arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Bottom) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.center.x, tooltipCanvasSpaceRect.yMax+extraDistance);
        } else if(arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Top) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.center.x, tooltipCanvasSpaceRect.yMin-extraDistance);
        } else if(arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Left) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.xMax+extraDistance, tooltipCanvasSpaceRect.center.y);
        } else if(arrowDirection == TooltipParams.TooltipPositionParams.ArrowDirection.Right) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.xMin-extraDistance, tooltipCanvasSpaceRect.center.y);
        }
        return positionParams;
    }


    
    static Vector2[] corners2D = new Vector2[4];
    static Vector3[] corners3D = new Vector3[4];
	static Rect CanvasToScreenRect (Canvas canvas, RectTransform rectTransform) {
		rectTransform.GetWorldCorners(corners3D);
		return MinMaxRect(RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners3D[0]), RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners3D[2]));
	}

    static Vector3? ScreenPointToLocalPointInRectangle (Canvas canvas, RectTransform rectTransform, Vector2 screenPoint) {
		Camera camera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
		Vector2 localPosition;
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, camera, out localPosition))
			return localPosition;
		else return null;
	}

    static Rect MinMaxRect (Vector2 min, Vector2 max) {
		return Rect.MinMaxRect (min.x, min.y, max.x, max.y);
	}

    /// <summary>
	/// Creates new rect that encapsulates a list of vectors.
	/// </summary>
	/// <param name="vectors">Vectors.</param>
	static Rect CreateEncapsulating (params Vector2[] vectors) {
		float xMin = vectors[0].x;
		float xMax = vectors[0].x;
		float yMin = vectors[0].y;
		float yMax = vectors[0].y;
		for(int i = 1; i < vectors.Length; i++) {
			var vector = vectors[i];
			xMin = Mathf.Min (xMin, vector.x);
			xMax = Mathf.Max (xMax, vector.x);
			yMin = Mathf.Min (yMin, vector.y);
			yMax = Mathf.Max (yMax, vector.y);
		}
		return Rect.MinMaxRect (xMin, yMin, xMax, yMax);
	}
}

/* Example of this in use, as of 30/07/2021
public class TooltipTest : MonoBehaviour {
    public new Camera camera;
    public Transform target;
    public TooltipView tooltip = null;

    void Update() {
        var screenRect = camera.WorldToScreenRect(new Bounds(target.position, target.localScale));
        var tooltipPositionParams = TooltipViewManager.instance.GetPositionParamsFromScreenRect(screenRect, TooltipParams.TooltipPositionParams.ArrowDirection.Top);
        if(tooltip == null) {
            var tooltipContent = TooltipViewManager.instance.CreateTooltipLabelContent(new TooltipLine("Where should Veronica should go next?", TooltipLine.TooltipLineType.Normal));
            tooltip = TooltipViewManager.instance.CreateAndShow(new TooltipParams(this, tooltipPositionParams, tooltipContent.layout, null));
        }
        tooltip.tooltipParams.positionParams = tooltipPositionParams;
        tooltip.RefreshPosition();
    }
}
*/