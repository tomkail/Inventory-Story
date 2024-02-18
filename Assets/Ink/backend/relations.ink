=== function parent(item) 
    ~ return whatIs(ItemHoldsItem, item) 


LIST Relations = ItemHoldsItem

// LIST Ints = INT1 = 1, INT2, INT3, INT4, INT5, INT6, INT7, INT8, INT9, INT10, INT11, INT12, INT13, INT14, INT15, INT16, INT17, INT18, INT19, INT20

// Tell ink which lists each relation links
=== function relationDatabase(rel, lhs)
    { rel:
    -   ItemHoldsItem: 
            ~ return levelItems + SceneTop
        
    }        
            
            
            


VAR pairstore = "" 

=== function _debugPrintAssignments(keyList, relation) 
    ~ temp key = pop(keyList) 
    { key: 
        ~ temp assignedKey = whatIs(key, relation)
        {assignedKey:
            <> {key} - {assignedKey} ({assignedKey}) 
        }
        ~ _debugPrintAssignments(keyList, relation) 
    }  

/*--------------------
    Relation API
--------------------*/

=== function validate(x1, x2, rel) 
/*
    { x1 && not list_item_is_member_of(x1, relationDatabase(rel, true)):
        [ ERROR: {x1} not a valid lhs for {rel} ]
        ~ return false
    }
    { x2 && not list_item_is_member_of(x2, relationDatabase(rel, false)):
        [ ERROR: {x2} not a valid rhs for {rel} ]
        ~ return false
    }
*/
    ~ return true

=== function relate(x1, rel, x2) 
    { not validate(x1, x2, rel):
        ~ return false
    }
    ~ return _relate(x1, x2, rel) 
    
    
=== function unrelate(x1, rel, x2)  
    { validate(x1, x2, rel):
        // rebuild the whole pair string 
        // this could be much, much faster if externalised 
        // or if we had a string subtraction
        ~ return _unrelate(x1, rel, x2)
        
    }
    ~ return false 
    

=== function unrelateAll(a1, a2)      
    {
    - list_item_is_member_of(a2, Relations): 
        ~ return unrelate(a1, a2, whatIs(a1, a2))
    - list_item_is_member_of(a1, Relations): 
        ~ return unrelate(whatIs(a1, a2), a1, a2)
    - else: 
        [ ERROR:    unrelateAll needs a relation!
    }    

=== function unrelateEverything() 
    ~ pairstore = ""

=== function whatIs(a1, a2) 
    {
    - list_item_is_member_of(a2, Relations): 
        ~ return getRelatesTo(a1, a2)
    - list_item_is_member_of(a1, Relations): 
        ~ return getRelatedFrom(a2, a1)
    - else: 
        [ ERROR:    whatIs needs a relation!
    }

=== function getRelatesTo(x1, rel)
    { not validate(x1, (), rel):
        ~ return ()
    }
    ~ return _getRelatesTo(x1, rel)


=== function getRelatedFrom(x2, rel)
    { not validate((), x2, rel):
        ~ return ()
    }
    ~ return _getRelatedFrom(x2, rel)


=== function isRelated(x1, rel, x2 ) 
    { validate(x1, x2, rel):
        ~ return _isRelated(x1, x2, rel)  
    }



/*--------------------

    Internal datastore functions. This level can be externalised.

--------------------*/    

EXTERNAL _getRelatesTo(x1, rel)
=== function _getRelatesTo(x1, rel)
    ~ temp searchSpace = LIST_ALL(relationDatabase(rel, false))
    ~ return __getMatchedPairs(x1, searchSpace, "{rel}", false)


EXTERNAL _getRelatedFrom(x2, rel)
=== function _getRelatedFrom(x2, rel)
    ~ temp searchSpace = LIST_ALL(relationDatabase(rel, true))
    ~ return __getMatchedPairs(searchSpace, x2, "{rel}", true)   

EXTERNAL _isRelated(x1, x2, rel) 
=== function _isRelated(x1, x2, rel) 
    {LIST_COUNT(x1) > 1 || LIST_COUNT(x2) > 1:
        [ERROR: Testing isRelated for {rel} on non-unary lists {x1} and {x2}. This would need adding as a feature, though it works on the game side! ] 
        ~ return false 
    }
    ~ temp relString = __pairString(x1, x2, "{rel}")  
    ~ return pairstore ? relString
    
    
EXTERNAL _relate(list1, list2, rel) 
=== function _relate(list1, list2, rel) 
    ~ temp additionalPairs = __getPairStringsFor(list1, list2, "{rel}", false) 
    ~ pairstore += additionalPairs
    ~ return additionalPairs != ""    

EXTERNAL _unrelate(list1, rel, list2) 
=== function _unrelate(x1, rel, x2)
    ~ temp removalPairs = __getPairStringsFor(x1, x2, "{rel}", true) 
    {removalPairs != "":
        ~ temp newPairstore = __rebuildPairStringExcept(LIST_ALL(Relations), x1, x2, rel) 
        ~ pairstore = newPairstore
    }
    ~ return removalPairs != ""

/*--------------------

    Internal datastore functions. This level relies on string handling

--------------------*/  

/*--------------------
    Relations are expressed via string elements
--------------------*/    
    

=== function __pairString(x1, x2, rel) 
    ~ return ":{x1}>{rel}>{x2};"


/*--------------------
    Relations can be searched for
--------------------*/    
    
=== function __getMatchedPairs(list1, list2, rel, getLhs)     
    ~ temp ret = ()
    { LIST_COUNT(list1):
    - 0:    ~ return () 
    - 1:    ~ temp el2 = pop(list2) 
            { el2: 
                { _isRelated(list1, el2, rel): 
                    { getLhs: 
                        ~ ret = list1        
                    - else: 
                        ~ ret = el2 
                    }
                }
                ~ return ret + __getMatchedPairs(list1, list2, rel, getLhs) 
            }
            ~ return () 
    - else: 
            ~ temp el1 = pop(list1) 
            ~ return __getMatchedPairs(el1, list2, rel, getLhs) + __getMatchedPairs(list1, list2, rel, getLhs) 
    }     
    

/*--------------------
    Relations can be appended
--------------------*/    
    
=== function __getPairStringsFor(list1, list2, rel, requireRelated) 
    { LIST_COUNT(list1):
    - 0:    ~ return ""
    - 1:    ~ temp el2 = pop(list2) 
            { el2: 
                ~ temp ret = ""
                ~ temp isCurrentlyRelated = _isRelated(list1, el2, rel)
                { isCurrentlyRelated == requireRelated: 
                    ~ ret = __pairString(list1, el2, "{rel}") 
                }
                ~ return __getPairStringsFor(list1, list2, rel, requireRelated) + ret
            }
            ~ return "" 
    - else: 
            ~ temp el1 = pop(list1) 
            ~ return __getPairStringsFor(el1, list2, rel, requireRelated) + __getPairStringsFor(list1, list2, rel, requireRelated) 
    }     
  
 
/*--------------------
    Unrelations require a full rebuild
--------------------*/    
  
    

    
=== function __rebuildPairStringExcept(allRels, x1, x2, rel)     
    ~ temp relEl = pop (allRels) 
    {
    - not relEl: 
        ~ return ""
    - relEl == rel:
        ~ return __validPairsIn(LIST_ALL(relationDatabase(rel, true)), LIST_ALL(relationDatabase(rel, false)), relEl, x1, x2) + __rebuildPairStringExcept(allRels, x1, x2, rel) 
    - else:
        ~ return __validPairsIn(LIST_ALL(relationDatabase(relEl, true)), LIST_ALL(relationDatabase(relEl, false)), relEl, (), ()) + __rebuildPairStringExcept(allRels, x1, x2, rel) 
    }
    
=== function __validPairsIn(list1, list2, rel, not1, not2) 
    ~ temp ret = ""
    { LIST_COUNT(list1):
    - 0:    ~ return "" 
    - 1:    ~ temp el2 = pop(list2) 
            { el2: 
                { _isRelated(list1, el2, rel) && not (not1 ? list1 && not2 ? el2): 
                    ~ ret = __pairString(list1, el2, "{rel}")        
                }
                ~ return ret + __validPairsIn(list1, list2, rel, not1, not2) 
            }
            ~ return ""
    - else: 
            ~ temp el1 = pop(list1) 
            ~ return __validPairsIn(el1, list2, rel, not1, not2) + __validPairsIn(list1, list2, rel, not1, not2) 
    }  
    
    