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
{~  Pale, and cold sunshine are spilling from the window. | No matter how you open them wide, the sunlight won't be able to fill the entire room, leaving half of your living space in darkness.} ->END

