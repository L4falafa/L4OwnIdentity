using Rocket.Core.Plugins;
using SDG.Unturned;
using Rocket.API;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using Rocket.API.Collections;
using System.Collections.Generic;
using Steamworks;
using System.Collections;
using UnityEngine;
using System.Threading;
using SDG.Framework.Devkit;
using LiteDB;
using System.Reflection;
using System.Collections.ObjectModel;
using Rocket.Unturned.Effects;
using Character = Lafalafa.L4OwnIdentity.Extras.Character;
using System.Linq;
using Rocket.Unturned.Events;
using L4OwnIdentity.Util;
using Lafalafa.L4OwnIdentity.Extras;
using System.Globalization;
using Logger = Rocket.Core.Logging.Logger;

namespace Lafalafa.L4OwnIdentity
{
    public class RPCharacterPlugin : RocketPlugin<RPCharacterPluginConfig>
    {


        #region Load&Unload

        protected override void Load()
        {
            EffectManager.onEffectTextCommitted += onTextCommited;
            EffectManager.onEffectButtonClicked += onButtonClicked;
            U.Events.OnPlayerConnected += onPlayerConnected;
            U.Events.OnPlayerDisconnected += onPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerUpdateGesture += UnturnedPlayerEvents_OnPlayerUpdateGesture;
            Instance = this;
            registerin = new Dictionary<CSteamID, Character>();
            Logger.Log("############################", ConsoleColor.Cyan);
            Logger.Log("#      L4OwnIdentity       #", ConsoleColor.Cyan);
            Logger.Log("#       By: Lafalafa       #", ConsoleColor.Cyan);
            Logger.Log("#    discord.gg/eAkMRkv    #", ConsoleColor.Cyan);
            Logger.Log("############################", ConsoleColor.Cyan);
            string path = $"{Rocket.Core.Environment.PluginsDirectory}/{Assembly.GetName().Name}/Database";

            try
            {
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path);
                }
                DbRP.initializeDatabase($"{path}/L4OwnIdentity.db");
            }
            catch (Exception e)
            {
                Console.WriteLine("We can`t make a new dir, the error is:  {0}", e.ToString());
            }


        }

        protected override void Unload()
        {
            EffectManager.onEffectTextCommitted -= onTextCommited;
            EffectManager.onEffectButtonClicked -= onButtonClicked;
            U.Events.OnPlayerConnected -= onPlayerConnected;
            U.Events.OnPlayerDisconnected -= onPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= UnturnedPlayerEvents_OnPlayerUpdateGesture;
        }

        #endregion
        #region eventos_jugadores
        private void UnturnedPlayerEvents_OnPlayerUpdateGesture(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            if (gesture == UnturnedPlayerEvents.PlayerGesture.Point && player.HasPermission("l4ownidentity.checkdni"))
            {


                //Character character = DbRP.getCharacter(player);

                //if (character == null) { return; }
                //string sex = "W";
                //if (!character.Sex)
                //{
                //    sex = "M";
                //}
                //EffectManager.sendUIEffect(Configuration.Instance.EffectDni, 11022, true, character.Name, character.Lastname, character.Birth.ToString("dd/MM/yyyy"), sex);
                //EffectManager.sendUIEffectImageURL(11022, player.CSteamID, true, "Image", player.SteamProfile.AvatarFull.OriginalString);
                //EffectManager.sendUIEffectText(11022, player.CSteamID, true, "SteamIDDNI", player.CSteamID.m_SteamID.ToString());
                //StartCoroutine(showDni(player));
                Player vplayer = RaycastHelper.GetPlayerFromHits(player.Player, 3);
                Character character = null;
                string sex;
                if (vplayer == null)
                {


                    try
                    {
                        character = DbRP.getCharacter(player);
                    }
                    catch (Exception)
                    {

                        return;
                    }

                    if (character == null) { return; }
                    sex = "M";
                    if (!character.Sex)
                    {
                        sex = "W";
                    }
                    EffectManager.sendUIEffect(Configuration.Instance.EffectDni, 11022, true, character.Name, character.Lastname, character.Birth.ToString("dd/MM/yyyy"), sex); ;
                    EffectManager.sendUIEffectImageURL(11022, player.CSteamID, true, "Image", player.SteamProfile.AvatarFull.OriginalString);
                    EffectManager.sendUIEffectText(11022, player.CSteamID, true, "SteamIDDNI", player.CSteamID.m_SteamID.ToString());
                    StartCoroutine(showDni(player));
                    return;
                }
                UnturnedPlayer victim = UnturnedPlayer.FromPlayer(vplayer);
                if (victim.Player.animator.gesture != EPlayerGesture.SURRENDER_START) { return; }

                try
                {
                    character = DbRP.getCharacter(victim);
                }
                catch (Exception)
                {

                    return;
                }

                if (character == null) { return; }
                sex = "M";
                if (!character.Sex)
                {
                    sex = "W";
                }
                EffectManager.sendUIEffect(Configuration.Instance.EffectDni, 11022, true, character.Name, character.Lastname, character.Birth.ToString("dd/MM/yyyy"), sex); ;
                EffectManager.sendUIEffectImageURL(11022, player.CSteamID, true, "Image", player.SteamProfile.AvatarFull.OriginalString);
                EffectManager.sendUIEffectText(11022, player.CSteamID, true, "SteamIDDNI", player.CSteamID.m_SteamID.ToString());
                StartCoroutine(showDni(player));
                return;

            }
            if (gesture != UnturnedPlayerEvents.PlayerGesture.Wave) { return; };
            EffectManager.askEffectClearByID(Configuration.Instance.EffectDni, player.CSteamID);

        }

        private void onPlayerDisconnected(UnturnedPlayer player)
        {
            if (registerin.ContainsKey(player.CSteamID))
            {
                registerin.Remove(player.CSteamID);
            }
        }

        private void onButtonClicked(Player p, string buttonName)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
            switch (buttonName)
            {

                case "RPCharacter_Register":
                    if (registerin[player.CSteamID].Name == null || registerin[player.CSteamID].Lastname == null || registerin[player.CSteamID].Birth.Date == DateTime.Now.Date)
                    {
                        ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("BAD_REGISTER").Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                        return;
                    }
                    string[] parameters = registerin[player.CSteamID].getParameters();
                    ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("SUCCESSFULLY_REGISTERED", parameters[0], parameters[1], parameters[2], parameters[3]).Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                    EffectManager.askEffectClearByID(Configuration.Instance.EffectRegister, player.CSteamID);
                    player.Player.disablePluginWidgetFlag(EPluginWidgetFlags.Modal);
                    DbRP.addCharacter(registerin[player.CSteamID]);
                    registerin.Remove(player.CSteamID);
                    player.Player.disablePluginWidgetFlag((EPluginWidgetFlags)8);
                    break;

                case "RPCharacter_Man":
                    ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("SEX_CHANGE_MAN").Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                    registerin[player.CSteamID].Sex = false;
                    break;
                case "Character_Woman":
                    ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("SEX_CHANGE_WOMAN").Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                    registerin[player.CSteamID].Sex = true;
                    break;
                case "RPCharacter_Discord":
                    OpenUrl(player, "https://discord.gg/eAkMRkv", "Discord for a custom UI or requeset a plugin");
                    break;
            }

        }

        private void onTextCommited(Player p, string buttonName, string text)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(p);
            try
            {



                text = text.Trim();
                bool containsInt = text.Any(char.IsDigit);

                bool result1 = text.All(Char.IsLetter);
                switch (buttonName)
                {
                    case "RPRegistration_Name":
                        if (text.Length > 10)
                        {

                            ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("TOO_LONG", "Name", 10).Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                            return;
                        }

                        if (result1 == false || containsInt == true || string.IsNullOrEmpty(text))
                        {

                            ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("BAD_CHARACTERS").Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                            return;
                        }
                        text = UppercaseFirst(text);
                        ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("REGISTER_DATE_NAME", text).Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                        registerin[player.CSteamID].Name = text;
                        Console.Write(text);
                        break;
                    case "RPRegistration_Subname":
                        if (text.Length > 10)
                        {
                            ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("TOO_LONG", "Lastname", 10).Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                            return;
                        }
    ;
                        if (result1 == false || containsInt == true || string.IsNullOrEmpty(text))
                        {

                            ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("BAD_CHARACTERS").Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                            return;
                        }
                        text = UppercaseFirst(text);
                        ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("REGISTER_DATE_LASTNAME", text).Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                        registerin[player.CSteamID].Lastname = text;
                        break;
                    case "RPRegistration_Birthday":
                        try
                        {
                            String[] textsplitted;
                            String[] spearator = { "/" };
                            Int32 count = 3;
                            textsplitted = text.Split(spearator, count,
                              StringSplitOptions.RemoveEmptyEntries);
                            if (text.Length > 10 || textsplitted.Length > 3)
                            {
                                ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("TOO_LONG", "Birth", 10).Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                                return;
                            }
                            foreach (String s in textsplitted)
                            {

                                int i;
                                bool result = int.TryParse(s, out i);
                                if (result == false)
                                {
                                    ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("BAD_DATE").Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);

                                    return;
                                }

                            }

                            string[] formats = {
                   "MM/dd/yyyy","M/d/yyyy",
                   "MM/dd/yyyy", "yyyy/MM/dd","yyyy/dd/MM"};

                            DateTime dateValue;



                            if (DateTime.TryParseExact(text, formats,
                                               new CultureInfo("en-US"),
                                               DateTimeStyles.None,
                                               out dateValue))
                            {
                                ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("REGISTER_DATE_BIRTH", dateValue.ToString("MMMM dd, yyyy")).Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);

                                registerin[player.CSteamID].Birth = dateValue;


                            }
                            else
                            {
                                ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("BAD_DATE").Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);

                            }
                        }
                        catch (Exception)
                        {

                            ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("BAD_DATE").Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
                        }





                        break;

                }
            }
            catch (Exception)
            {

                ChatManager.serverSendMessage(string.Format($"{MessageName}{Instance.Translate("BAD_CHARACTERS").Replace('(', '<').Replace(')', '>')}"), Color.white, null, player.SteamPlayer(), EChatMode.WELCOME, ChatLogo, true);
            }



        }
        private void onPlayerConnected(UnturnedPlayer player)
        {

            if (DbRP.getCharacter(player) != null) { return; }
            if (!registerin.ContainsKey(player.CSteamID))
            {
                registerin.Add(player.CSteamID, new Character(null, null, null, DateTime.Now.Date, player.CSteamID));
            }
            player.Player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
            if (Configuration.Instance.DisableNames)
            {
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, false);
            }
            if (Configuration.Instance.ChangeNames)
            {

                player.Player.character.name = $"{DbRP.getCharacter(player).Name} {DbRP.getCharacter(player).Lastname}";
            }
            EffectManager.sendUIEffect(Configuration.Instance.EffectRegister, 11046, player.CSteamID, true);
            EffectManager.sendUIEffectImageURL(11046, player.CSteamID, true, "RPCharacter_Logo", Instance.Configuration.Instance.Logo);
            EffectManager.sendUIEffectImageURL(11046, player.CSteamID, true, "RPCharacter_Fondo", Instance.Configuration.Instance.Fondo);


        }

        #endregion
        #region private_events
        private static string UppercaseFirst(string s)
        {

            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private IEnumerator showDni(UnturnedPlayer player)
        {

            yield return new WaitForSeconds(5);
            EffectManager.askEffectClearByID(Configuration.Instance.EffectDni, player.CSteamID);
        }




        private static void OpenUrl(UnturnedPlayer player, string url, string desc)
        {
            player.Player.sendBrowserRequest(desc, url);
        }

        #endregion
        public override TranslationList DefaultTranslations =>
          new TranslationList
          {
               {"TOO_LONG","El campo de {0} es muy largo, el maximo de este es: {1}"},
               {"SEX_CHANGE_MAN","You changed the sex to man"},
               {"SEX_CHANGE_WOMAN","You changed the sex to woman"},
               {"BAD_CHARACTERS","Please use only valid characters"},
               {"BAD_DATE","Bad date in Birth the formats is DD/MM/YYYY {/17/03/2000}"},
               {"BAD_REGISTER","At the moment that you tried to register you do something bad, plis fill all the fields again"},
               {"REGISTER_DATE_BIRTH","You registered the field birth to {0}"},
               {"REGISTER_DATE_NAME","You registered the field name to {0}"},
               {"REGISTER_DATE_LASTNAME","You registered the fiel lastname to {0}"},
               {"SUCCESSFULLY_REGISTERED","You registered with the next parameters: Name:{0} Lastname: {1} Birth: {2} Sex: {3}"}
          };
        public static RPCharacterPlugin Instance;
        private static Dictionary<CSteamID, Character> registerin;
        private const string ChatLogo = "https://cdn.discordapp.com/attachments/427158067285721088/754530225546592317/lista.png";
        private const string MessageName = "<color=red>[</color><color=cyan>Register</color><color=red>]</color>: ";
    }

}
