// Inventory dialogu
INCLUDE Globals.ink
EXTERNAL UseItem()
EXTERNAL CanUseItem()
EXTERNAL InspectItem()

->invent

=== invent ===
What are you going to do with it?

 + [Use]
 {CanUseItem():
    ~ UseItem()
    ->END
 - else: 
 You can't use it now. ->back
 }
 + [Inspect]
 ~ InspectItem()
    ->END
 + [Back] -> END
 
 ===back===
 + [Back] ->invent

