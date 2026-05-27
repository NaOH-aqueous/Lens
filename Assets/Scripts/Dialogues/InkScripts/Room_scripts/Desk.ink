//Desk dialogue
//bool variables
INCLUDE Globals.ink
EXTERNAL ReadDiary()
EXTERNAL WriteDiary()
EXTERNAL InspectPlanter()

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
{not get_pages:
~ read_diary = true
    ~ ReadDiary()
-> END
- else: 
There are currently only blank pages in this diary.
}

+ [No]
Maybe not now.
-> back

//---------------------------------
= pencil
Write something?
{read_notes && read_diary:

+ [Yes] You seem to be interested in writing something at the start of today. #audio:writing2
    ~ WriteDiary()
-> diary
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
Nothing resides in this planter right now, expect some lifeless and malnutritional soil. #item:planter
* [Inspect]<>
  ~ InspectPlanter()
  ->END
* [Back] -> back

=== back ===
+ [Back] <> #item:clearDisplay
->Inspect