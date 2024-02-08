 /*
    ErnDiesEarly
 */
 
 
=== ErnDiesEarly 
    
    LIST EarlyGraveitems = NewInstructionsFromKosakov, (ShadowyFigure) 
    -> scene (GraveyardItems + EarlyGraveitems, GraveyardInteractables + ShadowyFigure, "I would have been forced to betray him eventually. It's better this way.") 
=== function ErnDiesEarly_fn(act, item) 
    {act: 
    - Sequence: {item:
        -   ():             ~ return 1 
        -   WeddingRing:    ~ return Wedding
        - NewInstructionsFromKosakov:   ~ return QuentinGetsDeviceAnnieWatching 
        }
    - Tooltip: {item:
        
        - NewInstructionsFromKosakov: 
            "You'll need to monitor Roch without him seeing you."
        - ShadowyFigure: 
            { got(NewInstructionsFromKosakov): 
                "My advice, Ana, is to throw yourself back into your work."
            - else: 
                "That phase of your life is over. Time to leave it behind."
            }
        }
    - Requirement: {item: 
         - ShadowyFigure : ~ return WeddingRing 
         }
    - Generation: {item: 
        - ShadowyFigure : ~ return (NewInstructionsFromKosakov)
        }
    }
    ~ return () 
    
 
 
 /*
    QuentinGetsDeviceAnnieWatching
 */
 
 
=== QuentinGetsDeviceAnnieWatching 
    LIST QuentinGetsDeviceAnnieWatchingItems = (QuentinGetsDeviceAnnieWatchingItem) 
    VAR QuentinGetsDeviceAnnieWatchingInteractables = (QuentinGetsDeviceAnnieWatchingItem)
    
    -> scene ( QuentinReceivesCylinderItems + QuentinGetsDeviceAnnieWatchingItems, QuentinReceivesCylinderInteracts + QuentinGetsDeviceAnnieWatchingInteractables, "Remark") 
=== function QuentinGetsDeviceAnnieWatching_fn(act, item)
    {act: 
    - Sequence: {item: 
            -   (): ~ return 1 
        TODO: A solve 
        }
    }
    ~ return () 




     