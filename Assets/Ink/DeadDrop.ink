
VAR levelItems = ()
VAR levelSolutionItems = () 
VAR currentItems = () 

CONST DEBUG = false 

VAR next = -> pinboard



-> pinboard 


=== function getItemName(item)
    {item: 
    - ManilaEnvelope:   {pinboard.envelope:torn} manila envelope
    -   BunchOfFlowers:             lilies 
    -   AnotherBunchOfFlowers:      tulips
    - else:         {item} 
    }

=== function getItemTooltip(item) 
    {item: 
    -   WhiteFabricScrap:   Torn, and slightly stained.
    -   IDCard:             "Suspected attempted theft. Killer was disturbed and escaped down tunnel. Evidence of drugs."   
    -   BusinessCard:       "Ernst Richards, office clerk, UN."
    -   OtherBusinessCard:  "Bolera Taxis." 
    -   OtherOtherBusinessCard:  "Gamblers Anonymous. DON'T GET LUCKY GET HELP."
    -   WeddingRing:        "Inscribed 'Annabel and Ernst October 1962'"
    -   ManilaEnvelope:     "ER surveillance"  
    -   CarOutsideUNPhoto:  "ER, 19th April 68"
    -   ManInAirportPhoto:  "Puerto Rico, 26th April 68"
    -   MetalCylinderPhoto: "Device, taken 3rd April 68 by COBRA"
    -   GravestonePhoto:    "2nd May 68"
    
    }
    ~ return


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
*   (envelope) [ {ManilaEnvelope} ]
    VO:     The envelope tears open.
    ~ addItems((CarOutsideUNPhoto, ManInAirportPhoto, GravestonePhoto, MetalCylinderPhoto))
    
-   ->  opts


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
 
LIST MortuaryTrayItems =  (IDCard), (KeyFob), YaleKey, BrassKey, SealedMetalCylinder, (Wallet), BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, PlayingCard

VO:     But something is definitely very wrong.

-> scene("Mortuary, 4th Quartier", "24th April 1968", MortuaryTrayItems + WeddingRing, (PlayingCard, SealedMetalCylinder), -> metro_platform) -> 

- (opts)
<-  offer(levelItems, -> opts) 
-> keys_wallet(-> opts)

== keys_wallet(-> goto)
+   {require(SealedMetalCylinder)} [{ KeyFob} ]
    ~ addItems((YaleKey, BrassKey, SealedMetalCylinder))
    
+  {require(PlayingCard)} [{ Wallet} ]
    ~ addItems((BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, PlayingCard))

-    -> goto

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
<- keys_wallet(-> opts)
*   [ {Scarf} ] 
    ~ addItems(PianoWire)
-   -> opts 



=== in_the_kitchens

VO:     Certainly it was before Ernst died. 

    -> END 


/*



 
Kitchens of Hotel. A waiter heads towards the door. 
 Order slip: Coffee, table 15. 
 White Apron 
  [ Roll of piano wire ]
  [ Ten thousand franc note ]
 [ Glass vial of powder ]
 Coffee cup 
 Tray 
 
 
 
Black bag - bathroom of hotel 

Bag
 Apron 
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



Back alleyway. Cardboard box by door marked "KITCHENS" 

Box labelled: "Claude. Rat poison - DO NOT OPEN."
 Glass vial 
 Ten thousand franc note 
 Photograph of the dead guy with name: "Ernst Richards" 
 Photograph of a sealed metal cylinder device
 [ Two black casino chips from the KING OF DIAMONDS nightclub ]
 
  
  
Poker Table 

Pile of KING OF DIAMONDS chips 

Hand of cards
  Ace of Hearts 
  Three Clubs 
  [ Ace of Spades [ the back of this card is very slightly different ] ]
  Three Diamonds 
  Seven Hearts 
Valet parking receipt - Blue Chevy for Ernst Richards

*/






=== scene(title, date, items, success, -> toNext)
    >>> Scene ({title})
    [ {title} / { date } ]
    ~ levelItems = items 
    ~ levelSolutionItems = success
    ~ currentItems = () 
    ~ next = toNext
    ->->
    
=== function addItems(items) 
    ~ levelItems += items

=== function require(item) 
    ~ return levelItems !? item

=== offer(items, -> backto)
    {not DEBUG: 
        -> ingame
    }
    ~ temp item = pop(items) 
    { item: 
        <- slot(item, backto) 
    }
    {items: 
        -> offer(items, backto)
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



 


