
=== function getItemName(item)
    {item: 
    -   ManilaEnvelope:             manila envelope
    -   BunchOfFlowers:             black lilies 
    -   AnotherBunchOfFlowers:      white lilies
    -   MoreFlowers:                yellow lilies
    -   CardboardBox:   {not back_alleyway:empty} cardboard box
    -   WhiteApron: {not hotel_bathroom:stained} white apron
    -   else:         {item} 
    }

=== function getItemTooltip(item) 
    {item: 
    -   DeskPlate:  "Ernst Richards, Clerk"
    -   WeddingPhoto:   "Annabel & Ernst 3/10/62"
    -   GlassVialOfPowder:  "COCAINE"
    -   PlayingCard:        King of Diamonds. 
    -   WhiteFabricScrap:   Torn, and slightly stained.
    -   PoliceNotes:             "Attempted theft. Killer was disturbed and escaped. Narcotics in victim's blood."   
    -   BusinessCard:       "Ernst Richards, office clerk, UN."
    -   KingDiamondsCard:   "KING OF DIAMONDS: cards / slots / roulette / girls"
    -   OtherBusinessCard:  "Bolera Taxis." 
    -   OtherOtherBusinessCard:  "Gamblers Anonymous. DON'T GET LUCKY GET HELP."
    -   WeddingRing:        "Annie and Ernie -- 3 Oct 1962"
    -   ManilaEnvelope:     "ER surveillance"  
    -   ManEnteringCarOutsideUNPhoto:  "ER, 19th April 68"
    -   ManInAirportPhoto:  "Unknown subject. Puerto Rico, 26th April 68"
    -   MetalCylinderPhoto: "The Hopburg-Steiner Device"
    -   PianoWire:      {in_the_kitchens:
                            It's coiled and clean.
                        - else:
                            It's blood-soaked.
                        }
    - CardboardBox:     "Claude. Rat poison - DO NOT OPEN."
    - CoffeeOrderSlip:  "Coffee, table 15."
    - CoffeeSpoon:      A few white grains are still visible on the spoon.
    - FoodPeelings:     Onion skins, coffee grounds, potato peel.
    - EmptyGlassVial:   Containing a few grains of a white powder.
    - WaiterNameBadge:  "CARL. Ask me for service!"
    - DupontMetroPass:  Metro Pass: C. DUPONT
    - PhotoOfErnst:     "Ernst Richards. 33.y.o."
    - PhotoOfCylindricalDevice:  "LOCATE"
    - CasinoChips:      KING OF DIAMONDS nightclub
    - AceSpadesReversed:        The back of this card is slightly different from the others.
    - ValetReceipt:         Parking receipt for a Blue Chevy, registered to Ernst Richards
    - Timeline: Timeline of the Hopburg-Steiner Device
    - Inception: "Device created. March 1961. Dakota."
    - DeviceStolenFromResearchLab: "Device stolen. April 1962."
    - ErnstRichardsDies:  "Paris. May 1968. Device found by chance on unknown dead man."
    - NoteFromQuentin:  "Ernie - Hold onto this for me. Keep it safe. Dead drop by the Champs du Mars. April 23rd, 4:30pm."
    - DupontInstructions:   "Further instructions: outside kitchens, Hotel de Champs de Mars."
    }
    ~ return
    
=== function itemRequiresItem(item) 
    { item: 
    -   CardboardBox:   { back_alleyway: 
            ~ return FlickKnife   
        } 
    -   MetalLockBox:   ~ return KingKey
    -   Waiter: ~ return MetalSoapBottle
    -   WallSafe:       ~ return WeddingPhoto
    }
    ~ return () 
    
=== function itemGeneratesItems(item, ref asReplacement) 
    {item: 
    - ManilaEnvelope: 
        ~ asReplacement = true
        ~ return (ManEnteringCarOutsideUNPhoto, ManInAirportPhoto,  MetalCylinderPhoto)
    - BunchOfFlowers:   ~ return EvenMoreFlowers
    - EvenMoreFlowers:  ~ return WeddingRing 
    - AnotherBunchOfFlowers:    ~ return MoreFlowers
    - Wallet: 
        ~ return (BusinessCard, OtherBusinessCard, OtherOtherBusinessCard, KingDiamondsCard,DupontMetroPass, SealedMetalCylinder)
    - Scarf: ~ return PianoWire
    - Bin: ~ return (FoodPeelings, EmptyGlassVial)
    - WhiteApron: ~ return (CoffeeOrderSlip, PianoWire)
    - UnconciousWaiter: ~ return (WhiteApron, WaiterNameBadge)
    - BlackKitBag: ~ return (PianoWire, BlackVelvetBag, FlickKnife, SmallGun, Cigarettes, DupontMetroPass, CardboardBox)
    - BlackVelvetBag: ~ return (GlassVialOfPowder, ChloroformBottle)
    - CoffeeOnTray:     ~ return CoffeeSpoon
    - CardboardBox: 
        ~ return (GlassVialOfPowder, PhotoOfErnst, PhotoOfCylindricalDevice, CasinoChips)
    - Waiter: 
        ~ return replaceAs(asReplacement, UnconciousWaiter)
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
    - Jacket:               
        ~ return (Wallet)
    - Timeline: 
        ~ return replaceAs(asReplacement,  (Inception, DeviceStolenFromResearchLab, ErnstRichardsDies ) )
    - WallSafe: 
        ~ return (SealedMetalCylinder, TwoThousandFrancs, NoteFromQuentin)
    - Stranger: 
        ~ return (DupontMetroPass)
    - KeyHook: 
        ~ return CarKey
    
    - UNBin: 
        ~ return NoteFromQuentin
    - Envelope: 
        ~ return SealedMetalCylinder
    - else: ERROR: {item} has no generator list 
    
    }
    
=== function replaceAs(ref asReplacement, item) 
    ~ asReplacement = true
    ~ return item 