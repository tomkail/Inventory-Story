using UnityEngine;
using EasyButtons;

public class ModalMenuTester : MonoBehaviour
{
    // public ModalMenuPageModel page1 = new ModalMenuPageModel();
    public Rect screenRect {
        get {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if(rectTransform != null) {
                return rectTransform.GetScreenRect(rectTransform.GetComponentInParent<Canvas>());
            } else {
                return Camera.main.WorldToScreenRect(new Bounds(transform.position, transform.localScale));
            }
        }
    }
    public Sprite option1Icon;
    public Sprite option2Icon;
    public Sprite option3Icon;
    public Sprite option4Icon;

    [Button("Create")]
    public void Create () {
        ModalMenu.Instance.ShowPage(CreatePage1());
    }
    ModalMenuPageModel CreatePage1 () {
        var model = new ModalMenuPageModel();
        model.positionParams = ModalMenu.Instance.GetPositionParamsFromScreenRect(screenRect, ModalMenu.GetPreferredArrowDirection(screenRect));
        model.items.Add(new ModalMenuHeaderModel("Home Page"));
        model.items.Add(new ModalMenuButtonModel("Option 1", option1Icon, () => {
            ModalMenu.Instance.ShowPage(CreateOption1Page());
        }));
        model.items.Add(new ModalMenuSeparatorModel());
        model.items.Add(new ModalMenuButtonModel("Option 2", option2Icon, () => {
            ModalMenu.Instance.ShowPage(CreateOption2Page());
        }));
        model.items.Add(new ModalMenuSeparatorModel());
        model.items.Add(new ModalMenuButtonModel("Option 3", option3Icon, () => {
            ModalMenu.Instance.ShowPage(CreateOption3Page());
        }));
        model.items.Add(new ModalMenuSeparatorModel());
        model.items.Add(new ModalMenuButtonModel("Option 4", option4Icon, () => {
            ModalMenu.Instance.ShowPage(CreateOption4Page());
        }));
        return model;
    }
    ModalMenuPageModel CreateOption1Page () {
        var model = new ModalMenuPageModel();
        model.positionParams = ModalMenu.Instance.GetPositionParamsFromScreenRect(screenRect, ModalMenu.GetPreferredArrowDirection(screenRect));
        model.items.Add(new ModalMenuHeaderModel("Option 1"));
        model.items.Add(new ModalMenuButtonModel("Do something destructive", option1Icon, () => {
            DialogPanel.Instance.ShowPage(CreateDialogPanelPage());
        }));
        model.items.Add(new ModalMenuSeparatorModel());
        model.items.Add(new ModalMenuButtonModel("Back", option1Icon, () => {
            ModalMenu.Instance.ShowPage(CreatePage1());
        }));
        return model;
    }

    ModalMenuPageModel CreateOption2Page () {
        var model = new ModalMenuPageModel();
        model.positionParams = ModalMenu.Instance.GetPositionParamsFromScreenRect(screenRect, ModalMenu.GetPreferredArrowDirection(screenRect));
        model.items.Add(new ModalMenuHeaderModel("Option 2"));
        model.items.Add(new ModalMenuSeparatorModel());
        model.items.Add(new ModalMenuButtonModel("Back", option1Icon, () => {
            ModalMenu.Instance.ShowPage(CreatePage1());
        }));
        return model;
    }
    ModalMenuPageModel CreateOption3Page () {
        var model = new ModalMenuPageModel();
        model.positionParams = ModalMenu.Instance.GetPositionParamsFromScreenRect(screenRect, ModalMenu.GetPreferredArrowDirection(screenRect));
        model.items.Add(new ModalMenuHeaderModel("Option 3"));
        model.items.Add(new ModalMenuButtonModel("Option 1", option1Icon, () => {
            
        }));
        model.items.Add(new ModalMenuButtonModel("Option 2", option1Icon, () => {
            
        }));
        model.items.Add(new ModalMenuButtonModel("Option 3", option1Icon, () => {
            
        }));
        model.items.Add(new ModalMenuSeparatorModel());
        model.items.Add(new ModalMenuButtonModel("Option 4", option1Icon, () => {
            
        }));
        model.items.Add(new ModalMenuButtonModel("Option 5", option1Icon, () => {
            
        }));
        model.items.Add(new ModalMenuSeparatorModel());
        model.items.Add(new ModalMenuButtonModel("Back", option1Icon, () => {
            ModalMenu.Instance.ShowPage(CreatePage1());
        }));
        return model;
    }
    ModalMenuPageModel CreateOption4Page () {
        var model = new ModalMenuPageModel();
        model.positionParams = ModalMenu.Instance.GetPositionParamsFromScreenRect(screenRect, ModalMenu.GetPreferredArrowDirection(screenRect));
        model.items.Add(new ModalMenuHeaderModel("Option 4"));
        model.items.Add(new ModalMenuButtonModel("Option 1", option1Icon, () => {
            
        }));
        model.items.Add(new ModalMenuSeparatorModel());
        model.items.Add(new ModalMenuButtonModel("Option 2", option1Icon, () => {
            
        }));
        model.items.Add(new ModalMenuSeparatorModel());
        model.items.Add(new ModalMenuButtonModel("Back", option1Icon, () => {
            ModalMenu.Instance.ShowPage(CreatePage1());
        }));
        return model;
    }
    
    DialogPanelModel CreateDialogPanelPage() {
        var model = new DialogPanelModel();
        model.canCloseByClickingBackground = true;
        model.items.Add(new DialogPanelLabelModel("Delete something important?", DialogPanelLabelModel.TextType.Header));
        model.items.Add(new DialogPanelLabelModel("This will do something really nasty that you probably don't want to do.", DialogPanelLabelModel.TextType.Header));
        model.items.Add(new DialogPanelButtonModel("Yeah!", () => {
            var model = new DialogPanelModel();
            model.canCloseByClickingBackground = true;
            model.items.Add(new DialogPanelLabelModel("Oh god you really did it!", DialogPanelLabelModel.TextType.Header));
            model.items.Add(new DialogPanelButtonModel("Ooops.", () => {
                ModalMenu.Instance.ShowPage(CreatePage1());
            }));
            DialogPanel.Instance.ShowPage(model);
        }));
        model.items.Add(new DialogPanelButtonModel("Best not.", () => {
            DialogPanel.Instance.Clear();
        }));
        return model;
    }
}