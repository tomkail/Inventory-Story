
=== proceedTo(sceneID)
    ~ previousSceneID += currentSceneID
    ~ currentSceneID = sceneID
    -> tunnelOut(-> next ) 
    
= next 
    ~ levelDataFunction = sceneData(currentSceneID)
    
    >>> SAVE
    
    ~ temp title = "{levelDataFunction(Title, ())}"
    ~ temp date = "{levelDataFunction(Date, ())}"
    ~ temp solnCount = levelDataFunction(SolutionSize, ()) 
    
    ~ StartScene (currentSceneID, title, date, solnCount )
    ~ currentItems = () 

    // do VO line. Note the speaker is included with the line.     
    
    
    VAR unlocked = ()
    
- (enter) 
    ~ currentlyVisibleItems = getItemsIn(currentContainer) 
    { not carrying: 
        {getItemTooltip(currentContainer)}
    }
- (act)
    [ Container: {currentContainer} ]
    [ Items: {currentlyVisibleItems} ]
    ~ Unlockables = filterByFunction(currentlyVisibleItems, -> requiresKey)
    ~ Zoomables = filterByFunction(currentlyVisibleItems - Unlockables, -> children)
    ~ Pickupables = currentlyVisibleItems - Zoomables
    { not currentContainer: 
        ~ Pickupables += currentItems
    }
    [ Zoomables: {Zoomables} ]
    [ Unlockables: {Unlockables} ]
    [ Pickables: {Pickupables} ]
    <- slot
    <- drop 
    <- pickup(Pickupables)
    <- using(Unlockables)
    <- zooming(Zoomables)
    -> finishup

= slot
    +   { carrying } {not currentContainer} 
        [ SLOT {carrying} ] 
        ~ currentItems += carrying
        ~ carrying = () 
        -> act 
    
= drop    
    +   {carrying} [ DROP {carrying} ]
        ~ carrying = () 
        -> act 

= pickup(pickupables)
- (pickupopts) 
    ~ temp picked = pop(pickupables) 
    { picked :
        +   {not carrying} [ PICKUP {picked} {niceName(picked)} ] 
            ~ carrying = picked
            ~ currentItems -= picked 
            {getItemTooltip(picked)}
            [ {picked} now held ]
            -> act 
    }   
    { pickupables: -> pickupopts } 
    -> DONE 
    
= using(unlockables)
- (useopts)
    ~ temp user = pop(unlockables) 
    // assumes only require per item. could build a second loop here if reqd.
    ~ temp withItem = requiresKey(user)
    { user && not ( DEBUG && carrying != withItem):
        +   [ USE {user} - {withItem} ] 
            ~ unlocked += user
            ~ carrying = ()
            [ {user} Unlocked ]
            -> act 
    }
    { unlockables: -> useopts } 
    -> DONE 
    
= zooming(zoomables)
- (zoomopts) 
    ~ temp zoomer = pop(zoomables) 
    { zoomer: 
        +   [ ZOOM INTO {zoomer} {niceName(zoomer)} ]
            ~ temp oldparent = currentContainer
            ~ currentContainer = zoomer
            -> enter -> // boom (!)
            ~ currentContainer = oldparent 
            -> enter 
    } 
    { zoomables: 
        -> zoomopts 
    } 
    -> DONE 

= finishup
    ~ temp solutionFound = levelDataFunction(Sequence, currentItems)
    +   {currentContainer} [ ZOOM OUT ] 
        ->->
    +  {solutionFound} {not currentContainer} [ SOLVED ] 
        -> proceedTo(solutionFound)      
  
        

   
=== function niceName(item) 
    {DEBUG:
        ({getItemName(item)})
    }       

=== function requiresKey(item)
    { unlocked !? item: 
        ~ return levelDataFunction(Requires, item) 
    } 
    ~ return () 


=== function children(item) 
   ~ return levelDataFunction(Children, item)
    