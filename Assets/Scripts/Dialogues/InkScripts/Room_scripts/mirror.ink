INCLUDE Globals.ink
This is your mirror. #audio:mirror_sound


* [Look at the mirror]
-> check_read_times

===check_read_times===

{ 
- read_times_mirror == 0 : 
    ~ read_times_mirror ++
    //[This is your {read_times} time(s) read the dialogue.]
    -> mirror1

- read_times_mirror > 0:
    ~ read_times_mirror ++
    //[This is your {read_times} time read the dialogue.]
    -> mirror2
}
->output

===mirror1===
You look at the mirror, that person you see looks quite like...you.
* [go away]
-> output

===mirror2===
You try to avoid eye contact with the mirror, worrying that person inside is someone other than you.
-> output

===output===
- You walked away.
-> END