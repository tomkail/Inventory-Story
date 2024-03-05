INCLUDE systems/gameloop.ink
INCLUDE content/ernst.ink
INCLUDE scenes.ink
INCLUDE data/items.ink
INCLUDE systems/helpers.ink
INCLUDE systems/gamefuncs.ink



VAR previousSceneID = ()

VAR DEBUG = false


-> proceedTo(MortuaryScene)

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
    
    All items are Pickupable or Zoomable, never both, so we don't need to disambiguate between "zoom into me" and "pick me up".
    
    Unlockables can be Pickaupable; mostly because we pick things up to "look" at them. (e.g. Pickup the box to get "It's got a cominbation lock", then "drop the code" on it, to open the box. It'll then become a zoomable, and is no longer pickupable.) 
    
    ACTIONS
    =======
    
    
    ZOOM INTO
    When the player zooms into an item, there should always be a matching ink choice:
        ZOOM INTO {item} 
    Select this choice when the zoom is confirmed by the player (including when carrying an item). This will roll the ink, update the currentVisibleItems list, and thus tell you what's inside the container. We may well push art asset info from ink this way too as we move forwards. 
    It'll also update the currentContainer
    
    ZOOM OUT 
    When the player zooms out of a container (including while carrying an item), call this choice. It has no parameter. It'll update currentlyVisibleItems, to tell you what the container above has in it (which may have changed due to player actions).
    It'll also update the currentContainer
    
    
    PICKUP 
    When the player begins to drag an item, take the ink choice:
        PICKUP {item} 
        
    This will allow ink to inject a VO line, and track the carried item via the 'carrying' variables. (You might want to track it gameside, of course.)
    
    
    DROP 
    When an item is dropped, call 
        DROP {item} 
        
    
    SLOT / UNSLOT {item} 
    Called when an item is passed to the solution slot or removed from it. 
    Ink will keep the "currentItems" variable up to date.
    Slotting / Unslotting can ONLY HAPPEN when the player is in the top level container. (We can change this if we hate it).
    
    
    SOLVED 
    This option only appears when a valid solution has been slotted. 
    Thus if it's present, you can choose it directly. 
    We also provide "checkForSolution()" in case that's useful, but it's probably redundant. 
    Note that this choice only appears in the top level container.
    
    
    USE {item} - {carriedItem}
    Called when drag/dropping a carried item onto an item. 
    Note ink will surface this choice even if the player isn't carrying the correct item, so please check that!
    
    
    
    CONTENT
    =======
    
    [...] - any text in square brackets is a comment / debug info from ink, and should be skipped by the game.
    
    Voice-over lines:     VO lines are fired by the ink, and prefaced by a speaker. eg. 
        VO:     It's a silver device.
    Since I expect multiple actors/voices, we need to look for "NAME: text". Hope that's not too annoying. 
    - Can change this format if you have a different format you prefer.
    
    
    
*/

    

    