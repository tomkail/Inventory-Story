
 /*
    AgentUnknownBeat
 */
 
 
=== AgentUnknownBeat 
    LIST AgentUnknownBeatItems = (AgentUnknownBeatItem) 
    VAR AgentUnknownBeatInteractables = (AgentUnknownBeatItem)
    
    -> scene ( AgentUnknownBeatItems, AgentUnknownBeatInteractables, "Remark") 
=== function AgentUnknownBeat_fn(x) 
    { x: 
    -   (): ~ return 1 
TODO: A solve 
    }
    ~ return () 

/*


- AgentUnknownBeatScene:
    { prop: 
    -   Title:  AgentUnknownBeatTtle
    -   Time:   AgentUnknownBeatTime 
    -   Knot:   ~ return -> AgentUnknownBeat  
    -   ExitKnot: ~ return -> AgentUnknownBeat_fn
    }
    
*/