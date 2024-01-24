
VAR levelItems = ()
VAR levelSolutionItemCount = 0
VAR currentItems = () 
VAR levelInteractables = ()
VAR levelCheckerFunction = -> FALSE_
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


=== scene(title, date, items, interactables, basicSuccessCase, -> altSuccessFn, -> toNext, VOLine)
    >>> Scene ({title})
    [ {title} / { date } ]
    ~ levelItems = items 
    ~ levelInteractables = interactables
    ~ levelCheckerFunction = altSuccessFn
    ~ levelSolutionBasic = basicSuccessCase
    ~ levelSolutionItemCount = LIST_COUNT(basicSuccessCase) // returns an int
    ~ currentItems = () 
    ~ nextScene = toNext
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
    +   [ SOLVED ] 
        >>> SAVE
        -> nextScene
= slot(item, canSlot) 
    +   { currentItems  ? item } 
        [  UNSLOT {getItemName(item)} ]
        ~ currentItems -= item 
    +   { currentItems  !? item } { canSlot }
        [  SLOT {getItemName(item)} - {getItemTooltip(item)}]
        ~ currentItems += item 
        { checkForSolution():
            >>> SAVE
            -> nextScene
        }
    -   ->->
    
=== function checkForSolution() 
    {
    - currentItems == levelSolutionBasic: 
        ~ return true 
    - else: 
        ~ return levelCheckerFunction(currentItems) 
    }
    
    

=== function got(item) 
    ~ return levelItems ? item


