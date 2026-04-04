// Bed Inspection Script

This is your bed, {~ you can stay inside forever in rainy days. | it's the place you spend time the longest amongst all of the furniture you own.}

-> inspect
=== inspect ===
Where do you want to inspect?

 * [On the bed]
 -> on_the_bed
 
 * [Under the bed]
 -> under_the_bed
 
/* //check if any spots have been inspected
 * { inspect.on_the_bed || inspect.under_the_bed	 } [Back] 
 */
 
 * [Leave]
 Nothing catches your interest at the moment.
 -> END
 
 = on_the_bed
 You inspect the bed. The bedsheet has been soaked with sunshine and dust. It's the smell of home.
  * { inspect.on_the_bed && inspect.under_the_bed	 } -> conclude
  * -> inspect

  
 = under_the_bed
 You inspect under the bed. Nothing is there, expect darkness.
  * { inspect.on_the_bed && inspect.under_the_bed	 } -> conclude
  * -> inspect
  

=== conclude ===
- You have finished the inspection of the bed.
    -> END
