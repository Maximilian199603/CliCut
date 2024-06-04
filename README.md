# CLIMP3Cut
CLIMP3Cut is an abstraction between the hard to use FFmpeg and the user.

## Commands
### Cut  
this command takes in a target file and parameters for either front and or back and creates a new File in the directory named after the input with an [cut] appended  
Examples:  
1. cut C:\user\selected.mp3 -f 01:30 -b 02:00  
2. cut C:\user\selected.mp3 -f 01:30  
3. cut C:\user\selected.mp3 -b 02:00  