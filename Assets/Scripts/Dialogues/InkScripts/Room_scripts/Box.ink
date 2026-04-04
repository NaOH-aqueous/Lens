// Box Dialogue
//declare a VAR to save look times
VAR look_times = 0

This is a box. 
Do you want to look inside?

-> take_a_look
=== take_a_look ===

 * [Yes]
 ~ look_times ++
 -> check_box
 
 * [No]
- Nothing catches your interest.
-> END
    
=== check_box ===
// switch blocks

{
- look_times == 4 : 
    You have checked 5 times. This task has already got a bit tedious.
    -> look_again
    
 - look_times < 7 :
    There is a box inside a box. 
    ->look_again
        
 - else:->7_times
}    
    
===look_again===
    Look again?
        + [Yes]
        ~ look_times ++
        -> check_box
        * [No]
        -> leave

=== leave ===
-You walk away.
-> END

=== 7_times ===
- You checked the box 7 times. There is nothing inside.
 -> END