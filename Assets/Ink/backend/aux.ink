
=== function pop(ref _list) 
    ~ temp el = LIST_MIN(_list) 
    ~ _list -= el
    ~ return el 

=== function pop_top(ref _list) 
    ~ temp el = LIST_MAX(_list) 
    ~ _list -= el
    ~ return el     

=== NOPE 
    -> END
    

=== function FALSE_(x) 
    ~ return false   
    
=== function FALSE 
    ~ return false 


=== function seen_more_recently_than(-> link, -> marker)
	{ TURNS_SINCE(link) >= 0: 
        { TURNS_SINCE(marker) == -1: 
            ~ return true 
        } 
        ~ return TURNS_SINCE(link) < TURNS_SINCE(marker) 
    }
    ~ return false 

   
=== function BOOL(x) 
    ~ return not not x