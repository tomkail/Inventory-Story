
=== function getItemName(item)
    {item: 
    -   ManilaEnvelope:             manila envelope
    -   BunchOfFlowers:             black lilies 
    -   AnotherBunchOfFlowers:      white lilies
    -   MoreFlowers:                yellow lilies
    -   BlackLeatherFolder:   black leather folder
    -   WhiteApron: {not hotel_bathroom:stained} white apron
    -   ERNameBadge:    security badge
//    -   KoDStamp:   stamp
    -   else:         {item} 
    }

=== function getItemTooltip(item) 
    {item: 
    -   WaiterHandsUp:  "Whatever you say, Monsieur!" 
    -   Waiter:     "What are you looking at, huh?"
    -   UnconciousWaiter:   "Uhhhh...."
    -   DoorLock:   "KITCHENS"
    -   MapOfParisMetro:    "(X) Champ de Mars"
    -   ERNameBadge:    "Ernst Richards. 67834-A-X-2"
    -   DeskPlate:  "Ernst Richards, Clerk"
    -   WeddingPhoto:   "Annabel & Ernst 3/10/62"
    -   GlassVialOfPowder:  "COCAINE"
    -   PlayingCard:        King of Diamonds. 
    -   WhiteFabricScrap:   Torn, and slightly stained.
    -   PoliceNotes:             "Attempted theft. Killer was disturbed and escaped. Narcotics in victim's blood."   
    -   BusinessCard:       "Ernst Richards, office clerk, UN."
    -   KingDiamondsCard:   "KING OF DIAMONDS: cards / slots / roulette / girls"
    -   OtherBusinessCard:  "Bolera Taxis." 
    -   OtherOtherBusinessCard:  "Gamblers Anonymous. DON'T GET LUCKY GET HELP."
    -   WeddingRing:        "Annie and Ernie -- 3 Oct 1962"
    -   ManilaEnvelope:     "ER surveillance"  
    -   ManEnteringCarOutsideUNPhoto:  "ER, 23rd April 68"
    -   ManInAirportPhoto:  "Unknown subject. Puerto Rico, 26th April 68"
    -   MetalCylinderPhoto: "The Hopburg-Steiner Device"
    -   PianoWire:      {in_the_kitchens:
                            It's coiled and clean.
                        - else:
                            It's blood-soaked.
                        }
    - BlackLeatherFolder:   "KoD"
    - CoffeeOrderSlip:  "Coffee, table 15."
    - CoffeeSpoon:      A few white grains are still visible on the spoon.
    - FoodPeelings:     Onion skins, coffee grounds, potato peel.
    - EmptyGlassVial:   Containing a few grains of a white powder.
    - WaiterNameBadge:  "CARL. Ask me for service!"
    - DupontMetroPass:  Metro Pass: T. DUPONT
    - PhotoOfErnst:     "Ernst Richards. 33.y.o."
    
    - CasinoChips:      KING OF DIAMONDS nightclub
    - AceSpadesReversed:       
            { kingdiamondsclub:
                The back of this card is slightly different from the others.
            }
    - ValetReceipt:         Parking receipt for a Blue Chevy, registered to Ernst Richards
    - Timeline: Timeline of the Hopburg-Steiner Device
    - Inception: "Device created. March 1961. Dakota."
    - DeviceStolenFromResearchLab: "Device stolen. April 1962."
    - ErnstRichardsDies:  "Paris. May 1968. Device found by chance on unknown dead man."
    - NoteFromQuentin:  "Ernie - Hold onto this for me. Keep it safe. Q."
    - DeadDropNoteFromQuentin: "Handover at the Champs du Mars. Midnight tonight. Q."
  //  - DupontInstructions:   "Further instructions: outside kitchens, Hotel de Champs de Mars."
  //  - KoDStamp:     "KoD"
    - LoyalAssurance:   "I'll get over to the UN right away, sir."
    }
    ~ return
    
=== function itemRequiresItem(item) 
    { item: 
    -   LockedDrawer:   ~ return KeyOnChain
    -   QuentinsAide: ~ return (ERNameBadge, TwoThousandFrancs, DeadDropNoteFromQuentin)
    -   MetalLockBox:   ~ return KingKey
    -   Waiter:         ~ return (SmallGun, FlickKnife)
    -   WallSafe:       ~ return WeddingPhoto
    -   DoorLock:   ~ return FlickKnife 
    }
    ~ return () 
    
=== function itemGeneratesItems(item, ref asReplacement) 
    {item: 
    - DoorLock: ~ return replaceAs(asReplacement, BrokenDoorLock)
    - LockedDrawer: ~ return (TwoThousandFrancs, MapOfParisMetro)
    - RumpledShirt: ~ return KeyOnChain
    - BlueChevy: ~ return TinCanString
    - BlackCar: ~ return QuentinsAide    
    - QuentinsAide: 
        { quentin_gives_aide_money:
            ~ return ( LoyalAssurance ) 
        - else: 
            ~ return (  DeadDropNoteFromQuentin, TwoThousandFrancs )  
        }
    - ManilaEnvelope: 
        ~ asReplacement = true
        ~ return (ManEnteringCarOutsideUNPhoto, ManInAirportPhoto,  MetalCylinderPhoto)
    - BunchOfFlowers:   ~ return EvenMoreFlowers
    - EvenMoreFlowers:  ~ return WeddingRing 
    - AnotherBunchOfFlowers:    ~ return MoreFlowers
    - Wallet: 
        { not metro_platform: 
            ~ return (BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, KingDiamondsCard, SealedMetalCylinder, MetroTicket )
        - else: 
            ~ return (BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, KingDiamondsCard, SealedMetalCylinder)
        }
    // - DupontInstructions:   ~ return KoDStamp
    - Scarf: ~ return PianoWire
    - Bin: ~ return (FoodPeelings, EmptyGlassVial)
    - WhiteApron: ~ return (CoffeeOrderSlip, PianoWire)
    - UnconciousWaiter: ~ return (WhiteApron, WaiterNameBadge)
    - BlackKitBag: ~ return (PianoWire, FlickKnife, SmallGun, Cigarettes, DupontMetroPass, BlackLeatherFolder)
    
    - CoffeeOnTray:     ~ return CoffeeSpoon
    - BlackLeatherFolder: 
        ~ return (GlassVialOfPowder, PhotoOfErnst, CasinoChips)
    - Waiter: 
        ~ return replaceAs(asReplacement, WaiterHandsUp)
    - HandCards: ~ return (AceHearts, ThreeClubs, SevenHearts, AceSpades, PlayingCard)
    - AceHeartsReversed:   
        ~ return replaceAs(asReplacement, AceHearts)
    - ThreeClubsReversed:   
        ~ return replaceAs(asReplacement, ThreeClubs)
    - SevenHeartsReversed:  
        ~ return replaceAs(asReplacement,  SevenHearts )
    - AceSpadesReversed:    
        ~ return replaceAs(asReplacement,  AceSpades )
    - AceHearts:            
        ~ return replaceAs(asReplacement,  AceHeartsReversed )
    - ThreeClubs:           
        ~ return replaceAs(asReplacement,  ThreeClubsReversed )
    - SevenHearts:          
        ~ return replaceAs(asReplacement,  SevenHeartsReversed )
    - AceSpades:            
        ~ return replaceAs(asReplacement,  AceSpadesReversed )
    - PlayingCard:          
        ~ return replaceAs(asReplacement,  PlayingCardReversed )
    - PlayingCardReversed:  
        ~ return replaceAs(asReplacement,  PlayingCard )
    - MetalLockBox:         ~ return PileOfChips
    - PileOfChips:          
        ~ return replaceAs(asReplacement, EvenMoreChips  )
    - EvenMoreChips:          
        ~ return replaceAs(asReplacement,  (EvenEvenMoreChips , ValetReceipt) )
    - ValetReceipt: 
        ~ return replaceAs(asReplacement, EvenEvenMoreChips)
    - Jacket:               
        ~ return (Wallet)
    - Timeline: 
        ~ return replaceAs(asReplacement,  (Inception, DeviceStolenFromResearchLab, ErnstRichardsDies ) )
    - WallSafe: 
        ~ return (SealedMetalCylinder , AceSpades)
    // - Stranger:    ~ return (DupontMetroPass)
    - KeyHook: 
        ~ return CarKey
    
    - UNBin: 
        ~ return NoteFromQuentin
    - DeskPlate: 
        ~ return Envelope 
    - Envelope: 
        ~ return SealedMetalCylinder
    - else: ERROR: {item} has no generator list 
    
    }
    
=== function replaceAs(ref asReplacement, item) 
    ~ asReplacement = true
    ~ return item 