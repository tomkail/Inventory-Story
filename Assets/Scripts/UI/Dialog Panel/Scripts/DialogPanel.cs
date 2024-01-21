using System;
using UnityEngine;
using UnityEngine.UI;
using EasyButtons;

// This is a centered, full-screen panel that dims the rest of the screen and blocks input to other areas of the app. 
// Only one may be shown at the same time, although developers must currently enforce this themselves (sorry! Potential upgrade if anyone wants it)
// Unlike the modal menu, it's not created by passing a model but by directly showing and adding views to it - I'd prefer a model but it would have taken more time to build.
// Panel can optionally be set to close when the user clicks the background by setting canCloseByClickingBackground true.
[RequireComponent(typeof(SLayout))]
public class DialogPanel : MonoSingleton<DialogPanel> {
    public SLayout backgroundLayout;
    public Button backgroundButton;
    
    [Space]
    public DialogPanelPage pagePrefab;
    public DialogPanelTextUIView headerTextPrefab;
    public DialogPanelTextUIView bodyTextPrefab;
    public DialogPanelInputFieldUIView inputFieldPrefab;
    public DialogPanelButtonUIView buttonPrefab;
    public DialogPanelButtonUIView labelButtonPrefab;
    public DialogPanelLoadingBarUIView loadingBarPrefab;

    [Space]
    [Disable] public DialogPanelPage currentPage;
    public bool showing => currentPage != null;

    
    protected override void Awake () {
        base.Awake();
        backgroundButton.onClick.AddListener(OnClickBackgroundButton);
        backgroundLayout.canvasGroup.blocksRaycasts = backgroundLayout.canvasGroup.interactable = false;
        backgroundLayout.groupAlpha = 0;
        Clear();
    }
    
    void Update () {
        var shouldShowBackground = showing;
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

    public void HideAllPages() {
        var pages = GetComponentsInChildren<DialogPanelPage>();
        foreach(var page in pages) HidePage(page);
    }

    public void HidePage(DialogPanelPage page) {
        if (page == null) return;
        page.layout.CompleteAnimations();
        page.layout.groupAlpha = 1;
        page.layout.Animate(Styling.FastAnimationTime, () => {
            page.layout.groupAlpha = 0;
        }).Then(() => {
            Destroy(page.gameObject);
        });
    }

    public void HidePage () {
        if(currentPage == null) return;
        var _page = currentPage;
        currentPage = null;
        HidePage(_page);
    }


    public void ShowConfirmDialog(string areYouSureYouWantToDiscardYourChanges, string body, string confirmButtonText, Action confirmAction) {
        DialogPanelPage page = null;
        var dialogPanelModel = new DialogPanelModel();
        dialogPanelModel.canCloseByClickingBackground = true;
        if(!string.IsNullOrEmpty(areYouSureYouWantToDiscardYourChanges)) dialogPanelModel.items.Add(new DialogPanelLabelModel(areYouSureYouWantToDiscardYourChanges, DialogPanelLabelModel.TextType.Header));
        if(!string.IsNullOrEmpty(body)) dialogPanelModel.items.Add(new DialogPanelLabelModel(body, DialogPanelLabelModel.TextType.Body));
        dialogPanelModel.items.Add(new DialogPanelButtonModel(confirmButtonText, () => {
            HidePage(page);
            confirmAction?.Invoke();
        }));
        page = ShowPage(dialogPanelModel);
    }
    public void ShowConfirmDialog(string areYouSureYouWantToDiscardYourChanges, Action confirmAction) {
        ShowConfirmDialog(areYouSureYouWantToDiscardYourChanges, null, "Yes", confirmAction);
    }

    public void ShowConfirmDialog(string areYouSureYouWantToDiscardYourChanges, string confirmButtonText, Action confirmAction) {
        ShowConfirmDialog(areYouSureYouWantToDiscardYourChanges, null, confirmButtonText, confirmAction);
    }
    
    public DialogPanelPage ShowPage (DialogPanelModel model) {
        if (!Application.isPlaying) return null;
        // HidePage();

        currentPage = Instantiate(pagePrefab, transform);
        currentPage.model = model;
        foreach(var item in model.items) {
            if (item is DialogPanelLabelModel) {
                AddLabel((DialogPanelLabelModel)item);
            }
            // if(item is DialogPanelSeparatorModel) AddSeparator((DialogPanelSeparatorModel)item);
            if(item is DialogPanelButtonModel) AddButton((DialogPanelButtonModel)item);
            if(item is DialogPanelInputFieldModel) AddInputField((DialogPanelInputFieldModel)item);
            if(item is DialogPanelLoadingBarModel) AddLoadingBar((DialogPanelLoadingBarModel)item);
        }
        if(model.hasCloseButton) AddCloseButton();
        Layout();
        
        currentPage.layout.groupAlpha = 0;
        currentPage.layout.Animate(Styling.FastAnimationTime, () => {
            currentPage.layout.groupAlpha = 1;
        });
        return currentPage;
    }

    void AddLabel (DialogPanelLabelModel model) {
        DialogPanelTextUIView view = null;
        if(model.textType == DialogPanelLabelModel.TextType.Header) view = Instantiate(headerTextPrefab, currentPage.transform);
        else if(model.textType == DialogPanelLabelModel.TextType.Body) view = Instantiate(bodyTextPrefab, currentPage.transform);
        view.Init(model);
        currentPage.items.Add(view);
    }
    void AddButton (DialogPanelButtonModel model) {
        DialogPanelButtonUIView prefab = null;
        if (model.buttonType == DialogPanelButtonModel.ButtonType.Standard) prefab = buttonPrefab;
        else if (model.buttonType == DialogPanelButtonModel.ButtonType.Label) prefab = labelButtonPrefab;
        var view = Instantiate(prefab, currentPage.transform);
        view.Init(model);
        currentPage.items.Add(view);
    }
    // void AddSeparator (DialogPanelSeparatorModel model) {
    //     var view = Instantiate(separatorPrefab, currentPage.transform);
    //     view.Init(model);
    //     currentPage.items.Add(view);
    // }
    void AddInputField (DialogPanelInputFieldModel model) {
        var view = Instantiate(inputFieldPrefab, currentPage.transform);
        view.Init(model);
        currentPage.items.Add(view);
    }
    void AddLoadingBar (DialogPanelLoadingBarModel model) {
        var view = Instantiate(loadingBarPrefab, currentPage.transform);
        view.Init(model);
        currentPage.items.Add(view);
    }
    
    void AddCloseButton (Action onClick = null) {
        var page = currentPage;
        page.closeButton.gameObject.SetActive(true);
        page.closeButton.onClick.RemoveAllListeners();
        page.closeButton.onClick.AddListener(() => {
            HidePage(page);
            onClick?.Invoke();
        });
    }

    void Layout () {
        currentPage.Layout();
    }

    [Button("Clear")]
    public void Clear () {
        if(currentPage == null) return;
        var _page = currentPage;
        currentPage = null;

        backgroundLayout.CompleteAnimations();
        _page.layout.groupAlpha = 0;
        backgroundLayout.Animate(Styling.StandardAnimationTime, () => {
            _page.layout.groupAlpha = 1;
        }).Then(() => {
            Destroy(_page.gameObject);
        });
    }

    void OnClickBackgroundButton () {
        if(currentPage != null && currentPage.model.canCloseByClickingBackground)
            HidePage(currentPage);
    }
}