
VAR levelItems = ()
VAR levelSolutionItems = () 
VAR currentItems = () 
VAR levelInteractables = ()


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


=== scene(title, date, items, interactables, success, -> toNext, VOLine)
    >>> Scene {title} 
    [ {title} / { date } ]
    ~ levelItems = items 
    ~ levelInteractables = interactables
    ~ levelSolutionItems = success
    ~ currentItems = () 
    ~ next = toNext
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
    ~ temp canSlot = LIST_COUNT(levelSolutionItems) - LIST_COUNT(currentItems)
    [ {LIST_COUNT(currentItems)} / {LIST_COUNT(levelSolutionItems)} ]
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
        -> next 
= slot(item, canSlot) 
    +   { currentItems  ? item } 
        [  UNSLOT {getItemName(item)} ]
        ~ currentItems -= item 
    +   { currentItems  !? item } { canSlot }
        [  SLOT {getItemName(item)} - {getItemTooltip(item)}]
        ~ currentItems += item 
        { currentItems == levelSolutionItems:
            -> next
        }
    -   ->->

=== function got(item) 
    ~ return levelItems ? item


