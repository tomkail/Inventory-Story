
=== function getItemName(item)
    {item: 
    -   ManilaEnvelope:             manila envelope
    -   BunchOfFlowers:             black lilies 
    -   AnotherBunchOfFlowers:      white lilies
    -   MoreFlowers:                yellow lilies
    -   BlackLeatherFolder:   black leather folder
    -   WhiteApron: {( HotelKitchens, HotelBathroom) ? currentSceneID  :stained} white apron
    -   ERNameBadge:    security badge
//    -   KoDStamp:   stamp
    -   else:         {item} 
    }

=== function getItemTooltip(item) 
    {item: 
    -   DeviceOperatedPhoto:    "Operation of device observed from earthquake monitoring station, Jan 61"
    -   QuentinsAide: 
        { currentSceneID:
        -   NoteInCar:  "Ernst Richards?"
        -   QGivesNoteToAide: "Whatever it is, you can trust me, sir."
        -   QGivesItemToErnst: "Ernst Richards?"
        }
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
    -   ManilaEnvelope:     "Hopburg-Steiner Device Timeline"  
    -   ManEnteringCarOutsideUNPhoto:  "ER, 23rd April 68"
  
    -   MetalCylinderPhoto: "Device created. March 1958. Nevada."
    -   PianoWire:      {currentSceneID == MetroPlatform:
                            It's blood-soaked.
                        - else:
                            It's coiled and clean.
                            
                        }
    - Nothing:          The cylinder is empty. 
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
            { currentSceneID == CardTableAtClub:
                The back of this card is slightly different from the others.
            }
    - ValetReceipt:         Parking receipt for a Blue Chevy, registered to Ernst Richards
    
    
    - DeviceStolenFromResearchLab: "Device stolen. April 1962."
    - ErnstRichardsDies:  "Paris. May 1968. Device shell found on unknown dead man."
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
    -   QuentinsAide: ~ return (ERNameBadge,  DeadDropNoteFromQuentin, DeskPlate)
    -   MetalLockBox:   ~ return KingKey
    -   Waiter:         ~ return (SmallGun, FlickKnife)
    -   WallSafe:       ~ return WeddingPhoto
    -   DoorLock:   ~ return FlickKnife 
    -   LooseBrick: ~ return  Screwdriver
    }
    ~ return () 
    
=== function itemGeneratesItems(item, ref asReplacement) 
    {item: 
    
    - Device:   ~ return Warp
    - ElectricLamp: ~ return SealedMetalCylinder
     - Wall: ~ return LooseBrick
     - LooseBrick: ~ return SmallPackage
     - Toolbox: ~ return (Screwdriver, Pliers, Wrench)
    - SealedMetalCylinder: 
        { currentSceneID < DeviceRemovedFromCylinder: 
            ~ return Nothing 
        - else: 
            ~ return Device
        }
    - DoorLock: ~ return replaceAs(asReplacement, BrokenDoorLock)
    - LockedDrawer: ~ return (TwoThousandFrancs, MapOfParisMetro)
    - RumpledShirt: ~ return KeyOnChain
    - BlueChevy: ~ return TinCanString
    - BlackCar: ~ return QuentinsAide    
    - QuentinsAide: 
        ~ asReplacement = true 
        { currentSceneID:
        - QGivesNoteToAide:
            ~ return ( LoyalAssurance ) 
        - NoteInCar: 
            ~ return (  DeadDropNoteFromQuentin, TwoThousandFrancs )  
        - QGivesItemToErnst: 
            ~ return Envelope 
        }
    - ManilaEnvelope: 
        ~ asReplacement = true
        ~ return PhotosInOpeningEnvelope
    - BunchOfFlowers:   ~ return EvenMoreFlowers
    - EvenMoreFlowers:  ~ return WeddingRing 
    - AnotherBunchOfFlowers:    ~ return MoreFlowers
    - Wallet: 
        { currentSceneID == MetroPlatform: 
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

    - WallSafe: 
        ~ return (SealedMetalCylinder , AceSpades)
    // - Stranger:    ~ return (DupontMetroPass)
    - KeyHook: 
        ~ return CarKey
    
    - Envelope: 
        ~ return ( NoteFromQuentin, SealedMetalCylinder)
    }
    ERROR: {item} has no generator list 
    
    
=== function replaceAs(ref asReplacement, item) 
    ~ asReplacement = true
    ~ return item 