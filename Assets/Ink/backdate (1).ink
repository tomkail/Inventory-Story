-> grave

=== grave 
>>> BACKGROUND (gravestone.png)
>>> TITLE (Paris Cemetery, May 5th 1968, 8:30pm)

LIST Flowers = (WeddingRing), (BunchFlowers), HeldFlowers, HeldWeddingRing, (Cigarette)
~ items(Flowers)
[ Wedding Ring reads: "E.R 1965" ]
>>> ITEMS: BunchFlowers
- (opts)
* [ INTERACT WeddingRing ]
    ~ swap(WeddingRing, HeldWeddingRing) 
* [ INTERACT BunchFlowers ]
    ~ swap(BunchFlowers, HeldFlowers) 
* ->  next("cafe_man.png", WornWeddingRing) ->  cafe 
-  -> opts 

=== cafe
>>> TITLE: Cafe de Opera, May 5th 1968, 5:30pm
[ A man in a dark suit sits across the table, smoking  ]

LIST CafeItems = (WeddingRingOnTable), WornWeddingRing, FullCoffee, (EmptyCoffee)
~ items(CafeItems)
- (opts)
*   [ INTERACT EmptyCoffee ]
    ~ swap(EmptyCoffee, FullCoffee) 
    
*   [ INTERACT WeddingRingOnTable ]
    MAN:    On the grave? Whatever you say.
    ~ swap(WeddingRingOnTable, WornWeddingRing) 
    
    
*   -> next("waiter.png", FullCoffee) ->  waiter 
-   -> opts 


=== waiter
>>> TITLE:  Cafe de Opera, May 5th 1968, 5:18pm
[ A waiter, holding a tray. There is a tattoo visible on his hand: a curved serpent. ] 
LIST WaitersTray = (FiveFrancNote), (CoffeeOnTray), (EmptyVial), FullVial
~ items(WaitersTray) 
- (opts) 
*   [ INTERACT EmptyVial ON CoffeeOnTray ] 
    ~ swap(EmptyVial, FullVial) 
    
*   -> next("cafe.png", FiveFrancNote) ->  cafe_earlier 
-   -> opts 

=== cafe_earlier
>>> TITLE: Cafe de Opera, May 5th 1968, 5:07pm
[ the other seat is empty ] 
LIST CafeEarlierItems =  (PocketBook) , OpenPocketBook, CondolenceCard
~ items(CafeEarlierItems + FiveFrancNote)
- (opts) 
*   (pk) [ INTERACT PocketBook ] 
    ~ swap(PocketBook, OpenPocketBook)
    ~ items(CondolenceCard)
    [ Card reads: "My condolences for your loss. But they will be watching for you. Q." ]
*   {spk} [ INTERACT OpenPocketBook ] 
    ~ swap(OpenPocketBook, PocketBook)    
*   (spk) {pk} [ INTERACT FiveFrancNote ON OpenPocketBook ]
    >>> REMOVE: {FiveFrancNote} 
*   -> next("limo.png", CondolenceCard) ->  END
-   -> opts



/* codey bits */

=== next(image, itemList)
    +   [ DRAGOVER {image} / {itemList} ] 
        >>> BACKGROUND: {image} 
        ->->

=== function items(list) 
    >>> ITEMS: {list}

=== function swap(olditem, newItem) 
    >>> REMOVE: {olditem}  
    {items(newItem)}  
    