
VAR levelItems = ()
VAR levelSolutionItemCount = 0
VAR currentItems = () 
VAR levelInteractables = ()
VAR levelSuccessFunction = -> FALSE_


=== use(item) 
    { levelItems !? item:
        -> DONE 
    }
    ~ temp withItems = itemRequiresItem(item)
    
    { withItems && not (levelItems ^ withItems) :  
        -> DONE // need an item you don't have 
    }
- (opts)
    ~ temp withItem = pop(withItems) 
    ~ temp asReplacement = false 
    ~ temp toGenerate = itemGeneratesItems(item, asReplacement)
    { levelItems ? withItem || not withItem: 
        +   { levelItems !? toGenerate}
            [ {DEBUG:USE} {item}  {withItem: {DEBUG:WITH|-} {withItem} } ]
            
            ~ addItems(toGenerate) 
            { 
            - levelItems ? Warp:
                ~ removeItem(levelItems - Warp)
            - asReplacement:
                ~ removeItem(item) 
            }
            [ now {levelItems} ]
            ->-> 
    } 
    { withItems:    // handle multiple solutions 
        -> opts 
    } 



=== function addItems(items) 
    ~ levelItems += items
    
=== function removeItem(items)
    ~ levelItems -= items

=== function require(item) 
    ~ return levelItems !? item


=== scene(items, interactables, VOLine)
    ~ temp title = "{getSceneData(currentSceneID, Title)}"
    ~ temp date = "{getSceneData(currentSceneID, Time)}"
    [ {currentSceneID}  ] 
    >>> Scene (title={title}) (date={date})
    ~ levelItems = items 
    ~ levelInteractables = interactables
    ~ levelSuccessFunction = getSceneData(currentSceneID, ExitKnot)
    ~ levelSolutionItemCount = levelSuccessFunction(()) // returns an int
    ~ currentItems = () 
    VO: {VOLine}
    -> play 
    
=== play
    [{previousSceneID}]
    {previousSceneID && currentSceneID > previousSceneID: 
        // && LoopCount > 1:
        +   [BACK] 
            ~ currentSceneID = ()
            ~ temp nextScene = pop_top(previousSceneID)
            -> proceedTo(nextScene)
            
    }
    -> act(levelInteractables) -> play

= act(interactables)
    ~ temp item = pop(interactables) 
    { item: 
        <- use( item )
    }
    { interactables: 
        -> act(interactables)
    }
    -> offer   
    

=== offer()
    ~ temp items = levelItems
    {not DEBUG: 
        -> ingame
    }
    ~ temp freeSlots = levelSolutionItemCount - LIST_COUNT(currentItems)
    [ {LIST_COUNT(currentItems)} / {levelSolutionItemCount} ]
- (opts)    
    ~ temp item = pop(items) 
    { item: 
        <- slot(item, freeSlots) 
    }
    {items: 
        -> opts
    }
    -> DONE
= ingame 
    +   (solved) [ SOLVED ] 
        >>> SAVE
        -> proceedTo(levelSuccessFunction(currentItems))
    
= slot(item, freeSlots) 
    +   { currentItems  ? item } 
        [  UNSLOT {getItemName(item)} ]
        ~ currentItems -= item 
    +   { currentItems  !? item } { freeSlots }
        [  SLOT {getItemName(item)} - {getItemTooltip(item)}]
        ~ currentItems += item 
        {
        - checkForSolution():
            ->-> solved
        - freeSlots == 1: 
            [ UNSLOTTING ] 
            ~ currentItems = ()
        }
    -   ->->
    
=== function checkForSolution() 
    // don't bother unless the count is right, covers the 0-slotted case
    { LIST_COUNT(currentItems) == levelSolutionItemCount: 
        ~ return BOOL( levelSuccessFunction(currentItems) )
    - else: 
        ~ return false 
    }
    
    
    

=== function got(item) 
    ~ return levelItems ? item

=== proceedTo(nextSceneIDToHit)
    ~ previousSceneID += currentSceneID
    ~ currentSceneID = nextSceneIDToHit
    ~ temp nextSceneToHit = getSceneData(currentSceneID, Knot)
    -> nextSceneToHit

=== function is(sc)
    ~ return currentSceneID == sc
=== function before(sc)
    ~ return currentSceneID > sc
=== function after(sc)
    ~ return currentSceneID < sc
=== function isOrBefore(sc)
    ~ return currentSceneID >= sc
=== function isOrAfter(sc)
    ~ return currentSceneID <= sc