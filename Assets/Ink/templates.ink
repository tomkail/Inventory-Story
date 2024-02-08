
 /*
    template
 */
 
 
=== template_knot 
    LIST templateItems = (templateItem) 
    VAR templateInteractables = (templateItem)
    
    -> scene ( templateItems, templateInteractables, "Remark") 


=== function template_gameplay(act, item) 
    {act: 
    -   Sequence: {item: 
        - (): ~ return 1 
        TODO: A solve 
        }
    -   Tooltip: {item: 
        }
    }
    ~ return () 

/*


- templateScene:
    { prop: 
    -   Title:  templateTtle
    -   Time:   templateTime 
    -   Knot:   ~ return -> template_knot 
    -   GameplayKnot: ~ return -> template_gameplay
    }
    
*/