



LIST SceneProps = Title, Time, ExitKnot, Knot

=== function getSceneData(sceneID, prop) // Title, Time, Knot, Exit
{ sceneID: 
- OpeningSequence:     
    { prop: 
    -   Title: Desktop
    -   Knot:   ~ return -> opening_sequence 
    -   Time:   10th May 1968, 4:32pm
    -   ExitKnot: ~ return -> opening_sequence_fn
    }
- Pinboard:     
    { prop: 
    -   Title: East Berlin
    -   Time:   10th May 1968, 11:32pm
    -   Knot:   ~ return -> pinboard 
    -   ExitKnot: ~ return -> pinboard_exit
    }

- BerlinDeadDropScene:
    { prop: 
    -   Title:  Mink Strasse, East Berlin
    -   Time:   10th May 1968, 7:19pm 
    -   Knot:   ~ return -> BerlinDeadDrop  
    -   ExitKnot: ~ return -> BerlinDeadDrop_fn
    }  
- DroppingBerlinDeadDropScene:
    { prop: 
    -   Title:  Mink Strasse, East Berlin
    -   Time:   10th May 1968, 2:34pm 
    -   Knot:   ~ return -> DroppingBerlinDeadDrop  
    -   ExitKnot: ~ return -> DroppingBerlinDeadDrop_fn
    }
- BorderCheckpointScene:
    { prop: 
    -   Title:  Checkpoint Ralph, West Berlin Side
    -   Time:   10th May 1968, 4:31pm 
    -   Knot:   ~ return -> BorderCheckpoint  
    -   ExitKnot: ~ return -> BorderCheckpoint_fn
    }        
        
- Graveyard: 
    { prop: 
    -   Title:  Graveyard near Rue Clemins
    -   Time:      29th April 1968, 11:07am
    -   Knot:   ~ return -> graveyard 
    -   ExitKnot: ~ return -> graveyard_fn
    }

- AnnieHearsOfDeathScene:
    { prop: 
    -   Title:  AnnieHearsOfDeathTtle
    -   Time:   AnnieHearsOfDeathTime 
    -   Knot:   ~ return -> AnnieHearsOfDeath  
    -   ExitKnot: ~ return -> AnnieHearsOfDeath_fn
    }    
- Mortuary: 
    { prop: 
    -   Title: Mortuary, 4th Quartier 
    -   Time:   24th April 1968, 2:38pm
    -   Knot:   ~ return -> mortuary 
    -   ExitKnot: ~ return -> mortuary_fn
    }
    
- AnnieGivesInnerDeviceToContact: 
    { prop: 
    - Title:    The Montmatre Tunnel 
    - Time:     23rd April 1968, 11:27pm
    - Knot: ~ return -> annie_gives_inner_device
    - ExitKnot: ~ return -> annie_gives_inner_device_fn
    }    
    
- MetroPlatformScene: 
    { prop: 
    -   Title:  Metro Platform, Champ de Mars 
    -   Time:   23rd April 1968, 11:25pm
    -   Knot:   ~ return -> MetroPlatform 
    -   ExitKnot: ~ return -> metro_platform_fn
    }
- HotelKitchens:
    { prop: 
    -   Title: Kitchens of the Hotel de la Tour 
    -   Time:   23rd April 1968, 10:35pm
    -   Knot:   ~ return -> in_the_kitchens 
    -   ExitKnot: ~ return -> kitchen_fn
    }
- HotelBathroom:
    { prop: 
    -   Title:Lobby Bathroom, Hotel de la Tour 
    -   Time:   23rd April 1968, 10:28pm
    -   Knot:   ~ return -> hotel_bathroom 
    -   ExitKnot: ~ return -> bathroom_fn
    }
- BackAlleyway:
    { prop: 
    -   Title:Alleyway Behind l'Hotel de l'Opera 
    -   Time:   23rd April 1968, 10:25pm
    -   Knot:   ~ return -> back_alleyway 
    -   ExitKnot: ~ return -> alleyway_fn
    }
- BackOfClub:
    { prop: 
    -   Title: Backroom, King of Diamonds Club
    -   Time:   23rd April 1968, 9:18pm
    -   Knot:   ~ return -> back_of_kingdiamondsclub 
    -   ExitKnot: ~ return -> back_club_fn
    }
- CardTableAtClub:
    { prop: 
    -   Title: The Table, King of Diamonds Club
    -   Time:   23rd April 1968, 6:25pm
    -   Knot:   ~ return -> kingdiamondsclub 
    -   ExitKnot: ~ return -> king_diamond_club_fn
    }
- ParkingLot: 
    { prop: 
    -   Title: Parking Lot, King of Diamonds Club
    -   Time:   23rd April 1968, 5:159m
    -   Knot:   ~ return -> parking_lot 
    -   ExitKnot: ~ return -> parking_lot_fn
    }

- Apartment: 
    { prop: 
    -   Title:  Apartment, Montpellier
    -   Time:   23rd April 1968, 4:23pm
    -   Knot:   ~ return -> apartment 
    -   ExitKnot: ~ return -> apartment_fn 
    }
 - ApartmentBeforeErnst:
    { prop: 
    -   Title:  Apartment, Montpellier
    -   Time:   23rd April 1968, 8:09pm
    -   Knot:   ~ return -> apartment_after_ernst
    -   ExitKnot: ~ return -> apartment_after_ernst_fn 
    }   
 
    
- AnnieComesFromWork: 
    { prop: 
    -   Title:  Outside the UN Building 
    -   Time:   23rd April 1968, 3:19pm
    -   Knot:   ~ return -> annie_in_car 
    -   ExitKnot: ~ return -> annie_in_car_fn 
    }
- NoteInCar:
    { prop: 
    -   Title:  Outside the UN Building 
    -   Time:   23rd April 1968, 3:15pm
    -   Knot:   ~ return -> quentin_passes_note 
    -   ExitKnot: ~ return -> quentin_note_fn 
    }
- QGivesNoteToAide:
    { prop: 
    -   Title:  Dirty Office above Champs de Mars 
    -   Time:   23rd April 1968, 2:29pm
    -   Knot:   ~ return -> quentin_gives_aide_money 
    -   ExitKnot: ~ return -> gives_aide_money_fn 
    }
- QGivesItemToErnst: 
    { prop: 
    -   Title:The UN Building  
    -   Time:   7th August 1967, 11:45pm
    -   Knot:   ~ return -> item_from_quentin  
    -   ExitKnot: ~ return -> item_question_fn
    }
- StealCardFromKingDiamonds: 
    { prop: 
    -   Title:  Card Table, King of Diamonds Club
    -   Time:   18th July 1967, 10:16pm
    -   Knot:   ~ return -> king_clubs_steal_card  
    -   ExitKnot: ~ return -> king_clubs_steal_card_fn
    }
- QGetsDevice: 
    { prop: 
    - Title:    Park, Mid-France
    - Time:      25th July, 1967
    - Knot: ~ return -> quentin_receives_metal_cylinder
    - ExitKnot: ~ return -> q_receives_cylinder_fn
    }
    

- QuentinGetsDeviceAnnieWatchingScene:
    { prop: 
    - Title:    Park, Mid-France
    - Time:      25th July, 1967
    -   Knot:   ~ return -> QuentinGetsDeviceAnnieWatching  
    -   ExitKnot: ~ return -> QuentinGetsDeviceAnnieWatching_fn
    }    
    
- GamblersAnonymous: 
    { prop: 
    -   Title:  Missionary Hall 
    -   Time:   5th May, 1964 
    -   Knot: ~ return -> gamblers_anonymous 
    -   ExitKnot: ~ return -> gamblers_anonymous_fn 
    }


- ErnDiesEarlyScene:
    { prop: 
    -   Title:  Graveyard near Rue Clemins
    -   Time:   8th Oct, 1963, 11:21am 
    -   Knot:   ~ return -> ErnDiesEarly  
    -   ExitKnot: ~ return -> ErnDiesEarly_fn
    }
    
- InBedWithErnstScene:
    { prop: 
    -   Title:  Bedroom, Apartment, Montpellier
    -   Time:   4th Oct 1963, 2:32am
    -   Knot:   ~ return -> InBedWithErnst  
    -   ExitKnot: ~ return -> InBedWithErnst_fn
    }
    
    
- DriveAfterWedding:
    { prop: 
    -   Title:Leaving the Chapel St Jean 
    -   Time:   3rd Oct 1962, 10:35pm
    -   Knot:   ~ return -> wedding_drive_away 
    -   ExitKnot: ~ return -> wedding_car_fn 
    }
- Wedding:
    { prop: 
    -   Title:  The Chapel St Jean, Montpellier 
    -   Time:   3rd Oct 1962, 1:18pm
    -   Knot:   ~ return -> wedding  
    -   ExitKnot: ~ return -> wedding_fn
    }
- GoThroughWithWedding:
    { prop: 
    -   Title:  TemplateTtle
    -   Time:   TemplateTime 
    -   Knot:   ~ return -> go_through_with_wedding  
    -   ExitKnot: ~ return -> go_through_with_wedding_fn
    }  
- MonitoringStationMorning: 
    { prop: 
    -   Title:  TemplateTtle
    -   Time:   TemplateTime 
    -   Knot:   ~ return    -> monitoring_station  
    -   ExitKnot: ~ return  -> monitoring_station_fn
    }
- DeviceOperated:
    { prop: 
    -   Title:  TemplateTtle
    -   Time:   18th Jan 1961, 1:18pm 
    -   Knot:   ~ return -> device_operated  
    -   ExitKnot: ~ return -> device_operated_fn
    }


- else: [ ERROR: need scene data for {sceneID} ]
}