﻿using GorillaNetworking;
using iiMenu.Classes;
using iiMenu.Notifications;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine;
using static iiMenu.Classes.RigManager;
using static iiMenu.Menu.Main;

namespace iiMenu.Mods
{
    public class Safety
    {
        public static void NoFinger()
        {
            ControllerInputPoller.instance.leftControllerGripFloat = 0f;
            ControllerInputPoller.instance.rightControllerGripFloat = 0f;
            ControllerInputPoller.instance.leftControllerIndexFloat = 0f;
            ControllerInputPoller.instance.rightControllerIndexFloat = 0f;
            ControllerInputPoller.instance.leftControllerPrimaryButton = false;
            ControllerInputPoller.instance.leftControllerSecondaryButton = false;
            ControllerInputPoller.instance.rightControllerPrimaryButton = false;
            ControllerInputPoller.instance.rightControllerSecondaryButton = false;
            ControllerInputPoller.instance.leftControllerPrimaryButtonTouch = false;
            ControllerInputPoller.instance.leftControllerSecondaryButtonTouch = false;
            ControllerInputPoller.instance.rightControllerPrimaryButtonTouch = false;
            ControllerInputPoller.instance.rightControllerSecondaryButtonTouch = false;
        }

        public static bool lastjsi = false;
        public static bool isActive = true;
        public static void ToggleIgloo()
        {
            if (rightJoystickClick && !lastjsi)
            {
                isActive = !isActive;
                GameObject.Find("Mountain/Geometry/goodigloo").SetActive(isActive);
            }
            lastjsi = rightJoystickClick;
        }

        public static void DisableGamemodeButtons()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/ModeSelector_Group").SetActive(false);
        }

        public static void EnableGamemodeButtons()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/ModeSelector_Group").SetActive(true);
        }

        public static void SpoofSupportPage()
        {
            GorillaComputer.instance.screenText.Text = GorillaComputer.instance.screenText.Text.Replace("STEAM", "QUEST").Replace(GorillaComputer.instance.buildDate, "05/30/2024 16:50:12\nBUILD CODE 4893\nMANAGED ACCOUNT: NO");
        }

        private static float lastCacheClearedTime = 0f;
        public static void AutoClearCache()
        {
            if (Time.time > lastCacheClearedTime)
            {
                lastCacheClearedTime = Time.time + 60f;
                System.GC.Collect();
            }
        }

        public static int antireportrangeindex = 0;
        public static float threshold = 0.35f;

        public static void ChangeAntiReportRange()
        {
            string[] names = new string[]
            {
                "Default", // The report button
                "Large", // The report button within the range of 3 people
                "Massive" // The entire fucking board
            };
            float[] distances = new float[]
            {
                0.35f,
                0.7f,
                1.5f
            };
            antireportrangeindex++;
            if (antireportrangeindex >= names.Length)
                antireportrangeindex = 0;

            threshold = distances[antireportrangeindex];
            GetIndex("Change Anti Report Distance").overlapText = "Change Anti Report Distance <color=grey>[</color><color=green>" + names[antireportrangeindex] + "</color><color=grey>]</color>";
        }

        public static bool smartarp;
        public static int buttonClickTime;
        public static string buttonClickPlayer;

        public static float antiReportNotificationDelay;
        public static void AntiReportDisconnect()
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        if (GetIndex("Visualize Anti Report").enabled)
                            VisualizeAura(report.position, threshold, Color.red);
                        
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != VRRig.LocalRig)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                if (D1 < threshold || D2 < threshold)
                                {
                                    if (!smartarp || (smartarp && line.linePlayer.UserId == buttonClickPlayer && Time.frameCount == buttonClickTime && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                    {
                                        NetworkSystem.Instance.ReturnToSinglePlayer();
                                        RPCProtection();

                                        if (Time.time > antiReportNotificationDelay)
                                        {
                                            antiReportNotificationDelay = Time.time + 0.1f;
                                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        public static void AntiReportReconnect()
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        if (GetIndex("Visualize Anti Report").enabled)
                            VisualizeAura(report.position, threshold, Color.red);
                        
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != VRRig.LocalRig)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                if (D1 < threshold || D2 < threshold)
                                {
                                    if (!smartarp || (smartarp && line.linePlayer.UserId == buttonClickPlayer && Time.frameCount == buttonClickTime && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                    {
                                        Important.Reconnect();
                                        RPCProtection();

                                        if (Time.time > antiReportNotificationDelay)
                                        {
                                            antiReportNotificationDelay = Time.time + 0.1f;
                                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be reconnected shortly.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        public static void AntiReportJoinRandom()
        {
            try
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform report = line.reportButton.gameObject.transform;
                        if (GetIndex("Visualize Anti Report").enabled)
                            VisualizeAura(report.position, threshold, Color.red);
                        
                        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                        {
                            if (vrrig != VRRig.LocalRig)
                            {
                                float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                if (D1 < threshold || D2 < threshold)
                                {
                                    if (!smartarp || (smartarp && line.linePlayer.UserId == buttonClickPlayer && Time.frameCount == buttonClickTime && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                    {
                                        RPCProtection();
                                        Important.JoinRandom();

                                        if (Time.time > antiReportNotificationDelay)
                                        {
                                            antiReportNotificationDelay = Time.time + 0.1f;
                                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be connected to a random lobby shortly.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        private static float delaysonospam = 0f;
        public static void AntiReportNotify()
        {
            try
            {
                if (Time.time > delaysonospam)
                {
                    foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                    {
                        if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                        {
                            Transform report = line.reportButton.gameObject.transform;
                            if (GetIndex("Visualize Anti Report").enabled)
                                VisualizeAura(report.position, threshold, Color.red);
                            
                            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                            {
                                if (vrrig != VRRig.LocalRig)
                                {
                                    float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                                    float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                                    if (D1 < threshold || D2 < threshold)
                                    {
                                        if (!smartarp || (smartarp && line.linePlayer.UserId == buttonClickPlayer && Time.frameCount == buttonClickTime && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
                                        {
                                            delaysonospam = Time.time + 0.1f;
                                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " is reporting you.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { } // Not connected
        }

        public static void AntiReportFRT(Player subject, bool doNotification = true)
        {
            if (!smartarp || (smartarp && PhotonNetwork.CurrentRoom.IsVisible && !PhotonNetwork.CurrentRoom.CustomProperties.ToString().Contains("MODDED")))
            {
                int antiReportType = 0;
                string[] types = new string[]
                {
                    "Disconnect",
                    "Reconnect",
                    "Join Random"
                };
                for (int i = 0; i < types.Length - 1; i++)
                {
                    ButtonInfo buttonInfo = GetIndex("Anti Report <color=grey>[</color><color=green>" + types[i] + "</color><color=grey>]</color>");
                    if (buttonInfo.enabled)
                    {
                        antiReportType = i;
                        break;
                    }
                }
                switch (antiReportType)
                {
                    case 0:
                        NetworkSystem.Instance.ReturnToSinglePlayer();
                        RPCProtection();
                        if (doNotification)
                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + subject.NickName + " attempted to report you, you have been disconnected.");
                        
                        break;
                    case 1:
                        Important.Reconnect();
                        RPCProtection();
                        if (doNotification)
                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + subject.NickName + " attempted to report you, you have been disconnected and will be reconnected shortly.");
                        
                        break;
                    case 2:
                        RPCProtection();
                        Important.JoinRandom();
                        if (doNotification)
                            NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + subject.NickName + " attempted to report you, you have been disconnected and will be connected to a random lobby shortly.");
                        
                        break;
                }
            }
        }

        public static void AntiModerator()
        {
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (!vrrig.isOfflineVRRig && vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAK") || vrrig.concatStringOfCosmeticsAllowed.Contains("LBAAD") || vrrig.concatStringOfCosmeticsAllowed.Contains("LMAPY."))
                {
                    try
                    {

                        VRRig plr = vrrig;
                        NetPlayer player = GetPlayerFromVRRig(plr);
                        if (player != null)
                        {
                            string text = "Room: " + PhotonNetwork.CurrentRoom.Name;
                            float r = 0f;
                            float g = 0f;
                            float b = 0f;
                            try
                            {
                                
                                r = plr.playerColor.r * 255;
                                g = plr.playerColor.r * 255;
                                b = plr.playerColor.r * 255;
                            }
                            catch { LogManager.Log("Failed to log colors, rig most likely nonexistent"); }

                            try
                            {
                                text += "\n====================================\n";
                                text += string.Concat(new string[]
                                {
                                    "Player Name: \"",
                                    player.NickName,
                                    "\", Player ID: \"",
                                    player.UserId,
                                    "\", Player Color: (R: ",
                                    r.ToString(),
                                    ", G: ",
                                    g.ToString(),
                                    ", B: ",
                                    b.ToString(),
                                    ")"
                                });
                            }
                            catch { LogManager.Log("Failed to log player"); }

                            text += "\n====================================\n";
                            text += "Text file generated with ii's Stupid Menu";
                            string fileName = "iisStupidMenu/" + player.NickName + " - Anti Moderator.txt";

                            File.WriteAllText(fileName, text);
                        }
                    }
                    catch { }
                    NetworkSystem.Instance.ReturnToSinglePlayer();
                    NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-MODERATOR</color><color=grey>]</color> There was a moderator in your lobby, you have been disconnected. Their Player ID and Room Code have been saved to a file.");
                }
            }
        }

        public static void ChangeIdentity()
        {
            string randomName = "gorilla";
            for (var i = 0; i < 4; i++)
                randomName += Random.Range(0, 9).ToString();

            ChangeName(randomName);

            byte randA = (byte)Random.Range(0, 255);
            byte randB = (byte)Random.Range(0, 255);
            byte randC = (byte)Random.Range(0, 255);
            ChangeColor(new Color32(randA, randB, randC, 255));
        }

        public static void ChangeIdentityRegular()
        {
            ChangeName(names[Random.Range(0, names.Length - 1)]);

            Color[] colors = new Color[]
            {
                Color.cyan,
                Color.yellow,
                Color.blue,
                Color.gray,
                Color.black,
                Color.white,
                Color.magenta,
                Color.yellow,
                Color.green,
                new Color(1f, 0.5f, 1f, 255f),
                new Color(0f, 0.5f, 0f, 255f),
                new Color32(113, 0, 198, 255),
                new Color32(170, 198, 170, 255),
                new Color32(170, 170, 170, 255),
                new Color32(227, 170, 85, 255),
                new Color32(0, 226, 255, 255)
            };
            ChangeColor(colors[Random.Range(0, colors.Length - 1)]);
        }

        private static bool lastinlobbyagain = false;
        public static void ChangeIdentityOnDisconnect()
        {
            if (!PhotonNetwork.InRoom && lastinlobbyagain)
                ChangeIdentity();
            
            lastinlobbyagain = PhotonNetwork.InRoom;
        }
        public static void ChangeIdentityRegularOnDisconnect()
        {
            if (!PhotonNetwork.InRoom && lastinlobbyagain)
                ChangeIdentityRegular();
            
            lastinlobbyagain = PhotonNetwork.InRoom;
        }
        public static void ChangeIdentityMinigamesOnDisconnect()
        {
            if (!PhotonNetwork.InRoom && lastinlobbyagain)
                Fun.BecomeMinigamesKid();
            
            lastinlobbyagain = PhotonNetwork.InRoom;
        }

        public static void FPSSpoof()
        {
            Patches.FPSPatch.enabled = true;
            Patches.FPSPatch.spoofFPSValue = Random.Range(88, 92); //  - (UnityEngine.Random.Range(0, 400) > 399 ? UnityEngine.Random.Range(15, 60) : 0)
        }

        public static string[] names = new string[]
        {
            "0", "SHIBAGT", "PBBV", "J3VU", "BEES", "NAMO", "MANGO", "FROSTY", "FRISH", "LITTLETIMMY",
            "SILLYBILLY", "TIMMY", "MINIGAMES", "MINIGAMESKID", "JMANCURLY", "VMT", "ELLIOT", "POLAR", "3CLIPCE", "GORILLAVR",
            "GORILLAVRGT", "GORILLAGTVR", "GORILLAGT", "SHARKPUPPET", "DUCKY", "EDDIE", "EDDY", "RAKZZ", "CASEOH", "SKETCH",
            "WATERMELON", "CRAZY", "MONK", "MONKE", "MONKI", "MONKEY", "MONKIY", "GORILL", "GOORILA", "GORILLA",
            "REDBERRY", "FOX", "RUFUS", "TTT", "TTTPIG", "PPPTIG", "K9", "BTC", "TICKLETIPJR", "BANANA",
            "PEANUTBUTTER", "GHOSTMONKE", "STATUE", "TURBOALLEN", "NOVA", "LUNAR", "MOON", "SUN", "RANDOM", "UNKNOWN",
            "GLITCH", "BUG", "ERROR", "CODE", "HACKER", "MODDER", "INVIS", "INVISIBLE", "TAGGER", "UNTAGGED",
            "BLUE", "RED", "GREEN", "PURPLE", "YELLOW", "BLACK", "WHITE", "BROWN", "CYAN", "GRAY",
            "GREY", "OG", "BANNED", "LEMON", "PLUSHIE", "CHEETO", "TIKTOK", "YOUTUBE", "TWITCH", "DISCORD"
        };
    }
}
