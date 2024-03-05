
// All the variabels required by the game side should be here 

VAR currentContainer = () 
    
VAR currentSceneID = () 
VAR currentlyVisibleItems = ()
VAR currentItems = () // slotted items
VAR carrying = () // set/unset by the game, so tooltips can react

VAR Zoomables = ()
VAR Unlockables = ()
VAR Pickupables = ()

// All the functions required by the game side should be here 

   

=== function checkForSolution() 
    ~ temp result = levelDataFunction(Sequence, currentItems)
    
TODO: replayable scenes, etc
/*         
    { previousSceneID ? result && ReplayableScenes !? result:
        // can't repeat a scene (!) 
        ~ return false 
    }
 */       
    { DEBUG:
        // assert result is a list and we've not return a read count by accident
        ~ temp i = LIST_MIN(result)
    }
    ~ return BOOL(result)


=== function getItemName(item)
    ~ temp specific = "{levelDataFunction(Name, item)}"
    { specific != "": 
        ~ return specific 
    } 
    ~ return defaultItemName(item)     
 
 

EXTERNAL StartScene  (sceneID, titleText, dateText, slotCount)     
=== function StartScene  (sceneID, titleText, dateText, slotCount) 
    [ {sceneID}: {titleText} / {dateText} ] 
    

=== function getItemTooltip(item) 
    ~ temp specific = "{levelDataFunction(Tooltip, item)}"
    { specific != "": 
        ~ return specific 
    }
    ~ return defaultItemTooltip(item)
    
    
=== function getItemsIn(item) 
   ~ return TransformByFunction(levelDataFunction(Children, currentContainer) - currentItems, -> _findReplacement)  - currentItems
 
    
 === function _findReplacement(item) 
    { unlocked ? item: 
        ~ temp updated = levelDataFunction(Becomes, item)
        { updated: 
            ~ item = updated 
        }
    }
    ~ return item 