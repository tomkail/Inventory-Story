
VAR ReplayableScenes = (OpeningSequence, Pinboard)

// OpeningSequence
=== opening_sequence
    LIST OpeningItems = Agent, Briefcase, KeyOnWristChain
    { stopping: 
    -   -> scene(Device, () , "Nevada, 1958. The Hopburg-Steiner device was constructed in a military research facility using a technology of unknown origin.") 
    -   -> scene(SealedMetalCylinder, SealedMetalCylinder , "Despite being only palm-sized, be assured this is a device of extreme consequence. Mildly radioactive, it is stored inside a beryllium container.") 
    -   -> scene(Agent, (Agent, Briefcase, SealedMetalCylinder) , "In April 1962, an unknown foreign agent exited the lab in Area 51 with the device in a briefcase.")
    
    -   -> proceedTo(MonitoringStationMorning)
    }
    
=== function opening_sequence_gameplay(type, item) 
    { type: 
    - Sequence: {item:
    -   ():         ~ return 1 
    -   Device:     ~ return OpeningSequence 
    -   SealedMetalCylinder: ~ return ContainerOpeningBeatScene 
    -   Agent:  
        { previousSceneID !? AgentUnknownBeatScene:
            ~ return AgentUnknownBeatScene
        }
    -   Briefcase:  ~ return AgentUnknownBeatScene
     }
    - Name:
        { item: 
        -   Briefcase:  {levelItems ? SealedMetalCylinder:open} briefcase 
        }
    - Tooltip:
        { item: 
        - Agent: "Plane lands in three hours."
        - KeyOnWristChain:  "L731"
        - Briefcase:    "Lloyds of London" 
        }
//    - Replacement:
    - Requirement:
        { item: 
        - Briefcase: ~ return KeyOnWristChain
        }
    - Generation:
        { item: 
        - Agent: ~ return  (Briefcase, KeyOnWristChain)
        - Briefcase: ~ return SealedMetalCylinder 
        }
    }
    ~ return () 

    
=== ContainerOpeningBeat 
    -> scene ( SealedMetalCylinder, SealedMetalCylinder, "The container itself is of no importance. The device within, however, is of the utmost significance.") 
    


=== AgentUnknownBeat 
    
    -> scene (Agent, (Agent, Briefcase, SealedMetalCylinder), "The identity of the agent is still unknown, except for one thing: he didn't work for us.") 

