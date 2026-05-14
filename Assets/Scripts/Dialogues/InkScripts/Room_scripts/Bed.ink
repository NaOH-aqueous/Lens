// Bed Inspection Script

INCLUDE Globals.ink

This is your bed, {~ you can stay inside forever in rainy days. | it's the place you spend time the longest amongst all of the furniture you own.}

-> inspect
=== inspect ===
Where do you want to inspect?

 * [On the bed]
 -> on_the_bed
 
 * {not magnifier_get} [Under the bed] -> under_the_bed
 * {magnifier_get && not shovel_get} [Under the bed] -> have_magnifier_dialogue
 * {shovel_get} [Under the bed] -> leave
 
/* //check if any spots have been inspected
 * { inspect.on_the_bed || inspect.under_the_bed	 } [Back] 
 */
 
 * [Leave]
 Nothing catches your interest at the moment.
 -> END
 
 = on_the_bed
 You inspect the bed. The bedsheet has been soaked with sunshine and dust. It's the smell of home. #audio:search_bed
  * { inspect.on_the_bed && inspect.under_the_bed	 } -> conclude
  * -> inspect

  
 = under_the_bed
 You inspect under the bed. Nothing is there, expect darkness. #audio:search_bed
  * { inspect.on_the_bed && inspect.under_the_bed	 } -> conclude
  * -> inspect

#------------------------------------------------------------------------------------

 === have_magnifier_dialogue
  There's an ominous feeling arises... #item:Under_the_bed
  Something seems to be different than before.
  * [Inspect] -> monster
  * [Leave] ->END
  
  = monster
  Hello...? #speaker:Lens
  -> question
  
  = question
  Is there anything you want, little one? #speaker:Monster 
  + [Ask who he is] ->q1
  + [Ask for the thing you lost] ->q2
  
  = q1
Who are you, sir? #speaker:Lens
I’ve been beneathe your bed all these years. And YOU, aren't even familar with me? #speaker:Monster
... #speaker:Lens
-> back0

  = q2
  I'm looking for um... a thing I lost. #speaker:Lens
  This one, you say? #speaker:Monster #item:sock
  No, not this one. #speaker:Lens
  -> last_question
  = last_question
  Uh huh? Then which one are you looking for? #speaker:Monster 
  + [Gold] It's better if you're not so greedy, child. #speaker:Monster 
  -> back
  + {not shovel_get} [Shovel] I don't remember I have taken things like this before. #speaker:Monster 
  ->shovel_dialogue
  + {shovel_get} [Shovel] I have just returned it, child. #speaker:Monster
  -> back
  + [Gloves] I don't think I still own it anymore. #speaker:Monster 
  Don't you remember? I've returned it back last year. #speaker:Monster
  -> back
  + [Leave]<> #item:clearDisplay
  -> END

= shovel_dialogue
... #speaker:Monster
... ... ... #speaker:Monster
~ shovel_get = true
Oh wait...Here it is. #speaker:Monster #item:shovel
Thank you, sir. #speaker:Lens
There is no need for it, But take care, child. #speaker:Monster 
And remember, don't leave it to me next time. #speaker:Monster 
-> back 

=== conclude ===
- You have finished the inspection of the bed.
    -> END
 
=== back0 ===   
+ [Back] -> have_magnifier_dialogue.question
=== back ===
+ [Back] -> have_magnifier_dialogue.last_question
=== leave ===
Nothing catches your interest anymore. -> END
