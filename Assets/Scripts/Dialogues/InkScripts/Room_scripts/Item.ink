// Inventory dialogu
INCLUDE Globals.ink
EXTERNAL UseItem()
EXTERNAL CanUseItem()

->invent

=== invent ===
What are you going to do with it?

 + [Use]
 {CanUseItem():
    ~ UseItem()
    You used the item. ->END
 - else: 
 You can't use it now. ->back
 }
 + [Inspect]->END
 + [Back] -> END
 
 ===back===
 + [Back] ->invent

