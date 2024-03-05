
=== proceedTo(sceneID)
    ~ previousSceneID += currentSceneID
    ~ currentSceneID = sceneID
    -> tunnelOut(-> next ) 
    
= next 
    ~ levelDataFunction = sceneData(currentSceneID, Function)
    
    >>> SAVE
    
    ~ temp title = "{sceneData(currentSceneID, Title)}"
    ~ temp date = "{sceneData(currentSceneID, Date)}"
    ~ temp solnCount = levelDataFunction(SolutionSize, ()) 
    
    ~ StartScene (currentSceneID, title, date, solnCount )

    // do VO line. Note the speaker is included with the line.     
    {sceneData(currentSceneID, VOIntro)}

    VAR unlocked = ()
    
- (opts) 
    ~ currentlyVisibleItems = getItemsIn(currentContainer) 
    [ Container: {currentContainer} ]
    [ Items: {currentlyVisibleItems} ]
    ~ Unlockables = filterByFunction(currentlyVisibleItems, -> requiresKey)
    ~ Zoomables = filterByFunction(currentlyVisibleItems - Unlockables, -> children)
    ~ Pickupables = currentlyVisibleItems - Zoomables
    [ Zoomables: {Zoomables} ]
    [ Unlockables: {Unlockables} ]
    [ Pickables: {Pickupables} ]
    <- slotunslot
    <- drop 
    <- pickup(Pickupables)
    <- using(Unlockables)
    <- zooming(Zoomables)
    
- (finishup) 
    ~ temp solutionFound = levelDataFunction(Sequence, currentItems)
    +   {currentContainer} [ ZOOM OUT ] 
        ->->
    +  {solutionFound} {not currentContainer} [ SOLVED ] 
        -> proceedTo(solutionFound)   
= slotunslot 
    +   { carrying } {not currentContainer} 
        [ SLOT {carrying} ] 
        ~ currentItems += carrying
        ~ carrying = () 
        -> opts 
        
    +   { currentItems && not carrying && not currentContainer }
        [ UNSLOT {currentItems} ] 
        ~ currentItems = () 
        -> opts 
    
= drop    
    +   {carrying} [ DROP {carrying} ]
        ~ carrying = () 
        -> opts 

= pickup(pickupables)
- (pickupopts) 
    ~ temp picked = pop(pickupables) 
    { picked :
        +   {not carrying} [ PICKUP {picked} {niceName(picked)} ] 
            ~ carrying = picked
            ~ temp VOText = "{getItemTooltip(picked)}"
            {VOText != "":
                VO: {getItemTooltip(picked)}
            }
            [ {picked} now held ]
            -> opts 
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
            -> opts 
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
            -> opts -> // boom (!)
            ~ currentContainer = oldparent 
            -> opts 
    } 
    { zoomables: 
        -> zoomopts 
    } 
    -> DONE 
    
  
        

   
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
    