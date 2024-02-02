 /*
    ErnDiesEarly
 */
 
 
=== ErnDiesEarly 
    
    LIST EarlyGraveitems = NewInstructionsFromKosakov, (ShadowyFigure) 
    -> scene (GraveyardItems + EarlyGraveitems, GraveyardInteractables + ShadowyFigure, "I would have been forced to betray him eventually. It's better this way.") 
=== function ErnDiesEarly_fn(x) 
    { x: 
    -   ():             ~ return 1 
    -   WeddingRing:    ~ return Wedding
    - NewInstructionsFromKosakov:   ~ return QuentinGetsDeviceAnnieWatching 
TODO: A solve 
    }
    ~ return () 
    
 
 
 /*
    QuentinGetsDeviceAnnieWatching
 */
 
 
=== QuentinGetsDeviceAnnieWatching 
    LIST QuentinGetsDeviceAnnieWatchingItems = (QuentinGetsDeviceAnnieWatchingItem) 
    VAR QuentinGetsDeviceAnnieWatchingInteractables = (QuentinGetsDeviceAnnieWatchingItem)
    
    -> scene ( QuentinReceivesCylinderItems + QuentinGetsDeviceAnnieWatchingItems, QuentinReceivesCylinderInteracts + QuentinGetsDeviceAnnieWatchingInteractables, "Remark") 
=== function QuentinGetsDeviceAnnieWatching_fn(x) 
    { x: 
    -   (): ~ return 1 
TODO: A solve 
    }
    ~ return () 




     