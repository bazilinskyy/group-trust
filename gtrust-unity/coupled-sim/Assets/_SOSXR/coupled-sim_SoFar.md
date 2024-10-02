---
date_created: 2024-08-13
date_modified: 2024-08-13
date_deferred:
date_due:
archived: false
tags: [Work/Meetingnotes, Work/Projects/SOLO/Francesco/Coupled_Sim, Work/DevLog, Programming/Unity]
---

# 02-10-2024
- [x] Remove animation from tree
- [x] Put billboard in front of City to try to help the occlusion culling
- [x] Driver HMD head
- [x] ~~Emperor's Rating unto button~~ Not doing this since the rating as is can work fine for the trust/no-trust situation. You tell your participant to press+hold the button for when they trust (or don't trust). It will log a timestamp every frame that it's held down. Use this as your ongoing measure of trust (or no trust).
- [x] Check textures roadworks
- [x] Make skybox smaller (was 32mb, now 8mb)
- [x] Disabled reflection probes 
- [x] Check further why there's such low FPS
- [x] Reduce texture sizes
- [x] Fix Emperor's Data handedness: make it work for both hands.
- [x] Turn off FPS counter
- [x] Turn off GazeTarget
- [x] Enlargen clippignplane (was 500, now 750)
- [ ] Create branch for Jom/Ugne to continue work in
- [ ] Turn off second monitor. 

- File `EyeTracking Keycodes` shows which keycode is used for what eyetracking system. You should turn off the eyetracking calibration in Varjo, and request it using the keycode
- Amended Emperor's Rating. It now takes input from either hand (not both at the same time). Hold down the trigger to indicate what you want them to indicate, keep holding it. Release to indicate the opposite of before (e.g. hold to trust, release to dispair). It no longer logs the rotational data. 
- Spedometer networked by having the Speedometer.cs only enabled on the Driver, the Client receives the rotation of the speedometer by network transform. 
- Explained to Jom how to make his own waypoint circuit


# 01-10-2024

Added new PC with Varjo Aero
Installed required software
Added Francesco_Walker acct
Downloaded repo
Installed driving steering wheel
Staticified IP (192.168.1.13), and set Firewall settings to connect (tested, worked)
Changed default IP address for Client to connect to, to this new machine.

Will get the environment that Jom made now, because they were in a different project.
Imported perfectly. Added the environment to `SOSXR` branch. 
This means that the `Roadworks` branch, and the branch `SOSXR_BeforeEnvironmentMerge` are now no longer needed.
Not sure on the `Routes` branch, I think that still needs syncing. 

On running: it is a bit slow in the editor. 12 FPS not doing anything.
Going through the scene in the Scene view is painfully slow. 
- Made everything in this scene to be 'static' -- Still 12FPS
- Bake occlusion culling with default parameters: 28 FPS
- There's quite a lot of opaque geometry that needs rendering
- Trees: Turned off 
  - Turned off Ambient Occlusion
  - Set Shadow Strength to 0 (was 0.8)
  - Set cast shadow off
- Removed all windows and glass (except cars)
- Still 30FPS. 
  - Now a lot of rendering goes to shadows. 
  - Will now bake the lights and GI. This will take all night. 
    - Baking was much faster than I thought, took about half an hour. 
    - FPS is now 45 at most parts (in the Editor then), 30 sometimes. 
    - Will have to test tomorrow if this is good enough. 
  - Set Pixel Light Count to 2 (was 4)
  - Set Realtime Reflection Probes off (was on)
  - Much better now: 90FPS in the city in some parts, and still 45FPS in the countryside. 
  - I think there may be a little too many trees. 
    - Disabling them does give us a little better results in the out-of-city environment, but that's not a viable option. 
- Set the culling dist at 250 (was 500) just makes it weird, doesn't improve the FPS. Put back to 500. 

I'll leave it for now. 


# 25-09-2024


## Autonomous Driving
Today we're going to need to test the Autonomous Driving thing. IDK why it sometimes flies off into oblivion.

Car went up a wall when coming back to the start of the test-route
Changed box collider on Waypoints to Sphere Colliders (these also needed to be Triggers, duh)
This seems to work for now. It could be that simply in the process of doing so I've set them to be at better positions.

The Car still ran into the wall after the first loop, on entering the first of the Waypoints, even with the SphereColliders.
Jom suggested to put the starting point a little further away from the corner, and to have it further away from the endpoint.
This seemed to work during 3+ tests


## Adding Autonomous Roles

Setting a new 'Role' is done in the Experiment Definition under 'Roles'
- Removed 'Pedestrian' role
- Removed HMIs from Automated
- Renamed to: Manual (instead of Driven) && Automated
- Spwawn point for 'driver' in automated car was called 'AI_Passenger'. I set it to identical spawn points of the Manual version (transform of 'Car + Driver')
- Created new Roles: Automated Fron Passenger / Backseat Passenger, with same spawn points to the Manual version, used same settings as Manual (Passive Passenger)
- Set camera index of all Autonomous to 0 (was 1), just like it is in Manual


## Others
Toggled the HMIs to be off (done in ClientHMIController.cs)
In de Manual car moet er geen BoxCollider op de main prefab parent (aan) staan. Deze is alleen nodig (en ook echt nodig) op de Automated car
Check that in the Console is an option to do 'Error Pause', this pauses the game when hitting a `Debug.LogError("")`, but none of the above levels. Could be useful, but to me it's confusing: I turned it off. 

## WIP
Also have the driven participant not be tied to the wheel but be able to move their hands





# 19-09-2024


Index 1 on the Managers > Player System > Driver Prefabs used to be "DrivableSmartCommon-no_driver", now replaced with copy of the manual car, now called "DrivableHatchbackCommon_DrivenPassengers_Automated". But maybe it should be Index 2?

Copied AICar script from bachelors

Added collider to car

Speed setting and acceleration setting was a bit confusing. Apparently there's another SpeedSetter in the scene, that was messing everything up. These were the "BrakeX_WaitInput" and the "BrakeZ_StartTrial".

Finally got auto driving working, at least some of the time. Needs more testing! Sometimes the car veers of elsewhere and crashes and/or flies across. 



# 11-09-2024

Was supposed to have a meeting with Jom and Ugne but they cancelled last minute due to planned train strikes this morning. Haven't heard from them re the 'roadworks'.

Will get the Obey from `coupled-sim` Bachelor's branch so that we can actually have any automated cars respond to the traffic lights. I want to have the Prefab especially since that was quite useful IIRC.

I also want to get from the other `coupled-sim` how it operates with the self-driven version. Does that have a waypointsystem, and how does that operate? Yes waypoint system. This is quite convenient actually. Will need to figure out what makes it tick so that it can come cleanly into this repo. 

Ik moet er voor zorgen dat alle Obey's uit zijn als het een Manual Car is, en aan zijn als het een AI car is. Or at least that the Obeys don't affect the manually driven car. 

To my knowledge moeten de AI car en de Manual Car vanaf hetzelfde punt starten, dus bij deze. Amend that: they shouldn't come from exactly the same point, because then they get spawned in the same thing, causing chaos. 

I added a basic waypoint route to test how the AI cars are faring. None are driving yet by themselves. With some coaxing I can get them to drive and stop for the Obey traffic lights, but that's not yet the same as driving for themselves. For instance, they don't want to restart driving once they've had to stop for a traffic light. 

Will look into this more another day. 

Will contact the students re a new appointment. 




# 31-08-2024

Disabled recurring Debug warnings about Lights not being part of car (since it's passenger)

## Meeting Pavlo & Jom (until 12:00)

### THINGS TO DO
Enable head colliders again (do check, since gave issues of violent flying last time).
Do also torso
Do hands (if possible) or elsewise steering wheel

Agreed not to create a unified log where all 3 PCs send over their log data to one central hub where it gets correlated. Pavlo was super insistent about this one, and worried that UNIX timestamp of the three logs would not be precise enough to combine the separate logs post hoc. Maarten does not have the time to organise this, and suggested that the UNIX timestamp should be precise enough since the logs happen every frame.

As an alternative:
Log a sync between devices if possible (?).
Log visual sync if possible (?)

Would be good to be able to have the eye tracking dot possible for recording, without participant seeing it. This can then be screen recorded for posterity.

Install new PC with Varjo + Car.
Swap out Varjo AERO for other Aero that has been electrically checked.

Still unclear on how/what kind of trust data is desired. Pavlo suggested that a hold/release of a button could indicate continuous trust/notrust. This would hopefully be alongside the Vector3/4 data, if possible. Maarten will check Emperor's rating, and change to allow Press/NoPress continuous __AFTER__ information from rest of team on what is the indention behind this.

Put spedometer in synced Host/Client mode. It is currently not synced to Client.

Check what happens with Avatar movement jerky headmotion. Pavlo noticed when sitting on the back seat that every minute or so the front _passenger_ jerked their head which didn't seem to correlate with normal head movent. Also passenger avatar is slightly through chair, but this may be due to not having used controllers/avatar hands.

Enable mirrors for Clients. On Client they are currently disabled. This looks weird.


-----


01-08-2024


Put an FPS counter on HMD

FPS was (when happy) between 30-40. Not good enough.

The Main scene has a tonnnnn of DrawCalls (734 if I'm not mistaken). Mostly due to Shadows. This can be greatly optimized, but however I don't have time to delve into this. I turned off all shadows from all the lightsources (come on… they were ALL set to Soft Shadows…?), and am rebaking the lightmaps.

Baking lightmaps failed.

Rebaked Occlusion Culling (4-0.2) since some of the buildings were still not working properly.

The system eats up 15gb of memory. Will install memory profiler to see if we can improve this.

Disabled Rear Mirrors
    Get's enabled automatically agian?

Disabled GLASS… Why is there glass?

NIGHT ENVIRONMENT BOEHNOOEH

Turned off SideWalkEast Big Main


-----

22-07-2024

Disabled gitignoring Logs because Jom needed logs to test their own pipeline. Uploaded all the prev logs to github. This feature needs be disabled when there's proefpersonen.

Used REAPER to create a loopable verison of the city audioscape, and placed it instead of original. Sounds quite nice. Alse upped the volume since it got downscaled on export from Reaper.

Since in previous testing the colliders that are used for the eyetracking on the avatars were causing the characters to be thrown violently from the car, I now try to disable in Prefab, only to re-enable 1 second after playing.
This seems to work out pretty fine: the passenger is not hurled out of the car upon spwaning.

However: Don't look out of the window or you'll be toast.

This means we'll also need to do something with layers: Varjo has own layers, as does Rig, as does the Driver and Passneger.
The Rig and the car shouldn't collide: that causes the passenger to be thrown out of the car.
The Varjo and the Passenger + Car and Driver shoudl collide, otherwise no repsonse on the eyetrack.

Major issue that took me 5 hours to figure out: another case of mistaken component. The eyetracker was broken… In the previous setup, the XR_Rig was already on the avatar. Now it gets spawned. When it was already on the avatar, the search starting at the 'root' of that gameobject was starting effectively from hte avatar on.
Now since it gets spawned, it started from whatever the 'new root' of the gameobject was: which could mean the car itself…. therefore the `transform.root.GetComponentInChildren<>(lalal); returned a completely different component than the intended one.
It didn't find the correct components for the eyetracker, namely the MAIN camera. Instead of finding us the MainCam, it found us one of the mirror cams…. Because of this, the calculation for the placement of the gazeTarget was off in some but not all directions (e.g. sideways was a mess, behind us was quite good, v confusing).
I was fully on the wrong warpath, thinking something in the calculation was fundamentally wrong, but not finding out where that may have gotten me astray.
Luckily, now fixed.

Made the Logs have much better names, it is now including a PC identifier. Currently that's the local IP, whcih may or may not be useful. But at least it's now obvious which PC made which log.

Will also rebake light since many issues with different light versions (?)
---

18-07-2024

Upgraded to 2022.3.37 because 2022.3.05 was really too old.

To allow a connection between multiple PCs
In Windows Defender Firewall -> Set to Advanced Settings. Look at Inbound connections. Filter by name. Scroll down to Unity. Select correct version of Editor. Check the entry for ' Public' which will have a red icon. Double click that one and set it to Allow.
Don't try to make another Rule 'over' it, because that will be conflicting with the already existing 'deny' rule.

I set all three computers to have all inbound connections for the Editor (2022.3.37) to be enabled.

G-Hub needs to be turned on for the car to get input from the steering wheel.

Set the eyetracking calibration of the Varjo to be 'never', and have Unity trigger it with a buttonpress (currently set to Keypad1)
That was also why the eye tracking got randomly triggered previously: it used ot be on Spacebar.

In the Car prefab there was a gameobject holding all the mirrors, and it was disabled (but got enabled on play), I enabled it by default.

Added speedometer into list of SynchyTransformy.

City ambience noize is not yet loopable

Car has sounds now.

We'll need to check the lighthouse placement, and/or instellingen van Varjo omdat als je de bril aanraakt die gelijk in pure paniek schiet.

Test with 2 ppl (one driver and one scared bijrijder went quite well). Will need to test with 3 people.


---


17-07-2024

Rig needed to go from inside the Prefab to getting spawned, but only on the local player. The wrong HMD kept getting linked to the wrong rig.
Turning off the 'remote' rig was niet helping out, since this was often the second instantiated rig, and the HMD would have bound to that one instead of to it's prior one.
This caused some headache, since a lot of components were relying on the instantiated rig, and those all needed to go and find the correct (!) rig.
It had to look for a rig inside it's own hierarchy, but not being able to find the other rig (such as what would happen on the Host who has as a car also the rig of the Client in it's hierarchy).
So that needed to be searched for in a subsection of the hierarchy.

The spawning of the rig caused some issues with the colliders, esp the one that consistitutes the head. This is a problem for the Eye Tracking, since that relies on a collider. I wonder if that would work with a non-rigidbody collider though? Maybe remove RB? Maybe kinematic? I don't know yet, will find out.

Repositioning the camera was an issue again, but only on the Passenger. Turns out that the CameraCounter had a reposition script on there as well, which I (once upon a time) rejigged to run with the saem keycode as the reposition proper script. That took me quite a while to figure out why it was behaving so very badly.
It wouldn't position the camera to the thing it was supposed to, but unlike last time: all the references were good. I found out because a component ABOVE my repositioner was also getting repositioned… That was due to the CameraCounter.cs madness. Removing that one made my life a lot better.

The transforms of the characters are not synced, which is still a bit of a mystery to me. Why is the position of the car proper being synced, but of the components inside the car, not?
My god this is a bit of a spaghetti bowl here. There's stuff all over the place.

IS THIS THE THING? "public Transform[] SyncTransforms;" in PlayerAvatar.cs?

YESSSS

So each PlayerAvatar has a list of Transforms called SyncTransforms. These are the transforms that are synced across the network.
The way I found out was because in the PlayerSystem are two methods, one which 'gathers' 'poses', and another which 'applies' them.
Now the (Avatar)Pose is a custom struct which deals with some intermediate stuff. On the PlayerAvatar are also two methods that are hooked into the things on the PlayerSystem I just mentioned.
These two methods actually reference the 'SyncTransforms', that's part of how I found out. Also: queriyng the length of the list of Vectors that made up the LocalPositions gave me 6 always, so that made me look for lists with 6 things innem.
Now this list of 'SyncTransforms' was burried somewhere under the header "Other" in the Inspector part of the PlayerAvatar script, so I made them a little more prominent.

The other reason was that the steering wheel strangely wasn't synced, but the actual wheels were. So if I turned left, the steering wheel stayed put, but the wheels themselves were always turning.
That meant that the entire car wasn't synced.

Right. So now I added the Targets for the IK system to the lists of SyncTransforms. That way we still keep the number of Transforms getting psyched to the minimum, whilst still having all the prettiness!

Good day.

----

15-07-2024

Took me a long minute to get the old repo into the new repo. I wanted to have it copy the entire commit history, but that was not to be. I needed to have Francescos credentials for this, which I don't have. I tried to get it done with my own creds, but that was not working out either. The inelegant way was to get the folders from the original repo, and simply dump them into the folder here in the new repo.

There had been a nasty Native Collection bug that kept throwing errors at shutdown. Turns out the netcode shutdown was not fully managed yet. It was not really shutting down. On the Host the problem is now solved, but on the Client not so much just yet.

Having multiple XROrigins in the scene was causing me a real headache. They cause the HMDs to respond to the wrong rigs, and messing up any chance of inhabiting just the one character. It turned out that the XROrigin rig from the Remote players was causing havoc. That one needed to go. After a bunch of testing I found out that simply disabling all child components of the Rig was best (not Destroying, which causes major issues, and also not simply disabling components, which doesn't disable all (e.g. camera is not a component).

Now what needs to happen is that the remote player actually syncs their own (local) position to their corresponding remote halves. This seems to happen in the PlayerSystem..?

Ran Rider code silent cleanup for more consistent code.

Finally: rebaking lgihtmaps



-----

25-06-2024

Exciting day! Got the steering wheel to work. Turns out that none of the mappings in Unity were correct in relation to the steering wheel.
Also, exciting enough: most values are mentioned at least twice (e.g. "Horizontal", "Throttle"). So we had a good hour or so trying to see what would change
the Throttle, before realizing that the second reference was effectively blocking our testing process. Anyhoo. Good fun.

We now have Throttle, Break, Steering, and what was already there: blinky light.

Next up: we need to install that steering wheel unto a PC that can handle a Varjo XR headset. Not all of them can, and the one the steering wheel is on, is unfortunately 'old'.
Then we can start testing the steering wheel icm with the HMD.

I added the possibility for the avatars to follow the position of the HMD, meaning that aside from head-rotation, we now also have a bit of upper-body movement (yeah baby).
This effectively makes it a little more realistic, since you can move around and your avatar actually does this too.
This effect can easily be broken, since if you move too far, you're gonna get some exorcist kinda moves. So there's gonna be some "participant participation" in maintaining this fragile illusion.

In conjunction to the previous comment: clothes now also render on the inside, so that if your HMD moves out of the area where the neck would meet the face, you don't just look into a bottomless void.
Also: your own face is now not rendered! That's good because you kept looking at your own eyeballs. Freaky.

Will have to test whether you can actually still see other people's eyeballs though.

Jom had some issues with data not being stored, that was due to something in the StreamWriter not working too well. ChatGPT was kind enough to fix it. However, later, Jom was having similar issues.
Those I've not been able to reproduce, the thing spits out data like a firehose. (data now lives in /Assets/_SOSXR/Logs/ btw)

When networking 2 characters, some very interesting things are happening however. Still a fair bit of work needs to be done here.


-----


24-06-2024

Trying to remove XROrigin. It seems to work just fine if we get rid of it, but I'm not 100% sure just yet.

That turned out to be not necessary! Jarik came by and we talked about the project. He noticed some objects that are meant to be Target points for the HMD,
such as for the Carrot etc. Turns out they were not finding their correct Target objects, and had to be manually placed.
This solved the problem with the positioning of the Client's recentering.

Will need to jiggle the layers on the faces. Shouldn't see your own face, but should see others's faces. Min-clip is not going to cut that.

Tried attaching some steer, but that didn't provide input to the car yet. Have asked Francesco re input re the input.

-----


13-06-2024
Characters are at least sort of bound to their own HMD and controllers.
The recentering is not always working correctly, and I have not yet an idea why. The recentering system is exactly the same across devices.
Maybe it's a Varjo / Steam setting?

I cannot get the recentering to work. On the Passenger it simply won't work as I expect it to do.


---

12-06-2024

Networking characters across the host-clients. Fairly slow process, trying to figure out how this is done in the first place,
and then why it's not working rn.






------


27-05-2024

Importing assets as requested in email.

We're now also logging the name of the collider we're looking at correctly. (Future Maarten: this was not yet correct: see collider/transform debacle).

Fixed rear wheel position (the WheelColliders referenced in the WheelVehicle where in the wrong place)

Removed global box collider (that was for when we still had some damn traffic)

Made car material 2-sided. Before you could look out the back and see through the metal bits of the car.

Added colliders to windows of vehicle. They still a bit opaque, so hard to see the road. Good luck! (will of course remove the Renderer later, but is good for positioning now).

It needs to poll for the name of the COLLIDER, not the TRANSFORM. With the Transform it will look towards the thing that has the RigidBody.
We're trying to get info from the thing that has the Collider. Therefore: _hit.collider.name and not _hit.transform.name

Added collider to driver and passenger both head and body. They're a little big, might have to play with making them smaller at the risk of losing hits.
Also risk hitting your own collider. The head positioning of your own avatar is still really messy. I haven't touched it so far, but regardless, it looks like it's going to give me some grief in the future.

Colliders like so seem to work quite well. Will probably have to rename the collider for head and for body to be clear which passenger /driver is being looked at:
e.g. driver should have "Driver_HEAD", and "Passenger_1_HEAD" etc or somesuch.

It does look like eyetracking has it's own data export and could be happiest just to leave it like that and combine with coupled-sim log post-study.




-----

08 May 2024

Taking out eyeballs.

Lot of refactoring of eyetracking and logeyetracking to make it easier to understand what it's actually doing.

It's now separated into two classes: the eyetracking and the logging class.
I think we're gonna use the projects' own logging, but this is useful to see what Varjo desires of us.



Maarten


----

07-05-2024

Will work on trust rating system, since WFH.

It looks like the XR_Origin doesn't have the controllers mapped out too much? None of the 'InputActions' are properly referenced.

I'm going to need at least the haptic system, and a button for starting the rating. Possibly the trigger button?
I've now mapped that under the 'Activate Action'.

I just wrote the TrustRater:
Should be fairly ambidextrous, and vibrates the left/right controller every x seconds. Then, the participant presses and holds a button (suggestion: trigger), while rotating the controller like a Roman Emperor. Thumbs up should provide us with a rating near the 0 degree mark, whereas doubtful is around 90 degrees. To die, is around 180 degrees. The rating shouldn't fall below 0 or above 180, but that requires some testing.

Where should the TrustRater live? I presume on the XR Rig? Will need to test.

What's then done with the trust-rating? That's the next thing to figure out. I'm assured there's some kind of Log?
It should export it's trust ratings frequently.
A lot (a lot) of refactoring went into this.
Stored much more data now: all the way from the original Quaternions.

I renamed the trustrater into EmperorsRating.


Maarten

----

02-05-2024


Testing revised Varjo script on XR3 HMD.
Testing eye tracking with HMD.
Gaze dot holds up well, tracks perfectly where I'm looking at.

No AIOs yet, but that shouldn't be too much of an issue.

We should probably control when the calibration is being done.
When it set to 'Always' in Varjo Base, it does sometimes want to calibrate mid-playthrough.

Positioning of the head is better, still not great. Due to limited movement of the avatar, it's still very common to look into your own eyeballs.

Testing new setup which enables both Bachelor && Master students to work on the same setup, without having to replug cables.



Maarten

----


30-04-2024

Setting some limits to the rotational axis of the Driver's face rig helps: -40 to 40 seems to be reasonable. No more exorcist.

Making the face look towards a 'carrot' also works much better. Now HMD rotations are taken into account, instead of only positional information.

Making the head point (with it's uppity bit) to a hat (aka 'puntmuts') also allows for the head to tilt.

Both the carrot as the puntmuts are attached to the XR Rig. The carrot is a bit in front of the camera, the puntmuts is above the camera.

Amended passenger model to more reflect the components of the Driver model

The chainIK constraint needs to be on something that's not rotating along with the XRRig, because otherwise the -40 to 40
limits simply won't apply. With the driver this is done by having that Target be sort of the headrest of the car.
With the passenger I should do something similar. Gonna need a car for that though, and something networked. So for another day.
--> I had the Target_Head_Position in there, which took its pos-rot from an object with tag HMD_AttachPoint.

Refactoring VarjoEyeTracking to understand it.


That's it for today!

Maarten



------

11-04-2024

Got XR3 to behave much better through Varjo Base 4.2.1. Less restart / diva issues.

Now reassigning shortcuts, because 'R' is resetting the camera position. No longer directly unto floor.
However, it does not match well with the 'Press R to Reverse'.
I know I removed the CameraCounter.cs from our prefabs, but just to be sure I've also set a bool in the inspector to turn it off.
This is because the CameraCounter gets spawned automatically.
It clearly is the Cameracounter that's doing this.

Problem is also that CameraCounter is calling something that's obsolete, and only works in stationary, not roomscale.
I'm going to keep the CameraCounter's implementation off, and set a keycode for my own recentering.
---> RECENTERING IS NOW NUM-0 (so the '0' key on the numpad)

Weird thing was that RecenterXROrigin.cs kept getting disabled in Start. No indication as to why or where.
Only when I removed the component from the prefab, and re-added it, did it get fixed. Now the thing stays enabled,
and we can use '0' to RecenterAndFlatten.

Updated VarjoEyeTracking to allow enabling/disabling moving of eyeballs. This should be disabled in our model.
Checked with model: eyes rotate according to tracking. Will need a second person with eyeballs to help fine-tune.

Will start making sure the passenger can have this too.

Before I got to the passenger, worked with Vennila on the eye tracking of the driver. It was behaving strangely.
It doesn't yet move in the same direction real-life eyeballs are moving, but something like this:
Left = down
Down = left
Right = up
up = right
Worked a little with the offset to see what's what, because also the model's eyeball is actually two-sided. It could
be that we're looking at the back of the eyeball instead of the front.
Should test next time by placing coloured markers on both the front and the back of the eyeball.

Also need to get more Varjo info.


Ttyl!

Maarten



--------------------------


29-02-2024
Moved worldlogger into uncommented file and got back the working version via Git.
Removed eye-ball-rotation from animation clip (that was 'blocking' eyeball for varjo)
Scratch that last bit. The Eyeball rotation in the animation clip wasn't the issue.
The thing was that the eyeball movement from Varjo needed to be done in LateUpdate, that makes it happen AFTER animation
I applied an offset to the thing, because the model is 84.354 off on Z.
IT now at least looks forward, but doesn't yet match my eye movement (e.g. looks down right when I am looking up)
However, here I really need a secodn pair of eyes, since I cannot both see and adjust the rotation of the offset to match my own eyemovements

Gtg for now.
Best
Maarten



19-02-2024

### HMD / CONTROLLER CONTROLLED AVATAR ###
I got the passenger very much hooked unto the XR rig. Hands and Face move along to where the HMD / controllers are.
It's a little rudimentary (e.g. controllers map to wrists, which is just a little weird) but that is really details for some later stage.
At some point we're gonna have to do something about the draw-distance, or possibly layers. Since it is a littl uncomfortable
to gaze into your own eyeballs. Also, the rigs should have limits. There's some definite exorcist vibes going on now.

Driver has now HMD positioning too. Controllers naturally do not sync his hands, since the good man needs hands on the wheel at all times.

The 'Press R to enter floorLevel hell' was caused by the CameraCounter.cs script. I don't think it did anything majestic,
so I took it off the prefabs we're using.


### EYE TRACKING ###
Started new branch SOSXR_Varjo. Here I can test a little with the eyetracking from Varjo.

Ok, the eyetrackingexample is quite clear, is good starting point. Also good: their HDRP Sample thing. That's a separate Unity Project.

Eyeball-rotation from Varjo interferes with the animator or the rig. Actually the other way around --> rig (or animator?) interferes with Varjo.
When animator off, rig doesn't work, but Varjo can move the eyeballs. Requires fixing.

That brings me to: moving the eyeballs. That really shouldn't happen. Is creepy AF. A little rotation is good, a little move is
baaaad. But that's a possibility to change in the Varjo SDK:
if (gazeData.leftStatus != VarjoEyeTracking.GazeEyeStatus.Invalid)
{
    leftEyeTransform.position = xrCamera.transform.TransformPoint(gazeData.left.origin);
    leftEyeTransform.rotation = Quaternion.LookRotation(xrCamera.transform.TransformDirection(gazeData.left.forward));
}

That's simply removing that position line. I think.

That's for next time to do :).

Next time: get eyeballs rotate nicely with gaze. This shouldn't be a lot of work.
Then test whether that works (you need a second set of… well.. eyeballs,
because the system de-inits the eye-tracker as soon as you remove your face from the HMD).

Then, we need to get that data synced.

I'm 85% confident that these above things can be done within about 1 day work.

After that there's the creating the AIOs, which is little to no work at all.
After that there's the logging the gaze data in some reasonable way. This will be unknown amount of work, since I'm not
familiar at all with the data logging procedure. Pavlo: any ideas?


Best,
Maarten





15-02-2024

Goal for today: go from 'car as passenger' to 'passenger as passenger'.

I'm encountering two errors, one dealing with Unity Transport, the other with a Stack Overflow. Both seem to be able to overcome by updating:
    <https://gamedev.stackexchange.com/questions/186134/a-native-collection-has-not-been-disposed-resulting-in-a-memory-leak-enable-fu>
    <https://issuetracker.unity3d.com/issues/resetting-event-queue-with-pending-events-error-thrown-after-connecting-client-to-server>
However I'll wait with updating. Don't know where it's coming from, it wasn't there last week, and there may be something simpler going on.

Now there's passengers inside the car who are actually passengers. Their prefab is still a mess and includes so much Car stuff. Will clean that up now.

Glad I didn't update the UnityTransport package. The issues seemed to have gone away once I picked up all the projects' null-errors.
Null errors dealing with car-lights etc, which are caused because a passenger doesn't really have a personal own spawned car, ergo no lights.
Once I dealt with all those, the connection errors seemed to have gone away.

Passengers are now happily in the car. I first disabled all car-components 'around them', and then once that was happy, I deleted all that.
The passenger has new 'Passenger' layer, which collides with nothing.
His RB cannot be kinematic, but can do !Gravity.

They do have an AnimationRig, which needs some placing.
I added new targets for Head, Hand and Hand. The Head has two constraints: a pos/rot and a lookat.
I don't think the lookat is needed for what we're after. I left it on there, but set the weight to 0.
The passengers respond fairly well to their rigs. The rig is not yet connected to HMD/Controllers

I noticed that only 192.168.0.11 can be host? 12 at least doesn't want to just yet. Maybe due to some firewall setting?

I will now start making sure that the VR portion of this experience works at least somewhat.

XR works in general. A passenger can have an HMD, as can the driver. Both HMDs neatly drive along with a driven car.
Need to test whether two passengers can both have an HMD. Then need to test whether 3 in car can all have HMD.

'R'-key is clearly a button to yoink the HMD to floor level. Need to figure out why that's there, and whether we can get rid of it.

The positioning of the HMD camera is horrendous. I'll first try to set it to the location of the face-target.

The repositioning of the HMD camera now works to a degree. We can reposition&flatten it to another object, which we can then add to the Prefab.
There's a Target_XROrigin thing on the prefab, with a tag, and the RecenterXROrigin component will go and look for that.
A weird thing that happens is that because there's this phase of initialisation prior to hitting "Start Simulation", the OnEnable method gets called and then
I think there's some messing around with the Time settings? Maybe the program is paused that way. Anyway, doesn't matter, but it does provide some interesting thing for the
recenter script to figure out when it should run.

That's all for today now.
Bye
Maarten




---------------------------


08-02-2024

You can either have 1 human passenger, or 2 cars as passenger, inside your manually driven car…

You can have 1 passenger (clone of pedestrian), as a passenger is fine. One manual car and the passenger sync perfect.
However, 2 is too many, then then they don't sync over the network.

This is also the case if you run with 3 pedestrians. None sync.

However, if you have three manually driven cars, all is good. They sync perfect.

So I tried making a car as a pedestrian: "Driven_Passenger_Madtest"…. and this works!
To make this work, it needs two separate spawn points, so that they're not colliding.
Needs to have the tag "ManualCar" removed, and in the PlayerAvatar set it as 'Driven_Avatar',
so that the PlayerSystem can parent it to the 'real' car.

Conclusion:
There's simply something about the prefab of the pedestrian which is conflicting with it being used as a 2nd passenger.

Next steps:
Create a prefab (maybe stripped down of the Hatchback car?) for a passenger which works, even when having 2 of them in the same manual car.



Best,
Maarten
