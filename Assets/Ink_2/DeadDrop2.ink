INCLUDE systems/gameloop.ink

INCLUDE scenes.ink
INCLUDE data/items.ink
INCLUDE systems/helpers.ink
INCLUDE systems/gamefuncs.ink
INCLUDE data/template.ink
INCLUDE content/opening.ink
INCLUDE content/ernst.ink


VAR FirstLevel = OpeningCaseScene

VAR previousSceneID = ()

VAR DEBUG = false


-> proceedTo(FirstLevel)

/*
    For testing: 
    Mortuary scene 
    - slot TornMapOfParisMetro and QsBusinessCard 
    
    Metro scene: 
    - use pocket knife to cut knotted piano wire 
    - use piano wire to open wallet (this makes no sense!) 
    
*/

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
    This includes dragging an item *out* of a solution slot. 
    
    
    DROP 
    When an item is dropped, call 
        DROP {item} 
        
    
    SLOT {item} 
    Called when an item is passed to the solution slot or removed from it. 
    Ink will keep the "currentItems" variable up to date.
    Slotting can ONLY HAPPEN when the player is in the top level container. (We can change this if we hate it).
    
    Note that UNSLOTting is not an action, this is just a PICKUP choice. The PICKUP choice will only be available when the player is at the top level container. 
    
    Note that if the game wants to "autounslot" an incorrect solution, it will need to zero the currentItems variable itself. 
    
    
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
    
    Raw content (no markup) - these are lines for the subtitle field. They are either text content (the default) - used for "descriptions" - or they are #tagged with a character name
    
    eg. 
    
    "Meet me at the Station" // text content, the player is "reading" the item 
    We found a few personal effects. #agent // the "agent" character speaks the line 
    
    
    
*/

    

    