   
=== function filterByFunction(list, -> filter) 
    ~ temp el = pop(list) 
   { el: 
        { not filter(el): 
            ~ el = () 
        } 
        ~ return el + filterByFunction(list, filter) 
    } 
    ~ return ()

=== function pop(ref _list) 
    ~ temp el = LIST_MIN(_list) 
    ~ _list -= el
    ~ return el 

=== function BOOL(x) 
    ~ return not(not x)
    
   
=== function TransformByFunction(list, -> transform) 
    ~ temp el = pop(list) 
    { el: 
        ~ return transform(el) + TransformByFunction(list, transform) 
    } 
    ~ return ()
    
=== tunnelOut(-> thenGoTo) 
    { tunnelDepth() > 1:
        [ Tunnelling out! ] 
        ->-> tunnelOut(thenGoTo)
    }
    -> thenGoTo      
    
EXTERNAL tunnelDepth() 
/*
    story.BindExternalFunction("tunnelDepth", () =>
        {
            return story.state.callstackDepth;
        });
*/
=== function tunnelDepth() 
    ~ return 1 