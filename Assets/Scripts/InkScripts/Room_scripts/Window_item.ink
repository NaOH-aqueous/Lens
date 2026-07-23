INCLUDE Globals.ink

 {
 - window_clean && not magnifier_get:
You cleaned up the window, now you can have a better view of the outdoor scene.
-> END

 - window_clean && magnifier_get:
You looked through the window. Nothing's better than the fresh air and sunshine in Monday morning. 
-> END

 - else:
The window is covered by a thick layer of dust.  
->END
 }