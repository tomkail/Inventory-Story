LIST TurnedOnClubLight = CLUB_LIGHT_ON = 1000, FLIPPED_ACE, AceSpadesReversed

=== function BackRoomClubScene_data(key, item) 
    { key: 
    -   Init:   
           
    -   Title: Backroom, King of Diamonds Club
    -   Date:   23rd April 1968, 9:18pm
    -   SolutionSize:   ~ return 2
    -   Sequence: {item: 
        - (ValetReceipt, AceSpades): ~ return AtTheCardTableScene
        - else:         ~ return () 
        
        }
    -   Tooltip: {item: 
        - ():               
            Someone's trying to bury something here... #gangster 
        - MetalLockBox:     
            Something's hidden away. #gangster
        - Lamp:     
            { reach(CLUB_LIGHT_ON):
                \*Click\*
            }
        - AceSpades: 
            The Ace of Spades 
            
       
        - AceSpadesReversed: 
            An upside-down card 
             
        - else: {defaultItemTooltip(item)}    
        } 
    - Requires: {item: 
        -   MetalLockBox:   ~ return KingKey
        // THERE! can't put X on Y when Y's already a zoomybox
        -   BackroomTable:  ~ return AceSpades
        }
    - Becomes:  {item:
        -   AceSpades: 
                { reached(FLIPPED_ACE):
                    ~ return AceSpadesReversed 
                }
        -   Lamp:   {reached(CLUB_LIGHT_ON): 
                ~ return LitLamp 
            } 
        }
    - Children: {item: 
        - ():   
            ~ return BackroomTable
        - BackroomTable: 
            { reached(CLUB_LIGHT_ON):
                ~ return (TableDrawer, MetalLockBox, Lamp)
                
            - else: 
                ~ return ( MetalLockBox, Lamp)
            }
        - TableDrawer:  
            ~ return KingKey 
        - MetalLockBox:         
            ~ return PileOfChips
        - PileOfChips:          
            ~ return (EvenMoreChips, ValetReceipt)
        - EvenMoreChips:          
            ~ return (EvenEvenMoreChips, AceSpades)
        }
    }
    ~ return () 