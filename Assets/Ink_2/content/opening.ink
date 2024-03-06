
=== function OpeningCaseScene_data(key, item) 
    { key: 
    -   Title:      Briefing Room 7, Langley
    -   Date:       10th May 1968, 4:32pm
    -   SolutionSize:   ~ return 1 
    -   Sequence: {item: 
        - SealedMetalCylinder: ~ return OpeningDeviceScene
        }
    -   Children: {item: 
        - ():   ~ return SealedMetalCylinder
        }
    -   Tooltip: {item: 
        - ():  
            Nevada, 1958. The Hopburg-Steiner device was constructed in a military research facility using a technology of unknown origin. #deepthroat 
        } 
    }
    ~ return () 



=== function OpeningDeviceScene_data(key, item) 
    { key: 
    -   Title:      Briefing Room 7, Langley
    -   Date:       10th May 1968, 4:33pm
    -   SolutionSize:   ~ return 1 
    -   Sequence: {item: 
        - Device: ~ return  OpeningStolenScene
        }
    -   Children: {item: 
        - (): ~ return SealedMetalCylinder 
        - SealedMetalCylinder: ~ return Device 
        }
    -   Tooltip: {item: 
        - ():  The casing is radiation-proof beryllium. 
        - SealedMetalCylinder: The device itself, being palm-sized, is a discovery of extreme consequence. 
        } 
    }
    ~ return () 


=== function OpeningStolenScene_data(key, item) 
    { key: 
    -   Title:      Briefing Room 7, Langley
    -   Date:       10th May 1968, 4:35pm
    -   SolutionSize:   ~ return 1 
    -   Sequence: {item: 
        - Device:       ~ return DeviceActivatedScene 
        }
    -   Children: {item: 
        - ():           ~ return Agent 
        - Agent:        ~ return (DarkGlasses, Briefcase, KeyOnWristChain)
        - Briefcase:    ~ return SealedMetalCylinder 
        - SealedMetalCylinder:       ~ return Device 
        }
    -   Tooltip: {item: 
        -   ():  
                In April 1962, an unknown foreign agent exited the lab in Area 51 with the device in a briefcase. #deepthroat
        -   Agent: 
                "Wheels down in three hours. Move it."
        -   DarkGlasses:  
                The identity of the agent remains unknown, except for one thing: he didn't work for us. #deepthroat
        -   Briefcase:  
                Lead-lined briefcase. They knew what they were taking. #deepthroat
        } 
    -   Requires: {item: 
        -   Briefcase:  ~ return KeyOnWristChain   
        } 
    }
    ~ return () 



=== function DeviceActivatedScene_data(key, item) 
    { key: 
    -   Title:  Earthquake Monitoring Station, Pasedena
    -   Date:   18th Jan 1961, 6:27pm  
    -   SolutionSize:   ~ return 1 
    -   Sequence: {item: 
        - else: ~ return () 
        }
    -   Children: {item: 
        - (): // initial setup 
        }
    -   Tooltip: {item: 
        - ():   Our best evidence suggests the device was indeed activated.
        } 
    /*
    -   Name: {item:
        }     
    -   Becomes: {item: 
        } 
    -   Requires: {item: 
        } 
    */  
    }
    ~ return () 


