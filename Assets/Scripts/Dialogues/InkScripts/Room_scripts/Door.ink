//Door Dialogue
INCLUDE Globals.ink
EXTERNAL EndScene()

->door_status

=== door_status ===

 * { not key_get} 
 -> door_locked
 * { key_get }
 -> door_opened

= door_opened
~ door_unlocked = true
 You unlocked the door.
* [Go outside]
    ~ EndScene()
-> END

= door_locked
You try to open the door, but the handle doesn't rotate a bit. #audio:door_lock
It's locked. Maybe try unlocking it first. #speaker:Lens #portrait:normal #audio:lens_normal
* [Leave]
-> END