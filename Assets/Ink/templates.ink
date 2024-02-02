
 /*
    TEMPLATE
 */
 
 
=== template 
    LIST TemplateItems = (TemplateItem) 
    VAR TemplateInteractables = (TemplateItem)
    
    -> scene ( TemplateItems, TemplateInteractables, "Remark") 
=== function template_fn(x) 
    { x: 
    -   (): ~ return 1 
TODO: A solve 
    }
    ~ return () 

/*


- TemplateScene:
    { prop: 
    -   Title:  TemplateTtle
    -   Time:   TemplateTime 
    -   Knot:   ~ return -> template  
    -   ExitKnot: ~ return -> template_fn
    }
    
*/