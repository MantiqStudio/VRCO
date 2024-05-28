![VRCO](https://github.com/MantiqStudio/VRCO/assets/167381007/2f454ad1-07b6-4d2a-994a-bb05768a1e06)
## VRCO : Virtual reality command operator 
This is the system responsible for running simplified commands and the owner of the translation system into it

# The translator
because of vrco It is very difficult for human writing, so a responsible translator was created: vecoc the set of rules is implemented by an external administrator, such as a takwin language

# The method of work
First, there is a list containing the elements to be used in the following operations, which are added to the previous operations, as each command contains two elements:
- Key: the key of the task
- Value: the value for the task
example for add string to the list:
```py
[V]STRING>(string value)
```
example for use the last value in the list for print it:
```py
[O]GET>Console/Print
[T]Invoke>
```