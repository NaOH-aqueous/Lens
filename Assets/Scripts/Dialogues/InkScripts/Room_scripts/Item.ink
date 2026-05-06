// Inventory dialogu
INCLUDE Globals.ink

->invent

=== invent ===
What are you going to do with it?

 {can_use:
 + [Use] You used the item. ->back
 - else:
 + [Use] You can't use it now. ->back
 }
 + [Inspect]->END
 + [Back] -> END
 
 ===back===
 + [Back] ->invent

