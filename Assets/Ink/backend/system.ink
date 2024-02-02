
VAR levelItems = ()
VAR levelSolutionItemCount = 0
VAR currentItems = () 
VAR levelInteractables = ()
VAR levelSuccessFunction = -> FALSE_
VAR generatedItems = ()

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
    VAR asReplacement = false 
    { levelItems ? withItem || not withItem: 
        ~ asReplacement = false  // default to false 
        ~ temp toGenerate = itemGeneratesItems(item)
        <- use_item(item, withItem, toGenerate, asReplacement)
    } 
    { withItems:    // handle multiple solutions 
        -> opts 
    } 
=  use_item(item, withItem, toGenerate, replacing)
    +   { not (levelItems ^ toGenerate) }
        [ {DEBUG:USE} {item}  {withItem: {DEBUG:WITH|-} {withItem} } ]
        ~ addItems(toGenerate) 
        { 
        - levelItems ? Warp:
            ~ removeItem(levelItems - Warp)
        - replacing:
            ~ removeItem(item) 
        }
        [ now {levelItems} ]
        ->-> 


=== function addItems(items) 
    ~ generatedItems += items
    ~ levelItems += items
    ~ temp replacements = itemReplacesItemWhenGenerated(items) 
    ~ removeItem(replacements) 
    
    
=== function removeItem(items)
    ~ levelItems -= items

=== function require(item) 
    ~ return levelItems !? item


=== scene(items, interactables, VOLine)
    ~ temp title = "{getSceneData(currentSceneID, Title)}"
    ~ temp date = "{getSceneData(currentSceneID, Time)}"
    ~ levelSuccessFunction = getSceneData(currentSceneID, ExitKnot)
    ~ temp solnCount = levelSuccessFunction(())
    ~ StartScene (currentSceneID, title, date, solnCount, items)
// only set globals after scene instruction in case the observer fires
    ~ levelItems = items 
    ~ levelInteractables = interactables
    ~ levelSolutionItemCount = solnCount // returns an int
    ~ currentItems = ()
    ~ generatedItems = ()
    VO: {VOLine}
    -> play 
    
=== play
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
    
EXTERNAL StartScene  (sceneID, titleText, dateText, slotCount, startingItems)     
=== function StartScene  (sceneID, titleText, dateText, slotCount, startingItems) 
    [ {sceneID}: {titleText} / {dateText} ] 
    
=== function checkForSolution() 
    // don't bother unless the count is right, covers the 0-slotted case
    { LIST_COUNT(currentItems) == levelSolutionItemCount: 
        ~ temp result = levelSuccessFunction(currentItems)
        
        { previousSceneID ? result:
            // can't repeat a scene
            ~ return false 
        }
        
        { DEBUG:
            // assert result is a list 
            ~ temp i = LIST_MIN(result)
        }
        ~ return BOOL( result )
    - else: 
        ~ return false 
    }
    
    
=== function replaceAs(item) 
    ~ asReplacement = true
    ~ return item     

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