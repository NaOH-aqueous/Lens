//origami planting dialogue
INCLUDE Globals.ink
EXTERNAL TakeFlower()

{
- shovel_get && origami_get:
You plant the origami flower into the soil. #audio:shiny #item:planter_with_flower
The light is too dim here.#speaker:Lens
Mabye take it to somewhere brighter could help it grow better.#speaker:Lens
* [Take it away]
    ~ planter_get = true
    ~ TakeFlower()
-> END

- not shovel_get && origami_get:
...I can't plant it without using a shovel. #speaker:Lens

-> END
- else:
I haven't got anything to plant yet. #speaker:Lens
-> END
}
