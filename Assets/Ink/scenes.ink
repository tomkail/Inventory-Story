
=== pinboard
/*

Photographs on pinboard in background 
Manila envelope - "ER surveillance"
 Car outside UN building - labelled ER arrives 19th April 
 Man in shades walking in airport - labelled ER visits Puerto Rico, retrieves the device
 [ Gravestone - unlabelled ]
 A metal cylinder - labelled whereabouts of device unknown
 
*/

LIST PinboardItems =  (ManilaEnvelope), ManEnteringCarOutsideUNPhoto, ManInAirportPhoto, MetalCylinderPhoto  

-> scene("Pinboard", "10th May 1968", PinboardItems, ManilaEnvelope, (MetalCylinderPhoto), -> timeline_board, "Something's not right.") 



=== timeline_board
/*

*/


LIST TimelineItems = (Timeline), Inception, DeviceStolenFromResearchLab, ErnstRichardsDies

-> scene("Hopburg-Steiner Device Timeline", "10th May 1968", TimelineItems, Timeline, (ErnstRichardsDies), -> graveyard, "It's hard to be sure. There's a lot we don't know.")

=== graveyard
/*
Gravestone - Ernst Richards
 Flowers 
 Flowers
 [ Wedding ring - inscribed "Annabel and Ernst October 1962"  ]
 */
LIST GraveyardItems =  WeddingRing, (BunchOfFlowers), (AnotherBunchOfFlowers)


-> scene("Graveyard new Rue Clemins", "29th April 1968", GraveyardItems, (BunchOfFlowers), (WeddingRing), -> mortuary, "I know I loved him. I would have loved him all my life, if I could have.") 

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
 
LIST MortuaryTrayItems =  (PoliceNotes), SealedMetalCylinder, (Wallet), BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, DupontMetroPass, KingDiamondsCard

-> scene("Mortuary, 4th Quartier", "24th April 1968", MortuaryTrayItems + WeddingRing,  Wallet, (DupontMetroPass), -> metro_platform, "Something is out of place.") 




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


-> scene("Metro Platform, Champ de Mars", "4:14pm, 23rd April 1968", MetroPlatformItems , (Jacket, Wallet, Scarf), (PianoWire, WhiteFabricScrap, DupontMetroPass), -> in_the_kitchens, "Something belongs... elsewhere. In other hands.")



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

LIST KitchenItems =  CoffeeOrderSlip, (WaiterNameBadge), (WhiteApron), (Bin), FoodPeelings, EmptyGlassVial


-> scene("Kitchens of the Hotel de la Tour", "23rd April 1968", KitchenItems, (Bin, WhiteApron) , (EmptyGlassVial, PianoWire),  -> hotel_bathroom, "In bad hands, things go badly. It's that simple.") 


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

LIST HotelBathroomItems = (UnconciousWaiter), CardboardBox, (BlackKitBag) , BlackVelvetBag, GlassVialOfPowder, ChloroformBottle, FlickKnife, SmallGun, Cigarettes

-> scene("Lobby Bathroom, Hotel de la Tour", "23rd April 1968", HotelBathroomItems , (UnconciousWaiter, BlackKitBag, BlackVelvetBag) , (CardboardBox),  -> back_alleyway, "I think that's why Ernst died. Someone... gave him... something.") 


=== back_alleyway
 /*
Back alleyway. Cardboard box by door marked "KITCHENS" 

Box labelled: "Claude. Rat poison - DO NOT OPEN."
 Glass vial 
 Ten thousand franc note 
 Photograph of the dead guy with name: "Ernst Richards" 
 Photograph of a sealed metal cylinder device
 [ Two black casino chips from the KING OF DIAMONDS nightclub ]
 */
 
LIST HotelAlleywayItems = (BrokenLock), PhotoOfErnst, PhotoOfCylindricalDevice, CasinoChips



-> scene("Alleyway Behind Hotel de Opera", "23rd April 1968", HotelAlleywayItems + CardboardBox + BlackKitBag, (CardboardBox, BlackKitBag),  (CasinoChips), -> back_of_kingdiamondsclub, "I want to know where it came from.") 

  
   
  
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

-> scene("Backroom, King of Diamonds Club", "12th April 1968", KingDiamondsBoxItems, (MetalLockBox, PileOfChips, EvenMoreChips), (ValetReceipt), -> kingdiamondsclub, "Because of something buried...")  

    
    
    
  
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

LIST KingDiamondsClubItems = CarKey, (HandCards), AceHearts, ThreeClubs, SevenHearts, AceSpades, (ValetReceipt), AceHeartsReversed, ThreeClubsReversed, SevenHeartsReversed, AceSpadesReversed, PlayingCardReversed, PlayingCard, ( GinAndTonic )

VAR useables = (HandCards, AceHeartsReversed, AceHearts, ThreeClubsReversed, ThreeClubs, SevenHeartsReversed, SevenHearts, AceSpadesReversed, AceSpades)
  
-> scene("The Table, King of Diamonds Club", "12th April 1968", KingDiamondsClubItems, useables, (AceSpadesReversed), -> apartment, "Because someone tried to change things. To turn them to their own advantage. Only the house always wins.")


=== apartment
    
    LIST ApartmentItems = (CarKey), (WallSafe), TwoThousandFrancs, NoteFromQuentin
    
    -> scene("Apartment, Rue de Chavelier", "12th April 1968", ApartmentItems, WallSafe, (NoteFromQuentin), -> final, "I told him not to go. But he wouldn't listen. He said this time it would be different.")  
    

    
    
    
    
 
