using System.Runtime.CompilerServices;
using BepInEx;
using System;
using On;
using SlugpupsPurrr;
using IL;
using Newtonsoft.Json.Linq;
using System.Media;
using UnityEngine;

namespace SlugpupsPurrs;
[BepInPlugin("SlugpupsPurrs", "SlugpupsPurrs", "1.0")]

public class SlugpupPurrr : BaseUnityPlugin{

    public ConditionalWeakTable<Player, ChunkDynamicSoundLoop> SoundLoops = new ConditionalWeakTable<Player, ChunkDynamicSoundLoop>();
    
    private void LogInfo(object data){
        Logger.LogInfo(data);
    }

    public void OnEnable(){
        LogInfo("Purrrrr, Meow~, Purrrrr~~ I'm Awaken");

        On.RainWorld.OnModsInit += RainWorld_OnModsInit;

    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        try
        {
            On.Player.Update += Player_Update;
            CryAboutIt.RegisterValues();
            On.Player.ThrowObject += Player_ThrowObject;
            On.Player.SlugOnBack.SlugToBack += SlugOnBack_SlugToBack; 
        }
        catch (Exception data)
        {
            LogInfo(data);
            throw;
        }
        finally
        {
            orig.Invoke(self);
        }
    }

    private void SlugOnBack_SlugToBack(On.Player.SlugOnBack.orig_SlugToBack orig, Player.SlugOnBack self, Player playerToBack)
    {
        orig(self, playerToBack);

        if (SoundLoops != null && !SoundLoops.TryGetValue(self.slugcat, out _))
        {
            ChunkDynamicSoundLoop chunkDynamicSoundLoop = new ChunkDynamicSoundLoop(self.slugcat.bodyChunks[0])
            {
                sound = CryAboutIt.purrr
            };
            SoundLoops.Add(self.slugcat, chunkDynamicSoundLoop);
        }
    }

    private void Player_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
    {
        if (self.grasps[grasp].grabbed is Player pup && (pup.isNPC || pup.isSlugpup))
        {
            self.room.PlaySound(CryAboutIt.meow, self.mainBodyChunk);
        }
        orig(self, grasp, eu);
    }

    private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        orig(self, eu);
        try
        {
            if (SoundLoops != null && SoundLoops.TryGetValue(self, out var value))
            {
                if (self.onBack != null && (!self.onBack.isSlugpup || !self.onBack.isNPC) && self.isSlugpup && self.room?.world.game.session is GameSession && !self.room.game.GamePaused && !self.dead)
                {
                    //Debug.Log("Normal Sound");
                    value.Volume = 0.4f;
                }
                else
                {
                    //Debug.Log("Supressed sound");
                    value.Volume = 0f;
                }
                value.Update();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Something went wrong!, PlayerUpdate");
            Debug.LogException(e);
        }
    }
}