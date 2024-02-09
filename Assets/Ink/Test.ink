VAR DEBUG = true 

~ Log("1")
1
~ Log("Pre save")
~ Save()
~ Log("After save")
2
~ Log("2 After save")
3
* choice 1
 1b
~ Save()
2b
3b  
    
    
    
EXTERNAL Save ()
=== function Save ()
~return


EXTERNAL Log (logStr)
=== function Log (logStr)
{logStr}