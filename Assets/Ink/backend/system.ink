LIST Container = SceneTop

VAR levelItems = ()
VAR levelSolutionItemCount = 0
VAR currentItems = () 
VAR levelInteractables = ()

VAR generatedItems = ()
VAR levelGameplayFunction = -> FALSE_

VAR withItem = ()
VAR usedItems = ()

=== use(item) 
    { levelItems !? item:
        -> DONE 
    }
    ~ temp withItems = itemRequiresItem(item)
    
    { withItems && not (levelItems ^ withItems) :  
        -> DONE // need an item you don't have 
    }
- (opts)
    ~ withItem = pop(withItems) 
    VAR asReplacement = false 
    { levelItems ? withItem || not withItem: 
        ~ asReplacement = false  // default to false 
        ~ temp toGenerate = itemGeneratesItems(item) 
        {  toGenerate:
            <- use_item(item, toGenerate, withItem, asReplacement)
        }
    } 
    { withItems:    // handle multiple solutions 
        -> opts 
    } 
=  use_item(item, toGenerate, _withItem, replacing)
    +   { not (levelItems ^ toGenerate) }
        [ {DEBUG:USE} {item}  {_withItem: {DEBUG:WITH|-} {_withItem} } ]
        ~ withItem = _withItem
        ~ addItems(toGenerate) 
        { 
        - levelItems ? Warp:
            ~ removeItem(levelItems - Warp)
        - replacing:
            ~ removeItem(item) 
        }
        { OneUseOnlyItems ? item: 
            ~ levelInteractables -= item
        }
        [ now {levelItems} ]
        ~ postItemInteraction(item)
        ->-> 


=== function addItems(items) 
    ~ generatedItems += items
    ~ levelItems += items
    ~ temp replacements = itemReplacesItemWhenGenerated(items) 
    // [ adding {items} means removing {replacements} ... ]
    ~ removeItem(replacements) 
    
    
=== function removeItem(items)
    ~ levelItems -= items

=== function require(item) 
    ~ return levelItems !? item


=== scene(items, interactables, VOLine)
// Tom notes that he'd have preferred to use the Save() external ink function, but it seems like ink is still in the middle of processing the line when the save occurs, which means that it loads from the line before the Save() function, causing it to save immediately on load. This approach forces ink to complete the line before it's processed.
    >>> SAVE
    ~ temp title = "{getSceneData(currentSceneID, Title)}"
    ~ temp date = "{getSceneData(currentSceneID, Time)}"
    ~ levelGameplayFunction = getSceneData(currentSceneID, GameplayKnot)
    ~ temp solnCount = levelGameplayFunction(Sequence, ())
    ~ StartScene (currentSceneID, title, date, solnCount, items)
// only set globals after scene instruction in case the observer fires
    ~ levelItems = items 
    ~ relate(SceneTop, ItemHoldsItem, items)
    ~ levelInteractables = interactables
    ~ levelSolutionItemCount = solnCount // returns an int
    
    ~ currentItems = ()
    ~ generatedItems = ()
    ~ usedItems = ()
    ~ inside = (SceneTop)
    ~ carrying = ()
    ~ unrelateEverything() 
    
    ~ relate(SceneTop, ItemHoldsItem, levelItems)
    VO: {VOLine}
    {not DEBUG || not DEBUG_BEAT_BY_BEAT:  -> play } 
- (loop)
    -> detailed_play -> loop

    
VAR carrying = () 
VAR inside = (SceneTop) 
=== detailed_play
    <- specific
    <- overview
    -> DONE 
    
= overview
    ~ temp freeSlots = levelSolutionItemCount - LIST_COUNT(currentItems)
    [carrying: {carrying} ]
    +   { inside == SceneTop && carrying } 
        [ SLOT {carrying} ] 
        ~ currentItems += carrying 
        {
        - checkForSolution():
            ->-> solved
        - freeSlots == 1: 
            [ UNSLOTTING ] 
            ~ currentItems = ()
        }
    +   {inside != SceneTop} [ ZOOM OUT ] 
        ~ inside = parent(inside)
        
    +   { carrying } [ DROP {carrying} ]
        ~ carrying = ()
        
    -   ->->
= specific
    ~ temp interactables = whatIs(inside, ItemHoldsItem) ^ levelItems
   [{inside} > {list_with_commas(interactables, -> getItemName)}]
   
- (looptop)
    ~ temp item = pop(interactables) 
    
    { item: 
        +   { not carrying } [ PICK UP {item} ]
            {getItemTooltip(item)} 
            ~ carrying = item 
            ->-> 
    }
        
    { levelInteractables ? item: 
        ~ temp toGenerate = itemGeneratesItems(item)
        ~ temp withItems = itemRequiresItem(item)
        ~ temp childItems = whatIs(item, ItemHoldsItem)
        
        {  withItems ^ levelItems && toGenerate:
            <- useWithItems(item, withItems)
        }
    
        +   { (not withItems && toGenerate) || childItems } 
            { carrying != item } { not carrying || childItems }
            [ ZOOM INTO {item} ]
            ~ inside = item 
            -> generateFrom(item)
    }
    { interactables: -> looptop }
    -> DONE  
    
= useWithItems (item, withItems) 
- (withloop) 
    ~ temp _withItem = pop(withItems) 
    
    +   {carrying == _withItem} 
        { usedItems !? item || OneUseOnlyItems !? item }
        [ USE {item} - {_withItem} ] 
        ~ carrying = ()
        -> generateFrom(item)
        
    
= generateFrom(item)
    ~ asReplacement = false     // default to false 
    ~ temp toGenerate = itemGeneratesItems(item)
    ~ addItems(toGenerate) 
    ~ usedItems += item
    { asReplacement:
        ~ removeItem(item) 
        -> applyItemAtLevel(toGenerate, parent(item)) -> 
    - else: 
        -> applyItemAtLevel(toGenerate, item) ->
    }
    
    ->->  
    
= applyItemAtLevel(toGenerate, parentItem)
    ~ temp newItem = pop(toGenerate) 
    { newItem: 
        ~ temp specificParent = levelGameplayFunction(Parent, newItem) 
        {specificParent:
            ~ relate(specificParent, ItemHoldsItem, newItem)  
        - else: 
            ~ relate(parentItem, ItemHoldsItem, newItem)  
        }
        -> applyItemAtLevel(toGenerate, parentItem)
    } 
    ->-> 
  
=== function isZoomable(item) 
    ~ temp toGenerate = itemGeneratesItems(item)
    ~ temp childItems = whatIs(item, ItemHoldsItem)
    ~ return BOOL(toGenerate || childItems)
    
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
    +   [ SOLVED ] 
        -> solved 
    
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
    
=== solved
    -> proceedTo(levelGameplayFunction(Sequence, currentItems))
    
    
EXTERNAL StartScene  (sceneID, titleText, dateText, slotCount, startingItems)     
=== function StartScene  (sceneID, titleText, dateText, slotCount, startingItems) 
    [ {sceneID}: {titleText} / {dateText} ] 
    
=== function checkForSolution() 
    // don't bother unless the count is right, covers the 0-slotted case
    { LIST_COUNT(currentItems) == levelSolutionItemCount: 
    
        ~ temp result = levelGameplayFunction(Sequence, currentItems)
         
        { previousSceneID ? result && ReplayableScenes !? result:
            // can't repeat a scene (!) 
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
    

=== function generateOnce(item) 
    { generatedItems !? item: 
        ~ return item 
    } 
    ~ return () 
    
=== function replaceAs(item) 
    ~ asReplacement = true
    ~ return item     

=== function noLongerGot(item) 
    ~ return not got_any(item)  && generatedItems ? item 
=== function got(item) 
    ~ return levelItems ? item
=== function got_any(item) 
    ~ return levelItems ^ item

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