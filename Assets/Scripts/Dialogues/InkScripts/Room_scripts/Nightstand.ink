// Nightstand Dialogue

//the switch is on
VAR switch = true
VAR have_paper = false

This is your nightstand.
Where do you want to inspect?

 * [On the table]
 There is a lamp and an alarm on the table. 
 -> On_the_table
 * [Drawer]
 There are two drawers, top and bottom.
 -> Drawer
 * [Leave]
 You walked away.
-> DONE

//--------------------On the table---------------------
=== On_the_table ===
Which one would you inspect ?

* [Alarm]
-> alarm
* [Lamp]
->lamp

= alarm
It's 8:00 am now. 
-> Leave

= lamp
{- switch : 
The light switch is on.
-> switch_on
- else:
The light switch is off.
-> switch_off
}

= switch_on
Turn off?
* [Yes]
You turned off the switch.
~ switch = false
-> Leave
* [No]
-> Leave

= switch_off
Turn on?
* [Yes]
You turned on the switch.
~ switch = true
-> Leave
* [No]
-> Leave

//--------------------drawer------------------------
=== Drawer ===
Where would you inspect ?
* {top_drawer && bottom_drawer}[Leave]
-> drawer_conclude
* [Top drawer]
-> top_drawer
* [Bottom drawer]
->bottom_drawer

= top_drawer
 There're some trivial stuffs:
 some draft paper and pencil can be seen there.
 
 * { bottom_drawer} [Leave]
 -> leave2
 * [Back] ->Drawer
 
= bottom_drawer
There lies several blank copy paper.
* [Look closer]
-> look_closer
* { top_drawer} [Leave]
->leave2
* [Back] ->Drawer


= look_closer
You look closer and saw one unused paper towel.
* [Take it away]
~ have_paper = true
You took it away.
-> take_the_paper_but_never_inspect_top_drawer

= take_the_paper_but_never_inspect_top_drawer
* {not top_drawer} [Inspect top drawer?]
-> top_drawer
* [Leave]
-> DONE
//--------------------Leave & END -------------------

=== Leave ===
* { not On_the_table.alarm || not On_the_table.lamp} [Continue inspecting on the table]
-> On_the_table
* /*{ not On_the_table.lamp}*/ [Leave] 
You walked away.
->END
* {On_the_table.alarm && On_the_table.lamp } -> on_the_table_conclude
*-> END

=== on_the_table_conclude ===
  You have finished the inspection on the table.
  {not Drawer: ->drawer_not_check} 
  * [Leave] -> END
  
  = drawer_not_check
  Inspect the drawer?
  * [Yes]-> Drawer
  * [No] 
  You walked away.
  -> END

//--------------------Leave2 & END -------------------
=== leave2 ===
* {Drawer.top_drawer && Drawer.bottom_drawer } -> drawer_conclude
- You walked away.
-> END
=== drawer_conclude ===
You have finished the inspection of drawer.
-> END
