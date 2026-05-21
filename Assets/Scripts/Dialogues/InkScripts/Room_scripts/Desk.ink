//Desk dialogue
//bool variables
INCLUDE Globals.ink
EXTERNAL ReadDiary()
EXTERNAL WriteDiary()

This is your desk. 
A diary, pencil, and a planter can be seen here. You spend most of time here when you're not in bed.

-> Inspect
=== Inspect ===
Where do you want to inspect? 

 + [Diary]
 ->diary
 + [Pencil]
 ->pencil
 + [Planter]
 ->planter
 + [Leave] You walked away. -> END
 
= diary 
Read the diary?
+ [Yes]
~ read_diary = true
You read the diary. 
    ~ ReadDiary()
-> END

+ [No]
Nothing catches your interest at the moment.
-> back

//---------------------------------
= pencil
Write something?
{read_notes && read_diary:

+ [Yes] You seem to be interested in writing something at the start of today.<> #audio:writing2
    ~ WriteDiary()
-> END
+ [No] You are not in the mood right now. ->back

- else:

+ [Yes] Maybe not now. ->back
+ [No]  You are not in the mood right now. ->back
}

/*= write
{- read_note:
-> notetrue
- else:
 -> notefalse
}
= notetrue
You seems to be interested in writing something at the start of today. -> back

= notefalse
 -> back*/
//---------------------------------
= planter
Nothing resides in this planter right now, expect some bone-dry and malnutritional soil. -> back

=== back ===
+ [Back] ->Inspect