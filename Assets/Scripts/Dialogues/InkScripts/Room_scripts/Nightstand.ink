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
 There is a lamp and an alarm clock on the table. 
 -> On_the_table
 + [Drawer]
The nightstand has two drawers, top and bottom.
 -> Drawer
 + [Leave]
You step away from the nightstand.
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
You checked the alarm. It's 8:01 am now. 
-> Tback

= lamp
{- lamp_switch : 
The lamp is on. 
-> switch_on
- else:
The lamp is off.
-> switch_off
}

= switch_on
Turn off?
+ [Yes]
You turned off the lamp. #audio:switch_off
~ lamp_switch = false
-> Tback
+ [No]
The lamp is on.
-> Tback

= switch_off
Turn on?
+ [Yes]
You turned on the lamp. #audio:shiny
~ lamp_switch = true
-> Tback
+ [No]
The lamp is off.
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
   Nothing else here seems useful. -> after
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
You looked closer and saw one unused paper towel. #item:paper towel
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
{ - not time_get:
Take TIME with you?
* [Yes]
~ time_get = true
TIME has joined your team! #audio:shiny2 #item:time
Time's up! Lets go! #speaker:Time #audio:alarm
-> END
- else: 
No time to waste! LET'S GO! #speaker:Time #audio:alarm
-> END
}