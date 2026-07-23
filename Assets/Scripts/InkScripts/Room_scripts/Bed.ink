// Bed Inspection Script

INCLUDE Globals.ink

This is your bed, {~ you can stay here forever in rainy days. | You've probably spent more time here than anywhere else in the house.}

-> inspect
=== inspect ===
Where do you want to inspect? #item:clearDisplay

 * [On the bed]
 -> on_the_bed
 
 * {not magnifier_get && not shovel_get} [Under the bed] -> under_the_bed
 * {magnifier_get && not shovel_get} [Under the bed] -> have_magnifier_dialogue
 * {shovel_get} [Under the bed] -> leave
 
/* //check if any spots have been inspected
 * { inspect.on_the_bed || inspect.under_the_bed	 } [Back] 
 */
 
 * [Leave]
 Nothing catches your interest at the moment.
 -> END
 
 = on_the_bed
 You inspected the bed. The bedsheet had been soaked with sunshine and dust. It's the smell of home. #audio:grab
  * { inspect.on_the_bed && inspect.under_the_bed	 } -> conclude
  * -> inspect

  
 = under_the_bed
 You inspected under the bed. Nothing is there, expect darkness. #audio:grab #item:normal_bed
  * { inspect.on_the_bed && inspect.under_the_bed	 } -> conclude
  * -> inspect

#------------------------------------------------------------------------------------

 === have_magnifier_dialogue
  There's an ominous feeling arising... #item:Under_the_bed
  Something feels... different from before.
  * [Inspect] -> monster
  
  = monster
  #cutscene:show_monster
  Hello...? #speaker:Lens #portrait:nervous
  -> question
  
  = question
  Is there anything you want, little one? #speaker:Monster #audio:monster_normal
  + [Ask who he is] ->q1
  + [Ask for the thing you lost] ->q2
  
  = q1
Who are you, may I ask? #speaker:Lens #audio:lens_normal #portrait:nervous
I’ve been living underneath your bed all these years. And YOU, aren't even familar with me? #speaker:Monster
... #speaker:Lens #audio:lens_speechless #portrait:nervous
-> back0

  = q2
  
   I'm looking for um... a thing I lost. #speaker:Lens #portrait:nervous
 { sock_get == false: 
  This one, perhaps? #speaker:Monster #item:sock
  ~ sock_get = true
  No, not this one. This is the sock I lost long a ago. #speaker:Lens #item:clearItemOnly #portrait:normal 
  -> last_question
 - else:
 -> last_question
 }
  
  
  = last_question
  Uh huh? Then which one are you looking for? #speaker:Monster #audio:monster_serious
  + [Gold] It's better if you're not so greedy, child. #speaker:Monster
  -> back
  + {not shovel_get} [Shovel] I don't recall I have taken things like this before. #speaker:Monster 
  ->shovel_dialogue
  + {shovel_get} [Shovel] I have just returned it, child. #speaker:Monster
  -> back
  + [Gloves] I don't believe I still have those. #speaker:Monster 
  Don't you remember? I've returned it back last year already. #speaker:Monster
  -> back
  + [Leave]<> #item:clearDisplay
  -> END

= shovel_dialogue
... #speaker:Monster #audio:monster_speechless
... ... ... #speaker:Monster #audio:monster_speechless
~ shovel_get = true
Oh wait...Here it is. #speaker:Monster #item:shovel
Thank you, sir. #speaker:Lens #item:clearItemOnly #portrait:happy
There is no need for it, But take care, child. #speaker:Monster 
Just try not to leave your things with me again. #speaker:Monster 
-> back 

=== conclude ===
- You've finished inspecting the bed. #item:clearDisplay
    -> END
 
=== back0 ===   
+ [Back] -> have_magnifier_dialogue.question
=== back ===
+ [Back] -> have_magnifier_dialogue.last_question
=== leave ===
Nothing catches your interest anymore. -> END
