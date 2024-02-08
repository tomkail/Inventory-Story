


VAR OneUseOnlyItems = (SealedMetalCylinder, LinePrinter, Hotline, Analyst) 

=== function defaultItemName(item)
    {item: 
    -   ManilaEnvelope:             manila envelope
    -   BlackLeatherFolder:   black leather folder
    -   WhiteApron: {after(BackAlleyway):stained} white apron
    
    -   ERNameBadge:    security badge
    - MetalCylinderPhoto: photograph of thin device
    - ErnstRichardsDies: photo of civilian corpse
    - DeviceOperatedPhoto: printout of earthquake spike
    - SealedMetalCylinder: 
        { generatedItems ^ (Nothing, Device): empty | sealed } metal cylinder
    - Lipstick:      empty lipstick tube
    
//    -   KoDStamp:   stamp
    -   else:         {item} 
    }

=== function  defaultItemTooltip(item)
    {item: 
    - NoWeddingRing:    "I took it off." 

    -   SealedMetalCylinder:    \*WARNING\*
    
    -   Camera:     "Property: A. Richards."
    
    -   Kosakov:    "The device, please, Ana."
           
    -   Device:  "Property of the US Army"
    -   Quentin:    
            {
            - got ( Wife ):  
                "You're a lucky man, Ern." 
            - not got ( OtherWeddingRing ): 
                "Ready?"
            - else:
                "Go on, then!"
            }
    
    
    -   DeviceOperatedPhoto:    "Operation of device observed from California earthquake monitoring station, Jan '61"
    
    -   DeviceRemovedFromCylinderPhoto: "Aug 15th, 1964. Device is removed from protective sheath."
    
    -   QuentinsAide: 
        { currentSceneID:
       
        -   QGivesNoteToAide: "Whatever it is, you can trust me, sir."
        
        }
    
    
    
    -   MapOfParisMetro:    "(X) Champ de Mars. Midnight. Q."
    -   TornMapOfParisMetro: "...mp de Mars. Mi.."
    -   WeddingPhoto:   "Annabel & Ernst 3/10/62"
    -   GlassVialOfPowder:  "COCAINE"
    -   PlayingCard:        King of Diamonds. 
    
    
    -   BusinessCard:       "Ernst Richards, office clerk, UN."
    -   KingDiamondsCard:   "KING OF DIAMONDS: cards / slots / roulette / girls"
    -   QsBusinessCard:  "Quentin Roch, Private Investigator. Champs de Mars. No matter too small. Divorce a speciality." 
    -   OtherOtherBusinessCard:  "Gamblers Anonymous. DON'T GET LUCKY GET HELP."
    -   WeddingRing:        "Annie and Ernie -- 3 Oct 1962"
    -   ManilaEnvelope:     "Known Timeline of the Hopburg-Steiner Device"  
    -   ManEnteringCarOutsideUNPhoto:  "ER, 23rd April 68"
  
    -   MetalCylinderPhoto: "Created 3/58 Nevada."
    
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
    
    
    - KosakovCard: "Paris 15643. Ask for K."
 
     
   
     
    -   KosakovsThanks:  
            { is(GoThroughWithWedding): 
                "This is excellent cover. You have done very well, Ana. Do not worry. Your reward will come."
                
          
                
            }
      
    
    
    }
    ~ return
    

=== function  defaultItemReplacesItem(item)
    {item:
        - Nothing: ~ return Nothing
    }
    ~ return ()
    
  
=== function  defaultRequiresItem(item)
    { item: 
    -   WallSafe:       ~ return WeddingPhoto
    -   Lipstick : ~ return Device 
    }
    ~ return () 
    
    

=== function  defaultGeneratesItem(item)
    {item: 
    
    - YellowSkoda: ~ return ComradeAna

    
    - LipstickHidingDevice: 
            ~ return replaceAs ( ( Device , Lipstick )  ) 
    - Lipstick: 
        ~ return replaceAs( LipstickHidingDevice  )
        
     - BlackChanelBag: 
        ~ return (Lipstick)
        
        
    - Device:   
        ~ return Warp

     
    - SealedMetalCylinder: 
        { after( ApartmentBeforeErnst ) : 
            ~ return Nothing 
        - else: 
            ~ return Device
        }
   
    
    - Wallet: 
        ~ temp contents = (BusinessCard, QsBusinessCard, OtherOtherBusinessCard, KingDiamondsCard)
        
        {
        - isOrAfter(Mortuary): 
            ~ contents += TornMapOfParisMetro
        - after(NoteInCar): 
            ~ contents += MapOfParisMetro 
        } 
        { after(Apartment): 
            ~ contents += SealedMetalCylinder
        }
        
        
        ~ return contents

    
    

    - BlackKitBag: ~ return (PianoWire, FlickKnife, SmallGun, Cigarettes, DupontMetroPass, BlackLeatherFolder)
    
    
    - BlackLeatherFolder: 
        ~ return (GlassVialOfPowder, PhotoOfErnst, CasinoChips)
    
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

    - ValetReceipt: 
        ~ return replaceAs( EvenEvenMoreChips)
    
   
    - WallSafe: 
        ~ return (SealedMetalCylinder , AceSpades)
    
    - KeyHook: 
        ~ return CarKey
    
    - Jacket:   ~ return (Wallet)
    }
    ERROR: {item} has no generator list 
    
    
