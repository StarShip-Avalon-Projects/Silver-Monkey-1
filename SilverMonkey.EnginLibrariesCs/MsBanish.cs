﻿using Furcadia.Net.DreamInfo;
using Monkeyspeak;
using System;
using System.Collections.Generic;
using System.Linq;
using static Engine.Libraries.MsLibHelper;

namespace Engine.Libraries
{
    // '' <summary>
    // '' Cause:s (0:50) - (0:62
    // '' <para>
    // '' Conditions: (1:50) - (1:53)
    // '' </para>
    // '' Effects: (5:49) - (5:56)
    // '' <para>
    // '' Banish Monkey Speak
    // '' </para>
    // '' This system mirrors Furcadia's banish system by tracking the banish
    // '' commands sent to the game aerver and keep a list of banished furres
    // '' locally. To help keep the list accurate, It is reconmended to ask
    // '' the server for a banish list upon joining the dream. It maybe a good
    // '' idea to run a daily schedule to refresh the list for temp banishes
    // '' to drop off.
    // '' </summary>
    // '' <remarks>
    // '' This Lib contains the following unnamed delegates
    // '' <para>
    // '' (0:50) When the bot fails to banish a furre,
    // '' </para>
    // '' (0:54) When the bot sees the banish list,
    // '' <para>
    // '' (0:55) When the bot fails to remove a furre from the banish list,
    // '' </para>
    // '' <para>
    // '' (0:57) When the bot successfully removes a furre from the banish list,
    // '' </para>
    // '' (0:59) When the bot fails to empty the banish list,
    // '' <para>
    // '' (0:60) When the bot successfully clears the banish list,
    // '' </para>
    // '' (0:61) When the bot successfully temp banishes a furre,
    // '' </remarks>
    public class MsBanish : MonkeySpeakLibrary
    {
        #region Public Properties

        private List<string> BanishedFurres = new List<string>();
        public override int BaseId => 51;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Initializes this instance. Add your trigger handlers here.
        /// </summary>
        /// <param name="args">Parametized argument of objects to use to pass runtime objects to a library at initialization</param>
        public override void Initialize(params object[] args)
        {
            base.Initialize(args);
            // (0:51) When the bot fails to banish a furre,
            Add(TriggerCategory.Cause,
                r => WhenTriggeringFurreBanished(r),
                "When the bot fails to banish a furre,");

            // (0:52) When the bot fails to banish the furre named {...},
            Add(TriggerCategory.Cause,
                r => WhenFurreNamedBanished(r),
                "When the bot fails to banish the furre named {...},");

            // (0:53) When the bot successfully banishes a furre,
            Add(TriggerCategory.Cause,
                r => WhenTriggeringFurreBanished(r),
                "When the bot successfully banishes a furre,");

            // (0:54) When the bot successfully banishes the furre named {...},
            Add(TriggerCategory.Cause,
                r => WhenFurreNamedBanished(r),
                "When the bot successfully banishes the furre named {...},");

            // (0:55) When the bot sees the banish list,
            Add(TriggerCategory.Cause,
                r => WhenTriggeringFurreBanished(r),
                "When the bot sees the banish list,");

            // (0:56) When the bot fails to remove a furre from the banish list,
            Add(TriggerCategory.Cause,
                r => WhenTriggeringFurreBanished(r),
                "When the bot fails to remove a furre from the banish list,");

            // (0:57) When the bot fails to remove the furre named {...} from the banish list,
            Add(TriggerCategory.Cause,
                 r => WhenFurreNamedBanished(r),
                 "When the bot fails to remove the furre named {...} from the banish list,");

            // (0:58) When the bot successfully removes a furre from the banish list,
            Add(TriggerCategory.Cause,
                r => WhenTriggeringFurreBanished(r),
                "When the bot successfully removes a furre from the banish list,");

            // (0:59) When the bot successfully removes the furre named {...} from the banish list,
            Add(TriggerCategory.Cause,
                 r => WhenFurreNamedBanished(r),
                 "When the bot successfully removes the furre named {...} from the banish list,");

            // (0:60) When the bot fails to empty the banish list,
            Add(TriggerCategory.Cause,
                r => throw new NotImplementedException(),
                "When the bot fails to empty the banish list,");

            // (0:61) When the bot successfully clears the banish list,
            Add(TriggerCategory.Cause,
                r =>
                {
                    var banisList = r.GetParametersOfType<List<string>>().FirstOrDefault();
                    if (banisList != null)
                    {
                        BanishedFurres = banisList;
                        return true;
                    }
                    return false;
                },
                "When the bot successfully clears the banish list,");

            // (0:62) When the bot successfully temp banishes a furre,
            Add(TriggerCategory.Cause,
                r => WhenTriggeringFurreBanished(r),
                "When the bot successfully temp banishes a furre,");

            // (0:63) When the bot successfully temp banishes the furre named {...},
            Add(TriggerCategory.Cause,
                 r => WhenFurreNamedBanished(r),
                 "When the bot successfully temp banishes the furre named {...},");

            // (1:50) and the triggering furre is not on the banish list,
            Add(TriggerCategory.Condition,
                r => TrigFurreIsNotBanished(r),
                "and the triggering furre is not on the banish list,");

            // (1:51) and the triggering furre is on the banish list,
            Add(TriggerCategory.Condition,
                r => TrigFurreIsBanished(r),
                "and the triggering furre is on the banish list,");

            // (1:52) and the furre named {...} is not on the banish list,
            Add(TriggerCategory.Condition,
                r => FurreNamedIsNotBanished(r),
                "and the furre named {...} is not on the banish list,");

            // (1:53) and the furre named {...} is on the banish list,
            Add(TriggerCategory.Condition,
                r => FurreNamedIsBanished(r),
                "and the furre named {...} is on the banish list,");

            //  (5: ) save the banish list to the variable % .
            Add(TriggerCategory.Effect,
                 r => BanishSave(r),
                 "save the banish list to the variable % .");

            // (5:x) as the server for the banish-list.
            Add(TriggerCategory.Effect,
                r => BanishList(r),
                "ask the server for the banish-list.");

            // (5:x) banish the triggering furre.
            Add(TriggerCategory.Effect,
                 r => BanishTrigFurre(r),
                 "banish the triggering furre.");

            // (5:x) banish the furre named {...}.
            Add(TriggerCategory.Effect,
                r => BanishFurreNamed(r),
                "banish the furre named {...}.");

            // (5:x) temporarily  banish the triggering furre for three days.
            Add(TriggerCategory.Effect,
                r => TempBanishTrigFurre(r),
                "temporarily  banish the triggering furre for three days.");

            // (5:x) temporarily banish the furre named {...} for three days.
            Add(TriggerCategory.Effect,
                 r => TempBanishFurreNamed(r),
                 "temporarily banish the furre named {...} for three days.");

            // (5:x) unbanish the triggering furre.
            Add(TriggerCategory.Effect,
                 r => UnBanishTrigFurre(r),
                 "unbanish the triggering furre.");

            // (5:x) unbanish the furre named {...}.
            Add(TriggerCategory.Effect,
                 r => UnBanishFurreNamed(r),
                 "unbanish the furre named {...}.");
        }

        /// <summary>
        /// Called when page is disposing or resetting.
        /// </summary>
        /// <param name="page">The page.</param>
        public override void Unload(Page page)
        {
        }

        #endregion Public Methods

        #region Private Methods

        private bool BanishFurreNamed(TriggerReader reader)
        {
            string Furre = reader.ReadString();
            return SendServer($"banish { Furre.ToFurcadiaShortName()}");
        }

        private bool BanishList(TriggerReader reader)
        {
            return SendServer("banish-list");
        }

        private bool BanishSave(TriggerReader reader)
        {
            IVariable NewVar = reader.ReadVariable(true);
            NewVar.Value = string.Join("", BanishedFurres.ToArray());
            return true;
        }

        private bool BanishTrigFurre(TriggerReader reader)
        {
            return SendServer($"banish { Player.ShortName}");
        }

        private bool FurreNamedIsBanished(TriggerReader reader)
        {
            return !FurreNamedIsNotBanished(reader);
        }

        private bool FurreNamedIsNotBanished(TriggerReader reader)
        {
            string f = reader.ReadString();
            foreach (var fur in BanishedFurres)
            {
                if (fur.ToFurcadiaShortName() == f.ToFurcadiaShortName())
                {
                    return false;
                }
            }

            return true;
        }

        private bool TempBanishFurreNamed(TriggerReader reader)
        {
            return SendServer($"tempbanish { Player.ShortName}");
        }

        private bool TempBanishTrigFurre(TriggerReader reader)
        {
            string Furre = reader.ReadString();
            return SendServer($"tempbanish { Furre.ToFurcadiaShortName()}");
        }

        private bool TrigFurreIsBanished(TriggerReader reader)
        {
            return !TrigFurreIsNotBanished(reader);
        }

        private bool TrigFurreIsNotBanished(TriggerReader reader)
        {
            foreach (var fur in BanishedFurres)
            {
                if (fur.ToFurcadiaShortName() == Player.ShortName)
                {
                    return false;
                }
            }

            return true;
        }

        private bool UnBanishFurreNamed(TriggerReader reader)
        {
            string Furre = reader.ReadString();
            return SendServer($"banish-off { Furre.ToFurcadiaShortName()}");
        }

        private bool UnBanishTrigFurre(TriggerReader reader)
        {
            return SendServer($"banish-off { Player.ShortName}");
        }

        private bool WhenFurreNamedBanished(TriggerReader reader)
        {
            var name = reader.GetParametersOfType<string>().FirstOrDefault();
            if (name != null)
            {
                Player = new Furre(name);
                ((ConstantVariable)reader.Page.GetVariable(BanishNameVariable)).SetValue(name);
                return true;
            }
            string Furre = reader.ReadString();
            return Furre.ToFurcadiaShortName()
                == reader.GetParametersOfType<string>().FirstOrDefault();
        }

        private bool WhenTriggeringFurreBanished(TriggerReader reader)
        {
            var name = reader.GetParametersOfType<string>().FirstOrDefault();
            if (name != null)
            {
                Player = new Furre(name);
                UpdateTriggerigFurreVariables(Player, reader.Page);
                ((ConstantVariable)reader.Page.GetVariable(BanishNameVariable)).SetValue(name);
                return true;
            }
            return false;
        }

        #endregion Private Methods
    }
}