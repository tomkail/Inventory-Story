INCLUDE systems/gameloop.ink
INCLUDE content/ernst.ink
INCLUDE scenes.ink
INCLUDE data/items.ink
INCLUDE systems/helpers.ink
INCLUDE systems/gamefuncs.ink



VAR previousSceneID = ()

VAR DEBUG = false




-> proceedTo(MetroPlatformScene)

/*

    Initiailisation:
    - ensure DEBUG ink var. is forced to FALSE 
    - ensure TunnelDepth External is defined (code is commented in gamefuncs.ink)

    Setup:
    - every scene begins with a >>> SAVE instruction and a call to StartScene external function
    - StartScene  (sceneID, titleText, dateText, slotCount)
        - note: we've remove the "levelSolutionItemCount" global so the slotCount parameter is the source of truth for number of output elements
    
    - the players current active "container" is stored in the 'currentContainer' variable 
    
    - the game should monitor "currentlyVisibleItems" for the items present within this container. 
        - - the game is responsible for tracking what has been scanned in the scene
    
    - items in the currentVisibleItems list can be zoomed into, or picked up, or have items dropped onto them (called "unlockables" in the ink) 
    - to know what an item is we supply lists (only scoped to the current container)
        - Zoomables 
        - Unlockables
        - Pickupables
        
    
    
    ACTIONS
    =======
    
    PICKUP 
    When the player begins to drag an item, take the ink choice:
        PICKUP {item} 
        
    This will allow ink to inject a VO line, and track the carried item via the 'carrying' variables. (You might want to track it gameside, of course.)
    
    DROP 
    When an item is dropped, call 
        DROP {item} 
    
    SLOT/UNSLOT are NOT provided to the game. This should just "happen".
    
    ZOOM INTO
    When the player zooms into an item, there should always be a matching ink choice:
        ZOOM INTO {item} 
    Select this choice when the zoom is confirmed by the player (including when carrying an item). This will roll the ink, update the currentVisibleItems list, and thus tell you what's inside the container. We may well push art asset info from ink this way too as we move forwards. 
    It'll also update the currentContainer
    
    ZOOM OUT 
    When the player zooms out of a container (including while carrying an item), call this choice. It has no parameter. It'll update currentlyVisibleItems, to tell you what the container above has in it (which may have changed due to player actions).
    It'll also update the currentContainer
    
    
    SOLVED 
    
    
    
    CONTENT
    =======
    
    [...] - any text in square brackets is a comment / debug info from ink, and should be skipped by the game.
    
    Voice-over:     VO lines are fired by the ink, and prefaced by a speaker. eg. 
        VO:     It's a silver device.
    Since I expect multiple actors/voices, we need to look for "NAME: text". Hope that's not too annoying. 
    
    
    
*/

    

    