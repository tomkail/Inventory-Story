
=== function getItemName(item)
    {item: 
    -   ManilaEnvelope:             manila envelope
    -   BunchOfFlowers:             black lilies 
    -   AnotherBunchOfFlowers:      white lilies
    -   MoreFlowers:                yellow lilies
    -   EvenMoreFlowers:            white roses
    -   BlackLeatherFolder:   black leather folder
    -   WhiteApron: {after(BackAlleyway):stained} white apron
    -   MetroTicket:        { isOrAfter( MetroPlatformScene ): bloodstained} metro ticket
    -   ERNameBadge:    security badge
    - MetalCylinderPhoto: photograph of thin device
    - ErnstRichardsDies: photo of civilian corpse
    - DeviceOperatedPhoto: printout of earthquake spike
    - SealedMetalCylinder: 
        { generatedItems ^ (Nothing, Device): empty | sealed } metal cylinder

    - StolenCard: a stolen Ace of Spades
//    -   KoDStamp:   stamp
    -   else:         {item} 
    }
=== function getItemTooltip(item) 
    {item: 
    - BorderGuard:  "Papers." 
- NoWeddingRing:    "I took it off." 

- BorderGuardWaving:    "You're free to cross." 

- Soldier: "Move along, please." 
- SoldierWithGunPointed:    "I don't want to see this. Move along." 


    -   SealedMetalCylinder:    \*WARNING\*
    - GroupSupport: "We're all here for you, Ernst."
    -   Camera:     "Property: A. Richards."
    - Valet: 
        { levelItems !? ValetReceipt:
            "Can I take your car, sir?"
        - else: 
            "Have a good night!"
        }
    -   Analyst: 
            "What's the big fuss, dude? Why'd you get me in here so early?" 
    -   SurprisedAnalyst: 
            "But a shockwave like that would have toppled a cityblock!"
    -   WifesPromise:   "Let's stay together, forever."
    -   Kosakov:    "The device, please, Ana."
    -   Matthews:   "Don't even think about giving it to him."
    -   Device:  "Property of the US Army"
    -   Quentin:    
            {
            - levelItems ? Wife : 
                "You're a lucky man, Ern." 
            - levelItems !? OtherWeddingRing : 
                "Ready?"
            - else:
                "Go on, then!"
            }
    -   Annie:      "I do." 
    -   Wife:       "Kiss me, Ernie..."
    -   Croupier:   
        {
        - is(StealCardFromKingDiamonds) && levelItems !? PileOfChips:
            "Whenever you're ready, sir." 
        - levelItems !? PileOfChips: 
            "If you're out of chips, sir, you cannot bet."
        - else: 
            "Sir?"
        }
    -   MetroTicket:    "CHAMP DE MARS to MONTPELLIER" 
    -   Gravestone:     "Ernst Richards. Died: 23rd April 1968"
    -   DeviceOperatedPhoto:    "Operation of device observed from California earthquake monitoring station, Jan '61"
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
  
    -   MetalCylinderPhoto: "Created 3/58 Nevada."
    -   PianoWire:      {currentSceneID == MetroPlatformScene:
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
    - DropNote:     "Suspected handover at UN building around 3 o'clock today." 
    - DeadDropNoteFromQuentin: "Handover at the Champs du Mars. Midnight tonight. Q."
  //  - DupontInstructions:   "Further instructions: outside kitchens, Hotel de Champs de Mars."
  //  - KoDStamp:     "KoD"
    - LoyalAssurance:   "I'll get over to the UN right away, sir."
    - KosakovCard: "Paris 15643. Ask for K."
 
     - ComradeAna: 
        {
        - is (BorderCheckpointScene): 
            "Let's just get this over with."
        - got(ManilaEnvelope): 
            "Do the drop, and let's get out of here." 
        - else: 
            "No time to lose. Back to the border."
        }
     - KosakovOnTelephone: "Yes? Do you have something for me?"
     
     - KosakovsDrop:    "We are out of time. Montmatre Tunnel. Tonight. 11:30pm"
     
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

=== function itemReplacesItemWhenGenerated(items)    
    ~ temp item = pop(items) 
    { not item: 
        ~ return () 
    }
    {item: 
    - BorderGuardWaving: ~ item = BorderPapers
    - PostboxWithHiddenEnvelope: ~ item = ManilaEnvelope 
    -   StolenCard: ~ item = AceSpades 
    -   HandCards: 
            ~ return PileOfChips 
    -  else:     ~ item = ()
    }
    ~ return item + itemReplacesItemWhenGenerated(items) 
    
=== function itemRequiresItem(item) 
    { item: 
     - Postbox: ~ return ManilaEnvelope 
 
 
    - Soldier: ~ return ManilaEnvelope
    - Valet: ~ return CarKey 
    - Briefcase: ~ return KeyOnWristChain
    - Analyst: ~ return DeviceOperatedPhoto
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
    -   Croupier:
        { is(StealCardFromKingDiamonds): 
            ~ return PileOfChips 
        - else: 
            ~ return ValetReceipt
        }
    -   ManNearBlackCar: ~ return Camera
    -   Telephone: ~ return KosakovCard
    -   KosakovOnTelephone:  ~ return Device 
    - Jacket: 
        { is(StealCardFromKingDiamonds):
            ~ return AceSpades 
        }
    - Border: ~ return BorderGuardWaving
    - BorderGuard: ~ return BorderPapers 
    }
    ~ return () 
    
    
    
=== function itemGeneratesItems(item) 
    {item: 
    

- BorderGuard: ~ return replaceAs(BorderGuardWaving)
- ComradeAna:  ~ return (BorderPapers, NoWeddingRing)

- Border: ~ return replaceAs(WayIntoEastGermany)
 
 - Postbox: ~ return replaceAs(PostboxWithHiddenEnvelope )
 
 - YellowSkoda: ~ return ComradeAna
 
 
    - Postbox: ~ return ManilaEnvelope
- Soldier: ~ return  replaceAs(SoldierWithGunPointed)
    - Circle: ~ return GroupSupport
    - Valet: ~ return ValetReceipt
    -  Analyst: ~ return replaceAs(SurprisedAnalyst)
    - LinePrinter: ~ return DeviceOperatedPhoto
    - ParkBench: ~ return Kosakov
    - Kosakov: 
        { 
        - isOrBefore(GoThroughWithWedding): 
            ~ return (KosakovsThanks, DeviceOperatedPhoto)
        - else: 
            ~ return KosakovsThanks
        }
    - Matthews: ~ return replaceAs( MatthewsRelief)
    - Lipstick: 
        { is(AnnieGivesInnerDeviceToContact):
            ~ return Device
        }
     - BlackChanelBag: 
        {
        - is(ApartmentBeforeErnst):
            ~ return (Lipstick, WeddingPhoto, KosakovCard )
        - is(AnnieComesFromWork): 
            ~ return (Lipstick, DropNote, WeddingPhoto)
        - else: 
            ~ return (Lipstick)
        }
     
     - Telephone: ~ replaceAs( KosakovOnTelephone)
     - KosakovOnTelephone:  ~ replaceAs( KosakovsDrop)
     
    - ManNearBlackCar: ~ return  ManEnteringCarOutsideUNPhoto
    - Wife:     ~ return WifesPromise
    -  Quentin: ~ return OtherWeddingRing
    - Annie: ~ return replaceAs( Wife)
 
    - Croupier: 
        { is(StealCardFromKingDiamonds):
            ~ return HandCards
        - else: 
            ~ return PileOfChips
        }
    - Device:   ~ return Warp

     - Wall: ~ return LooseBrick
     - LooseBrick: ~ return SmallPackage
     - Toolbox: ~ return (Screwdriver, Pliers, Wrench)
    - SealedMetalCylinder: 
        { after( ApartmentBeforeErnst ) : 
            ~ return Nothing 
        - else: 
            ~ return Device
        }
    - DoorLock: ~ return replaceAs( BrokenDoorLock)
    - LockedDrawer: ~ return (TwoThousandFrancs, MapOfParisMetro)
    - RumpledShirt: ~ return KeyOnChain
    - BlueChevy:
        {
        - is(DriveAfterWedding):
            ~ return (TinCanString, Dress)
        - isOrAfter(AnnieComesFromWork): 
            ~ return (Camera)
        }
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
        {
        - isOrBefore(Apartment):
            ~ return (BusinessCard, QsBusinessCard, OtherOtherBusinessCard, KingDiamondsCard)
        - before(MetroPlatformScene): 
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
        ~ return replaceAs( WaiterHandsUp)
    - HandCards: ~ return replaceAs((QueenHearts, ThreeClubs, SevenHearts, AceSpades, PlayingCard))
    - QueenHeartsReversed:   
        ~ return replaceAs( QueenHearts)
    - ThreeClubsReversed:   
        ~ return replaceAs( ThreeClubs)
    - SevenHeartsReversed:  
        ~ return replaceAs(  SevenHearts )
    - AceSpadesReversed:    
        ~ return replaceAs(  AceSpades )
    - QueenHearts:            
        ~ return replaceAs(  QueenHeartsReversed )
    - ThreeClubs:           
        ~ return replaceAs(  ThreeClubsReversed )
    - SevenHearts:          
        ~ return replaceAs(  SevenHeartsReversed )
    - AceSpades:            
        ~ return replaceAs(  AceSpadesReversed )
    - PlayingCard:          
        ~ return replaceAs(  PlayingCardReversed )
    - PlayingCardReversed:  
        ~ return replaceAs(  PlayingCard )
    - MetalLockBox:         ~ return PileOfChips
    - PileOfChips:          
        ~ return replaceAs( EvenMoreChips  )
    - EvenMoreChips:          
        ~ return replaceAs(  (EvenEvenMoreChips , ValetReceipt) )
    - ValetReceipt: 
        ~ return replaceAs( EvenEvenMoreChips)
    - Agent: ~ return  (Briefcase, KeyOnWristChain)
    - Briefcase: ~ return SealedMetalCylinder 

    - Jacket:      
        { 
        - is(StealCardFromKingDiamonds): 
            ~ return (StolenCard) 
        - else: 
            ~ return (Wallet)
        }
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
    
    
