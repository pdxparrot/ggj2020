# TODO

* https://docs.unity3d.com/ScriptReference/PrefabUtility.html
* https://github.com/Unity-Technologies/NavMeshComponents
* Tear these apart for ideas
  * https://www.youtube.com/watch?v=qsU4nM0L_n0
    * https://learn.unity.com/project/3d-game-kit
    * https://learn.unity.com/project/2d-game-kit
* Show a progress bar when initializing a project
* Add a button to the project settings window to run initialization
* Missing a few packages we should be adding
  * Asset Bundle Browser
  * Android Logcat
  * HD/Lightweight Render Pipeline (optional - whichever best fits the project)
  * Burst/Entities/DOTs stuff (if using ECS)
* Add Keijiro Kino
  * TODO: This currently isn't possible until 2019.3 is out (the installation process has completely changed)
  * Add "jp.keijiro.kino.post-processing": "https://github.com/keijiro/kino.git#upm" to package manifest.json dependencies
    * This has changed to a scoped registry? will have to investigate how to actually do this
* Need to create InputSystem settings asset
* Need to import TextMesh Pro Essentials
* Use Mirror for networking
* Add features for ENABLE_VR and ENABLE_GVR and ENABLE_SERVER_SPECTATOR and USE_NAVMESH
* Create Data/Animation/empty.controller Animation Controller in project setup
* Create Data/Audio/main.mixer Mixer in project setup
* Add button-click.mp3 and button-hover.mp3 to the common GitHub and download them to Data/Audio/UI
* Create the EventSystem prefab at project setup ?

# Project Creation

* Create a new Unity Project
* Copy .gitignore from common GitHub repo
  * https://raw.githubusercontent.com/pdxparrot/assets/master/.gitignore
* Copy LICENSE from common GitHub repo
  * https://raw.githubusercontent.com/pdxparrot/assets/master/LICENSE
* Create README.md
* git init
* Close the project and do not save it

# Pre-Setup

* Copy Engine Assets/Scripts/Core/Editor/Window/, Assets/Scripts/Core/Editor/Project/, Assets/Scripts/Core/Editor/Util.cs, and Assets/Scripts/Core/Editor/ScriptingDefineSymbols.cs
  * TODO: simplify this
  * Remove .meta files
* Copy Engine Assets/Editor contents
  * Remove .meta files
* Open and close the project once for the build process to setup
  * TODO: this shouldn't be necessary...
* Open the new Unity Project and the project should automatically initialize
  * Say No to enabling the new Input System backend (initializing will set this up instead)
* Copy the rest of the Assets/Scripts/Core contents
  * Remove .meta files
* Create ASMDEFs
  * Assets/Scripts/Core/com.pdxpartyparrot.Core.asmdef
    * References: Unity.InputSystem, com.unity.cinemachine, Unity.Postprocessing.Runtime, Unity.TextMeshPro TODO: , Kino.Postprocessing (whenever we can bring that back)
    * Reference com.unity.multiplayer-hlapi.Runtime if using networking
  * Assets/Scripts/Core/Editor/com.pdxpartyparrot.Core.Editor.asmdef
    * Editor platform only
    * References: com.pdxpartyparrot.Core.asmdef, Unity.TextMeshPro
* Clean up TODOs as necessary
* Remove any FormerlySerializedAs attributes

# Project Setup

* Graphics Settings
  * Set the Render Pipeline Asset if desired (https://github.com/Unity-Technologies/ScriptableRenderPipeline)
    * This will require creating the asset first, which itself may be configured as desired
* Input System Package
  * Create the Input System Settings asset if not already done
    * Process events in Fixed Update
* Tags and Layers
  * Add the following layers if they don't exist:
    * PostProcessing
    * NoPhysics
    * Vfx
    * Viewer
    * Player
    * NPC
    * World
    * Weather
* Physics Settings
  * Only enable the minimum necessary collisions
    * **TODO:** water?
    * Vfx -> Vfx
    * Viewer -> Weather, World
    * Player -> Weather, World, NPC (and Player if that's the desired behavior)
    * NPC -> Weather, World (and NPC if that's the desired behavior)
    * World -> Weather
* Physics 2D Settings
  * Only enable the minimum necessary collisions
    * **TODO:** water?
    * Vfx -> Vfx
    * Viewer -> Weather, World
    * Player -> Weather, World, NPC (and Player if that's the desired behavior)
    * NPC -> Weather, World (and NPC if that's the desired behavior)
    * World -> Weather
* Player Settings
  * Set any desired Splash Images/Logos
  * Color Space: Linear (or Gamma if targeting old mobile/console platforms)
    * Fix up any Grahics API issues that this might cause (generally this means disabling Auto Graphics APIs on certain platforms)
  * Enable Static and Dynamic Batching if they aren't already
  * Verify that the Bundle Identifer is set correctly
* TextMesh Pro
  * Import TMP Essentials if not already done
  * Optionally import TMP Examples & Extras if desired

# Packages

* Add release packages
  * Asset Bundle Browser
* Add preview packages
  * Android Logcat (optional)
  * HD/Lightweight Render Pipeline (optional - whichever best fits the project)
  * Burst/Entities (if using ECS)

# Asset Store Assets

* DOTween (not Pro)
  * Make sure to run the setup
  * Make sure to create ASMDEF
  * Make sure to enable DOTween in the PDX Party Parrot Project Settings
* If using Spine, download the latest Spine-Unity package (currently 3.8+) and import it
  * Assets/Spine* must be added to the .gitignore to prevent committing this
    * TODO: this should already be done in the common .gitignore
  * The ASMDEF will need to be force added to source control
    * If the ASMDEF does not exist, your version is too old!

# Engine Source

## Game Scripts

* Copy Game Scripts
  * Remove .meta files
* Create ASMDEF
  * Scripts/Game/com.pdxpartyparrot.Game.asmdef
    * References: com.pdxpartyparrot.Core.asmdef, Unity.InputSystem, Unity.TextMeshPro
    * Reference com.unity.multiplayer-hlapi.Runtime if using networking
  * Scripts/Game/Editor/com.pdxpartyparrot.Game.Editor
    * Editor platform only
    * References: com.pdxpartyparrot.Game.asmdef
* Clean up TODOs as necessary
* Remove any FormerlySerializedAs attributes

## Initial Project Scripts

* Create Loading Manager
  * Create a new project LoadingManager script that overrides Game LoadingManager
* Create ASMDEFs
  * Scripts/{project}/com.pdxpartyparrot.{project}.asmdef
    * References: com.pdxpartyparrot.Core.asmdef, com.pdxpartyparrot.Game.asmdef
    * Reference Unity.InputSystem, com.unity.multiplayer-hlapi.Runtime, and Unity.TextMeshPro as required

## Set Script Execution Order

* Unity EventSystem
* TextMeshPro
* InputSystem PlayerInput
* pdxpartyparrot.{project}.Loading.LoadingManager
* pdxpartyparrot.Core.Time.TimeManager
* pdxpartyparrot.Game.State.GameStateManager
* Cinemachine PixelPerfect
* Default Time
* Cinemachine Brain
* pdxpartyparrot.Core.Debug.DebugMenuManager
  * This must be run last

# Engine Asset Setup

* Create Data/Animation/empty.controller Animation Controller
* Create Data/Audio/main.mixer Mixer
  * 3 Master child groups
    * Music
      * Expose the Volume parameter and set it to -5db
        * Rename it to MusicVolume
    * SFX
      * Expose the Volume parameter and set it to 0db
        * Rename it to SFXVolume
    * Ambient
      * Expose the Volume parameter and set it to -10db
        * Rename it to AmbientVolume
  * Expose the Master Volume parameter and set it to 0db
    * Rename it to MasterVolume
  * Add a Lowpass filter to the Master group
  * Rename the default Snapshot to Unpaused
  * Create a new Snapshot and name it to Paused
    * Set the Lowpass filter to 350Hz
* Copy button-click.mp3 and button-hover.mp3 to Data/Audio/UI
* Data/Prefabs/Input/EventSystem.prefab
  * Create using default EventSystem that gets added automatically when adding a UI object
  * Replace Standalone Input Module with InputSystemUIInputModule
  * Add EventSystemHelper script to this
  * Copy the DefaultInputActions asset to Assets/Data/Input (can rename if desired)
    * Replace the EventSystem InputActions with this copy

## Server Spectator

* TODO: This is all wrong now
* Create Data/Input/ServerSpectator.inputactions
  * Generate C# Class
    * File: Assets/Scripts/Game/Input/ServerSpectatorControls.cs
      * Need to create containing directory first
    * Class Name: ServerSpectatorControls
    * Namespace: pdxpartyparrot.Game.Input
  * Action Maps
    * ServerSpectator
      * Actions
        * move forward button
          * press and release w
        * move backward button
          * press and release s
        * move left button
          * press and release a
        * move right button
          * press and release d
        * move up button
          * press and release space
        * move down button
          * press and release left shift
        * look axis
          * mouse delta
  * Add ENABLE_SERVER_SPECTATOR to the Scripting Define Symbols
* **TODO:** ServerSpectator prefab and viewer
  * These would attach to the GameStateManager

## Manager Prefabs Setup

* Managers go in Data/Prefabs/Managers
* ActorManager
  * Create an empty Prefab and add the ActorManager component to it
* AudioManager
  * Create an empty Prefab and add the AudioManager component to it
  * Create an AudioData in Data/Data and attach it to the manager
    * Attach the main mixer to the data
    * Ensure all of the Parameters look correct
  * Add 4 Audio Sources to the prefab
    * Disable Play on Awake
  * Attach each audio source to an audio source on the AudioManager component
* DebugMenuManager
  * Create an empty Prefab and add the DebugMenuManager component to it
* EffectsManager
  * Create an empty Prefab and add the EffectsManager component to it
* EngineManager
  * Create an empty Prefab and add the PartyParrotManager component to it
  * Create a UIData in Data/Data and attach it to the manager
    * Attach a TMP_Font Asset to the Default font
      * LiberationSans SDF is currently the default TMP font
  * Attach the frictionless physics materials
  * Set the UI layer to UI
* GameStateManager
  * Create an empty Prefab and add the GameStateManager component to it
  * Create an empty Prefab in Data/Prefabs/State and add the MainMenuState component to it
    * Set the Scene Name to main_menu
    * Set the MainMenuState as the Main Menu State Prefab in the GameStateManager
  * Create an empty Prefab in Data/Prefabs/State and add the NetworkConnectState component to it
    * Set the NetworkConnectState as the Network Connect State Prefab in the GameStateManager
  * Create an empty Prefab in Data/Prefabs/State and add the SceneTester component to it
    * **TODO:** This actually needs to be overriden so it can do stuff like allocate viewers
    * Check Make Scene Active
    * Set the SceneTester as the Scene Tester Prefab in the GameStateManager
* InputManager
  * Create an empty Prefab and add the InputManager component to it
  * Attach the EventSystem prefab
* LocalizationManager
  * Create an empty Prefab and add the LocalizationManager component to it
  * Create a LocalizationData in Data/Data and attach it to the manager
* NetworkManager
  * Create an empty Prefab and add the (not Unity) NetworkManager component to it
  * Uncheck Don't Destroy on Load
* ObjectPoolManager
  * Create an empty Prefab and add the ObjectPoolManager component to it
* SaveGameManager
  * Create an empty Prefab and add the SaveGameManager component to it
  * Set the Save File Name
* SceneManager
  * Create an empty Prefab and add the SceneManager component to it
* SpawnManager
  * Create an empty Prefab and add the SpawnManager component to it
  * Create a SpawnData in Data/Data and attach it to the manager
* TimeManager
  * Create an empty Prefab and add the TimeManager component to it
* UIManager
  * Create an empty Prefab and add the UIManager component to it
* ViewerManager
  * Create an empty Prefab and add the ViewerManager component to it

## GameManager

* Create a new GameManager script that overrides the Game GameManager
  * Implement the required interface
* Add a connection to the project GameManager in the project LoadingManager
  * Override CreateManagers() in the loading manager to create the GameManager prefab
* Create a new GameData script that overrides the Game GameData
* Create an empty Prefab and add the GameManager component to it
* Create a GameData in Data/Data and attach it to the manager
  * Set the Viewer Layer to Viewer
  * Set the World Layer to World
  * Configure the Players section as desired

## Player

* Create a new NetworkPlayer script that overrides the Game NetworkPlayer
  * Implement the required interface
  * This component should require a NetworkAnimation component
* Create a new Player script that overrides one of the Game Players
  * Implement the required interface
  * This component should require the NetworkPlayer component

### PlayerBehavior

* Create a new PlayerBehavior script that overrides one of the Game PlayerBehaviors
  * Implement the required interface
* Create a new PlayerBehaviorData script that overrides one of the Game PlayerBehaviorDatas

### PlayerInput

* Create a new PlayerInput script that overrides one of the Game PlayerInputs
  * Implement the required interface
* Create a new PlayerInputData script that overrides the Game PlayerInputData

### PlayerControls

* Create Data/Input/PlayerControls.inputactions
  * Generate C# Class
    * File: Assets/Scripts/{project}/Input/PlayerControls.cs
      * Need to create containing directory first
    * Class Name: PlayerControls
    * Namespace: pdxpartyparrot.{project}.Input
  * Add Action Maps as necessary
* Have the project PlayerInput implement the action interface
  * TODO: this is probably broken now that we're using the InputSystem PlayerInput
* The PlayerInput should set the actions callback handler to itself
  * TODO: also probably broken

### Player Prefab

* Create an empty Prefab and add the Player component to it
  * Layer: Player
  * This will require a collider to be added first
  * Check the Local Player Authority box in the Network Identity
  * Attach the empty animator controller to the Animator
    * This will stop potential animator error spam
  * Attach the Animator to the Network Animator
  * Attach the NetworkPlayer to the Network Player on the Player component
* Add a new empty GameObject under the Player prefab (Model)
  * Attach this to the Model on the Player component
  * The actual model for the player should go under this container
* Add a new empty GameObject under the Player prefab (Behavior) and add the PlayerBehavior component to it
  * Attach the Player to the Owner on the PlayerBehavior component
  * Attach the Player Behavior to the Behavior of the Player component
  * Add one of the Player Movement components to the Behavior (or another empty GameObject under the player) and attach it to the Movement of the PlayerBehavior
    * Attach the Player Behavior to the Player Movement component
    * Attach the Rigidbody on the Player to the Movement Rigidbody
  * **TODO:** Animator on the Player Behavior ???
* Add a new empty GameObject under the Player Prefab (Input) and add the PlayerInput component to it
  * Attach the Player to the Owner on the PlayerInput component
  * Add an InputSystem PlayerInput and a PlayerInputHelper as well
    * Hook up the required events
* Create a PlayerInputData in Data/Data and attach it to the PlayerInput component

### Player / Game Viewer




* Cinemachine Viewer
  * Add CinemachineBrain to Camera
  * Create a new layer for each potential viewer
  * **TODO:** Need to make sure we put each viewer on its own layer
  * Set the Virtual Camera Body to Transposer
  * Set the Virtual Camera Aim to Composer




* **TODO:** OLD
* Create a new Player/GameViewer script that overrides one of the Core/Game Viewers and implements the IPlayerViewer interface
  * Implement the required interface
* Create an empty Prefab and add the project Viewer script to it
  * Layer: Viewer
  * Add a camera under the prefab (Camera)
    * Clear Flags: Solid Color (or Skybox for a skybox)
    * Background: Opaque Black (or Default for a skybox)
    * Remove the Audio Listener
    * Add a Post Process Layer component to the Camera object
      * Set the Layer to PostProcessing
      * Uncheck Directly to Camera Target
  * Attach the Camera to the Viewer component
  * Add another camera under the prefab (UI Camera)
    * Clear Flags: Solid Color
    * Background: Opaque Black
    * Remove the AudioListener
    * Add the UICameraAspectRatio component to the UI Camera
  * Attach the UI Camera to the Viewer component
  * Add an empty GameObject under the prefab (PostProcessingVolume) and add a Post Process Volume to it
  * Attach the Post Process Volume to the Viewer component
  * **Create the Post Process Layer (one per-viewer, Viewer{N}_PostProcess)**

## PlayerManager

* Create a new PlayerManager script that overrides the Game PlayerManager
  * Implement the required interface
* Add a connection to the project PlayerManager in the project LoadingManager
  * Create the PlayerManager prefab in the overloaded CreateManagers() in the project LoadingManager
* Create an empty Prefab and add the PlayerManager component to it
* Attach the Player prefab to the Player Prefab on the PlayerManager
* Create a PlayerBehaviorData in Data/Data and attach it to the PlayerManager component
  * Set the Actor Layer to Player
  * Set the Collision Check Layer Mask to Worl

# Splash Scene Setup

* Create and save a new scene (Scenes/splash.unity)
  * The only object in the scene should be the Main Camera
* Setup the camera in the scene
  * Set the Tag to Untagged
  * Clear Flags: Solid Color
  * Background: Opaque Black
  * Culling Mask: Nothing
  * Projection: Orthographic
  * Uncheck Occlusion Culling
  * Disable HDR
  * Disable MSAA
  * Leave the Audio Listener attached to the camera for audio to work
  * Add the UICameraAspectRatio component to the camera
* Remove the Skybox Material
* Environment Lighting Source: Color
* Disable Realtime Global Illumination
* Disable Baked Global Illumination
* Disable Auto Generate lighting
* Add the scene to the Build Settings and ensure that it is Scene 0
* Add a new GameObject to the scene (SplashScreen) and add the SplashScreen component to it
* Attach the camera to the Camera field of the SplashScreen component
* Add whatever splash screen videos to the list of Splash Screens on the SplashScreen component
* Set the Main Scene Name to match whatever the name of your main scene is
  * The main scene should also have been added (or will need to be added) to the Build Settings along with any other required scenes

# Main Scene Setup

* Create and save a new scene (Scenes/main.unity)
  * The only object in the scene should be the Main Camera
* Setup the camera in the scene
  * Set the Tag to Untagged
  * Clear Flags: Solid Color
  * Background: Opaque Black
  * Culling Mask: Nothing
  * Projection: Orthographic
  * Uncheck Occlusion Culling
  * Disable HDR
  * Disable MSAA
  * Leave the Audio Listener attached to the camera for audio to work
  * Add the UICameraAspectRatio component to the camera
* Remove the Skybox Material
* Environment Lighting Source: Color
* Disable Realtime Global Illumination
* Disable Baked Global Illumination
* Disable Auto Generate lighting
* Add the scene to the Build Settings

## Loading Screen Setup

* Add a new LoadingScreen object to the scene with the LoadingScreen component
  * Layer: UI
  * Add a new Canvas object below the LoadingScreen and attach it to the LoadingScreen
    * UI Scale Mode: Scale With Screen Size
    * Reference Resolution: 1280x720
    * Match Width Or Height: 0.5
    * Remove the Graphic Raycaster
    * Remove the EventSystem object that gets added (or turn it into a prefab if that hasn't been created yet)
* Add a Panel under the Canvas
  * Disable Raycast Target
  * Color: (255, 0, 255, 255)
* Add a TextMeshPro - Text (Name) under the Panel
  * Text: "Placeholder"
  * Center the text
  * Disable Raycast Target
* Add an Empty GameObject (Progress) under the Panel and add the ProgressBar component to it
  * Pos Y: -125
* Attach the ProgressBar component to the LoadingScreen component
* Add an Image under the Progress Bar (Background)
  * Move the image below the Name text
  * Color: (0, 0, 0, 255)
  * Size: (500, 25)
  * Source Image: Core Progress Image
  * Disable Raycast Target
* And an Image under the Background Image (Foreground)
  * Position: (0, 0, 0)
  * Size: (500, 25)
  * Source Image: Core Progress Image
  * Disable Raycast Target
  * Image Type: Filled
  * Fill Method: Horizontal
  * Fill Origin: Left
  * Fill Amount: 0.25
* Attach the images to the ProgressBar component
* Add a TextMeshPro - Text (Status) under the Progress Bar
  * Pos Y: -75
  * Size: (750, 50)
  * Text: "Loading..."
  * Center the text
  * Disable Raycast Target
* Attach the Text to the LoadingScreen component

## Loader Setup

* Add an empty GameObject (Loader) and add the project LoadingManager component to it
* Attach the Main Camera
* Attach the LoadingScreen to the Loader
* Attach the Manager prefabs to the Loader

# Main Menu Setup

* **TODO:** In the future, make a Button prefab to stick in everything

* Create a new MainMenu script that overrides the Game MainMenu
* Create a MainMenu Prefab in Prefabs/Menus and add the Game Menu component to it
  * Layer: UI
  * Add a Canvas under the prefab
    * UI Scale Mode: Scale With Screen Size
    * Reference Resolution: 1280x720
    * Match Width Or Height: 0.5
    * Set the Canvas on the Menu object
  * Add a Panel under the Canvas (Main)
    * Remove the Image
    * Add a Vertical Layout Group
      * Spacing: 10
      * Alignment: Middle Center
      * Child Controls Width / Height
      * No Child Force Expand
    * Add the MainMenu script to the panel
      * Set Owner to the Menu object
      * Set the Main Panel on the Menu object to the Main panel
  * Add a Button (TextMeshPro) under the Main panel (Start)
    * Normal Color: (255, 0, 255, 255)
    * Highlight Color: (0, 255, 0, 255)
    * Add an On Click handler that calls the MainMenu OnStart method
    * Add a Button Helper to the button
    * Add a Layout Element to the Button
      * Preferred Width: 200
      * Preferred Height: 50
      * Text: "Start"
      * Center the text
      * Disable Raycast Target
    * Set the Main Menu Initial Selection to the Start Button
  * **TODO:** Multiplayer
  * **TODO:** Character Select
  * Duplicate the Start Button (High Scores)
    * Set the On Click handler to the MainMenu OnHighScores method
    * Set the Text to "High Scores"
    * **TODO:** Setup the High Scores panel
  * Duplicate the Start Button (Credits)
    * Set the On Click handler to the MainMenu OnCredits method
    * Set the Text to "Credits"
    * **TODO:** Setup the Credis panel
  * Duplicate the Start Button (Quit)
    * Set the On Click handler to the MainMenu OnQuitGame method
    * Set the Text to "Quit"
  * Attach the Start button to the Initial Selection on the Main Menu
  * Attach the MainMenu prefab to the MainMenuState Menu Prefab

## Main Menu Scene Setup

* Create and save a new scene (Scenes/main_menu.unity)
  * The only object in the scene should be the Main Camera
* Setup the camera in the scene
  * Set the Tag to Untagged
  * Clear Flags: Solid Color
  * Background: Opaque Black
  * Culling Mask: Everything
  * Projection: Orthographic
  * Uncheck Occlusion Culling
  * Disable HDR
  * Disable MSAA
  * Remove the Audio Listener
* Remove the Skybox Material
* Environment Lighting Source: Color
* **TODO:** Setup Lighting
* Add the scene to the Build Settings
* Add a new TitleScreen object to the scene
  * Layer: UI
  * Add a new Canvas object below the TitleScreen
    * UI Scale Mode: Scale With Screen Size
    * Reference Resolution: 1280x720
    * Match Width Or Height: 0.5
    * Remove the Graphic Raycaster
    * Remove the EventSystem object that gets added (or turn it into a prefab if that hasn't been created yet)
  * Add a Panel under the Canvas
    * Disable Raycast Target
    * Color: (255, 0, 0, 255)
  * Add a TextMeshPro - Text (Status)
    * Pos Y: 256
    * Text: "Placeholder"
    * Center the text
    * Disable Raycast Target
  * The scene should now load when the main scene is run as long as the name of the scene matches what was set in the MainMenuState prefab

# Game States

## MainGameState

* Create a new MainGameState script that overrides the Game MainGameState
  * Implement the required interface
* Create an empty Prefab and add the MainGameState component to it
* Update the state object to initialize the server and client as necessary
* Update the GameManager to initialize what it needs to as well

## TODO: GameOverState

## TODO

* **TODO:** More GameStates
* **TODO:** Player UI
* **TODO:** Pause / Pause Menu
* **TODO:** Create the PlayerManager script/prefab
  * This must be a prefab due to the abstract base class
* **TODO:** Create the Player script/prefab
* **TODO:** How to controls
* **TODO:** Creating Data
* **TODO:** Credits

# Game Scene Notes

* Game scenes must be added to the Build Settings
* Game scenes require at least one SpawnPoint tagged with the player spawn tag in order for a player to spawn

# Performance Notes

* Mark all static objects as Static in their prefab editor
