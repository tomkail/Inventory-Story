
 /*
    QuestionScientist
 */
 
 
=== QuestionScientist 
    LIST QuestionScientistItems = (QuestionScientistItem) 
    VAR QuestionScientistInteractables = (QuestionScientistItem)
    
    -> scene ( QuestionScientistItems, QuestionScientistInteractables, "Remark") 
=== function QuestionScientist_fn(x) 
    { x: 
    -   (): ~ return 1 
TODO: A solve t
    }
    ~ return () 

=== function QuestionScientist_gameplay(act, item) 
    {act: 
    -   Tooltip: {item: 
        }
    }
    ~ return () 

/*


- QuestionScientistScene:
    { prop: 
    -   Title:  QuestionScientistTtle
    -   Time:   QuestionScientistTime 
    -   Knot:   ~ return -> QuestionScientist  
    -   ExitKnot: ~ return -> QuestionScientist_fn
    -   GameplayKnot: ~ return -> QuestionScientist_gameplay
    }
    
*/