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

LIST PinboardItems =  (ManilaEnvelope), MetalCylinderPhoto , DeviceStolenFromResearchLab , ManEnteringCarOutsideUNPhoto, ErnstRichardsDies

-> scene("Pinboard", "10th May 1968, 4:32pm", PinboardItems, ManilaEnvelope, -> pinboard_exit,  "Something's not right.")

=== function pinboard_exit(x)
    { x: 
    -   ():                 ~ return 1 
    -   ErnstRichardsDies:  ~ return -> graveyard 
    -   ManEnteringCarOutsideUNPhoto:  ~ return   -> quentin_passes_note
    -   DeviceStolenFromResearchLab:    ~ return -> device_stolen
    -   else:               ~ return -> NOPE 
    }

=== graveyard
/*
Gravestone - Ernst Richards
 Flowers 
 Flowers
 [ Wedding ring - inscribed "Annabel and Ernst October 1962"  ]
 */
LIST GraveyardItems =  WeddingRing, (BunchOfFlowers), (AnotherBunchOfFlowers), MoreFlowers, EvenMoreFlowers


-> scene("Graveyard new Rue Clemins", "29th April 1968, 11:07am", GraveyardItems, (BunchOfFlowers, AnotherBunchOfFlowers, EvenMoreFlowers), -> graveyard_fn,  "I know I loved him. I would have loved him all my life, if I could have.") 

=== function graveyard_fn(x) 
    { x: 
    -   ():                 ~ return 1 
    -   MoreFlowers:        ~ return -> wedding 
    -   WeddingRing:        ~ return -> mortuary
    -   else:               ~ return -> NOPE 
    }


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
 
LIST MortuaryTrayItems =  (PoliceNotes), SealedMetalCylinder, (Wallet), BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, MetroTicket, KingDiamondsCard

-> scene("Mortuary, 4th Quartier", "24th April 1968, 2:38pm", MortuaryTrayItems + WeddingRing,  Wallet, -> mortuary_fn, "But something is out of place.") 

=== function mortuary_fn(x) 
    { x:
    - ():                   ~ return 1 
    - MetroTicket:          ~ return -> metro_platform 
    - SealedMetalCylinder:  ~ return -> apartment 
    - KingDiamondsCard:     ~ return  -> back_of_kingdiamondsclub
    - else:                 ~ return -> NOPE 
    }



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


-> scene("Metro Platform, Champ de Mars", "23rd April 1968, 11:25pm", MetroPlatformItems , (Jacket, Wallet, Scarf), -> metro_platform_fn, "Something belongs... elsewhere. In other hands.") 

=== function metro_platform_fn(x) 
    {x: 
    -   ():     ~ return 2 
    -   (PianoWire, WhiteFabricScrap):  ~ return -> in_the_kitchens
    -   else:   ~ return -> NOPE 
    }

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


-> scene("Kitchens of the Hotel de la Tour", "23rd April 1968, 10:35pm", KitchenItems, (Bin, WhiteApron) , -> kitchen_fn,  "In bad hands, things go badly. It's that simple.") 

=== function kitchen_fn(x)  // 2 items 
    { x: 
    -   (): ~ return 2 
    -   (WaiterNameBadge, WhiteApron):      ~ return -> hotel_bathroom
    -   (WaiterNameBadge, EmptyGlassVial): ~ return -> hotel_bathroom
    -   (CoffeeOnTray, EmptyGlassVial):     ~ return -> hotel_bathroom
    -   (CoffeeSpoon, EmptyGlassVial):      ~ return -> hotel_bathroom
    -   else:   ~ return -> NOPE 
    } 
    
    
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

-> scene("Lobby Bathroom, Hotel de la Tour", "23rd April 1968, 10:28pm", HotelBathroomItems , (UnconciousWaiter, BlackKitBag, BlackLeatherFolder) , -> bathroom_fn,  "I think that's why Ernst died. Someone... gave him... something.") 

=== function bathroom_fn (x)
    { x: 
    - ():   ~ return 1
    - UnconciousWaiter:         ~ return -> back_alleyway
    -   CasinoChips:            ~ return -> back_of_kingdiamondsclub
    -   PhotoOfErnst:           ~ return -> kingdiamondsclub
    - else:                     ~ return -> NOPE 
    }
    
  

=== back_alleyway
 /*
Back alleyway, door marked "KITCHENS" 

Box labelled: "Claude. Rat poison - DO NOT OPEN."
 Glass vial 
 Photograph of the dead guy with name: "Ernst Richards" 
 [ Two black casino chips from the KING OF DIAMONDS nightclub ]
 */
 
LIST HotelAlleywayItems = (DoorLock), PhotoOfErnst,  CasinoChips, BrokenDoorLock, (Waiter), WaiterHandsUp



-> scene("Alleyway Behind l'Hotel de l'Opera", "23rd April 1968, 10:25pm", HotelAlleywayItems + BlackLeatherFolder + BlackKitBag, (BlackLeatherFolder, BlackKitBag, DoorLock, Waiter),  -> alleyway_fn,   "My husband wasn't perfect. Just another poor unfortunate who got in the way.") 

=== function alleyway_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   CasinoChips:            ~ return -> back_of_kingdiamondsclub
    -   PhotoOfErnst:           ~ return -> kingdiamondsclub
    -   else:                   ~ return -> NOPE 
    }



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

-> scene("Backroom, King of Diamonds Club", "23rd April 1968, 9:18pm", KingDiamondsBoxItems + AceSpades, (MetalLockBox, PileOfChips, EvenMoreChips, AceSpades), -> back_club_fn,   "Because of something buried...")  

=== function back_club_fn(x) 
    {x: 
    -   ():     ~ return 1 
    -   (ValetReceipt, AceSpades):   ~ return -> kingdiamondsclub 
    -   else:   ~ return -> NOPE  
    }
    
    
  
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
  
-> scene("The Table, King of Diamonds Club", "23rd April 1968, 6:25pm", KingDiamondsClubItems, cards + Croupier, -> king_diamond_club_fn,  "Because someone tried to change things. To turn them to their own advantage. Only the house always wins.") 

=== function king_diamond_club_fn(x) 
    {x: 
    -   ():     ~ return 1 
    -   AceSpadesReversed:  ~ return -> apartment 
    -   else:   ~ return -> NOPE 
    }


=== apartment

LIST ApartmentItems = (WallSafe), (DeadDropNoteFromQuentin), (WeddingPhoto), (KeyHook), CarKey

-> scene("Apartment, Rue de Chavelier", "23rd April 1968, 4:23pm", ApartmentItems + WeddingRing, (WallSafe, KeyHook),  -> apartment_fn, "I told him not to go. But he wouldn't listen. He said this time it would be different.")  

=== function apartment_fn(x) 
    
    {x: 
    - ():   ~ return 2 
    TODO: some way to use the ( AceSpades ): 
    - ( DeadDropNoteFromQuentin, TwoThousandFrancs ): 
        ~ return -> quentin_passes_note 
    - ( DeadDropNoteFromQuentin, SealedMetalCylinder ): 
        ~ return -> quentin_passes_note     
    - ( WeddingPhoto,  CarKey ) : 
        ~ return -> wedding_drive_away
    - ( WeddingRing,  CarKey ) : 
        ~ return -> wedding_drive_away    
    - ( WeddingPhoto,  WeddingRing) : 
        ~ return -> wedding 
    - else: 
        ~ return -> NOPE 
    }
    
    
    

=== quentin_passes_note 

    LIST OutsideUNBuilding = (BlackCar), QuentinsAide, (ERNameBadge), TwoThousandFrancs
    
    -> scene("Outside the UN Building", "23rd April 1968, 3:15pm", OutsideUNBuilding, (BlackCar, QuentinsAide) , -> quentin_note_fn , "Not every opportunity is worth taking, however well paid." ) 
    
=== function quentin_note_fn(x) 
    { x: 
    -   ():     ~ return 1 
    -   DeadDropNoteFromQuentin: ~ return -> item_from_quentin 
    -   TwoThousandFrancs: ~ return -> quentin_gives_aide_money
    -   else:   ~ return -> NOPE 
    }





=== quentin_gives_aide_money 
  
    LIST QuentinGivesAideMoneyItems = (LockedDrawer) , (RumpledShirt) , KeyOnChain , MapOfParisMetro, LoyalAssurance
    -> scene ("Dirty Office above Champs de Mars", "23rd April 1968, 2:29pm", QuentinGivesAideMoneyItems +  QuentinsAide, (LockedDrawer, RumpledShirt, QuentinsAide), -> gives_aide_money_fn, "Things often happen by circuitous routes.") 
=== function gives_aide_money_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   LoyalAssurance:    ~ return -> item_from_quentin    
    -   else:   ~ return -> NOPE 
    }
    
    


    
=== item_from_quentin  
    LIST UNDeskJobItems =  (USFlag), (DeskPlate), (UNBin), Envelope, NoteFromQuentin
    -> scene("The UN Building", "7th August 1967", UNDeskJobItems + ERNameBadge + WeddingPhoto, (UNBin, DeskPlate, Envelope), -> item_question_fn,  "But it's hard to tell friends from enemies, sometimes.")   
=== function item_question_fn(x)  
    {x: 
    -   ():     ~ return 1 
    -   WeddingPhoto:   ~ return -> wedding 
    -   SealedMetalCylinder:    ~ return -> quentin_receives_metal_cylinder
    -  else:    ~ return -> NOPE 
    }
 
 
 
 
 === quentin_receives_metal_cylinder 
    LIST QuentinReceivesCylinderItems = (NoItem) 
    -> scene ("TemplateLocation", "TemplateData, TemplateTime", QuentinReceivesCylinderItems, (), -> q_receives_cylinder_fn, "Remark") 
=== function q_receives_cylinder_fn(x) 
    { x: 
    -   (): ~ return 1 
TODO: A solve 
    -   else:   ~ return -> NOPE 
    }
 
 
=== wedding_drive_away 
   LIST WeddingCarItems = (BlueChevy), TinCanString
   -> scene("Leaving the Chapel St Jean" ,"3rd Oct 1962, 10:35pm", WeddingCarItems + Dress + WeddingRing, (BlueChevy), -> wedding_car_fn, "I had never been so happy as the day we got married." ) 
   
 === function wedding_car_fn(x) 
     {x: 
    -   ():     ~ return 1 
    -  (WeddingRing, Dress):  ~ return -> wedding 
    -  else:    ~ return -> NOPE 
    }
 
=== wedding  
    LIST WeddingItems = (Dress)
    -> scene("The Chapel St Jean, Montpellier", "3rd Oct 1962, 1:18pm",  WeddingItems + MoreFlowers + WeddingRing,  (), -> wedding_fn,  "I thought we'd be together forever." )  
    
=== function wedding_fn(x) 
    {x: 
    -   ():     ~ return 1 
TODO: a solve    
    -  else:    ~ return -> NOPE 
    }
 
 
 
 
 
 /*
    Stolen Device 
 */
 === device_stolen
    LIST SmashedResearchLabItems = (ResearchCabinet) 
    -> scene ("Research Lab, Area 51", "April 1962, 3:14pm", SmashedResearchLabItems, (), -> device_stolen_fn, "Someone took something too precious to be allowed as stolen.") 
=== function device_stolen_fn(x) 
    { x: 
    -   (): ~ return 1 
TODO: A solve 
    -   else:   ~ return -> NOPE 
    }
    
    
    
 
 
 /*
    TEMPLATE
 */
=== template 
    LIST TemplateItems = (NoItem) 
    -> scene ("TemplateLocation", "TemplateDate, TemplateTime", TemplateItems, (), -> template_fn, "Remark") 
=== function template_fn(x) 
    { x: 
    -   (): ~ return 1 
TODO: A solve 
    -   else:   ~ return -> NOPE 
    }