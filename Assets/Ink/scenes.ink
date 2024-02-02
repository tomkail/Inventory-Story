
// -> proceedTo(TopSceneID)

LIST NoItemList = NoItem 

// OpeningSequence
=== opening_sequence
    LIST OpeningItems = Agent, Briefcase, KeyOnWristChain
    { stopping: 
    -   -> scene(Device, () , "Nevada, 1958. The Hopburg-Steiner is constructed.") 
    -   -> scene(SealedMetalCylinder, SealedMetalCylinder , "Despite being only palm-sized, be assured this is a device of extreme consequence.") 
    -   -> scene(Agent, (Agent, Briefcase, SealedMetalCylinder) , "In April 1962, an unknown foreign agent exited the lab in Area 51 with the device in a briefcase.")
    
    -   -> proceedTo(MonitoringStationMorning)
    }
    
=== function opening_sequence_fn (x)
    {x:
    -   ():         ~ return 1 
    -   Device:     ~ return OpeningSequence 
    
    }
    ~ return () 
    
    
=== monitoring_station 
    LIST MonitoringItems = (LinePrinter) , (EmptyCoffeeCup), (Seisometer), (Analyst), SurprisedAnalyst
    VAR MonitoringInteractables = (LinePrinter, Analyst)
    -> scene ( MonitoringItems, MonitoringInteractables, "Its location remains unknown, but it is believed that is has indeed been activated.") 
=== function monitoring_station_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   SurprisedAnalyst: ~ return DeviceOperated
    }
    ~ return () 
      

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

=== function pinboard_exit(x)
    { x: 
    -   ():                 ~ return 1 
    -   ErnstRichardsDies:  ~ return Graveyard 
    -   DeviceInWallSafe:   ~ return Apartment
    -   ManEnteringCarOutsideUNPhoto:  ~ return  NoteInCar
    -   DeviceOperatedPhoto:    ~ return MonitoringStationMorning
    -   CylinderInMortuaryPhoto:    ~ return Mortuary
    -   DeviceRemovedFromCylinderPhoto: ~ return ApartmentBeforeErnst
    }
    ~ return () 
    
=== function recordNewPinboardPhoto(photo)
    ~ PhotosInOpeningEnvelope += photo

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

=== function graveyard_fn(x) 
    { x: 
    -   (): 
            ~ return 1 
    -   (EvenMoreFlowers):    
            ~ return Wedding 
    -   EyesBrimmingWithTears: 
            ~ return AnnieHearsOfDeathScene
    -   WeddingRing:
            ~ return Mortuary
    }
    ~ return () 



 /*
    AnnieHearsOfDeath
 */
 
 
=== AnnieHearsOfDeath 
    LIST AnnieHearsOfDeathItems = (UnlitLamp) , PhonecallFromMortuary, (EmptySideOfTheBed), RingingTelephone, LitLamp
    VAR AnnieHearsOfDeathInteractables = (UnlitLamp, LitLamp, RingingTelephone)
    
    -> scene ( AnnieHearsOfDeathItems, AnnieHearsOfDeathInteractables, "Remark") 
=== function AnnieHearsOfDeath_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   PhonecallFromMortuary : ~ return Mortuary
    -   EmptySideOfTheBed:      ~ return Apartment
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
 
LIST MortuaryTrayItems =  (PoliceNotes), SealedMetalCylinder, (Wallet), BusinessCard, QsBusinessCard, OtherOtherBusinessCard, MetroTicket, KingDiamondsCard, Nothing

-> scene( MortuaryTrayItems + WeddingRing,  Wallet, "But something is out of place.") 

=== function mortuary_fn(x) 
    { x:
    - ():                   ~ return    2
    - (MetroTicket, SealedMetalCylinder):          
            ~ return    MetroPlatform
    - (WeddingRing, BusinessCard):          
            ~ return    Wedding
    - (KingDiamondsCard, OtherOtherBusinessCard):     
            ~ return    CardTableAtClub
    - (QsBusinessCard, MetroTicket): 
            ~ return QGivesNoteToAide
    - (QsBusinessCard, WeddingRing): 
            ~ return Wedding
    - (QsBusinessCard, SealedMetalCylinder):     
            ~ return QGetsDevice
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
LIST MetroPlatformItems =  (Jacket), (WhiteFabricScrap), (Scarf), PianoWire


-> scene( MetroPlatformItems , (Jacket, Wallet, Scarf),  "Something belongs... elsewhere. In other hands.") 

=== function metro_platform_fn(x) 
    {x: 
    -   ():     ~ return 2 
    -   (PianoWire, WhiteFabricScrap):  ~ return HotelKitchens
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

=== function kitchen_fn(x)  // 2 items 
    { x: 
    -   (): ~ return 2 
    -   (WaiterNameBadge, WhiteApron):      ~ return BackAlleyway
    -   (WaiterNameBadge, EmptyGlassVial): ~ return BackAlleyway
    -   (PianoWire, EmptyGlassVial):     ~ return HotelBathroom
    -   (CoffeeOnTray, EmptyGlassVial):     ~ return HotelBathroom
    -   (CoffeeSpoon, EmptyGlassVial):      ~ return HotelBathroom
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

=== function bathroom_fn (x)
    { x: 
    - ():   ~ return 2
    -  (  UnconciousWaiter, Cosh):      ~ return BackAlleyway
    -  ( CasinoChips, PhotoOfErnst):    ~ return  BackOfClub
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

=== function alleyway_fn(x) 
    { x: 
    -   (): ~ return 2
    -   (CasinoChips, PhotoOfErnst):
            ~ return BackAlleyway
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

=== function back_club_fn(x) 
    {x: 
    -   ():     ~ return 1 
    -   (ValetReceipt, AceSpades):   ~ return CardTableAtClub 
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

=== function king_diamond_club_fn(x) 
    {x: 
    -   ():     ~ return 1 
    -   AceSpadesReversed:      ~ return Apartment
    -   ValetReceipt:            ~ return ParkingLot
    }
    ~ return () 



=== parking_lot 
   
    LIST ParkingLotItems = (Valet) , (KingDiamondsNightclub)
    VAR ParkingLotInteractables = (Valet, BlueChevy)
    -> scene ( ParkingLotItems + BlueChevy + CarKey, ParkingLotInteractables, "Remark") 
    
=== function parking_lot_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   CarKey:     ~ return Apartment
    -   Camera:     ~ return AnnieComesFromWork
    }
    ~ return ()  




=== annie_gives_inner_device 
    LIST AnnieGivesDeviceToItems = ( Kosakov)  , KosakovsThanks, QuentinsRelief, QuentinDead
    
    VAR AnnieGivesDeviceToInteracts = ( Kosakov, Quentin , Device )
    -> scene ( AnnieGivesDeviceToItems + BlackChanelBag + Quentin, AnnieGivesDeviceToInteracts, "Remark") 
=== function annie_gives_inner_device_fn(x) 
    { x: 
    -   ():                 ~ return 1 
    -   Warp:               ~ return Pinboard
    -   KosakovsThanks:     ~ return GoThroughWithWedding
    -   QuentinsRelief:      ~ return Pinboard 
        TODO: change history !!
    }
    ~ return () 
 
 



=== apartment

LIST ApartmentItems = (WallSafe), (DeadDropNoteFromQuentin), (WeddingPhoto), (KeyHook), CarKey 

-> scene( ApartmentItems + WeddingRing + Annie + Wallet, (WallSafe, KeyHook, SealedMetalCylinder),   "I told him not to go. But he wouldn't listen. He said this time it would be different.")  

=== function apartment_fn(x) 
    { currentItems ? SealedMetalCylinder:
        ~ recordNewPinboardPhoto(DeviceInWallSafe) 
    }
    {x: 
    - ():   ~ return 2 
    TODO: some way to use the ( AceSpades ): 
    - (AceSpades, KingDiamondsCard):    
        ~ return StealCardFromKingDiamonds
    - ( Annie, CarKey):     
        ~ return AnnieComesFromWork
    - ( DeadDropNoteFromQuentin, TwoThousandFrancs ): 
        ~ return NoteInCar 
    - ( DeadDropNoteFromQuentin, SealedMetalCylinder ): 
        ~ return NoteInCar     
    - ( WeddingPhoto,  CarKey ) : 
        ~ return DriveAfterWedding
    - ( WeddingRing,  CarKey ) : 
        ~ return DriveAfterWedding  
    - ( WeddingPhoto,  Annie) : 
        ~ return Wedding
    - ( WeddingPhoto,  WeddingRing) : 
        ~ return Wedding 
    }
    ~ return () 
    
    
=== apartment_after_ernst 
    LIST ApartmentWithoutErnstItems = KosakovCard , (Telephone), KosakovOnTelephone, KosakovsDrop, (BlackChanelBag), LipstickHidingDevice
    
    VAR ApartmentWithoutErnstInteractables = (WallSafe , SealedMetalCylinder, Device, BlackChanelBag, Telephone, KosakovOnTelephone)
    
    -> scene ( ApartmentWithoutErnstItems + WallSafe + WeddingPhoto + KeyHook , ApartmentWithoutErnstInteractables, "Remark") 
=== function apartment_after_ernst_fn(x) 
    { got ( Device  ): 
        ~ recordNewPinboardPhoto(DeviceRemovedFromCylinderPhoto)
    }
    
    { x: 
    -   ():     ~ return 1
    -   (LipstickHidingDevice, KosakovsDrop):  
            ~ return AnnieGivesInnerDeviceToContact
            // forward (!)
    -   ( DropNote, ManEnteringCarOutsideUNPhoto) :
            ~ return AnnieComesFromWork
            
    -   Warp:           
            ~ return Pinboard
    } 
    ~ return ()  
     
     
    
=== annie_in_car 
    LIST AnnieCarItems = Camera , (ManNearBlackCar) ,DropNote
    VAR AnnieCarInteractables = (ManNearBlackCar, BlackChanelBag, BlueChevy )
    -> scene ( AnnieCarItems + CarKey + BlueChevy + BlackChanelBag, AnnieCarInteractables, "I was always watching...") 
=== function annie_in_car_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   ManEnteringCarOutsideUNPhoto: 
            ~ return NoteInCar
    -   DropNote:   
            ~ return ApartmentBeforeErnst // forwards

    }
    ~ return () 


    

=== quentin_passes_note 

    ~ recordNewPinboardPhoto(ManEnteringCarOutsideUNPhoto)

    LIST OutsideUNBuilding = (BlackCar), QuentinsAide, (ERNameBadge), TwoThousandFrancs
    
    -> scene( OutsideUNBuilding, (BlackCar, QuentinsAide) ,  "Not every opportunity is worth taking, however well paid." ) 
    
=== function quentin_note_fn(x) 
    { x: 
    -   ():     ~ return 2
    -   (TwoThousandFrancs, DeadDropNoteFromQuentin):
            ~ return QGivesNoteToAide
    }
    ~ return () 





=== quentin_gives_aide_money 
  
    LIST QuentinGivesAideMoneyItems = (LockedDrawer) , (RumpledShirt) , KeyOnChain , MapOfParisMetro, LoyalAssurance
    -> scene ( QuentinGivesAideMoneyItems +  QuentinsAide + DeadDropNoteFromQuentin, (LockedDrawer, RumpledShirt, QuentinsAide), "Things often happen by circuitous routes.") 
=== function gives_aide_money_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   LoyalAssurance:    ~ return QGivesItemToErnst  
    }
    ~ return () 
    
    




    
=== item_from_quentin  
    LIST UNDeskJobItems =  (USFlag), (DeskPlate), Envelope, NoteFromQuentin 
    -> scene(UNDeskJobItems +  WeddingPhoto + QuentinsAide, ( QuentinsAide, Envelope),  "But it's hard to tell friends from enemies, sometimes.")   
=== function item_question_fn(x)  
    {x: 
    -   ():     ~ return 2
    -   (SealedMetalCylinder, NoteFromQuentin):  
                ~ return QGetsDevice
    }
    ~ return () 
 
 
 
 === king_clubs_steal_card
 
    LIST KingClubsStealCardItems = StolenCard 
    VAR KingClubsStealCardInteractables = (HandCards, Jacket, Croupier)
    -> scene ( KingClubsStealCardItems + Croupier + PileOfChips + Jacket, KingClubsStealCardInteractables, "I think Ernst died because he took something he shouldn't have taken.") 
 === function king_clubs_steal_card_fn(x)
 
    { x: 
    -   (): ~ return 1
TODO: forwards?!
    - (PlayingCard): 
        ~ return CardTableAtClub 
    - (QueenHearts): 
        ~ return Wedding 
    - (StolenCard):  
        ~ return Apartment 
    
    }
    ~ return () 
 
 
 
 === quentin_receives_metal_cylinder 
    LIST QuentinReceivesCylinderItems = (Wall), LooseBrick, SmallPackage, Toolbox, Screwdriver, Wrench, Pliers
    VAR QuentinReceivesCylinderInteracts = (SealedMetalCylinder)
    -> scene ( QuentinReceivesCylinderItems + SealedMetalCylinder, QuentinReceivesCylinderInteracts, "Remark") 
=== function q_receives_cylinder_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   Device: ~ return DeviceOperated
    -   Warp:   ~ return TopSceneID 
    }
      ~ return ()   
      TODO: how did quentin find out? 
 
 
 
 

/// GamblersAnonymous
=== gamblers_anonymous
    LIST GamblersItems = (Circle) , GroupSupport
    VAR GamblersInteractables = (Circle)
    -> scene ( GamblersItems, GamblersInteractables, "Ernst tried to get help, but somehow, it would never stick.") 
=== function gamblers_anonymous_fn(x) 
    { x: 
    -   (): ~ return 1 
// this doesn't make sense    
    - GroupSupport:     InBedWithErnstScene 
    }
    ~ return () 
 
 
 
    


/*


*/ 

 /*
    InBedWithErnst
 */
 
 
 
 
 
=== InBedWithErnst

    LIST InBedWithErnstItems = (SleepingErnst) , LovingMumble, (Pillow), Gun , HusbandsBody, HiddenGun
    VAR InBedWithErnstInteractables = (SleepingErnst, Pillow )
    
    -> scene ( InBedWithErnstItems, InBedWithErnstInteractables, "I did love him. I shouldn't have. But I did.") 
=== function InBedWithErnst_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   LovingMumble: ~ return DriveAfterWedding
    -   HiddenGun:  ~ return GoThroughWithWedding
    
    TODO: allow early murder (!)
    -   HusbandsBody:   ~ return ErnDiesEarlyScene 
    
             // forward, timline switch 

    }
    ~ return () 


 
 
=== wedding_drive_away 
   LIST WeddingCarItems = (BlueChevy), TinCanString
   -> scene( WeddingCarItems  + WeddingRing, (BlueChevy), "I had never been so happy as the day Ernst and I got married." ) 
   
 === function wedding_car_fn(x) 
     {x: 
    -   ():     ~ return 1 
    -  (WeddingRing, Dress):  ~ return Wedding
    }
    ~ return () 
 
 
=== wedding  
    LIST WeddingItems = (Dress), (Annie), (Quentin), OtherWeddingRing, Wife, WifesPromise
    VAR WeddingInteracts = (Wife, Annie, Quentin)
    -> scene(  WeddingItems + MoreFlowers + WeddingRing,  WeddingInteracts,  "That day is one I'd never take back." )  
    
=== function wedding_fn(x) 
    {x: 
    -   ():             ~ return 1 
    -   WifesPromise:   ~ return GoThroughWithWedding
TODO: a solve    
    }
    ~ return () 
 
 
 
=== go_through_with_wedding 
    LIST GoThroughWithItems = (ParkBench) , Lipstick
    VAR GoThroughWithInteractables = (Kosakov, BlackChanelBag)
    -> scene ( GoThroughWithItems + Kosakov + WeddingRing + BlackChanelBag, GoThroughWithInteractables, "Remark") 
    
=== function go_through_with_wedding_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   DeviceOperatedPhoto:    ~ return DeviceOperated
TODO: A solve 
    }
    ~ return ()  
    

    

 === device_operated 
    LIST DeviceOperatedItems =  Device, Warp 
    VAR DeviceOperatedInteractables = (SealedMetalCylinder, Device)
    -> scene ( DeviceOperatedItems + SealedMetalCylinder, DeviceOperatedInteractables, "Everything has to start somewhere.") 
    
=== function device_operated_fn(x) 
    { x: 
    -   ():     ~ return 1
    -   Warp:   ~ return TopSceneID
    }
    ~ return () 
    
    
    
    