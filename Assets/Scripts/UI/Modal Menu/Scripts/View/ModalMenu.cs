using UnityEngine;
using UnityEngine.UI;

// This allows presenting the user with a list of options/labels. 
// It can be positioned to sit next to and point at the view that it relates to, such as a button.
// It may optionally dim the screen and steal clicks.
public class ModalMenu : MonoSingleton<ModalMenu> {
    public SLayout backgroundLayout;
    public Button backgroundButton;
    [Space]
    public ModalMenuPage pagePrefab;
    public ModalMenuSeparator separatorPrefab;
    public ModalMenuHeader headerPrefab;
    public ModalMenuLabel labelPrefab;
    public ModalMenuButton buttonPrefab;
    public ModalMenuShareWithView shareWithPrefab;

    [Space]
    [Disable] public ModalMenuPage currentPage;
    public bool showing => currentPage != null;

    protected override void Awake () {
        base.Awake();
        backgroundButton.onClick.AddListener(OnClickBackgroundButton);
        backgroundLayout.canvasGroup.interactable = backgroundLayout.canvasGroup.blocksRaycasts = false;
        backgroundLayout.groupAlpha = 0;
    }

    void OnClickBackgroundButton () {
        if(currentPage == null) return;
        if(currentPage.model.canCloseByClickingBackground) {
            Hide();
        }
    }

    void Update () {
        var shouldShowBackground = showing && currentPage.model.showBackground;
        if(shouldShowBackground && backgroundLayout.targetGroupAlpha != 1) {
            backgroundLayout.Animate(Styling.FastAnimationTime, () => {
                backgroundLayout.groupAlpha = 1;
            });
        } else if(!shouldShowBackground && backgroundLayout.targetGroupAlpha != 0) {
            backgroundLayout.Animate(Styling.StandardAnimationTime, () => {
                backgroundLayout.groupAlpha = 0;
            });
        }
        backgroundLayout.canvasGroup.interactable = backgroundLayout.canvasGroup.blocksRaycasts = shouldShowBackground;
    }
    
    public void Hide () {        
        HidePage();
    }

    // This is sort of a placeholder. Right now you can't have more than one page, but we expect to in the future.
    public void HidePage (ModalMenuPage page) {
        if(currentPage == page) HidePage();
    }

    public void HidePage () {
        if(currentPage == null) return;
        var _page = currentPage;
        currentPage = null;
        _page.model.OnClose?.Invoke();
        _page.layout.CompleteAnimations();
        _page.layout.groupAlpha = 1;
        _page.layout.Animate(Styling.FastAnimationTime, () => {
            _page.layout.groupAlpha = 0;
        }).Then(() => {
            Destroy(_page.gameObject);
        });
    }
    public ModalMenuPage ShowPage (ModalMenuPageModel model) {
        HidePage();

        currentPage = Instantiate(pagePrefab, transform);
        currentPage.model = model;
        foreach(var item in model.items) {
            if(item is ModalMenuHeaderModel) AddHeader((ModalMenuHeaderModel)item);
            if(item is ModalMenuLabelModel) AddLabel((ModalMenuLabelModel)item);
            if(item is ModalMenuSeparatorModel) AddSeparator((ModalMenuSeparatorModel)item);
            if(item is ModalMenuButtonModel) AddButton((ModalMenuButtonModel)item);
            if(item is ModalMenuShareWithModel) AddShareWithView((ModalMenuShareWithModel)item);
        }
        Layout();
        
        currentPage.layout.groupAlpha = 0;
        currentPage.layout.Animate(Styling.FastAnimationTime, () => {
            currentPage.layout.groupAlpha = 1;
        });
        return currentPage;
    }

    void AddHeader (ModalMenuHeaderModel model) {
        var view = Instantiate(headerPrefab, currentPage.transform);
        view.Init(model);
        currentPage.items.Add(view);
    }
    void AddLabel (ModalMenuLabelModel model) {
        var view = Instantiate(labelPrefab, currentPage.transform);
        view.Init(model);
        currentPage.items.Add(view);
    }
    void AddButton (ModalMenuButtonModel model) {
        var view = Instantiate(buttonPrefab, currentPage.transform);
        view.Init(model);
        currentPage.items.Add(view);
    }
    void AddShareWithView (ModalMenuShareWithModel model) {
        var view = Instantiate(shareWithPrefab, currentPage.transform);
        view.Init(model);
        currentPage.items.Add(view);
    }
    void AddSeparator (ModalMenuSeparatorModel model) {
        var view = Instantiate(separatorPrefab, currentPage.transform);
        view.Init(model);
        currentPage.items.Add(view);
    }

    void Layout () {
        currentPage.Layout();
    }








    // Returns a direction for the tooltip that best utilises empty screen space.
    // This function could be improved by choosing the direction that enables the screen rect to best fit on screen. 
    // For example, if the screen rect is as wide as the screen, it should return top or bottom because no left/right direction will prevent the menu from obscuring the rect
    public static ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection GetPreferredArrowDirection (Rect screenRect) {
        var screenSize = new Vector2(Screen.width, Screen.height);
        var centerDelta = screenSize * 0.5f - screenRect.center;
        if(Mathf.Abs(centerDelta.x) > Mathf.Abs(centerDelta.y)) {
            if(centerDelta.x > 0) return ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Left;
            else return ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Right;
        } else {
            if(centerDelta.y > 0) return ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Bottom;
            else return ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Top;
        }
    }

    public ModalMenuPageModel.ModalMenuPagePositionParams GetPositionParamsFromScreenPoint (Vector2 screenPoint, ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        return GetPositionParamsFromScreenRect(new Rect(screenPoint.x, screenPoint.y, 0, 0), arrowDirection, extraDistance);
    }

    public ModalMenuPageModel.ModalMenuPagePositionParams GetPositionParamsFromScreenRect (Rect screenRect, ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        return GetPositionParamsFromScreenRect((RectTransform)transform, screenRect, arrowDirection, extraDistance);
    }

    public static ModalMenuPageModel.ModalMenuPagePositionParams GetPositionParamsFromScreenRect (RectTransform container, Rect screenRect, ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        var positionParams = new ModalMenuPageModel.ModalMenuPagePositionParams();
        positionParams.arrowDirection = arrowDirection;

        var canvas = container.GetComponentInParent<Canvas>().rootCanvas;
        var screenPoints = new[] {
            new Vector2(screenRect.x, screenRect.y),
            new Vector2(screenRect.x+screenRect.width, screenRect.y),
            new Vector2(screenRect.x+screenRect.width, screenRect.y+screenRect.height),
            new Vector2(screenRect.x, screenRect.y+screenRect.height),
        };
        for(int i = 0; i < corners3D.Length; i++)
            corners2D[i] =  (Vector2)ScreenPointToLocalPointInRectangle(canvas, container, screenPoints[i]);
        var tooltipCanvasSpaceRect = CreateEncapsulating(corners2D);
        
        if(arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Bottom) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.center.x, tooltipCanvasSpaceRect.yMax+extraDistance);
        } else if(arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Top) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.center.x, tooltipCanvasSpaceRect.yMin-extraDistance);
        } else if(arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Left) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.xMax+extraDistance, tooltipCanvasSpaceRect.center.y);
        } else if(arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Right) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.xMin-extraDistance, tooltipCanvasSpaceRect.center.y);
        } else if(arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.BottomRight) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.xMin-extraDistance, tooltipCanvasSpaceRect.yMax+extraDistance);
        }
        return positionParams;
    }

    public ModalMenuPageModel.ModalMenuPagePositionParams GetPositionParamsFromTarget (RectTransform target, ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        return GetPositionParamsFromTarget((RectTransform)transform, target, arrowDirection, extraDistance);
    }

    public static ModalMenuPageModel.ModalMenuPagePositionParams GetPositionParamsFromTarget (RectTransform container, RectTransform target, ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection arrowDirection, float extraDistance = 0) {
        var positionParams = new ModalMenuPageModel.ModalMenuPagePositionParams();
        positionParams.arrowDirection = arrowDirection;

        var targetCanvas = target.GetComponentInParent<Canvas>().rootCanvas;
        var canvas = container.GetComponentInParent<Canvas>().rootCanvas;
        Rect tooltipCanvasSpaceRect = Rect.zero;
        target.GetWorldCorners(corners3D);
        if(canvas == targetCanvas) {
            for(int i = 0; i < corners3D.Length; i++)
                corners2D[i] = container.InverseTransformPoint(corners3D[i]);
        } else {
            for(int i = 0; i < corners3D.Length; i++)
                corners2D[i] =  (Vector2)ScreenPointToLocalPointInRectangle(canvas, container, RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, corners3D[i]));
        }
        tooltipCanvasSpaceRect = CreateEncapsulating(corners2D);
        
        if(arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Bottom) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.center.x, tooltipCanvasSpaceRect.yMax+extraDistance);
        } else if(arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Top) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.center.x, tooltipCanvasSpaceRect.yMin-extraDistance);
        } else if(arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Left) {
            positionParams.pivotPosition = new Vector2(tooltipCanvasSpaceRect.xMax+extraDistance, tooltipCanvasSpaceRect.center.y);
        } else if(arrowDirection == ModalMenuPageModel.ModalMenuPagePositionParams.ArrowDirection.Right) {
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