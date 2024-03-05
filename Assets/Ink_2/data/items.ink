
LIST Items = (Jacket), WhiteFabricScrap, (Scarf), PianoWire, (FaceDownBody), BloodstainedPlatform,   GamblersAnonymousCard, KingDiamondsCard, SealedMetalCylinder, MapOfParisMetro, (Wallet), PocketKnife, KnottedPianoWire, (PoliceNotes), BusinessCard, QsBusinessCard, TornMapOfParisMetro, Nothing, WeddingRing, PlayingCard

LIST Accessors = Sequence, Children, Requires, Becomes, SolutionSize, Tooltip, Name

// for items who have properties shared between scenes, we can put them here. 
// we don't have a way to do this right now for children items, I suggest we copy/paste those
// or use VARs

=== function defaultItemName(item)
    {item: 
   
    -   SealedMetalCylinder:  metal cylinder
    -   else:         {item} 
    }

=== function  defaultItemTooltip(item)
    {item: 
    -   PlayingCard:        A King of Diamonds. 
    -   TornMapOfParisMetro: "...mp de Mars. Mi.."
    -   MapOfParisMetro:    "(X) Champ de Mars. Midnight. Q."
    -   WeddingRing:        "Annie and Ernie -- 3 Oct 1962"
    -   BusinessCard:       "Ernst Richards, office clerk, UN."
    -   KingDiamondsCard:   "KING OF DIAMONDS: cards / slots / roulette / girls"
    -   QsBusinessCard:  "Quentin Roch, Private Investigator. Champs de Mars. No matter too small. Divorce a speciality." 
    -   GamblersAnonymousCard:  "Gamblers Anonymous. DON'T GET LUCKY GET HELP."
  

    
    
    }
    ~ return