// Inventory dialogu
VAR can_use = false

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

