
VAR levelItems = ()
VAR levelSolutionItems = () 
VAR currentItems = () 

CONST DEBUG = true 

VAR next = -> pinboard

-> metro_platform

-> pinboard 


=== function getItemName(item)
    {item: 
    -   ManilaEnvelope:           manila envelope
    -   BunchOfFlowers:             lilies 
    -   AnotherBunchOfFlowers:      tulips
    -   CardboardBox:   {not back_alleyway:empty} cardboard box
    -   WhiteApron: {not hotel_bathroom:stained} white apron
    - else:         {item} 
    }

=== function getItemTooltip(item) 
    {item: 
    -   WhiteFabricScrap:   Torn, and slightly stained.
    -   PoliceNotes:             "Suspected attempted theft. Killer was disturbed and escaped down tunnel. Evidence of drugs."   
    -   BusinessCard:       "Ernst Richards, office clerk, UN."
    -   OtherBusinessCard:  "Bolera Taxis." 
    -   OtherOtherBusinessCard:  "Gamblers Anonymous. DON'T GET LUCKY GET HELP."
    -   WeddingRing:        "Inscribed 'Annabel and Ernst October 1962'"
    -   ManilaEnvelope:     "ER surveillance"  
    -   CarOutsideUNPhoto:  "ER, 19th April 68"
    -   ManInAirportPhoto:  "Puerto Rico, 26th April 68"
    -   MetalCylinderPhoto: "Device, taken 3rd April 68 by COBRA"
    -   GravestonePhoto:    "2nd May 68"
    -   PianoWire:      {in_the_kitchens:
                            It's coiled and clean.
                        - else:
                            It's blood-soaked.
                        }
    - CardboardBox:     "Claude. Rat poison - DO NOT OPEN."
    - DupontMetroPass:  Metro Pass: C. DUPONT
    - PhotoOfErnst:     "Ernst Richards. 33.y.o."
    - PhotoOfCylindricalDevice:  "LOCATE"
    - CasinoChips:      KING OF DIAMONDS nightclub
    - AceSpadesReversed:        The back of this card is slightly different from the others.
    - ValetReceipt:         Parking receipt for a Blue Chevy, registered to Ernst Richards
    }
    ~ return
    
=== function itemGeneratesItems(item) 
    {item: 
    - ManilaEnvelope: ~ return (CarOutsideUNPhoto, ManInAirportPhoto, GravestonePhoto, MetalCylinderPhoto)
    
    - Wallet: ~ return (BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, PlayingCard, SealedMetalCylinder)
    - Scarf: ~ return PianoWire
    - Bin: ~ return (FoodPeelings, EmptyGlassVial)
    - WhiteApron: ~ return (TenThousandFrancs, CoffeeOrderSlip)
    - UnconciousWaiter: ~ return (WhiteApron, WaiterNameBadge)
    - BlackKitBag: ~ return (PianoWire, TenThousandFrancs, BlackVelvetBag, FlickKnife, SmallGun, Cigarettes, DupontMetroPass, CardboardBox)
    - BlackVelvetBag: ~ return (GlassVialOfPowder, ChloroformBottle)
    - CardboardBox: 
        ~ return (GlassVialOfPowder, TenThousandFrancs, PhotoOfErnst, PhotoOfCylindricalDevice, CasinoChips)
    - HandCards: ~ return (AceHearts, ThreeClubs, SevenHearts, AceSpades, PlayingCard)
    - AceHeartsReversed:    ~ return AceHearts 
    - ThreeClubsReversed:   ~ return ThreeClubs
    - SevenHeartsReversed:  ~ return SevenHearts
    - AceSpadesReversed:    ~ return AceSpades
    - AceHearts:            ~ return AceHeartsReversed
    - ThreeClubs:           ~ return ThreeClubsReversed
    - SevenHearts:          ~ return SevenHeartsReversed
    - AceSpades:            ~ return AceSpadesReversed
    - PlayingCard:          ~ return PlayingCardReversed
    - PlayingCardReversed:  ~ return PlayingCard
    - MetalLockBox:         ~ return PileOfChips
    - PileOfChips:          ~ return EvenMoreChips 
    - EvenMoreChips:          ~ return (EvenEvenMoreChips , ValetReceipt)
    - Jacket:               ~ return (Wallet)
    - else: ERROR: {item} has no generator list 
    
    }


=== use(item, withDestruction, -> done) 
    -> _use(item, (), withDestruction) -> done
=== use_with(withItem, onItem, withDestruction, -> done) 
    -> _use(onItem, withItem, withDestruction) -> done 

    
=== _use(item, withItem, withDestruction)     
    ~ temp toGenerate = itemGeneratesItems(item)
    +   { levelItems !? toGenerate} { levelItems ? item } { levelItems ? withItem || not withItem }
        [ {item} {withItem: - {withItem} } ]
        ~ addItems(toGenerate) 
        { withDestruction:
            ~ removeItem(item) 
        }
        [ now {levelItems} ]
        ->-> 


=== pinboard
/*

Photographs on pinboard in background 
Manila envelope - "ER surveillance"
 Car outside UN building - labelled ER arrives 19th April 
 Man in shades walking in airport - labelled ER visits Puerto Rico, retrieves the device
 [ Gravestone - unlabelled ]
 A metal cylinder - labelled whereabouts of device unknown
 
*/

LIST PinboardItems =  (ManilaEnvelope), CarOutsideUNPhoto, ManInAirportPhoto, GravestonePhoto, MetalCylinderPhoto 

-> scene("Pinboard", "10th May 1968", PinboardItems, (GravestonePhoto), -> graveyard) -> 


VO:     Something's not right.

- (opts)
<- offer(levelItems, -> opts) 
-> use(ManilaEnvelope, true, -> opts )
    




=== graveyard
/*
Gravestone - Ernst Richards
 Flowers 
 Flowers
 [ Wedding ring - inscribed "Annabel and Ernst October 1962"  ]
 */
LIST GraveyardItems =  (WeddingRing), (BunchOfFlowers), (AnotherBunchOfFlowers)

VO:     I don't know how I know. It's just a feeling.

-> scene("Graveyard new Rue Clemins", "29th April 1968", GraveyardItems, (WeddingRing), -> mortuary) -> 

- (opts)
-> offer(levelItems, -> opts) 


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
 
LIST MortuaryTrayItems =  (PoliceNotes), SealedMetalCylinder, (Wallet), BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, PlayingCard

VO:     But something is definitely very wrong.

-> scene("Mortuary, 4th Quartier", "24th April 1968", MortuaryTrayItems + WeddingRing, (PlayingCard, SealedMetalCylinder), -> metro_platform) -> 

- (opts)
<- offer(levelItems, -> opts) 
-> use(Wallet, false , -> opts)




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


-> scene("Metro Platform, Montpellier Station", "23rd April 1968", MetroPlatformItems , (PianoWire, WhiteFabricScrap), -> in_the_kitchens) -> 

VO:     I'm not sure exactly when it started. 

- (opts)
    <-  offer(levelItems, -> opts) 
    <- use(Jacket, false, -> opts) 
    <- use(Wallet, false , -> opts)
    -> use(Scarf, false, -> opts)



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

LIST KitchenItems =  CoffeeOrderSlip, (WaiterNameBadge), (WhiteApron), (Bin), FoodPeelings, EmptyGlassVial, TenThousandFrancs


-> scene("Kitchens of the Hotel de Opera", "23rd April 1968", KitchenItems, (EmptyGlassVial, TenThousandFrancs),  -> hotel_bathroom) -> 

VO:     Certainly it was before Ernst died. 


- (opts)
    <- offer(levelItems, -> opts) 
    <- use(Bin, false, -> opts) 
    -> use(WhiteApron, false, -> opts) 



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

LIST HotelBathroomItems = (UnconciousWaiter), CardboardBox, (BlackKitBag) , BlackVelvetBag, GlassVialOfPowder, ChloroformBottle, FlickKnife, SmallGun, Cigarettes, DupontMetroPass

-> scene("Lobby Bathroom, Hotel de Opera", "23rd April 1968", HotelBathroomItems, (CardboardBox),  -> back_alleyway) -> 
- (opts) 
    <- offer(levelItems, -> opts) 
    <- use(UnconciousWaiter, false, -> opts) 
    <- use(BlackKitBag, false, -> opts )
    -> use(BlackVelvetBag, false, -> opts )


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

-> scene("Alleyway Behind Hotel de Opera", "23rd April 1968", HotelAlleywayItems + CardboardBox + BlackKitBag, (CasinoChips), -> back_of_kingdiamondsclub) ->

- (opts) 
    <- use(BlackKitBag, false, -> opts)
    <- use_with(FlickKnife, CardboardBox, false, -> opts) 
    -> offer(levelItems, -> opts) 
  
  
  
   
  
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

-> scene("Backroom, King of Diamonds Club", "12th April 1968", KingDiamondsBoxItems, (ValetReceipt), -> kingdiamondsclub)  -> 

- (opts)  
    
    <- use(PileOfChips, true, -> opts)
    <- use(EvenMoreChips, true, -> opts)
    <- use_with(KingKey, MetalLockBox, false, -> opts)
    -> offer(levelItems, -> opts) 
    
  
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

LIST KingDiamondsClubItems = CarKey, (HandCards), AceHearts, ThreeClubs, SevenHearts, AceSpades, (ValetReceipt), AceHeartsReversed, ThreeClubsReversed, SevenHeartsReversed, AceSpadesReversed, PlayingCardReversed

-> scene("The Table, King of Diamonds Club", "12th April 1968", KingDiamondsClubItems, (AceSpadesReversed), -> final)  -> 

- (opts) 
    <- use(HandCards, true, -> opts)
    <- use(AceHeartsReversed, true, -> opts) 
    <- use(AceHearts, true, -> opts) 
    
    <- use(ThreeClubsReversed, true, -> opts) 
    <- use(ThreeClubs, true, -> opts) 
    
    <- use(SevenHeartsReversed, true, -> opts) 
    <- use(SevenHearts, true, -> opts) 
    
    <- use(AceSpadesReversed, true, -> opts) 
    <- use(AceSpades, true, -> opts) 
    
    -> offer(levelItems, -> opts) 
 
=== final 
    -> END


=== scene(title, date, items, success, -> toNext)
    >>> Scene {title} 
    [ {title} / { date } ]
    ~ levelItems = items 
    ~ levelSolutionItems = success
    ~ currentItems = () 
    ~ next = toNext
    ->->
    
=== function addItems(items) 
    ~ levelItems += items
    
=== function removeItem(items)
    ~ levelItems -= items

=== function require(item) 
    ~ return levelItems !? item

=== offer(items, -> backto)
    {not DEBUG: 
        -> ingame
    }
    [ {LIST_COUNT(currentItems)} / {LIST_COUNT(levelSolutionItems)} ]
- (opts)    
    ~ temp item = pop(items) 
    { item: 
        <- slot(item, backto) 
    }
    {items: 
        -> opts
    }
    -> DONE
= ingame 
    +   [ SOLVED ] 
        -> next 
= slot(item, -> backto) 
    +   { currentItems  ? item } 
        [  UNSLOT {getItemName(item)} ]
        
    +   { currentItems  !? item } 
        [  SLOT {getItemName(item)} - {getItemTooltip(item)}]
        ~ currentItems += item 
        { currentItems == levelSolutionItems:
            -> next
        }
    -   -> backto 

=== function got(item) 
    ~ return levelItems ? item

=== function pop(ref _list) 
    ~ temp el = LIST_MIN(_list) 
    ~ _list -= el
    ~ return el 



 


