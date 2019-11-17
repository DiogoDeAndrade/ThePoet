# The Poet

This is a game done for the 2st Game Design Club challenge (Poem) at the Universidade Lusófona Game Development degree.

In this game, the player has to build a poem in front of an audience, by combining sentences from a poetry database I've built.
Players are scored based on metric and rhymes.

The game is not a lot of fun, but it was very challenging and interesting to build.

![alt text](https://github.com/DiogoDeAndrade/obmr/raw/master/Screenshots/screen01.png "Title Screen")

## Tech stuff

This game was very interesting to build, a lot of different challenges to tackle.
I started by taking a text version of Fernando Pessoa's "Cancioneiro Negro", and breaking all poems into sentences. Then I used a Python script that used pyphen to break the sentences in syllables (not too accurately, but good enough for my needs). The script then goes to the last sentence of each phrase and builds a rhyme dictionary, using HTTP calls to https://www.rhymit.com. 
The script then exports a text file for both the phrases and rhymes. These are then ingested into Unity.
The game itself is pretty straightforward, except for the scoring system, which is still not very good, but it is a good starting point, if someone wants to take a stab at it (*wink* *wink*).
Each phrase the player selects gets matched to the current stanza to see if it has any rhymes, and how periodical they are.
For example, if you do a rhyme A-A-A-A, you get less points than if you do A-B-A-B.
The metric (length in syllables of a phrase) is also evaluated in the same way. 
If the player doesn't match either rhyme or metric, it gets a fail. You get enough fails, you get booed off-stage.
A possible improvement would be to add some sort of "structure score", which would evaluate the stanza as a whole. 
Another improvement would be to see how the stanzas measure up with each other.
A boredom coeficient could also be implemented: if the player is always using the same type of rhymes, it would get a fail.

The game basically fails as a game because of the randomness. If there is a lot of randomness in the phrases, the player will loose all the time, since the likelihood of having phrases that rhyme or have the same metric would be small. But the way it is (favouring things that rhyme and/or have the same metric) makes the game pretty trivial, except when the starting set is awful.

## Credits

* Code, art, game design done by Diogo de Andrade
* Sounds taken from freesounds.com (CC license)
* Music "My Love" by Shady Dave (https://freesound.org/people/ShadyDave/sounds/325611/)
* Font "Marck Script" by Denis Masharov

## Licenses

All code in this repo is made available through the [GPLv3] license.
The text and all the other files are made available through the 
[CC BY-NC-SA 4.0] license.

## Metadata

* Autor: [Diogo Andrade][]

[Diogo Andrade]:https://github.com/DiogoDeAndrade
[GPLv3]:https://www.gnu.org/licenses/gpl-3.0.en.html
[CC BY-NC-SA 4.0]:https://creativecommons.org/licenses/by-nc-sa/4.0/
[Bfxr]:https://www.bfxr.net/
