
LIST ItemDataTypes = Name, Tooltip, Replacement, Requirement, Generation, PostAction, Sequence

=== function getItemName(item)
    ~ temp specific = "{levelGameplayFunction(Name, item)}"
    { specific != "": 
        ~ return specific 
    } 
    ~ return defaultItemName(item) 
    
    
=== function getItemTooltip(item) 
    ~ temp specific = "{levelGameplayFunction(Tooltip, item)}"
    { specific != "": 
        ~ return specific 
    }
    ~ return defaultItemTooltip(item)
    
    
  
=== function itemRequiresItem(item) 
    ~ temp specific = levelGameplayFunction(Requirement, item) 
    { specific: 
        ~ return specific
    }
    ~ return defaultRequiresItem(item)
    
=== function postItemInteraction(item) 
    // called after item has been interacted with 
    ~ levelGameplayFunction(PostAction, item) 

=== function itemReplacesItemWhenGenerated(items)    
    ~ temp item = pop(items) 
    { item: 
        ~ temp replacement = _itemReplacesItemWhenGenerated(item)    
        ~ return replacement + itemReplacesItemWhenGenerated(items)    
    } 
    ~ return () 
    
=== function _itemReplacesItemWhenGenerated(item)     
    ~ temp specific = levelGameplayFunction(Replacement, item) 
    { specific: 
        ~ return specific
    }
    ~ return defaultItemReplacesItem(item)
    
    
    
=== function itemGeneratesItems(item) 
    ~ temp specific = levelGameplayFunction(Generation, item) 
    { specific: 
        ~ return specific
    }
    ~ return defaultGeneratesItem(item)    