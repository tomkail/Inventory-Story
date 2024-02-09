
// -> proceedTo(TopSceneID)

LIST NoItemList = NoItem 

  
      

=== pinboard
/*

Photographs on pinboard in background 
Manila envelope - "ER surveillance"
 Car outside UN building - labelled ER arrives 19th April 
 Man in shades walking in airport - labelled ER visits Puerto Rico, retrieves the device
 [ Gravestone - unlabelled ]
 A metal cylinder - labelled whereabouts of device unknown
 
*/

~ LoopCount++
~ previousSceneID = ()

LIST PinboardItems =  (ManilaEnvelope), MetalCylinderPhoto , DeviceStolenFromResearchLab , DeviceOperatedPhoto , ErnstRichardsDies, ManEnteringCarOutsideUNPhoto, DeviceInWallSafe, CylinderInMortuaryPhoto, DeviceRemovedFromCylinderPhoto

VAR PhotosInOpeningEnvelope = (MetalCylinderPhoto  , DeviceOperatedPhoto, ErnstRichardsDies)
TODO: add "checkpoints" in flow that drop in here 
TODO: reset at the end of run doesn't reset ink but just bounces. 

// allow in ManEnteringCarOutsideUNPhoto / DeviceStolenFromResearchLab if you've been there

-> scene( PinboardItems, ManilaEnvelope,  "Something's not right here.")

    
=== function recordNewPinboardPhoto(photo)
    ~ PhotosInOpeningEnvelope += photo


=== function pinboard_gameplay(type, item) 
    { type: 
    - Sequence: {item: 
         -   ():                 ~ return 1 
        -   ManilaEnvelope:     ~ return BerlinDeadDropScene
        -   ErnstRichardsDies:  ~ return Graveyard 
        -   DeviceInWallSafe:   ~ return Apartment
        -   ManEnteringCarOutsideUNPhoto:  ~ return  NoteInCar
        -   DeviceOperatedPhoto:    ~ return MonitoringStationMorning
        -   CylinderInMortuaryPhoto:    ~ return Mortuary
        -   DeviceRemovedFromCylinderPhoto: ~ return ApartmentBeforeErnst
        -   MetalCylinderPhoto: ~ return QGetsDeviceScene
    }
    - Generation: 
        { item : 
          - ManilaEnvelope: 
                ~ return replaceAs(PhotosInOpeningEnvelope) 
        }
    - Tooltip: 
        {item: 
        -   CylinderInMortuaryPhoto: "Found amongst possessions of ER, murder victim."
        }
        
    }
    ~ return () 

 
/*
    BerlinDeadDrop
 */
 
 
 
=== BerlinDeadDrop 
    LIST BerlinDeadDropItems = (Streetlamp) , ( Soldier) , (Postbox), SoldierWithGunPointed
    VAR BerlinDeadDropInteractables = (Soldier, Postbox, ManilaEnvelope) 
    
    -> scene ( BerlinDeadDropItems, BerlinDeadDropInteractables, "But I did as I was told to do. For what good it did me.") 
    
=== function BerlinDeadDrop_fn(act, item) 
    { act: 
    - Sequence: {item: 
        -   ManilaEnvelope: ~ return DroppingBerlinDeadDropScene
        -   Postbox:     ~ return DroppingBerlinDeadDropScene
        -   ManilaEnvelope:     ~ return Pinboard
        }
        ~ return pinboard_gameplay(act, item)
    - Tooltip: {item: 
        - Soldier: "Move along, please." 
        - SoldierWithGunPointed:    "I don't want to see this. Move along." 
        }
    - Requirement: {item: 
        - Soldier: ~ return ManilaEnvelope
        - Postbox: ~ return ManilaEnvelope 
        }
    
    - Generation: {item: 
        - Postbox: ~ return ManilaEnvelope
        - Soldier: ~ return  replaceAs(SoldierWithGunPointed)
        }
    }
    
    ~ return ()
    
    
    
 /*
    DroppingBerlinDeadDrop
 */

 
=== DroppingBerlinDeadDrop 
    LIST DroppingBerlinDeadDropItems = (YellowSkoda) , PostboxWithHiddenEnvelope, ComradeAna
    VAR DroppingBerlinDeadDropInteractables = (Postbox, YellowSkoda)
    
    -> scene ( DroppingBerlinDeadDropItems + Streetlamp + Postbox + ManilaEnvelope, DroppingBerlinDeadDropInteractables, "There was never time to think. Only to act.") 
    
=== function DroppingBerlinDeadDrop_fn(act, item) 
    { act: 
    - Sequence: {item: 
        -   (): ~ return 1 
        -   YellowSkoda:    ~ return BorderCheckpointScene
        -   ComradeAna:     ~ return BorderCheckpointScene
        -   PostboxWithHiddenEnvelope:  ~ return BerlinDeadDropScene
        }
    - Tooltip: {item: 
        - ComradeAna: 
            {
             - got(ManilaEnvelope): 
                "Do the drop, and let's get out of here." 
            - else: 
                "No time to lose. Back to the border."
            }
        }
    - Generation: {item: 
        - Postbox: ~ return replaceAs(PostboxWithHiddenEnvelope )
        - ComradeAna:  ~ return NoWeddingRing
        }
    - Replacement: {item: 
        - PostboxWithHiddenEnvelope: ~ return ManilaEnvelope 
        }
    }
    ~ return () 

 
 /*
    BorderCheckpoint
 */
 
 
=== BorderCheckpoint 
    LIST BorderCheckpointItems = (BorderGuard) , (Border), NoWeddingRing, BorderGuardWaving,BorderPapers,WayIntoEastGermany
    VAR BorderCheckpointInteractables = (BorderGuard, Border, ComradeAna)
    
    -> scene ( BorderCheckpointItems + ComradeAna, BorderCheckpointInteractables, "Remark") 
=== function BorderCheckpoint_fn(act, item)
    { act: 
    - Sequence: {item: 
        -   (): ~ return 1 
        -   NoWeddingRing: ~ return Graveyard
        -   WayIntoEastGermany:     ~ return DroppingBerlinDeadDropScene
        } 
    - Tooltip: {item: 
        - ComradeAna: 
            "Let's just get this over with."
        - BorderGuard:  "Papers." 
        - BorderGuardWaving:    "You're free to cross." 

        }
    - Requirement: {item: 
        - BorderGuard: ~ return BorderPapers
        - Border: ~ return BorderGuardWaving
        }
    - Generation: {item: 
        - ComradeAna:  ~ return (NoWeddingRing, BorderPapers)
        - BorderGuard: ~ return replaceAs(BorderGuardWaving)
        - Border: ~ return replaceAs(WayIntoEastGermany)
        }
    - Replacement: {item: 
        
        - BorderGuardWaving:  ~ return BorderPapers
        }
    }
    ~ return () 
   


=== graveyard
/*
Gravestone - Ernst Richards
 Flowers 
 Flowers
 [ Wedding ring - inscribed "Annabel and Ernst October 1962"  ]
 */
LIST GraveyardItems =  (Gravestone), WeddingRing, BunchOfFlowers, AnotherBunchOfFlowers, MoreFlowers, EvenMoreFlowers, (Veil), EyesBrimmingWithTears
VAR GraveyardInteractables = (Gravestone, BunchOfFlowers, AnotherBunchOfFlowers, EvenMoreFlowers, Veil)

-> scene( GraveyardItems, GraveyardInteractables,  "I know I loved him. I would have loved him all my life, if I could have.") 

=== function graveyard_fn(act, item)
    {act: 
    - Sequence: {item: 
        -   (): 
                ~ return 1 
        -   (EvenMoreFlowers):    
                ~ return Wedding 
        -   EyesBrimmingWithTears: 
                ~ return AnnieHearsOfDeathScene
        -   WeddingRing:
                ~ return Mortuary
        }
    - Name: {item: 
        -   BunchOfFlowers:             black lilies 
        -   AnotherBunchOfFlowers:      white lilies
        -   MoreFlowers:                yellow lilies
        -   EvenMoreFlowers:            white roses
        }
    - Tooltip: {item: 
        -   Gravestone:     "Ernst Richards. Died: 23rd April 1968"
        }
    - Generation: {item: 
         - Gravestone: ~ return ((BunchOfFlowers, AnotherBunchOfFlowers))
         - Veil: ~ return EyesBrimmingWithTears
         - BunchOfFlowers:   ~ return EvenMoreFlowers
        - EvenMoreFlowers:  ~ return WeddingRing 
        - AnotherBunchOfFlowers:    ~ return MoreFlowers
        }
    }
    ~ return () 



 /*
    AnnieHearsOfDeath
 */
 
 
=== AnnieHearsOfDeath 
    LIST AnnieHearsOfDeathItems = (UnlitLamp) , PhonecallFromMortuary, (EmptySideOfTheBed), RingingTelephone, LitLamp
    VAR AnnieHearsOfDeathInteractables = (UnlitLamp, LitLamp, RingingTelephone)
    
    -> scene ( AnnieHearsOfDeathItems, AnnieHearsOfDeathInteractables, "Remark") 

    
=== function AnnieHearsOfDeath_gameplay(type, item) 
    { type: 
    -   Sequence: {item: 
        -   (): ~ return 1 
        -   PhonecallFromMortuary : ~ return Mortuary
        -   EmptySideOfTheBed:      ~ return Apartment
        }
        
    -   Replacement: {item: 
        - UnlitLamp:    
            ~  return ( RingingTelephone, Telephone ) 
        }
    
    - Tooltip: 
        {item: 
        - EmptySideOfTheBed:   
            { generatedItems ? PhonecallFromMortuary: 
                "He's never coming home." 
            - else:
                "He's in so much trouble when he comes home."
            }
        - PhonecallFromMortuary: "I'm very sorry... but we need you to come in and identify your husband's body." 
        }
    -   Generation: {item: 
        - LitLamp: ~ return replaceAs(UnlitLamp)
        - UnlitLamp: 
            { got(PhonecallFromMortuary): 
                ~ return replaceAs((LitLamp, Telephone))
            - else: 
                ~ return replaceAs((LitLamp, RingingTelephone))
            }
        - RingingTelephone:    
            ~ return replaceAs((Telephone, PhonecallFromMortuary))    
        }
    }
    ~ return () 


=== mortuary 
    /*

Mortuary tray 

ID Card:  Victim unknown. Evidence he was drugged. Suspected attempted theft. Killer was disturbed and escaped down tunnel.

Wedding ring 
Key ring
 key 
 key
 [sealed metal cylinder ]
Wallet
 card: Ernst Richards, office clerk
 business card
 business card
 business card
 [ playing card - the KING OF DIAMONDS ]
 */
 
 ~ recordNewPinboardPhoto(CylinderInMortuaryPhoto)
 
LIST MortuaryTrayItems =  (PoliceNotes), SealedMetalCylinder, (Wallet), BusinessCard, QsBusinessCard, OtherOtherBusinessCard, TornMapOfParisMetro, KingDiamondsCard, Nothing

-> scene( MortuaryTrayItems + WeddingRing,  Wallet + SealedMetalCylinder, "But something is out of place.") 

=== function mortuary_fn(act, item) 
    { act: 
    - Sequence: {item: 
        - ():                   ~ return    2
        - (TornMapOfParisMetro, SealedMetalCylinder):         
                ~ return  MetroPlatformScene
                
        - (WeddingRing, BusinessCard):          
                ~ return    Wedding
        - (KingDiamondsCard, OtherOtherBusinessCard):     
                ~ return    CardTableAtClub
                
        - (QsBusinessCard, TornMapOfParisMetro): 
                ~ return MetroPlatformScene
                
        - (QsBusinessCard, WeddingRing): 
                ~ return Wedding
                
        - (QsBusinessCard, SealedMetalCylinder):     
                ~ return QGetsDeviceScene
        }
    - Tooltip: {item: 
        -   PoliceNotes: 
                "Attempted theft. Killer was disturbed and escaped. Narcotics in victim's blood."   
        }
    
    }
    ~ return () 



=== MetroPlatform
    /*


Body on Metro platform 

Jacket 
 wallet 
  cards & playing card 
 key ring 
  keys & sealed metal cylinder 
 scarf 
  [ piano wire ] 
[ Torn white scrap of fabric ]
    
    */
LIST MetroPlatformItems =  Jacket, WhiteFabricScrap, Scarf, PianoWire, (FaceDownBody), (BloodstainedPlatform)


-> scene( MetroPlatformItems , (Jacket, Wallet, Scarf, FaceDownBody),  "Something belongs... elsewhere. In other hands.") 

=== function metro_platform_fn(act, item)
    {act: 
    - Sequence: {item: 
        -   ():     ~ return 2 
        -   (PianoWire, WhiteFabricScrap):  ~ return HotelKitchens
        -   (PianoWire, BloodstainedPlatform):  ~ return OnPlatformWaitingForQScene
        -   (FaceDownBody, BloodstainedPlatform):  ~ return OnPlatformWaitingForQScene   
        -   (OtherOtherBusinessCard, KingDiamondsCard): ~ return CardTableAtClub
        -   (MapOfParisMetro, QsBusinessCard): ~ return QGivesNoteToAide
        }
    - Name:     {item: 
        - PianoWire:    bloodsoaked length of piano wire
        - WhiteFabricScrap:   coffee-stained scrap of white fabric
        }
    - Generation: {item: 
        - FaceDownBody: ~ return (Jacket, Scarf, WhiteFabricScrap)
        - Scarf:    ~ return PianoWire
        }
    }
    ~ return () 
    
    
 /*
    OnPlatformWaitingForQScene
 */
 
 
=== OnPlatformWaitingForQ 
    LIST OnPlatformWaitingForQSceneItems = (LoiteringFigure) , ShakingHead
    VAR OnPlatformWaitingForQSceneInteractables = (LoiteringFigure)
    
    -> scene ( Jacket + Scarf + OnPlatformWaitingForQSceneItems, Jacket + Wallet + OnPlatformWaitingForQSceneInteractables, "If only we could know what was about to happen... but we never can.") 


=== function OnPlatformWaitingForQ_gameplay(act, item) 
    {act: 
    -   Sequence: {item: 
        - (): ~ return 2 
        -   (LoiteringFigure, PianoWire): 
                ~ return  HotelKitchens
                
        -   (ShakingHead, PianoWire):    
                ~ return  HotelKitchens
        
        -   ( QsBusinessCard, MapOfParisMetro) : 
                ~ return QGivesNoteToAide
                
        -   (OtherOtherBusinessCard, KingDiamondsCard): 
              ~ return CardTableAtClub
              
        -   (BusinessCard, SealedMetalCylinder):   
                ~ return QGivesItemToErnst
        }
    
    -   Tooltip: {item: 
        -   LoiteringFigure:    "Hmph."
        -   ShakingHead:        "You got the wrong guy, Ernst."
        }
    -   Requirement: {item: 
        -   LoiteringFigure:  
                { not got(ShakingHead):
                    ~ return (MapOfParisMetro, QsBusinessCard) 
                }
        }
    -   Generation: {item: 
        -   LoiteringFigure:    
                { not got(ShakingHead):
                    ~ return ShakingHead
                - else: 
                    ~ return PianoWire 
                }
        }
    }
    ~ return () 
    
    
    

=== in_the_kitchens
/*

 
Kitchens of Hotel. A waiter heads towards the door. 
 Order slip: Coffee, table 15. 
 Name badge - Carl 
 White Apron 
  [ Ten thousand franc note ]
 Coffee cup 
 Tray 
 Bin  
  Food peelings 
  [ Empty glass vial ]
*/

LIST KitchenItems =  CoffeeOrderSlip, (WaiterNameBadge), (WhiteApron), (Bin), FoodPeelings, EmptyGlassVial, ( CoffeeOnTray ), CoffeeSpoon


-> scene( KitchenItems, (Bin, WhiteApron, CoffeeOnTray) ,  "In bad hands, things go badly. It's that simple.") 

=== function kitchen_fn(act, item)  
    { act: 
    - Sequence: {item: 
        -   (): ~ return 2 
        -   (WaiterNameBadge, WhiteApron):     
                ~ return BackAlleyway
        -   (WaiterNameBadge, EmptyGlassVial): 
                ~ return BackAlleyway
        -   (PianoWire, EmptyGlassVial):    
                ~ return HotelBathroom
        -   (CoffeeOnTray, EmptyGlassVial):    
                ~ return HotelBathroom
        -   (CoffeeSpoon, EmptyGlassVial):     
                ~ return HotelBathroom
        }
    - Generation:  {item: 
        - Bin: ~ return (FoodPeelings, EmptyGlassVial)
        - CoffeeOnTray:     ~ return CoffeeSpoon
        - WhiteApron: ~ return (CoffeeOrderSlip, PianoWire)
        }
    
    } 
    ~ return () 
    
    
TODO: coffee back to ernst drinking coffee ?

=== hotel_bathroom

/*
 
Black bag - bathroom of hotel 
Unconcious waiter 
  Apron
  Name badge: "Carl"
Bag
 Piano wire 
 10,000 franc note 
 Black velvet bag
    Glass vial of powder 
    Choloform bottle 
 Flick knife
 Small gun (that one Bond has) 
 Cigarettes 
 Metro Pass in name of C. Dupont 
[ Cardboard box labelled: "Claude. Rat poison - DO NOT OPEN.", empty ]
*/

LIST HotelBathroomItems = (UnconciousWaiter), BlackLeatherFolder, (BlackKitBag) ,  GlassVialOfPowder, FlickKnife, Cosh, SmallGun, Cigarettes, DupontMetroPass

-> scene( HotelBathroomItems , (UnconciousWaiter, BlackKitBag, BlackLeatherFolder) ,   "I think that's why Ernst died. Someone... gave him... something.") 

=== function bathroom_fn (act, item)
    { act: 
    - Sequence: {item: 
        - ():   ~ return 2
        -  (  UnconciousWaiter, Cosh):      ~ return BackAlleyway
        -  ( CasinoChips, PhotoOfErnst):    ~ return  BackOfClub
        }
    - Tooltip: {item: 
        -   UnconciousWaiter:   "Uhhhh...."
        }
    - Generation: {item: 
        - UnconciousWaiter: 
            ~ return (WhiteApron, WaiterNameBadge)
        }
    }
    ~ return () 
    
  

=== back_alleyway
 /*
Back alleyway, door marked "KITCHENS" 

Box labelled: "Claude. Rat poison - DO NOT OPEN."
 Glass vial 
 Photograph of the dead guy with name: "Ernst Richards" 
 [ Two black casino chips from the KING OF DIAMONDS nightclub ]
 */
 
LIST HotelAlleywayItems = (DoorLock), PhotoOfErnst,  CasinoChips, BrokenDoorLock, (Waiter), WaiterHandsUp



-> scene( HotelAlleywayItems + BlackLeatherFolder + BlackKitBag, (BlackLeatherFolder, BlackKitBag, DoorLock, Waiter),   "My husband wasn't perfect. Just another poor unfortunate who got in the way.") 

=== function alleyway_fn(act, item) 
    {act: 
    - Sequence: {item: 
        -   (): ~ return 2
        -   (CasinoChips, PhotoOfErnst):
                ~ return BackAlleyway
        } 
    - Tooltip: {item: 
        -   WaiterHandsUp:  "Whatever you say, Monsieur!" 
        -   Waiter:     "What are you looking at, huh?"
        -   DoorLock:   "KITCHENS"
        }
    - Requirement: {item: 
        -   DoorLock:   ~ return FlickKnife 
        -   Waiter:         ~ return (SmallGun, FlickKnife)
        }
    - Generation: {item: 
        - Waiter: 
            ~ return replaceAs( WaiterHandsUp)
        - DoorLock: ~ return replaceAs( BrokenDoorLock)
        
        }
    }
    ~ return ()



  /*
=== metro_dupont 
// pickpocket pass 

LIST MetroDupontItems = (DupontInstructions), (Stranger), KoDStamp, DupontMetroPass

-> scene("Montpellier Metro station", "23rd April 1968, 9:23pm", MetroDupontItems + BlackKitBag, (Stranger, DupontInstructions), -> dupont_metro_fn,  "I want to know who got hurt along the way.") 

=== function dupont_metro_fn(x) 
    { x: 
    -   ():         ~ return 1 
    -   KoDStamp:   ~ return -> back_of_kingdiamondsclub
    -   else:       ~ return -> NOPE
    }
*/

  
=== back_of_kingdiamondsclub  

   
 /* 
Metal Security Box 
 Pile of KING OF DIAMONDS chips 
    More chips 
        More chips 
        [ Valet parking receipt - Blue Chevy for Ernst Richards ]
 KingKey

*/

LIST KingDiamondsBoxItems = (KingKey), (MetalLockBox), PileOfChips, EvenMoreChips, EvenEvenMoreChips

-> scene( KingDiamondsBoxItems + AceSpades, (MetalLockBox, PileOfChips, EvenMoreChips, AceSpades),  "Because of something buried...")  

=== function back_club_fn(act, item) 
    {act: 
    - Sequence: {item: 
        -   ():     ~ return 1 
        -   (ValetReceipt, AceSpades):   ~ return CardTableAtClub 
        }
    - Requirement: {item: 
        -   MetalLockBox:   ~ return KingKey
        }
    - Generation: {item: 
        - MetalLockBox:         ~ return PileOfChips
        - PileOfChips:          
            ~ return replaceAs( EvenMoreChips  )
        - EvenMoreChips:          
            ~ return replaceAs(  (EvenEvenMoreChips , ValetReceipt) )
        }
    }
    ~ return () 
    
    
  
=== kingdiamondsclub  

   
 /* 
Poker Table 

Pile of KING OF DIAMONDS chips 

Hand of cards
  Ace of Hearts 
  Three Clubs 
  [ Ace of Spades [ the back of this card is very slightly different ] ]
  King Diamonds 
  Seven Hearts 
Valet parking receipt - Blue Chevy for Ernst Richards

*/

LIST KingDiamondsClubItems =  (HandCards), QueenHearts, ThreeClubs, SevenHearts, AceSpades, (ValetReceipt), QueenHeartsReversed, ThreeClubsReversed, SevenHeartsReversed, AceSpadesReversed, PlayingCardReversed, PlayingCard, ( GinAndTonic ), (Croupier) 

VAR KingDiamondsClubInteractables = ( HandCards, QueenHeartsReversed, QueenHearts, ThreeClubsReversed, ThreeClubs, SevenHeartsReversed, SevenHearts, AceSpadesReversed, AceSpades, Croupier )
  
-> scene(KingDiamondsClubItems, KingDiamondsClubInteractables, "Because someone tried to change things. To turn them to their own advantage. Only the house always wins.") 

=== function king_diamond_club_fn(act, item) 
    {act: 
    - Sequence: {item: 
        -   ():     ~ return 1 
        -   AceSpadesReversed:      ~ return Apartment
        -   ValetReceipt:            ~ return ParkingLot
        }
    - Tooltip: {item: 
        -   Croupier:   
            {  not got(PileOfChips ) : 
                "If you're out of chips, sir, you cannot bet."
            - else: 
                "Sir?"
            }
        }
    - Requirement: {item: 
        - Croupier: ~ return ValetReceipt
        }
    - Generation: {item: 
        - Croupier:     ~ return PileOfChips
        }
    
    }
    ~ return () 



=== parking_lot 
   
    LIST ParkingLotItems = (Valet) , (KingDiamondsNightclub)
    VAR ParkingLotInteractables = (Valet, BlueChevy)
    -> scene ( ParkingLotItems + BlueChevy + CarKey, ParkingLotInteractables, "Remark") 
    
=== function parking_lot_fn(act, item) 
    {act: 
    - Sequence: {item: 
        -   (): ~ return 1 
        -   CarKey:     ~ return Apartment
        -   Camera:     ~ return AnnieComesFromWork
        }
    - Tooltip: {item: 
        - Valet: 
            { not got(ValetReceipt):
                "Can I take your car, sir?"
            - else: 
                "Have a good night!"
            }
        }
    - Requirement: {item: 
        - Valet: ~ return CarKey 
        }
    - Generation: {item: 
        - Valet: ~ return ValetReceipt
        }
    }
    ~ return ()  




=== annie_gives_inner_device 
    LIST AnnieGivesDeviceToItems = ( Kosakov)  , KosakovsThanks, QuentinsRelief, QuentinDead
    
    VAR AnnieGivesDeviceToInteracts = ( Kosakov, Quentin , Device )
    -> scene ( AnnieGivesDeviceToItems + BlackChanelBag + Quentin, AnnieGivesDeviceToInteracts, "Remark") 
=== function annie_gives_inner_device_fn(act, item) 
    {act: 
    - Sequence: {item: 
        -   ():                 ~ return 1 
        -   Warp:               ~ return Pinboard
        -   KosakovsThanks:     ~ return GoThroughWithWedding
        -   QuentinsRelief:      ~ return Pinboard 
            TODO: change history !!
        }
    - Tooltip: {item: 
        - Quentin: 
            "Don't even think about giving it to him."
        - Kosakov: 
             "You are having second thoughts? You wish to be extracted?"
        - KosakovsThanks: 
            "Well done, comrade. You have our thanks. We will extract you when it is safe."
        -   QuentinsRelief:     "You've made the right decision. I'm glad you've seen what's right here."
        - QuentinDead:  "Annie... why..."
    
        }
    - Requirement: {item: 
        -   Kosakov:      ~ return Device
        -   Quentin:    ~ return Device
        }
    - Generation: {item: 
        - BlackChanelBag: 
            ~ return (LipstickHidingDevice,  KosakovCard )
            
        - Quentin: ~ return QuentinsRelief
        
        - Kosakov: 
           ~ return ( KosakovsThanks,  QuentinDead ) 
        
        }
    - Replacement: {item: 
         -   QuentinDead:   ~ return Quentin  
        }
    }
    ~ return () 
 
 



=== apartment

LIST ApartmentItems = (WallSafe), (MapOfParisMetro), (WeddingPhoto), (KeyHook), CarKey 

-> scene( ApartmentItems + WeddingRing + Annie + Wallet, (WallSafe, KeyHook, SealedMetalCylinder),   "I told him not to go. But he wouldn't listen. He said this time it would be different.")  

=== function apartment_fn(act, item) 
    {act: 
    - PostAction: 
        { currentItems ? SealedMetalCylinder:
            ~ recordNewPinboardPhoto(DeviceInWallSafe) 
        }
    - Sequence: {item: 
        - ():   ~ return 2 
        - (AceSpades, KingDiamondsCard):    
            ~ return StealCardFromKingDiamonds
        - ( Annie, CarKey):     
            ~ return AnnieComesFromWork
        - ( MapOfParisMetro, TwoThousandFrancs ): 
            ~ return NoteInCar 
        - ( MapOfParisMetro, SealedMetalCylinder ): 
            ~ return QGivesItemToErnst     
        - ( WeddingPhoto,  CarKey ) : 
            ~ return DriveAfterWedding
        - ( WeddingRing,  CarKey ) : 
            ~ return DriveAfterWedding  
        - ( WeddingPhoto,  Annie) : 
            ~ return Wedding
        - ( WeddingPhoto,  WeddingRing) : 
            ~ return Wedding 
        }
    }
    ~ return () 
    
    
    
    
    
    
=== apartment_after_ernst 
    LIST ApartmentWithoutErnstItems = KosakovCard , (Telephone), KosakovOnTelephone, KosakovsDrop, (BlackChanelBag), LipstickHidingDevice
    
    VAR ApartmentWithoutErnstInteractables = (WallSafe , SealedMetalCylinder, Device, BlackChanelBag, Telephone, KosakovOnTelephone)
    
    -> scene ( ApartmentWithoutErnstItems + WallSafe + WeddingPhoto + KeyHook , ApartmentWithoutErnstInteractables, "Remark") 
=== function apartment_after_ernst_fn(act, item) 
    {act: 
    - PostAction: 
        { got ( Device  ): 
            ~ recordNewPinboardPhoto(DeviceRemovedFromCylinderPhoto)
        }
    - Sequence: {item: 
        -   ():     ~ return 1
        -   (LipstickHidingDevice, KosakovsDrop):  
                ~ return AnnieGivesInnerDeviceToContact
                // forward (!)
        -   ( DropNote, ManEnteringCarOutsideUNPhoto) :
                ~ return AnnieComesFromWork
                
        -   Warp:           
                ~ return Pinboard
        } 
    - Tooltip: {item:
        - KosakovOnTelephone: "Yes? Do you have something for me?"
     
        - KosakovsDrop:    "We are out of time. Montmatre Tunnel. Tonight. 11:30pm"
        }
    - Requirement: {item: 
        -   Telephone: ~ return KosakovCard
        -   KosakovOnTelephone:  ~ return Device 
        }
    - Generation: {item: 
        - BlackChanelBag: 
            ~ return (Lipstick, WeddingPhoto, KosakovCard, DropNote, ManEnteringCarOutsideUNPhoto )
        - Telephone: ~ return replaceAs( KosakovOnTelephone)
        - KosakovOnTelephone:  ~ return replaceAs( KosakovsDrop)
        }
     
    }
    ~ return ()  
     
     
    
=== annie_in_car 
    LIST AnnieCarItems = Camera , (ManNearBlackCar) ,DropNote
    VAR AnnieCarInteractables = (ManNearBlackCar, BlackChanelBag, BlueChevy )
    -> scene ( AnnieCarItems + CarKey + BlueChevy , AnnieCarInteractables, "I was always watching...") 
=== function annie_in_car_fn(act, item)
    {act: 
    - Sequence: {item: 
        -   (): ~ return 1 
        -   ManEnteringCarOutsideUNPhoto: 
                ~ return NoteInCar
        -   DropNote:   
                ~ return ApartmentBeforeErnst // forwards
        }
    - Tooltip: {item: 
        - DropNote:     "Suspected handover at UN building around 3 o'clock today." 
        }
    - Generation: {item: 
        - BlueChevy: 
            ~ return BlackChanelBag
        - BlackChanelBag: 
            ~ return (Lipstick, DropNote, WeddingPhoto, Camera)
        - ManNearBlackCar: ~ return  ManEnteringCarOutsideUNPhoto
        }
    - Requirement: {item: 
        -   ManNearBlackCar: ~ return Camera
        }
    }
    ~ return () 


    

=== quentin_passes_note 

    ~ recordNewPinboardPhoto(ManEnteringCarOutsideUNPhoto)

    LIST OutsideUNBuilding = (BlackCar), QuentinsAide, (ERNameBadge), TwoThousandFrancs
    
    -> scene( OutsideUNBuilding, (BlackCar, QuentinsAide) ,  "Not every opportunity is worth taking, however well paid." ) 
    
=== function quentin_note_fn(act, item)
    {act: 
    - Sequence: {item: 
        -   ():     ~ return 2
        -   (TwoThousandFrancs, MapOfParisMetro):
                ~ return QGivesNoteToAide
        }
    - Tooltip: {item: 
         -   NoteInCar:  "Ernst Richards?"
         -   ERNameBadge:    "Ernst Richards. 67834-A-X-2"
    
        }
    
    - Requirement: {item: 
        - QuentinsAide: ~ return ERNameBadge
        }
    - Generation: {item: 
        - BlackCar: ~ return QuentinsAide  
        - QuentinsAide: 
            ~ return (  MapOfParisMetro, TwoThousandFrancs )  
        }
    }
    ~ return () 





=== quentin_gives_aide_money 
  
    LIST QuentinGivesAideMoneyItems = (LockedDrawer) , (RumpledShirt) , KeyOnChain ,  LoyalAssurance
    -> scene ( QuentinGivesAideMoneyItems +  QuentinsAide, (LockedDrawer, RumpledShirt, QuentinsAide), "Things often happen by circuitous routes.") 

=== function gives_aide_money_gameplay(act, item) 
    {act: 
    - Sequence: {item: 
        -   (): ~ return 1 
        -   LoyalAssurance:    ~ return QGivesItemToErnst  
        }
    -   Tooltip: {item:
        - LoyalAssurance:   "I'll get over to the UN right away, sir."
        }
    -   Requirement: {item: 
        -   LockedDrawer:   ~ return KeyOnChain
        -   QuentinsAide: ~ return (MapOfParisMetro, TwoThousandFrancs)
        }
    -   Generation: {item: 
        - QuentinsAide: ~ return ( LoyalAssurance ) 
        - RumpledShirt: ~ return KeyOnChain
        - LockedDrawer: ~ return (TwoThousandFrancs, MapOfParisMetro)
        }
    }




    
=== QGivesItemToErnst_knot  
    LIST UNDeskJobItems =  (USFlag), (DeskPlate), Envelope, NoteFromQuentin 
    -> scene(UNDeskJobItems +  WeddingPhoto + QuentinsAide, ( QuentinsAide, Envelope),  "But it's hard to tell friends from enemies, sometimes.")   
=== function QGivesItemToErnst_gameplay(act, item)  
    { act: 
    - Sequence: {item: 
        -   ():     ~ return 2
        -   (SealedMetalCylinder, NoteFromQuentin):  
                    ~ return QGetsDeviceScene
        }
    - Tooltip: {item: 
        -   DeskPlate:  "Ernst Richards. Clerk."
        -   QGivesItemToErnst: "Ernst Richards?"
        -   NoteFromQuentin:  "Ernie - Hold onto this for me. Keep it safe. Q."
        }
    - Requirement: {item: 
        - QuentinsAide: ~ return DeskPlate 
        }
    - Generation: {item: 
        - QuentinsAide: ~ return Envelope
        - Envelope: 
            ~ return ( NoteFromQuentin, SealedMetalCylinder)
        }
    - Replacement: {item: 
        - Envelope: ~ return QuentinsAide
        }
    }
    ~ return () 
 
 
 
 === king_clubs_steal_card
 
    LIST KingClubsStealCardItems = StolenCard 
    VAR KingClubsStealCardInteractables = (HandCards, Jacket, Croupier)
    -> scene ( KingClubsStealCardItems + Croupier + PileOfChips + Jacket, KingClubsStealCardInteractables, "I think Ernst died because he took something he shouldn't have taken.") 
 === function king_clubs_steal_card_fn(act, item)
    {act: 
    - Sequence: {item: 
        -   (): ~ return 1
        - (PlayingCard): 
            ~ return CardTableAtClub 
        - (QueenHearts): 
            ~ return Wedding 
        - (StolenCard):  
            ~ return Apartment 
        }
    - Name: {item: 
        - StolenCard: a stolen Ace of Spades
        }
    - Tooltip: {item: 
        - Croupier: 
            {not got(PileOfChips):
                "Whenever you're ready, sir." 
            - else: 
                "Sir?"
            }
        }
    -  Requirement: {item: 
        -   Croupier:   ~ return PileOfChips 
        -   Jacket:     ~ return AceSpades 
        }
    - Generation: {item: 
          - Croupier:   ~ return HandCards
          - Jacket:     ~ return StolenCard
        }
    - Replacement: {item: 
        -   HandCards:  ~ return PileOfChips
        -   StolenCard: ~ return  AceSpades 
        }
    }
    ~ return () 
 
 
 
 === QGetsDevice_knot 
    LIST QuentinReceivesCylinderItems = (Wall), LooseBrick, SmallPackage, (Toolbox), (ParkAttendantUniform), Screwdriver, Hammer, PocketKnife, ReplacedBrick, RavensFeather
    VAR QuentinReceivesCylinderInteracts = (Wall, SmallPackage,LooseBrick, SealedMetalCylinder, Toolbox)
    -> scene ( QuentinReceivesCylinderItems, QuentinReceivesCylinderInteracts, "The device was passed to an American agent by means of a dead drop.") 
=== function q_receives_cylinder_fn(act, item)
    {act: 
    - Sequence: {item: 
        -   (): ~ return 1 
        -   RavensFeather:  ~ return RavensNestScene
        -   Device: ~ return DeviceOperated
        -   Warp:   ~ return TopSceneID 
        }
    - Name: {item: 
        - LooseBrick:   hollow brick
        }
    - Tooltip: {item: 
        - ParkAttendantUniform: "Small Parks Maintenance"
        - Toolbox: "Property of Small Parks Maintenance"
        - SmallPackage: "For Q. Keep safe. High enemy interest."
        }
    - Requirement: {item: 
        -   LooseBrick:     ~ return  Screwdriver
        -   Wall:   
                { 
                - got(LooseBrick) && got(SmallPackage):
                    ~ return LooseBrick 
                - generatedItems !? LooseBrick : 
                    ~ return Hammer
                }
        - SmallPackage:     ~ return PocketKnife
        - Toolbox: 
            { got_any((Screwdriver, Hammer, PocketKnife, SealedMetalCylinder)):
                ~ return  (Screwdriver, Hammer, PocketKnife, SealedMetalCylinder)
            }
        }
    - Generation: {item: 
        -   Wall: 
                { 
                - not got(LooseBrick):
                    ~ return LooseBrick
                - got(SmallPackage): 
                    ~ return Nothing
                }
        
         - LooseBrick: 
                ~ return SmallPackage
         - SmallPackage: 
                ~ return replaceAs((SealedMetalCylinder, RavensFeather))
         - Toolbox: 
                { 
                - withItem && got(withItem): 
                    ~ return Nothing 
                - not got(SealedMetalCylinder) && generatedItems ? SealedMetalCylinder: 
                    ~ return (Screwdriver, Hammer, PocketKnife, SealedMetalCylinder)
                - else: 
                    ~ return (Screwdriver, Hammer, PocketKnife)
                }
        
        }
    
   
    }
      ~ return ()   
      
      TODO: how did quentin find out? 
 
 
 
 
 
 
 

/// GamblersAnonymous
=== gamblers_anonymous
    LIST GamblersItems = (Circle) , GroupSupport
    VAR GamblersInteractables = (Circle)
    -> scene ( GamblersItems, GamblersInteractables, "Ernst tried to get help, but somehow, it would never stick.") 
=== function gamblers_anonymous_fn(act, item)
    {act: 
    - Sequence: {item: 
        -   (): ~ return 1 
    // this doesn't make sense    
        - GroupSupport:     InBedWithErnstScene 
        }
    - Tooltip: {item: 
        - Circle:       "Ernst?"
        - GroupSupport: "We're all here for you, Ernst."
        }
    - Generation: {item: 
        - Circle: ~ return GroupSupport
        }
    }
    ~ return () 
 
 
 
 
 /*
    RavensNest
 */
 
 
=== RavensNest_knot 
    LIST RavensNestItems = (Perch), Raven, (DrownedBody), (Intercom), Assistant, (EdgeOverlookingCanal), DistantSplash, SwissTrainTicket
    
    -> scene ( RavensNestItems, RavensNest_gameplay(Interactables, ()), "Remark") 


=== function RavensNest_gameplay(act, item) 
    {act: 
    -   Sequence: {item: 
        - (): ~ return 1 
        - DrownedBody:          ~ return AgentThrownIntoSeineScene
        - SwissTrainTicket:     ~ return AgentThrownIntoSeineScene
        }
    - Interactables: ~ return (Perch, Raven, DrownedBody, Briefcase, Intercom, Assistant, EdgeOverlookingCanal)
    - Name: {item: 
        - Briefcase: torn briefcase 
        }
    -   Tooltip: {item: 
        - Briefcase:    "L...d's ... don..."
        - Assistant:
            { noLongerGot(SealedMetalCylinder): 
                "I'll see that one of our operatives receives it, Ma'am."
            - else: 
                "Ma'am?" 
            }
        
        }
    -   Requirement: {item: 
        -   Assistant: 
                { noLongerGot(SealedMetalCylinder): 
                    ~ return RavensFeather 
                - else: 
                     ~ return (SealedMetalCylinder) 
                    
                }
        -   EdgeOverlookingCanal: ~ return DrownedBody
        }
    - PostAction: 
            { noLongerGot((SealedMetalCylinder, RavensFeather)) : 
                ~ removeItem(Assistant) 
            }
    -   Generation: {item: 
            - EdgeOverlookingCanal:     
                { generatedItems ? Briefcase:
                    ~ return DistantSplash 
                }
            - Assistant: 
                ~ return Nothing
            - Intercom: 
                ~ return generateOnce(Assistant)
                
            - Briefcase: 
                ~ return generateOnce(SealedMetalCylinder)
                
            - DrownedBody: ~ return (Briefcase , SwissTrainTicket)
            - Perch: ~ return Raven
            - Raven: ~ return generateOnce(RavensFeather)
        }
    - Replacement:  {item:
        - DistantSplash: ~ return DrownedBody
        }
    }
    ~ return () 

 
 
    
 /*
    QuestionScientist
 */
 
 
=== QuestionScientist 
    VAR answerCount = 0
    ~ answerCount = 0 // reset at top of scene
    LIST QuestionScientistItems = (TiedUpScientist) , (LiveWires), (Questions), TerrifiedScientist, Answer, (CellGuard), DeadScientist
    VAR QuestionScientistInteractables = (TiedUpScientist, TerrifiedScientist) 
    
    -> scene ( QuestionScientistItems, QuestionScientistInteractables, "Remark") 
  

=== function QuestionScientist_gameplay(act, item) 
    {act: 
    -   Sequence: {item:
        - () : ~ return 1
TODO: A solve c
        }
    -   PostAction: {item: 
        - TerrifiedScientist: 
            { withItem == Questions: 
                ~ answerCount++ 
            }
        }
    -   Tooltip: {item: 
        - CellGuard: 
            { got(DeadScientist): 
                "You're in a lot of trouble, friend." 
            - else: 
                "You have all the time you need."
            }
        - DeadScientist: 
            "..."
        - TerrifiedScientist: 
            { answerCount:
            -   0: "Please. Please..."
            -   1: "Stop. Don't... please..." 
            }
            
        - TiedUpScientist: 
            { answerCount:
            -   0:  "You can't make me talk."
            -   1:  "I won't say anything more." 
            -   2:  "Please. No more." 
            }
        - Questions: 
            { answerCount:
            -   0:    "How does the device work?"
            -   1:  "What kind of marker?" 
            - 2:    "So explain."
            }
        - Answer: 
            { answerCount: 
            - 1:    "The device... it's hard to explain. Creates a signal. A marker."
            - 2:    "A marker... that can be seen... from a later point. Traced and relocated... it's complicated."
            - 3:    "Once activated, the device... provides... a floor... like the sea-bed... for a plumb-line..."
            }
            
        }
    -   Generation: {item: 
        - LiveWires: ~ return TerrifiedScientist
        - TerrifiedScientist: 
            { withItem: 
            - Questions: 
                ~ return (TiedUpScientist, Answer)
            - LiveWires: 
                ~ return DeadScientist
            }
        - TiedUpScientist:  
            { answerCount >= 3: 
                ~ return DeadScientist
            - else:
                ~ return TerrifiedScientist
            }
        }
    -   Requirement: {item: 
        - TiedUpScientist:      ~ return LiveWires 
        - TerrifiedScientist:   ~ return (LiveWires, Questions) 
        }
        
    -   Replacement: {item: 
        - DeadScientist:    
            ~ return (TerrifiedScientist, TiedUpScientist, Questions, Answer)
        - TiedUpScientist:      
            ~ return TerrifiedScientist
        - TerrifiedScientist:   
            ~ return (TiedUpScientist, Answer)
        }
    }
    ~ return () 



 /*
    InBedWithErnst
 */
 
 
 
 
 
=== InBedWithErnst

    LIST InBedWithErnstItems = (SleepingErnst) , LovingMumble, (Pillow), Gun , HusbandsBody, HiddenGun
    VAR InBedWithErnstInteractables = (SleepingErnst, Pillow )
    
    -> scene ( InBedWithErnstItems, InBedWithErnstInteractables, "I did love him. I shouldn't have. But I did.") 

=== function InBedWithErnst_gameplay(act, item) 
    {act: 
    -   Sequence: {item: 
        -   (): ~ return 1 
    -   LovingMumble: ~ return DriveAfterWedding
    -   HiddenGun:  ~ return GoThroughWithWedding
    
        TODO: allow early murder (!)
    -   HusbandsBody:   ~ return ErnDiesEarlyScene 
        // forward, timline switch
        }
    -   Tooltip: {item: 
        - LovingMumble: "I'm so lucky I found you..."
        }
    -   Requirement: {item: 
        - Pillow:  
            { got(Gun): 
                ~ return Gun 
            }
        - SleepingErnst: 
            { got(Gun):  // only if you've got it 
                ~ return Gun 
            }
        }
    -   Generation: {item: 
        - SleepingErnst:  
            {not got(Gun) : 
                ~ return LovingMumble
            - else: 
                ~ return replaceAs(HusbandsBody)
            }
          - Pillow:  
            {got(Gun): 
                ~ return HiddenGun
            - else: 
                ~ return Gun 
            }    
        }
    -   Replacement: {item: 
        -   HusbandsBody: 
                ~ return  (LovingMumble)
        -   Gun:        
                ~ return HiddenGun 
        -   HiddenGun:    
                ~ return Gun
        }
    }
    ~ return ()

 
 
=== wedding_drive_away 
   LIST WeddingCarItems = (BlueChevy), TinCanString
   -> scene( WeddingCarItems  + WeddingRing, (BlueChevy), "I had never been so happy as the day Ernst and I got married." ) 
   
 === function wedding_car_fn(act, item) 
    {act: 
    - Sequence: {item: 
        -   ():     ~ return 1 
        -  (WeddingRing, Dress):  ~ return Wedding
        }
    - Generation: {item: 
        - BlueChevy:   ~ return (TinCanString, Dress)
        }
    }
    ~ return () 
    
    
    
 
 
=== wedding  
    LIST WeddingItems = (Dress), (Annie), (Quentin), OtherWeddingRing, Wife, WifesPromise
    VAR WeddingInteracts = (Wife, Annie, Quentin)
    -> scene(  WeddingItems + MoreFlowers + WeddingRing,  WeddingInteracts,  "That day is one I'd never take back." )  
  
 
=== function wedding_gameplay(act, item) 
    { act: 
    - Sequence: {item: 
        -   ():             ~ return 1 
        -   WifesPromise:   ~ return GoThroughWithWedding
        }
    - Tooltip: {item: 
        -   Annie:      "I do." 
        -   Wife:       "Kiss me, Ernie..."
        -   WifesPromise:   "Let's stay together, forever."
        }
    - Requirement: {item: 
        -   Annie: ~ return OtherWeddingRing
        }
    - Generation: {item: 
        -   Quentin: ~ return OtherWeddingRing
        -   Annie: ~ return replaceAs( Wife)
        -   Wife:     ~ return WifesPromise
        }
    }
    ~ return ()
    
    
    
 
=== go_through_with_wedding 
    LIST GoThroughWithItems = (ParkBench) , Lipstick
    VAR GoThroughWithInteractables = (Kosakov, BlackChanelBag)
    -> scene ( GoThroughWithItems + Kosakov + WeddingRing + BlackChanelBag, GoThroughWithInteractables, "Remark") 
    
=== function go_through_with_wedding_fn(act, item) 
    { act: 
    - Sequence: { item: 
        -   (): ~ return 1 
        -   MetalCylinderPhoto:    ~ return DeviceOperated
TODO: A proper solve , this doens't quite make sense
        }
    - Requirement: {item: 
        - Kosakov:   ~ return WeddingRing
        }
    - Generation: {item: 
        - Kosakov: 
                ~ return  (KosakovsThanks, MetalCylinderPhoto)
        - ParkBench:    ~ return Kosakov
        }
    }
    ~ return ()  
    

   
=== monitoring_station 
    LIST MonitoringItems = (LinePrinter) , (Hotline), (EmptyCoffeeCup), Analyst, CoffeeCup,SmashedCoffeeCup
    VAR MonitoringInteractables = (LinePrinter, Analyst, Hotline)
    -> scene ( MonitoringItems , MonitoringInteractables, "Its location remains unknown, but our best evidence suggests the device has indeed been activated.") 

=== function monitoring_station_gameplay(act, item) 
    { act: 
    -   Sequence: {item: 
        -   (): ~ return 1 
        -   Analyst: 
            { got(SmashedCoffeeCup):
                ~ return DeviceOperated  
            }
        }
    -   Tooltip: {item: 
        -   EmptyCoffeeCup:   "I'll get more later. Not now."
        
        -   Analyst: 
            { generatedItems ? SmashedCoffeeCup: 
                "But a shockwave like that would have toppled a cityblock!"
            - else: 
                "What's the big fuss, dude? Why'd you get me in here so early?" 
            }
        
        }
    -   Generation: {item: 
        - Analyst: ~ return SmashedCoffeeCup // replaceAs(SurprisedAnalyst)
        - LinePrinter: ~ return DeviceOperatedPhoto
        - Hotline: ~ return (Analyst, CoffeeCup)
        } 
    -   Requirement: {item: 
        - Analyst: ~ return DeviceOperatedPhoto
        - Hotline: ~ return DeviceOperatedPhoto
        }
    -   Replacement: {item: 
        - SmashedCoffeeCup: ~ return CoffeeCup
       
        }
    }
    
    

 === device_operated 
    LIST DeviceOperatedItems =  Device, Warp 
    VAR DeviceOperatedInteractables = (SealedMetalCylinder, Device)
    -> scene ( DeviceOperatedItems + SealedMetalCylinder, DeviceOperatedInteractables, "Everything has to start somewhere.") 
    
=== function device_operated_fn(act, item) 
    {act: 
    - Sequence: {item: 
        -   ():     ~ return 1
        -   Warp:   ~ return TopSceneID
        }
    }
    ~ return () 
    
    
    
    