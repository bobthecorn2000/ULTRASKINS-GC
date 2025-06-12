# ULTRASKINS GC
Fixing up tonys ultraskins to work without UMM, this project is forked from that

# what do
UltraSkins is a mod that allows you to change the textures of your arms and the guns

# Install notes 
After installing you will need to run the game once before the mod will work
this will generate the files needed for the mod to function
PLEASE BACKUP YOUR CUSTOM SKINS BEFORE UPDATING OR UNINSTALLING

## IMPORTANT NOTE
after a new skin is selected swap your gun once and change your arm once to finish the update
skinsfolder should go in the folder where the dll is located

inside your directory create a folder (name it whatever) and put the skin files in that, (OG-SKINS has been provided as an example of what it should look like)

## so whats different and what should i know
1. removed umm as the manager is outdated, everything should still work
2. custom colors using the in game color customizer might be a bit funky lookin
3. pluginconfig is no longer needed

# if building
to build ultraskins yourself you will need a copy of ultrakill
in the in the repo navigate to the UltraSkins folder, in it create a folder called libs and add the following from your ultrakill installs ```ULTRAKILL_Data/Managed``` folder
1. Assembly-CSharp.dll
2. NewBlood.LegacyInput.dll
3. Unity.Addressables.dll
4. Unity.ResourceManager.dll
5. Unity.TextMeshPro.dll
6. UnityEngine.AssetBundleModule.dll
7. UnityEngine.UI.dll
8. plog.dll
9. usUI.dll (obtained through the official thunderstore Ultraskins Mod Download)

if you have any issues let me know on discord by submitting a bug report here: [https://discord.gg/Gp3bcJFj](https://discord.gg/FtuT5ys)
or submit an issue on github
