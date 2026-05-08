INCLUDE Globals.ink
//Window Dialogue

{
- not curtain_open:
->main
- else:
-> open_curtain
}

===main===
The window is covered by the curtain, isolating the room from the outside world.

 * [Pull the curtain open]
 ~ curtain_open = true
   The curtain has been pulled open. #audio:curtain_pull #speaker:Lens
    ->open_curtain
 * [Leave] ->END
 
===open_curtain===
{~  Pale, and cold sunshine are spilling from the window. | No matter how you open them wide, the sunlight won't be able to fill the entire room, leaving half of your living space in darkness.} 

 {window_clean:
 * [Inspect] You cleaned up the window, now you can have a better view of the outdoor scene. #item:window_clean
-> END
 - else:
 * [Inspect] The window is covered by a thick layer of dust. #item:window_dust
->END
 }
// * [Inspect] -> inspect
 * [Leave] ->END

/*=== inspect ===
{ window_clean: 
You cleaned up the window for a better view of the outdoor scene.
-> END
- else:
The window is covered by a thick layer of dust.
->END
} 
*/