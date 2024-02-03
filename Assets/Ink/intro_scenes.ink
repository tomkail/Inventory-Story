
VAR ReplayableScenes = OpeningSequence

// OpeningSequence
=== opening_sequence
    LIST OpeningItems = Agent, Briefcase, KeyOnWristChain
    { stopping: 
    -   -> scene(Device, () , "Nevada, 1958. The Hopburg-Steiner was constructed in a military research facility using a technology of unknown origin.") 
    -   -> scene(SealedMetalCylinder, SealedMetalCylinder , "Despite being only palm-sized, be assured this is a device of extreme consequence. Mildly radioactive, it is stored inside a beryllium container.") 
    -   -> scene(Agent, (Agent, Briefcase, SealedMetalCylinder) , "In April 1962, an unknown foreign agent exited the lab in Area 51 with the device in a briefcase.")
    
    -   -> proceedTo(MonitoringStationMorning)
    }
    
=== function opening_sequence_fn (x)
    {x:
    -   ():         ~ return 1 
    -   Device:     ~ return OpeningSequence 
    -   SealedMetalCylinder: ~ return ContainerOpeningBeatScene 
    -   Agent:  ~ return AgentUnknownBeatScene
    -   Briefcase:  ~ return AgentUnknownBeatScene
    
    }
    ~ return () 
    
=== ContainerOpeningBeat 
    -> scene ( SealedMetalCylinder, SealedMetalCylinder, "The container itself is of no importance. The device within, however, is of the utmost significance.") 
=== function ContainerOpeningBeat_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   Device: ~ return OpeningSequence
    }
    ~ return () 


=== AgentUnknownBeat 
    
    -> scene (Agent, (Agent, Briefcase, SealedMetalCylinder), "The identity of the agent is still unknown, except for one thing: he didn't work for us.") 
=== function AgentUnknownBeat_fn(x) 
    { x: 
    -   (): ~ return 1 
    -   Device:     ~  return OpeningSequence
    }
    ~ return () 
