# Battle Cats Game Modding Tool

A game modding tool for the game The Battle Cats<br>
Most features should work across all game versions (least likely is libnative patch)

I have a discord server: https://discord.gg/DvmMgvn5ZB, it's the best way report bugs and you can leave your suggestions for new features to be implemented in the tool

If you want to support my work and keep me motivated to continue to work on this project then maybe consider gifting me some ko-fi here: https://ko-fi.com/fieryhenry

## Thanks To

EasyMoneko for the original keys for decrypting/encrypting: https://www.reddit.com/r/battlecats/comments/41e4l1/is_there_anyone_able_to_access_bc_files_your_help/)<br>
Battle Cats Ultimate for what some of the numbers mean in various csvs.[BCU · GitHub](https://github.com/battlecatsultimate)<br>
This resource for unit csvs: https://pastebin.com/JrCTPnUV)<br>
Vi on discord for enemy csvs<br>

## Features

Decrypt .pack and .list files found in the apk and /data/data/jp.co.ponos..../files.<br>
Encrypt a folder of files and pack it into encrypted .pack and .list files<br>

Modify the libnative-lib.so file found in the:<br>
APK<br>
/data/data/jp.co.ponos..../files<br>
/data/app/jp.co.ponos..../<br>
To remove the check for the md5 sum for the .pack and .list files - avoids Data Read Error h01

Modify the stats of cat units<br>
Modify stage data<br>
Modify enemy stats<br>

## How To Edit Game Data

1. Unpack the apk file for the game using apktool/APK Easy Tool

2. Get the .pack and .list files that contain the files you want to edit:<br>
   Most stats are in DataLocal<br>
   Most text is in resLocal<br>
   Sprites are in various Server files<br>

3. Run the tool - Download exe from [releases](https://github.com/fieryhenry/Battle-Cats-Game-Modder/releases)

4. Select option to decrypt .pack

5. Select .pack files that you want

6. Once completed the files will be in /game_files

7. You can manually edit the data, or use the option in the editor that you want

8. Once edited, open the tool and select the encrypt option

9. Select the folder of the game files

10. Once complete the encrypted files will be /encrypted_files

11. Get your libnative-lib.so file for your system architecture. You can find it in the apk, /data/app/jp.co.ponos.battlecats.../{architecture}/, or in /data/data/jp.co.ponos.battlecats.../

12. Run the tool and select the option to patch the libnative-lib.so file

13. Enter the system architecture the file is from

14. Once done:
      Replace the file in the apk for permanent change(apk must be signed - APK Easy Tool - for most devices to install the apk) (must re-install app/replace apk in /data/app/jp.co.ponos.battlecats.../base.apk if you choose this).<br>
      /data/app to work without re-installing for your device.<br>
      /data/data only if you modifed server files for your device.<br>
    I recommend doing 1 and 2 for local files. And all 3 for server files.

15. Open the game and see if it works
    
    ---

16. If you modifed server files, you will need to find the associated download.tsv file for your .pack and .list files in the apk in /assets/{language}.

17. Open the file in notepad, you will see the name of the file, then a tab, then the file size in bytes, then a tab, then the md5 hash of that file.

18. You need to modify that md5 hash so that the game doesn't re-download the server data

19. Go to here: https://emn178.github.io/online-tools/md5_checksum.html and drag and drop the file in

20. Copy the hash and replace the one in the tsv with that one.

21. Replace the apk in /data/app/jp.co.ponos.battlecats.../base.apk with your apk, use apktool/APK Easy Tool to sign the app and pack it into an apk again.

22. Open the game and see if it works.

23. If it re-downloads game data maybe also try to replace the file size in bytes for the .pack file. Right click->properties->Size (not Size on disk)

24. In the future I might find a way to edit the libnative-lib.so file to skip that check also.
