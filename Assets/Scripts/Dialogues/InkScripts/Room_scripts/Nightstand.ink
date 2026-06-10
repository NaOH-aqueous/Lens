// Nightstand Dialogue
INCLUDE Globals.ink
//the switch is on
{ - door_unlocked:
-> clock

- else:
-> main
}
=== main ===
This is your nightstand.
->question
=== question ===
Where do you want to inspect?

 + [On the table]
 There is a lamp and an alarm on the table. 
 -> On_the_table
 + [Drawer]
 There are two drawers, top and bottom.
 -> Drawer
 + [Leave]
 You walked away.
-> DONE

//--------------------On the table---------------------
=== On_the_table ===
Which one would you inspect ?

+ [Alarm]
-> alarm
+ [Lamp]
->lamp
+ [Back]
-> question

= alarm
It's 8:00 am now. 
-> Tback

= lamp
{- lamp_switch : 
The light switch is on. 
-> switch_on
- else:
The light switch is off.
-> switch_off
}

= switch_on
Turn off?
+ [Yes]
You turned off the switch. #audio:switch_off
~ lamp_switch = false
-> Tback
+ [No]
The light is on.
-> Tback

= switch_off
Turn on?
+ [Yes]
You turned on the switch. #audio:shiny
~ lamp_switch = true
-> Tback
+ [No]
The light is off.
-> Tback

//--------------------drawer------------------------
=== Drawer ===
Where would you inspect ?
//* {top_drawer && bottom_drawer}[Leave]
//-> drawer_conclude

+ [Top drawer]
-> top_drawer
+ [Bottom drawer]
->bottom_drawer
+ [Back] -> question
//go back to the first question 
= top_drawer
 There're some trivial stuffs: some draft paper and pencil can be seen there.
  + [Back]->Drawer
//* { bottom_drawer} [Leave]
//-> leave2

= bottom_drawer
{ - have_paper == false: 
There lies several blank copy paper. -> before
  - else:
  There is nothing useful in the bottom drawer. -> after
}

= before
* [Look closer]
-> look_closer
//* { top_drawer} [Leave]
//->leave2
+ [Back] ->Drawer

=after
+ [Back] -> Drawer

= look_closer
You look closer and saw one unused paper towel. #item:paper towel
* [Take it away]
~ have_paper = true
You took it away. #item:clearDisplay
-> take_the_paper_but_never_inspect_top_drawer

= take_the_paper_but_never_inspect_top_drawer
* {not top_drawer} [Inspect top drawer?] -> top_drawer
+ {top_drawer && bottom_drawer}[Back] -> Drawer
=== Tback ===
// always a back
+ [Back] ->On_the_table

=== clock ===
Take TIME with you?
* [Yes]
~ time_get = true
TIME has joined your team! #audio:shiny2
Time's up! Lets go! #speaker:Time #audio:alarm
-> END
