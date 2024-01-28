
=== function getItemName(item)
    {item: 
    -   ManilaEnvelope:             manila envelope
    -   BunchOfFlowers:             black lilies 
    -   AnotherBunchOfFlowers:      white lilies
    -   MoreFlowers:                yellow lilies
    -   EvenMoreFlowers:            white roses
    -   BlackLeatherFolder:   black leather folder
    -   WhiteApron: {after(BackAlleyway):stained} white apron
    -   MetroTicket:        { isOrAfter( MetroPlatform ): bloodstained} metro ticket
    -   ERNameBadge:    security badge
    
//    -   KoDStamp:   stamp
    -   else:         {item} 
    }

=== function getItemTooltip(item) 
    {item: 
    -   Kosakov:    "The device, please, Ana."
    -   Matthews:   "Don't even think about giving it to him."
    -   Device:  "Property of the US Army"
    -   Quentin:    
            {
            - currentItems ? Wife : 
                "You're a lucky man, Ern." 
            - currentItems !? OtherWeddingRing : 
                "Ready?"
            - else:
                "Go on, then!"
            }
    -   Annie:      "I do." 
    -   Wife:       "Kiss me, Ernie..."
    -   Croupier:   
        { currentItems !? PileOfChips: 
            "If you're out of chips, sir, you cannot bet."
        - else: 
            "Sir?"
        }
    -   MetroTicket:    "CHAMP DE MARS to MONTPELLIER" 
    -   Gravestone:     "Ernst Richards. Died: 23rd April 1968"
    -   DeviceOperatedPhoto:    "Operation of device observed from California earthquake monitoring station, Oct '62"
    -   CylinderInMortuaryPhoto: "Found amongst possessions of ER, murder victim."
    -   DeviceRemovedFromCylinderPhoto: "Aug 15th, 1964. Device is removed from protective sheath."
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
    -   QsBusinessCard:  "Quentin Perdi, Private Investigator. Champs de Mars. No matter too small. Divorce a speciality." 
    -   OtherOtherBusinessCard:  "Gamblers Anonymous. DON'T GET LUCKY GET HELP."
    -   WeddingRing:        "Annie and Ernie -- 3 Oct 1962"
    -   ManilaEnvelope:     "Known Timeline of the Hopburg-Steiner Device"  
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
    - ErnstRichardsDies:  "Paris. May 1968. Device shell found on theft victim's corpse."
    - NoteFromQuentin:  "Ernie - Hold onto this for me. Keep it safe. Q."
    - DeadDropNoteFromQuentin: "Handover at the Champs du Mars. Midnight tonight. Q."
  //  - DupontInstructions:   "Further instructions: outside kitchens, Hotel de Champs de Mars."
  //  - KoDStamp:     "KoD"
    - LoyalAssurance:   "I'll get over to the UN right away, sir."
    - KosakovCard: "Paris 15643. Ask for K."
 
     
     - KosakovOnTelephone: "Yes? Do you have something for me?"
     
     - KosakovsDrop:    "The Montmatre Tunnel. Two days time. 11:30pm"
     
    -   KosakovsThanks:  
            { is(GoThroughWithWedding): 
                "This is excellent cover. You have done very well, Ana. Do not worry. Your reward will come."
                
            - else: 
                "Well done, comrade. You have our thanks. We will extract you when it is safe."
            }
    -   MatthewsRelief:     "You've made the right decision. I'm glad you've seen what's right here."
    - Kosakov: "You are having second thoughts? You wish to be extracted?"
     
    }
    ~ return
    
=== function itemRequiresItem(item) 
    { item: 
    - Kosakov:
        ~ return (Device, WeddingRing)
    - Matthews: ~ return Device
    - Annie: ~ return OtherWeddingRing
 
    -   LockedDrawer:   ~ return KeyOnChain
    -   QuentinsAide: ~ return (ERNameBadge,  DeadDropNoteFromQuentin, DeskPlate)
    -   MetalLockBox:   ~ return KingKey
    -   Waiter:         ~ return (SmallGun, FlickKnife)
    -   WallSafe:       ~ return WeddingPhoto
    -   DoorLock:   ~ return FlickKnife 
    -   LooseBrick: ~ return  Screwdriver
    -   Croupier: ~ return ValetReceipt
    -   ManNearBlackCar: ~ return Camera
    -   Telephone: ~ return KosakovCard
    -   KosakovOnTelephone:  ~ return Device 
    
    }
    ~ return () 
    
    
    
=== function itemGeneratesItems(item, ref asReplacement) 
    {item: 
    - ParkBench: ~ return Kosakov
    - Kosakov: 
        ~ replaceAs(asReplacement, KosakovsThanks)
    - Matthews: ~ replaceAs(asReplacement, MatthewsRelief)
     - BlackChanelBag: ~ return (SealedMetalCylinder, WeddingPhotograph, KosakovCard)
     
     - Telephone: ~ replaceAs(asReplacement, KosakovOnTelephone)
     - KosakovOnTelephone:  ~ replaceAs(asReplacement, KosakovsDrop)
     
    - ManNearBlackCar: ~ return  ManEnteringCarOutsideUNPhoto
    - Wife:     ~ return Warp
    -  Quentin: ~ return OtherWeddingRing
    - Annie: ~ return replaceAs(asReplacement, Wife)
 
    - Croupier: ~ return PileOfChips
    - Device:   ~ return Warp
    - ElectricLamp: ~ return SealedMetalCylinder
     - Wall: ~ return LooseBrick
     - LooseBrick: ~ return SmallPackage
     - Toolbox: ~ return (Screwdriver, Pliers, Wrench)
    - SealedMetalCylinder: 
        { after( DeviceRemovedFromCylinder ) : 
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
        { before(MetroPlatform): 
            ~ return (BusinessCard, QsBusinessCard, OtherOtherBusinessCard, KingDiamondsCard, SealedMetalCylinder)
            
        - else: 
            ~ return (BusinessCard, QsBusinessCard, OtherOtherBusinessCard, KingDiamondsCard, SealedMetalCylinder, MetroTicket )
            
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
    - Gravestone: ~ return ((BunchOfFlowers, AnotherBunchOfFlowers))
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