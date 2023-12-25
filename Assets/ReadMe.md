# Bugs

Fade Screen does not work in Game Scene so it is not used for now

## Only Web GL

- Once you chose reward skilss, and then you dead in the following games, after that if you do not choose any reward skills, you can use the same skills which you chose the first time
- If you chose the Lv2 Laser as reward skills, sometimes when you faced boss and you use that skill, the game stop.
- Opning Video cannot play: Instead of that It is played Sprite like Gif
- If you played this game in 2. and if you expand as full screen, the Health UI position does not fit
  - 1. https://sharemygame.com/@SodapopHiguchi/stellar-nexus-revival
  - 2. https://simmer.io/@SodapopHiguchi/stellar-nexus-revival

# I used the fowwling external assets

- Dotween

  - for fade music in / out
  - (In terms asteroid, I used just lerp instead of it)

- [UIBars](https://assetstore.unity.com/packages/tools/gui/uibars-27564)

  - for health bar
  - By using this, the colour of health bar and health bar can change smoothly.

- [free sound](https://freesound.org/)
  - for 2 BGM and 4 SFX

# MEMO

## Bugs

**Sometimes I can't use laser gun. I wonder if it has something to do with the timing of the function execution?**

- Design a functonal laser gun for the player. The laser atack is instantaneous.
  - This laser atack should be bound to the right mouse buton.
  - Laser should use a raycast and be accompanied by visual feedback.
  - The player should only be able to use this atack every 0.4 seconds.
  - The laser will destroy the enemy ship on contact and will not pass through it a�er first contact.

**After the second shot, the enemy suddenly disappears. I have no idea what the cause is.In addition, although they can touch the bullets fired by the enemy, they cannot extinguish the bullets or damage the player after touching them.**

- Make it so certain enemies can shoot bullets at the player. The bullets should disappear on
  contact with the player or a�er three seconds and deal damage to the player.

## What could not be implemented due to lack of time

- Animate the Player or Enemy ships (you may need to find new sprites for this)
- Implement temporary power-ups for the player (e.g., invincibility, extra health, double shot,
  screen-clearing bomb)
- Implement a scoring system (e.g., 10 points per enemy killed) and a hi-score table.

## What I implemented

Explain what features have been implemented.

### General

- Make it so the player cannot exit the visible area of the screen (Week 3)
  - It is done, however I disabled this function(GameObject) due to “looping” movement system
- Implement looping background music that plays during gameplay (Week 6)

#### Week 9

- In the PlayerController script, implement the ability for the Player’s ship to rotate to face the mouse position. Briefly comment this code explaining how it works.

- Create a new Bullet object and a new Bullet script.
- Using the Bullet script and the PlayerController script, implement the ability for the Player’s
  ship to shoot projec�les. These projec�les should:

  - Spawn at an “Origin Point” just in front of the ship.
  - Spawn when the player presses the Left Mouse Buton.
  - Travel in the direcction that the player’s ship is currently facing (Hint: this can be obtained from the PlayerController).
  - Self-destruct after 3 seconds.
  - Feature a speed variable initialised at 10 that can be directly edited in the Inspector.
    - The speed variable should control the bullet’s velocity, which should also be
      framerate independent.

- Edit the Enemy script so that enemies:

  - Can collide with the Player.
  - Are destroyed when they collide with the Player.
  - Can collide with bullets.
  - Are destroyed when they collide with bullets.

- Edit the PlayerController script so that:

  - The Player takes damage when they collide with an enemy ship.
  - When the Player’s currentHealth <= 0, a gameOver event is broadcast.

- Create a “Timer UI” child object on the Canvas. Use the Anchor Presets menu to expand the object to fill the Canvas.
- Create a Text – TextMeshPro object as a child of the Timer UI. Name this “Timer Text” and anchor it to the top right of the Canvas. Make it look cool.
- Create a Game Manager object in the Hierarchy. Reset its transform.
  - Add a GameManager script to the Game Manager object.
- In this script, add code to create a �mer in the Update() func�on. The �mer should:
  - Be displayed in minutes and seconds: e.g., 10:10.
  - Keep coun�ng as long as the game isn’t over.
  - Update the Timer Text on the Timer UI in the Canvas.
- In the same script, add a GameOver() func�on.
  - Use this func�on to switch oﬀ the �mer (hint: bool) and save the final �me (i.e., the
  - �me at which the player dies) in a variable to be displayed on the Game Over screen.
- Back in Unity, make sure the Timer UI text updates appropriately.
- Create a new, empty child object of the Canvas. Use Anchor Presets to stretch it to fill the
  screen. Name it “Game Over UI”.
  - Create a new Panel as a child of the Game Over UI and change the colour to
    something other than white.
  - Create a new Text – TextMeshPro object as a child of the Game Over UI and name it
    “Game Over Text”. Enter “Game Over”, centre it, and make it look cool.
  - Duplicate the above text and name it “Final Time Text”. Place it just below the Game
    Over text.
  - Assign the Final Time Text to the appropriate variable field in the Game Manager
    object.
  - Turn oﬀ the Game Over UI object.
- In the PlayerController on the Player object, make it so the Game Over event does the
  following:
  - Trigger the GameOver() func�on in the GameManager script.
  - Disable the Timer UI.
  - Turn on the Game Over UI.
  - Trigger the SelfDestruct() func�on in the PlayerController script.
- Create a new Scene file in your Scenes folder. Call this “Main Menu Scene” (Hint: duplicate the Game Scene and rename it)
- Open the Main Menu Scene and delete everything from the Hierarchy except:
  - The Game Manager
  - The Canvas
  - The Main Camera
  - The Event System
- In the Canvas:
  - Disable the Timer UI, Game Over UI, and (if you have one – see next sec�on) the Health UI.
  - Create a new child object of the Canvas, stretch it to fill, and name it “Main Menu UI”.
  - Add some �tle text for your game. Make it look cool!
  - Add clearly labelled “Start Game” and “Quit” TextMeshPro butons.
- Add func�onality into your Game Manager script so that:

  - When you press the “Start Game” buton, the Game Scene loads.
  - When you press the “Quit Game” buton, the game quits.

#### Week10

- Create a new empty GameObject in the Hierarchy. Call it “Enemy Manager”. Reset thetransform.
- Now add a new empty GameObject called “Spawn Points”. Reset the transform. This will be acontainer for our Spawn Points.
- Create at least four Spawn Points as children of the Spawn Points container object. Place these outside the camera view.

#### Week11

- Create an explosion par�cle eﬀect that features at least three of the following proper�es:
- One or mul�ple sub-emiters
  - Trails
  - Bursts
  - Value changes over �me (speed, size, colour)
  - Random-between-two-constants (speed, size, life�me)
- Prefab the par�cle eﬀect and name it “Death Explosion”. Remember to delete the eﬀect from the Hierarchy when it’s prefabbed.
- Spawn the par�cle eﬀect from the SelfDestruct() method in the PlayerController script. To do this, you’ll need to:
  - Instan�ate the par�cle eﬀect at the player’s posi�on.
  - Ensure the par�cle eﬀect plays for the en�rety of its dura�on.
- Add a simple “thruster” eﬀect to Enemy ships using the Trail component.

### Intermediate

- Implement a Health UI that displays the player’s remaining health and updates when it changes.
  - I implimated with external assets
- Change the aspect-ra�o of the game from the 4:3 default in the template to a more modern
  16:9. Remember to:
  - Make sure the new aspect ra�o applies to the Game Scene and Main Menu Scene.
  - Change the reference resolu�on in the Canvas.
  - Make sure the background and game elements are placed and scaled appropriately for the new aspect ra�o.
  - Make sure the UI looks good and is scaled appropriately for the new aspect ra�o.
- Develop an Enemy Wave system, spawning w/ at least five dis�nct waves featuring at least two dis�nct enemy varia�ons. Again, how you approach this is up to you, but you can find an example of a wave-based enemy spawning system using scriptable objects in the Week 10 lecture.
- Enemy varia�ons should be saved as prefabs. Diﬀerent enemy types should look and
  behave diﬀerently: e.g., use a diﬀerent sprite or colour, move more quickly, deal more damage, have more health etc.
- Your waves should loop indefinitely – when the final wave completes, a new loop should commence with the first wave.
- Each itera�on of the loop should increase the diﬃculty of the game: e.g., by making enemies slightly faster, decreasing �me between spawns, or increasing the number of enemies spawned in each wave. Comment the code to explain how you did this.
- Design a func�onal laser gun for the player. The laser atack is instantaneous.
  - This laser atack should be bound to the right mouse buton.
  - Laser should use a raycast and be accompanied by visual feedback.
  - The player should only be able to use this atack every 0.4 seconds.
  - The laser will destroy the enemy ship on contact and will not pass through it a�er first contact.
- Implement the following game feel/juice elements.
  - Screen shake when the player is damaged.
  - Sounds eﬀects for:
    - Bullets
    - Laser
    - Explosions
  - An animated background using sprite-based anima�on or par�cle systems (e.g., stars
    zooming downwards)

## Advanced

- Crea�vely implement Lerping. This could be used to:
  - Smoothly transi�on between values on a health bar
  - Smoothy transi�on between colours on a sprite (e.g., when damaged)
  - Smoothly move an object across the screen (e.g., a large obstacle, like an asteroid)
  - Smoothly fade music in/out
- Instead of having the player constrained within the boundaries of the screen, implement a “looping” movement system like the ones in Pac-Man or Asteroids: i.e., when the player exits the play area on one side of the screen, they are immediately “looped” to the other side.
