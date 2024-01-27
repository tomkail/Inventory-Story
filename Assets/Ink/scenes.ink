
-> proceedTo(TopSceneID)

LIST NoItemList = NoItem 

LIST Scenes = 
Pinboard, 
Graveyard, 
Mortuary, 
MetroPlatform, 
HotelKitchens,
HotelBathroom,
BackAlleyway,
BackOfClub, 
CardTableAtClub, 
Apartment, 
NoteInCar,
QGivesNoteToAide, 
QGivesItemToErnst, 
QGetsDevice,
DeviceRemovedFromCylinder, // TODO!
DriveAfterWedding,
Wedding,
DeviceOperated,
__Template




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

LIST PinboardItems =  (ManilaEnvelope), MetalCylinderPhoto , DeviceStolenFromResearchLab , DeviceOperatedPhoto , ErnstRichardsDies, ManEnteringCarOutsideUNPhoto

VAR PhotosInOpeningEnvelope = (MetalCylinderPhoto  , DeviceOperatedPhoto, ErnstRichardsDies)
TODO: add "checkpoints" in flow that drop in here 
TODO: reset at the end of run doesn't reset ink but just bounces. 

// allow in ManEnteringCarOutsideUNPhoto / DeviceStolenFromResearchLab if you've been there

-> scene( PinboardItems, ManilaEnvelope,  "Something's not right.")

=== function pinboard_exit(x)
    { x: 
    -   ():                 ~ return 1 
    -   ErnstRichardsDies:  ~ return Graveyard 
    -   ManEnteringCarOutsideUNPhoto:  ~ return  NoteInCar
    -   DeviceOperatedPhoto:    ~ return DeviceOperated
    }
    ~ return () 

=== graveyard
/*
Gravestone - Ernst Richards
 Flowers 
 Flowers
 [ Wedding ring - inscribed "Annabel and Ernst October 1962"  ]
 */
LIST GraveyardItems =  (Gravestone), WeddingRing, BunchOfFlowers, AnotherBunchOfFlowers, MoreFlowers, EvenMoreFlowers
VAR GraveyardInteractables = (Gravestone, BunchOfFlowers, AnotherBunchOfFlowers, EvenMoreFlowers)

-> scene( GraveyardItems, GraveyardInteractables,  "I know I loved him. I would have loved him all my life, if I could have.") 

=== function graveyard_fn(x) 
    { x: 
    -   ():                 ~ return 1 
    -   MoreFlowers:        ~ return Wedding 
    -   WeddingRing:        ~ return Mortuary
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
 
LIST MortuaryTrayItems =  (PoliceNotes), SealedMetalCylinder, (Wallet), BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, MetroTicket, KingDiamondsCard, Nothing

-> scene( MortuaryTrayItems + WeddingRing,  Wallet, "But something is out of place.") 

=== function mortuary_fn(x) 
    { x:
    - ():                   ~ return 1 
    - MetroTicket:          ~ return    MetroPlatform
    - SealedMetalCylinder:  ~ return    MetroPlatform
    - WeddingRing:          ~ return    Wedding
    - KingDiamondsCard:     ~ return    BackOfClub
    }
    ~ return () 



=== metro_platform
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


-> scene( KitchenItems, (Bin, WhiteApron) ,  "In bad hands, things go badly. It's that simple.") 

=== function kitchen_fn(x)  // 2 items 
    { x: 
    -   (): ~ return 2 
    -   (WaiterNameBadge, WhiteApron):      ~ return HotelBathroom
    -   (WaiterNameBadge, EmptyGlassVial): ~ return HotelBathroom
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
    - ():   ~ return 1
    - UnconciousWaiter:         ~ return BackAlleyway
    -   CasinoChips:            ~ return  BackOfClub
    -   PhotoOfErnst:           ~ return CardTableAtClub
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
    -   (): ~ return 1 
    -   CasinoChips:            ~ return BackOfClub
    -   PhotoOfErnst:           ~ return CardTableAtClub
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

LIST KingDiamondsClubItems =  (HandCards), AceHearts, ThreeClubs, SevenHearts, AceSpades, (ValetReceipt), AceHeartsReversed, ThreeClubsReversed, SevenHeartsReversed, AceSpadesReversed, PlayingCardReversed, PlayingCard, ( GinAndTonic ), (Croupier) 

VAR cards = (HandCards, AceHeartsReversed, AceHearts, ThreeClubsReversed, ThreeClubs, SevenHeartsReversed, SevenHearts, AceSpadesReversed, AceSpades)
  
-> scene(KingDiamondsClubItems, cards + Croupier, "Because someone tried to change things. To turn them to their own advantage. Only the house always wins.") 

=== function king_diamond_club_fn(x) 
    {x: 
    -   ():     ~ return 1 
    -   AceSpadesReversed:  ~ return Apartment 
    }
    ~ return () 


=== apartment

LIST ApartmentItems = (WallSafe), (DeadDropNoteFromQuentin), (WeddingPhoto), (KeyHook), CarKey

-> scene( ApartmentItems + WeddingRing, (WallSafe, KeyHook),   "I told him not to go. But he wouldn't listen. He said this time it would be different.")  

=== function apartment_fn(x) 
    
    {x: 
    - ():   ~ return 2 
    TODO: some way to use the ( AceSpades ): 
    - ( DeadDropNoteFromQuentin, TwoThousandFrancs ): 
        ~ return NoteInCar 
    - ( DeadDropNoteFromQuentin, SealedMetalCylinder ): 
        ~ return NoteInCar     
    - ( WeddingPhoto,  CarKey ) : 
        ~ return DriveAfterWedding
    - ( WeddingRing,  CarKey ) : 
        ~ return DriveAfterWedding  
    - ( WeddingPhoto,  WeddingRing) : 
        ~ return Wedding 
    }
    ~ return () 
    
    
    

=== quentin_passes_note 

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
 
 
 === quentin_receives_metal_cylinder 
    LIST QuentinReceivesCylinderItems = (Wall), LooseBrick, SmallPackage, Toolbox, Screwdriver, Wrench, Pliers
    VAR QuentinReceivesCylinderInteracts = (SealedMetalCylinder)
    -> scene ( QuentinReceivesCylinderItems + SealedMetalCylinder, QuentinReceivesCylinderInteracts, "Remark") 
=== function q_receives_cylinder_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   Nothing: ~ return DeviceRemovedFromCylinder
    }
      ~ return ()   
 
 
 
 
=== device_removed 
    LIST DeviceRemovedItems =  (ElectricLamp) 
    VAR DeviceRemovedInteractables = (ElectricLamp, SealedMetalCylinder, Device)
    -> scene ( DeviceRemovedItems + SealedMetalCylinder, DeviceRemovedInteractables, "Everything has to start somewhere.") 
=== function device_removed_fn(x) 
    { x: 
    -   (): ~ return 1
    -   Warp:   ~ return Pinboard
    }
    ~ return () 
  
 
 
=== wedding_drive_away 
   LIST WeddingCarItems = (BlueChevy), TinCanString
   -> scene( WeddingCarItems + Dress + WeddingRing, (BlueChevy), "I had never been so happy as the day we got married." ) 
   
 === function wedding_car_fn(x) 
     {x: 
    -   ():     ~ return 1 
    -  (WeddingRing, Dress):  ~ return Wedding
    }
    ~ return () 
 
 
=== wedding  
    LIST WeddingItems = (Dress)
    -> scene(  WeddingItems + MoreFlowers + WeddingRing,  (),  "I thought we'd be together forever." )  
    
=== function wedding_fn(x) 
    {x: 
    -   ():     ~ return 1 
TODO: a solve    
    }
    ~ return () 
 
 
 
 
  
 === device_operated 
    LIST DeviceOperatedItems =  Device, Warp 
    VAR DeviceOperatedInteractables = (SealedMetalCylinder, Device)
    -> scene ( DeviceOperatedItems + SealedMetalCylinder, DeviceOperatedInteractables, "Everything has to start somewhere.") 
=== function device_operated_fn(x) 
    { x: 
    -   ():     ~ return 1
    -   Warp:   ~ return Pinboard
    }
    ~ return () 
    
    
 
 /*
    TEMPLATE
 */
=== template 
    LIST TemplateItems = (TemplateItem) 
    VAR TemplateInteractables = (TemplateItem)
    -> scene ( TemplateItems, TemplateInteractables, "Remark") 
=== function template_fn(x) 
    { x: 
    -   (): ~ return 1 
TODO: A solve 
    }
    ~ return () 