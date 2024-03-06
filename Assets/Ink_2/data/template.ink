
/*

TemplateScene,

- TemplateScene:    ~ return -> TemplateScene_data 

*/

=== function TemplateScene_data(key, item) 
    { key: 
    -   Title: 
    -   Date: 
    -   SolutionSize:   ~ return 1 
    -   Sequence: {item: 
        - else: ~ return () 
        }
    -   Children: {item: 
        - (): // initial setup 
        }
    -   Tooltip: {item: 
        - ():  // opening line 
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

