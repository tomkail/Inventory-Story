
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
    }
    ~ return
    
=== function itemRequiresItem(item) 
    { item: 
    -   CardboardBox:   { back_alleyway: 
            ~ return FlickKnife   
        } 
    -   MetalLockBox:   ~ return KingKey
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
    
    - HandCards: ~ return (AceHearts, ThreeClubs, SevenHearts, AceSpades, PlayingCard)
    - AceHeartsReversed:   
        ~ asReplacement = true
        ~ return AceHearts 
    - ThreeClubsReversed:   
        ~ asReplacement = true
        ~ return ThreeClubs
    - SevenHeartsReversed:  
        ~ asReplacement = true
        ~ return SevenHearts
    - AceSpadesReversed:    
        ~ asReplacement = true
        ~ return AceSpades
    - AceHearts:            
        ~ asReplacement = true
        ~ return AceHeartsReversed
    - ThreeClubs:           
        ~ asReplacement = true
        ~ return ThreeClubsReversed
    - SevenHearts:          
        ~ asReplacement = true
        ~ return SevenHeartsReversed
    - AceSpades:            
        ~ asReplacement = true
        ~ return AceSpadesReversed
    - PlayingCard:          
        ~ asReplacement = true
        ~ return PlayingCardReversed
    - PlayingCardReversed:  
        ~ asReplacement = true
        ~ return PlayingCard
    - MetalLockBox:         ~ return PileOfChips
    - PileOfChips:          
        ~ asReplacement = true
        ~ return EvenMoreChips 
    - EvenMoreChips:          
        ~ asReplacement = true
        ~ return (EvenEvenMoreChips , ValetReceipt)
    - Jacket:               
        ~ return (Wallet)
    - Timeline: 
        ~ asReplacement = true
        ~ return (Inception, DeviceStolenFromResearchLab, ErnstRichardsDies )
    - WallSafe: 
        ~ return (SealedMetalCylinder, TwoThousandFrancs, NoteFromQuentin)
    - else: ERROR: {item} has no generator list 
    
    }