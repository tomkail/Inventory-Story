
LIST Items = (Jacket), WhiteFabricScrap, (Scarf), PianoWire, (FaceDownBody), BloodstainedPlatform,   GamblersAnonymousCard, KingDiamondsCard, SealedMetalCylinder, MapOfParisMetro, (Wallet), PocketKnife, KnottedPianoWire, (PoliceNotes), BusinessCard, QsBusinessCard, TornMapOfParisMetro, Nothing, WeddingRing, PlayingCard, (Gravestone), BunchOfFlowers, AnotherBunchOfFlowers, MoreFlowers, EvenMoreFlowers, (Veil), EyesBrimmingWithTears, Device, Agent, DarkGlasses, Briefcase, KeyOnWristChain, BackroomTable, (KingKey), (MetalLockBox), PileOfChips, EvenMoreChips, EvenEvenMoreChips, ValetReceipt, TableDrawer, Lamp, LitLamp, AceSpades

LIST Accessors = Sequence, Children, Requires, Becomes, SolutionSize, Tooltip, Name, Title, Date, Function, Init

// for items who have properties shared between scenes, we can put them here. 
// we don't have a way to do this right now for children items, I suggest we copy/paste those
// or use VARs

=== function defaultItemName(item)
    {item: 
    -   KeyOnWristChain:    key on wrist chain
    -   Device:     Hopburg-Steiner device
    -   SealedMetalCylinder:  metal cylinder
    -   DarkGlasses:    dark glasses
    -   else:         {item} 
    }

=== function  defaultItemTooltip(item)
    {item: 
    -   SealedMetalCylinder:    \*WARNING\*
    -   PlayingCard:        A King of Diamonds. 
    -   TornMapOfParisMetro: "...mp de Mars. Mi.."
    -   MapOfParisMetro:    "(X) Champ de Mars. Midnight. Q."
    -   WeddingRing:        "Annie and Ernie -- 3 Oct 1962"
    -   BusinessCard:       "Ernst Richards, office clerk, UN."
    -   KingDiamondsCard:   "KING OF DIAMONDS: cards / slots / roulette / girls"
    -   QsBusinessCard:  "Quentin Roch, Private Investigator. Champs de Mars. No matter too small. Divorce a speciality." 
    -   GamblersAnonymousCard:  "Gamblers Anonymous. DON'T GET LUCKY GET HELP."
    -   Device:  "Property of the US Army. Possession is a crime."
    - PileOfChips:      KING OF DIAMONDS nightclub
    - ValetReceipt: Parking receipt for a Blue Chevy, registered to Ernst Richards
    
    }
    ~ return