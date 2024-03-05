

=== function MortuaryScene_data(key, item) 
    { key: 
    -   Title:    Mortuary, 4th Quartier 
    -   Date:     24th April 1968, 2:38pm
    -   SolutionSize: ~ return 2 
    -   Sequence: {item: 
        - (TornMapOfParisMetro, QsBusinessCard):         
            ~ return  MetroPlatformScene
            
        - (TornMapOfParisMetro, QsBusinessCard):     
            ~ return BackRoomClubScene
        
        - (GamblersAnonymousCard, KingDiamondsCard):
            ~ return AtTheCardTableScene
    }
        
    - Children: {item: 
    - ():   ~ return (PoliceNotes, Wallet, WeddingRing, TornMapOfParisMetro)
    -   Wallet: ~ return  (BusinessCard, QsBusinessCard, GamblersAnonymousCard, KingDiamondsCard, PlayingCard, SealedMetalCylinder )
    }
    - Tooltip: {item: 
        -   ():     Something's out of place.
        -   SealedMetalCylinder: Empty.
        -   PoliceNotes: 
                Attempted theft. Killer was disturbed and escaped. Narcotics in victim's blood.
        }
    
    }
    ~ return () 


=== function MetroPlatformScene_data(key, item) 
    { key: 
    - Title:    Metro Platform, Champ de Mars
    - Date:     23rd April 1968, 11:25pm
    -   Sequence: {item: 
    TODO: exits 
    // -   (BloodstainedPlatform, PianoWire) : ~ return ()
    }
    -   SolutionSize: ~ return 2 
    -   Children:  {item: 
    -   ():     ~ return (FaceDownBody, BloodstainedPlatform)
    -   Wallet: ~ return  (BusinessCard, QsBusinessCard, GamblersAnonymousCard, KingDiamondsCard, SealedMetalCylinder )
    -   Scarf:  ~ return KnottedPianoWire 
    -   FaceDownBody: ~ return (Jacket, Scarf, WhiteFabricScrap) 
    -   Jacket: ~ return (Wallet , MapOfParisMetro, PocketKnife)
    } 
    -   Requires: {item: 
    -   KnottedPianoWire:   ~ return PocketKnife 
    -   Wallet:             ~ return PianoWire
    }
    -   Becomes: {item: 
    -   KnottedPianoWire:   ~ return PianoWire
    }
    -   Tooltip: {item:
    -   (): 
            Something belongs... elsewhere. In other hands. 
    -   Scarf: 
            The cloth is matted with blood.
    -   KnottedPianoWire: 
            The wire is nasty, tight and twisted together.
    -   PocketKnife:
            An old, rusty knife; something kept from boyhood, maybe.    
    }
    }
    ~ return () 