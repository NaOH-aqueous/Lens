//Door Dialogue

VAR door_open = false

Do you have the key?
* [Yes]
~ door_open = true
-> door_status
* [No]
-> door_status

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
You try to open the door, but the handle doesn't rotate a bit.
(maybe try to unlock it first) 
* [Leave]
-> END
