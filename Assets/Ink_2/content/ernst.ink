
=== function MetroPlatformScene_data(key, item) 
    { key: 
    -   Sequence: {item: 
    -   (BloodstainedPlatform, PianoWire) : ~ return PlatformMurderScene
    }
    -   SolutionSize: ~ return 2 
    -   Children:  {item: 
    -   ():     ~ return (FaceDownBody, BloodstainedPlatform)
    -   Wallet: ~ return  (BusinessCard, QsBusinessCard, OtherOtherBusinessCard, KingDiamondsCard, SealedMetalCylinder )
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
    }
    ~ return () 