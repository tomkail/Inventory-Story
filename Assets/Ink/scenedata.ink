



LIST SceneProps = Title, Time, Knot, GameplayKnot

=== function getSceneData(sceneID, prop) // Title, Time, Knot, Exit
{ sceneID: 
- OpeningSequence:     
    { prop: 
    -   Title: Desktop
    -   Knot:   ~ return -> opening_sequence 
    -   Time:   10th May 1968, 4:32pm
    
    -   GameplayKnot: ~ return -> opening_sequence_gameplay
    }
- ContainerOpeningBeatScene:
    { prop: 
    -   Title:  Desktop
    -   Time:    10th May 1968, 4:32pm 
    -   Knot:   ~ return -> ContainerOpeningBeat  
    -   GameplayKnot: ~ return -> opening_sequence_gameplay
    }
- AgentUnknownBeatScene:
    { prop: 
    -   Title:  Desktop
    -   Time:    10th May 1968, 4:32pm 
    -   Knot:   ~ return -> AgentUnknownBeat  
    
    -   GameplayKnot: ~ return -> opening_sequence_gameplay
    }    
- Pinboard:     
    { prop: 
    -   Title: East Berlin
    -   Time:   10th May 1968, 11:32pm
    -   Knot:   ~ return -> pinboard 
    
    -   GameplayKnot: ~ return -> pinboard_gameplay
    }

- BerlinDeadDropScene:
    { prop: 
    -   Title:  Mink Strasse, East Berlin
    -   Time:   10th May 1968, 7:19pm 
    -   Knot:   ~ return -> BerlinDeadDrop  
    -   GameplayKnot: ~ return -> BerlinDeadDrop_fn
    }  
- DroppingBerlinDeadDropScene:
    { prop: 
    -   Title:  Mink Strasse, East Berlin
    -   Time:   10th May 1968, 2:34pm 
    -   Knot:   ~ return -> DroppingBerlinDeadDrop  
    -   GameplayKnot: ~ return -> DroppingBerlinDeadDrop_fn
    }
- BorderCheckpointScene:
    { prop: 
    -   Title:  Checkpoint Ralph, West Berlin Side
    -   Time:   10th May 1968, 4:31pm 
    -   Knot:   ~ return -> BorderCheckpoint  
    -   GameplayKnot: ~ return -> BorderCheckpoint_fn
    }        
        
- Graveyard: 
    { prop: 
    -   Title:  Graveyard near Rue Clemins
    -   Time:      29th April 1968, 11:07am
    -   Knot:   ~ return -> graveyard 
    -   GameplayKnot: ~ return -> graveyard_fn
    }

- AnnieHearsOfDeathScene:
    { prop: 
    -   Title:  AnnieHearsOfDeathTtle
    -   Time:   AnnieHearsOfDeathTime 
    -   Knot:   ~ return -> AnnieHearsOfDeath  
    - GameplayKnot: ~ return -> AnnieHearsOfDeath_gameplay
    }    
- Mortuary: 
    { prop: 
    -   Title: Mortuary, 4th Quartier 
    -   Time:   24th April 1968, 2:38pm
    -   Knot:   ~ return -> mortuary 
    -   GameplayKnot: ~ return -> mortuary_fn
    }
    
- AnnieGivesInnerDeviceToContact: 
    { prop: 
    - Title:    The Montmatre Tunnel 
    - Time:     23rd April 1968, 11:27pm
    - Knot: ~ return -> annie_gives_inner_device
    - GameplayKnot: ~ return -> annie_gives_inner_device_fn
    }    
    
- MetroPlatformScene: 
    { prop: 
    -   Title:  Metro Platform, Champ de Mars 
    -   Time:   23rd April 1968, 11:25pm
    -   Knot:   ~ return -> MetroPlatform 
    -   GameplayKnot: ~ return -> metro_platform_fn
    }
- OnPlatformWaitingForQScene:    
    { prop: 
    -   Title:  Metro Platform, Champ de Mars 
    -   Time:   23rd April 1968, 11:21pm
    -   Knot:   ~ return -> OnPlatformWaitingForQ
    -   GameplayKnot: ~ return -> OnPlatformWaitingForQ_gameplay
    }    
    
- HotelKitchens:
    { prop: 
    -   Title: Kitchens of the Hotel de la Tour 
    -   Time:   23rd April 1968, 10:35pm
    -   Knot:   ~ return -> in_the_kitchens 
    -   GameplayKnot: ~ return -> kitchen_fn
    }
- HotelBathroom:
    { prop: 
    -   Title:Lobby Bathroom, Hotel de la Tour 
    -   Time:   23rd April 1968, 10:28pm
    -   Knot:   ~ return -> hotel_bathroom 
    -   GameplayKnot: ~ return -> bathroom_fn
    }
- BackAlleyway:
    { prop: 
    -   Title:Alleyway Behind l'Hotel de l'Opera 
    -   Time:   23rd April 1968, 10:25pm
    -   Knot:   ~ return -> back_alleyway 
    -   GameplayKnot: ~ return -> alleyway_fn
    }
- BackOfClub:
    { prop: 
    -   Title: Backroom, King of Diamonds Club
    -   Time:   23rd April 1968, 9:18pm
    -   Knot:   ~ return -> back_of_kingdiamondsclub 
    -   GameplayKnot: ~ return -> back_club_fn
    }
- CardTableAtClub:
    { prop: 
    -   Title: The Table, King of Diamonds Club
    -   Time:   23rd April 1968, 6:25pm
    -   Knot:   ~ return -> kingdiamondsclub 
    -   GameplayKnot: ~ return -> king_diamond_club_fn
    }
- ParkingLot: 
    { prop: 
    -   Title: Parking Lot, King of Diamonds Club
    -   Time:   23rd April 1968, 5:159m
    -   Knot:   ~ return -> parking_lot 
    -   GameplayKnot: ~ return -> parking_lot_fn
    }

- Apartment: 
    { prop: 
    -   Title:  Apartment, Montpellier
    -   Time:   23rd April 1968, 4:23pm
    -   Knot:   ~ return -> apartment 
    -   GameplayKnot: ~ return -> apartment_fn 
    }
 - ApartmentBeforeErnst:
    { prop: 
    -   Title:  Apartment, Montpellier
    -   Time:   23rd April 1968, 8:09pm
    -   Knot:   ~ return -> apartment_after_ernst
    -   GameplayKnot: ~ return -> apartment_after_ernst_fn 
    }   
 
    
- AnnieComesFromWork: 
    { prop: 
    -   Title:  Outside the UN Building 
    -   Time:   23rd April 1968, 3:19pm
    -   Knot:   ~ return -> annie_in_car 
    -   GameplayKnot: ~ return -> annie_in_car_fn 
    }
- NoteInCar:
    { prop: 
    -   Title:  Outside the UN Building 
    -   Time:   23rd April 1968, 3:15pm
    -   Knot:   ~ return -> quentin_passes_note 
    -   GameplayKnot: ~ return -> quentin_note_fn 
    }
- QGivesNoteToAide:
    { prop: 
    -   Title:  Dirty Office above Champs de Mars 
    -   Time:   23rd April 1968, 2:29pm
    -   Knot:   ~ return -> quentin_gives_aide_money 
    -   GameplayKnot: ~ return -> gives_aide_money_gameplay
    }
    
    

- StealCardFromKingDiamonds: 
    { prop: 
    -   Title:  Card Table, King of Diamonds Club
    -   Time:   18th July 1967, 10:16pm
    -   Knot:   ~ return -> king_clubs_steal_card  
    -   GameplayKnot: ~ return -> king_clubs_steal_card_fn
    }
  
        
    
- QGivesItemToErnst: 
    { prop: 
    -   Title:The UN Building  
    -   Time:   7th August 1966, 11:45pm
    -   Knot:   ~ return -> QGivesItemToErnst_knot  
    -   GameplayKnot: ~ return -> QGivesItemToErnst_gameplay
    }
    
- GamblersAnonymous: 
    { prop: 
    -   Title:  Missionary Hall 
    -   Time:   5th May, 1966
    -   Knot: ~ return -> gamblers_anonymous 
    -   GameplayKnot: ~ return -> gamblers_anonymous_fn 
    }    
    
    
- QGetsDeviceScene: 
    { prop: 
    - Title:    Park, Mid-France
    - Time:      25th April, 1966
    - Knot: ~ return -> QGetsDevice_knot
    - GameplayKnot: ~ return -> q_receives_cylinder_fn
    }



- QuentinGetsDeviceAnnieWatchingScene:
    { prop: 
    - Title:    Park, Mid-France
    - Time:      25th April, 1966
    -   Knot:   ~ return -> QuentinGetsDeviceAnnieWatching  
    -   GameplayKnot: ~ return -> QuentinGetsDeviceAnnieWatching_fn
    } 
    

- RavensNestScene:
    { prop: 
    -   Title:  Rooftop Terrace
    -   Time:   19th April, 1966
    -   Knot:   ~ return -> RavensNest_knot 
    -   GameplayKnot: ~ return -> RavensNest_gameplay
    }
    
  

- QuestionScientistScene:
    { prop: 
    -   Title:  QuestionScientistTtle
    -   Time:   QuestionScientistTime 
    -   Knot:   ~ return -> QuestionScientist  
    -   GameplayKnot: ~ return -> QuestionScientist_gameplay
    }
    

- ErnDiesEarlyScene:
    { prop: 
    -   Title:  Graveyard near Rue Clemins
    -   Time:   8th Oct, 1963, 11:21am 
    -   Knot:   ~ return -> ErnDiesEarly  
    -   GameplayKnot: ~ return -> ErnDiesEarly_fn
    }
    
- InBedWithErnstScene:
    { prop: 
    -   Title:  Bedroom, Apartment, Montpellier
    -   Time:   4th Oct 1963, 2:32am
    -   Knot:   ~ return -> InBedWithErnst  
    -   GameplayKnot: ~ return -> InBedWithErnst_gameplay
    }
    
    
- DriveAfterWedding:
    { prop: 
    -   Title:Leaving the Chapel St Jean 
    -   Time:   3rd Oct 1962, 10:35pm
    -   Knot:   ~ return -> wedding_drive_away 
    -   GameplayKnot: ~ return -> wedding_car_fn 
    }
    
- Wedding:
    { prop: 
    -   Title:  The Chapel St Jean, Montpellier 
    -   Time:   3rd Oct 1962, 1:18pm
    -   Knot:   ~ return -> wedding  
    - GameplayKnot: ~ return -> wedding_gameplay
    }
    
- GoThroughWithWedding:
    { prop: 
    -   Title:  TemplateTtle
    -   Time:   TemplateTime 
    -   Knot:   ~ return -> go_through_with_wedding  
    -   GameplayKnot: ~ return -> go_through_with_wedding_fn
    }  
    
- MonitoringStationMorning: 
    { prop: 
    -   Title:  Earthquake Monitoring Station, Pasedena
    -   Time:   18th Jan 1961, 6:27pm  
    -   Knot:   ~ return    -> monitoring_station  
    - GameplayKnot: ~ return -> monitoring_station_gameplay
    }
- DeviceOperated:
    { prop: 
    -   Title:  TemplateTtle
    -   Time:   18th Jan 1961, 1:18am 
    -   Knot:   ~ return -> device_operated  
    -   GameplayKnot: ~ return -> device_operated_fn
    }


- else: [ ERROR: need scene data for {sceneID} ]
}
// fallthrough  for optional paramenters 
{ prop == GameplayKnot: 
    ~ return -> twoParameterBlankList
}
// [ ERROR: no {prop} for {sceneID} ]