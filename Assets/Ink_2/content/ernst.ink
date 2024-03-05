

=== function MortuaryScene_data(key, item) 
    { key: 
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
        -   SealedMetalCylinder: Empty.
        -   PoliceNotes: 
                Attempted theft. Killer was disturbed and escaped. Narcotics in victim's blood.
        }
    
    }
    ~ return () 


=== function MetroPlatformScene_data(key, item) 
    { key: 
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
    -   KnottedPianoWire: 
            The wire is nasty, tight and twisted together.
    -   PocketKnife:
            An old, rusty knife; something kept from boyhood, maybe.    
    }
    }
    ~ return () 