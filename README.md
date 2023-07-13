# Somewhat Off-Kilter Rideshare

This project is a sample application for use as a code test for Toca Boca.

__TO BE CLEAR__: This is not an example of a well-built project, or indicative of how games are built at Toca Boca.

Its purpose is to provide a functional, if somewhat dysfunctional, codebase for candidates to start with rather than having to build an entire game from scratch.

## Overview of the solution

The aim of this task, as I understood it, was to be able to show a replay of how the car was previously driven and to allow the player to still drive the car during the replay, thus allowing the player to race against a "ghost".

My solution was to generate a series of snapshots that would capture changes in the state of the keys that control the player's car. Capturing changes in input state provids the most compact format for storing replay data and takes advantage of the deterministic behaviour of the Unity simlation loop to "regenerate" the rest of the states that make up the replay e.g car position at a certain point in time given that accleration and rotation speed are the same.

I designed the ghost system (see the GhostSystem Prefab in the scene) to be self-contained. Given the time and scope of this test, I had the PlayerControl script initialize the GhostSystem, but apart from that step, PlayerControl has no knowledge of input being recorded or played back.

## “Happy path” to using the playback system

There are three UI button:

- Hit `Record` to start recording the player's input at any point, hit it again to stop recording.
- Hit `Playback` to spawn the ghost car and watch it retrace the player's route. When you are happy with a recorded run, you can then hit...
- `Race the Ghost` to have the game position the player's car at the starting point of the recording and start playing back the recording so that the player can race against the ghost.

## Steps I took to arrive at the solution

I knew from the start that I wanted to record key state deltas rather than car positions or multiple streams of keydown events. Deltas would give the most compact raw representation needed to replay the player's input and would also be more robust than recording values like floating point vector positions which are lossy would result in a very janky replay solution.

To capture a key delta requires only three values, the keycode identifying the key, the new state of the key and the time into the recording when the state change occurred. So on each frame I checked for state transitions in the keys I was interested in and generated a new snapshot if a change has occurred for a particular key. This gave me a sequence of snapshots that represented all the player's input during the recorded session. To round out the recording, I needed to also store the start time of the recording and the start position and orientation of the car.

I initially added the recording and playback logic to the PlayerControl class just to confirm that Unity's simulation loop was deterministic enough to work with the values in the snapshots. Once I found that I was getting good results from this initial implementation I thought about how best to refactor the ghost system into it's own thing so that the codebase was kept cleaner overall and PlayerInput was not cluttered with logic that was really not it's responsibility to begin with.

PlayerControl was still slightly refactored so that it could accept key input from multiple sources, which in turn allowed the GhostSystem to control a second car during replay.

Last of all I added support for restarting the level and positioning the player's car so that it could race against the "ghost".

## Hurdles

I encountered two main problems. The first was that if the recording was started while the car was already in motion, my initial implementation did not capture enough state to produce a faithful replay. The ghost car basically started stationery which diverted from what had actually taken place.

The second problem was at the end of the replay, the ghost car did not finish in the exact spot that the player's car was in when the recording was stopped.

Both this issues were solved by capturing more simulation state. For the first problem, I added code to capture the initial velocity of the car and snapshots of the state of the keys at the moment the recording was started.

And for the second problem, I added more snapshots at moment of the recording ending. Here it was not key states there were important, but the time offsets. These final snapshots ensured that the last deltas were applied long enough so that the replay input did not end prematurely.

## Next steps

- Add dependency injection and stop with the GameObject.Find calls.
- Add a countdown before starting the replay so that the player has time to get ready to race against the ghost.
- Restructure the Player prefab so that different vehicles can be attached under the Display game object allowing multiple models to be driving around.
- Better division of responsibilities and abstraction between objects (e.g. add an interface for providing input to PlayerControl).
- Refine the interface of GhostSystem and find a more elegant way to set up a ghost race.
- Reduce/remove of duplicated code.
- Profile for hotspots and memory consumption.
- Serialize recordings to disk so replays can survive across game sessions.
- Support multiple recordings.
- Add comments.
