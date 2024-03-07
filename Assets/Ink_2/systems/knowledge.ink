VAR AllKnowledge = ()

=== function reach(x) 
    { not reached(x):
        {LIST_VALUE(x) < 1000:
            ~ x = LIST_RANGE(LIST_ALL(x), LIST_MIN(LIST_ALL(x)), x)
        }
        ~ AllKnowledge += x 
        ~ refreshVisibleItems()
        ~ return true 
    }
    ~ return false
    
=== function reached(x)
    ~ return AllKnowledge ? x
    
=== function reached_any(x) 
    ~ return AllKnowledge ^ x    

=== function between(x, y) 
    ~ return reached(x) && not reached_any(y)
        