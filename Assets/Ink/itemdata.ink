
VAR OneUseOnlyItems = (LinePrinter, Hotline, Analyst) 


=== function getItemName(item)
    {item: 
    - LooseBrick:   hollow brick
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
    - Lipstick:      empty lipstick tube
    - StolenCard: a stolen Ace of Spades
//    -   KoDStamp:   stamp
    -   else:         {item} 
    }
    
=== function getItemTooltip(item) 
    {item: 
    - SmallPackage: "For Q. Keep safe. High enemy interest."
- PhonecallFromMortuary: "I'm very sorry... but we need you to come in and identify your husband's body." 
     - BorderGuard:  "Papers." 
- NoWeddingRing:    "I took it off." 

- BorderGuardWaving:    "You're free to cross." 

- Soldier: "Move along, please." 
- SoldierWithGunPointed:    "I don't want to see this. Move along." 

     - ComradeAna: 
        {
        - is (BorderCheckpointScene): 
            "Let's just get this over with."
        - got(ManilaEnvelope): 
            "Do the drop, and let's get out of here." 
        - else: 
            "No time to lose. Back to the border."
        }

    -   SealedMetalCylinder:    \*WARNING\*
    - GroupSupport: "We're all here for you, Ernst."
    -   Camera:     "Property: A. Richards."
    - Valet: 
        { not got(ValetReceipt):
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
           
    -   Device:  "Property of the US Army"
    -   Quentin:    
            {
            - currentSceneID ? AnnieGivesInnerDeviceToContact:
                 "Don't even think about giving it to him."
            - got ( Wife ):  
                "You're a lucky man, Ern." 
            - not got ( OtherWeddingRing ): 
                "Ready?"
            - else:
                "Go on, then!"
            }
    -   Annie:      "I do." 
    -   Wife:       "Kiss me, Ernie..."
    -   Croupier:   
        {
        - is(StealCardFromKingDiamonds) &&  not got(PileOfChips):
            "Whenever you're ready, sir." 
        -  not got(PileOfChips ) : 
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
    -   QsBusinessCard:  "Quentin Roch, Private Investigator. Champs de Mars. No matter too small. Divorce a speciality." 
    -   OtherOtherBusinessCard:  "Gamblers Anonymous. DON'T GET LUCKY GET HELP."
    -   WeddingRing:        "Annie and Ernie -- 3 Oct 1962"
    -   ManilaEnvelope:     "Known Timeline of the Hopburg-Steiner Device"  
    -   ManEnteringCarOutsideUNPhoto:  "ER, 23rd April 68"
  
    -   MetalCylinderPhoto: "Created 3/58 Nevada."
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
    - DropNote:     "Suspected handover at UN building around 3 o'clock today." 
    - DeadDropNoteFromQuentin: "Handover at the Champs du Mars. Midnight tonight. Q."
  //  - DupontInstructions:   "Further instructions: outside kitchens, Hotel de Champs de Mars."
  //  - KoDStamp:     "KoD"
    - LoyalAssurance:   "I'll get over to the UN right away, sir."
    - KosakovCard: "Paris 15643. Ask for K."
 
     
     - KosakovOnTelephone: "Yes? Do you have something for me?"
     
     - KosakovsDrop:    "We are out of time. Montmatre Tunnel. Tonight. 11:30pm"
     
    -   KosakovsThanks:  
            { is(GoThroughWithWedding): 
                "This is excellent cover. You have done very well, Ana. Do not worry. Your reward will come."
                
            - else: 
                "Well done, comrade. You have our thanks. We will extract you when it is safe."
            }
    -   QuentinsRelief:     "You've made the right decision. I'm glad you've seen what's right here."
    - QuentinDead:  "Annie... why..."
    - Kosakov: "You are having second thoughts? You wish to be extracted?"
    - LovingMumble: "I'm so lucky I found you..."
    - NewInstructionsFromKosakov: 
        "You'll need to monitor Roch without him seeing you."  
    - ShadowyFigure: 
        { got(NewInstructionsFromKosakov): 
            "My advice, Ana, is to throw yourself back into your work."
        - else: 
            "That phase of your life is over. Time to leave it behind."
        }
    
    }
    ~ return

=== function itemReplacesItemWhenGenerated(items)    
    ~ temp item = pop(items) 
    { not item: 
        ~ return () 
    }
    {item: 
    - UnlitLamp:    
            ~  item = ( RingingTelephone, Telephone ) 
    -   Gun:        ~ item = HiddenGun 
    -   HiddenGun:    ~ item =  Gun
       - BorderGuardWaving: ~ item = BorderPapers
    - PostboxWithHiddenEnvelope: ~ item = ManilaEnvelope 
    - SurprisedAnalyst: 
            ~ item = DeviceOperatedPhoto
    -   StolenCard: 
            ~ item = AceSpades 
    -   QuentinDead: 
            ~ item = Quentin  
    -   HandCards: 
            ~ item = PileOfChips 
    -   HusbandsBody: 
            ~ item = (SleepingErnst, LovingMumble) 
    -  else:     
            ~ item = ()
    }
    ~ return item + itemReplacesItemWhenGenerated(items) 
    
    
=== function itemRequiresItem(item) 
    { item: 
    - Wall: ~ return Hammer
    - ShadowyFigure : ~ return WeddingRing 
    - Pillow:  
        { got(Gun): 
            ~ return Gun 
        }
    - SleepingErnst: 
        { got(Gun):  // only if you've got it 
            ~ return Gun 
        }
    - Valet: ~ return CarKey 
    - Briefcase: ~ return KeyOnWristChain
    - Analyst: ~ return DeviceOperatedPhoto
    - Kosakov:
        ~ return (Device, WeddingRing)
    - Quentin: ~ return Device
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
    - Lipstick : ~ return Device 
      - Postbox: ~ return ManilaEnvelope 
 
 
    - Soldier: ~ return ManilaEnvelope
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
    - RingingTelephone:    ~ return replaceAs((Telephone, PhonecallFromMortuary))
 - UnlitLamp: 
    { got(PhonecallFromMortuary): 
        ~ return replaceAs((LitLamp, Telephone))
    - else: 
        ~ return replaceAs((LitLamp, RingingTelephone))
    }
 - LitLamp: ~ return replaceAs(UnlitLamp)
    - Veil: ~ return EyesBrimmingWithTears
 
    - ShadowyFigure : ~ return (NewInstructionsFromKosakov)
    - SleepingErnst:  
        {not got(Gun) : 
            ~ return LovingMumble
        - else: 
            
            ~ return HusbandsBody
        }
    - Pillow:  
        {got(Gun): 
            ~ return HiddenGun
        - else: 
            ~ return Gun 
        }
    
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
            ~ return ( KosakovsThanks,  QuentinDead ) 
        }
    - Quentin: ~ return QuentinsRelief
    - LipstickHidingDevice: 
            ~ return replaceAs ( ( Device , Lipstick )  ) 
    - Lipstick: 
        ~ return replaceAs( LipstickHidingDevice  )
        
     - BlackChanelBag: 
        {
        - is(AnnieGivesInnerDeviceToContact): 
            ~ return (LipstickHidingDevice,  KosakovCard )
        - is(ApartmentBeforeErnst):
            ~ return (Lipstick, WeddingPhoto, KosakovCard, DropNote, ManEnteringCarOutsideUNPhoto )
        - is(AnnieComesFromWork): 
            ~ return (Lipstick, DropNote, WeddingPhoto)
        - else: 
            ~ return (Lipstick)
        }
     - Hotline: ~ return Analyst
     - Telephone: ~ return replaceAs( KosakovOnTelephone)
     - KosakovOnTelephone:  ~ return replaceAs( KosakovsDrop)
     
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
     - SmallPackage: ~ return SealedMetalCylinder
     - Toolbox: ~ return (Screwdriver, Hammer, Pliers)
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
    
    
