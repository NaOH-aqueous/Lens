This ink file is made to test read times

//declare a var that saves read times
VAR read_times = 0
* [Look at the mirror]
-> check_read_times

===check_read_times===

{ 
- read_times == 0 : 
    ~ read_times ++
    [This is your {read_times} time(s) read the dialogue.]
    -> mirror1

- read_times > 0:
    ~ read_times ++
    [This is your {read_times} time read the dialogue.]
    -> mirror2
}
->output

===mirror1===
You look at the mirror, that person you see looks quite like...you!
* look again -> check_read_times

* go away
-> output

===mirror2===
You try to avoid eye contact with the mirror, worrying that person inside is someone other than you.
-> output

===output===
- At the end, you have read the dialogue {read_times} times.
-> END