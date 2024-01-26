
VAR levelItems = ()
VAR levelSolutionItemCount = 0
VAR currentItems = () 
VAR levelInteractables = ()
VAR levelSuccessFunction = -> FALSE_
VAR levelSolutionBasic = ()

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
            { asReplacement:
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
    >>> Scene (title={title}) (date={date})
    [ {title} / { date } ]
    ~ levelItems = items 
    ~ levelInteractables = interactables
    ~ levelSuccessFunction = getSceneData(currentSceneID, ExitKnot)
    ~ levelSolutionItemCount = levelSuccessFunction(()) // returns an int
    ~ currentItems = () 
    VO: {VOLine}
    -> play 
    
=== play
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
    ~ temp canSlot = levelSolutionItemCount - LIST_COUNT(currentItems)
    [ {LIST_COUNT(currentItems)} / {levelSolutionItemCount} ]
- (opts)    
    ~ temp item = pop(items) 
    { item: 
        <- slot(item, canSlot) 
    }
    {items: 
        -> opts
    }
    -> DONE
= ingame 
    +   (solved) [ SOLVED ] 
        >>> SAVE
        -> proceedTo(levelSuccessFunction(currentItems))
    
= slot(item, canSlot) 
    +   { currentItems  ? item } 
        [  UNSLOT {getItemName(item)} ]
        ~ currentItems -= item 
    +   { currentItems  !? item } { canSlot }
        [  SLOT {getItemName(item)} - {getItemTooltip(item)}]
        ~ currentItems += item 
        { checkForSolution():
            ->-> solved
        }
    -   ->->
    
=== function checkForSolution() 
    // don't bother unless the count is right, covers the 0-slotted case
    { LIST_COUNT(currentItems) == levelSolutionItemCount: 
        ~ return levelSuccessFunction(currentItems) 
    - else: 
        ~ return false 
    }
    
    
    

=== function got(item) 
    ~ return levelItems ? item

=== proceedTo(nextSceneIDToHit)
    ~ previousSceneID = currentSceneID
    ~ currentSceneID = nextSceneIDToHit
    ~ temp nextSceneToHit = getSceneData(currentSceneID, Knot)
    -> nextSceneToHit
