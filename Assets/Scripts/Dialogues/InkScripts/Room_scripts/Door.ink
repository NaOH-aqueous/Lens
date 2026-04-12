//Door Dialogue
INCLUDE Globals.ink

->door_status

=== door_status ===

 * { not door_open } 
 -> door_locked
 * { door_open }
 -> door_opened

= door_opened
 You unlocked the door.
* [Go outside]
-> END

= door_locked
You try to open the door, but the handle doesn't rotate a bit. #audio:door_lock
It's locked. Maybe try unlocking it first. #speaker:Lens
* [Leave]
-> END
