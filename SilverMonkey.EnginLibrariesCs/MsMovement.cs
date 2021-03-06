﻿using Furcadia.Drawing;
using Furcadia.Net.DreamInfo;
using MonkeyCore.Logging;
using Monkeyspeak;
using Monkeyspeak.Libraries;
using System;
using System.Text.RegularExpressions;

namespace Libraries
{
    /// <summary>
    /// <para>
    /// Causes: (0:600) -
    /// </para>
    /// <para>
    /// Conditions: (1:600) - (1:631)
    /// </para>
    /// <para>
    /// Effects: (5:600) - (5:625)
    /// </para>
    /// Furcadia Movement MonkeySpeak
    ///<para>
    /// Processing of Furre Triggers around the map, Interact with avatar
    /// settings such as thier map location, Type of Wings, Which avatar a
    /// given furre has.. The Avatar colors ect..
    /// </para>
    /// <para>
    /// Note: A Furre var only contains a description after the bot sends a look command to the server.
    /// </para>
    /// </summary>
    public sealed class MsMovement : MonkeySpeakLibrary
    {
        #region Public Fields

        /// <summary>
        /// Regex syntax for Fircadia isometric directions
        /// </summary>
        public const string RGEX_Mov_Steps = "(nw|ne|sw|se|1|3|7|9)";

        #endregion Public Fields

        #region Public Properties

        public override int BaseId => 600;

        #endregion Public Properties

        #region Public Methods

        public override void Initialize(params object[] args)
        {
            Add(TriggerCategory.Cause,
                r => WhenAnyoneEnterView(r),
                "When anyone enters the bots view,");

            Add(TriggerCategory.Cause,
                r => WhenFurreNamedEnterView(r),
                "When a furre named {...} enters the bots view,");

            Add(TriggerCategory.Cause,
                r => WhenAnyoneLeaveView(r),
                "When anyone leaves the bots view, ");

            Add(TriggerCategory.Cause,
                r => WhenFurreNamedLeaveView(r),
                "When a furre named {...} leaves the bots view,");

            Add(TriggerCategory.Cause,
                r => ReadTriggeringFurreParams(r),
                "When the bot sees a furre description,");

            Add(TriggerCategory.Cause,
                r => ReadTriggeringFurreParams(r),
                "When a furre moves,");

            Add(TriggerCategory.Cause,
                r => WhenAnyoneMoveInto(r),
                "when anyone moves into (x,y),");

            Add(TriggerCategory.Condition,
                r => AndDescContains(r),
                "and triggering furre's description contains {...}");

            Add(TriggerCategory.Condition,
                r => AndNotDescContains(r),
                "and triggering furre's description does not contain {...}");

            Add(TriggerCategory.Condition,
                r => AndFurreNamedDescContains(r),
                "and the furre named {...} description contains {...} if they are in the dream,");

            Add(TriggerCategory.Condition,
                r => AndNotDescContainsFurreNamed(r),
                "and the furre named {...} description does not contain {...} if they are in the dream");

            Add(TriggerCategory.Condition,
                r => AndTriggeringFurreIsGender(r),
                "and the triggering furre is gender #,");

            Add(TriggerCategory.Condition,
                r => AndFurreNamedIsGender(r),
                "and the furre named {...}'s is male if they are in the dream,");

            Add(TriggerCategory.Condition,
                r => TriggeringFurreSpecies(r),
                "and the trigger furre is Species # (please see http://www.furcadia.com/dsparams/ for info),");

            Add(TriggerCategory.Condition,
                r => FurreNamedSpecies(r),
                "and the furre named {...} is Species # if they are in the dream (please see http://www.furcadia.com/dsparams/ for info),");

            Add(TriggerCategory.Condition,
                r => TriggeringFurreWings(r),
                "and the triggering furre has wings of type #, (please see http://www.furcadia.com/dsparams/ for info),");

            Add(TriggerCategory.Condition,
                r => TriggeringFurreNoWings(r),
                "and the triggering furre doesn't wings of type #, (please see http://www.furcadia.com/dsparams/ for info),");

            Add(TriggerCategory.Condition,
                r => FurreNamedWings(r),
                "and the furre named {...} has wings of type #, (please see http://www.furcadia.com/dsparams/ for info),");

            Add(TriggerCategory.Condition,
                r => FurreNamedNoWings(r),
                "and the furre named {...}  doesn't wings of type #, (please see http://www.furcadia.com/dsparams/ for info),");

            Add(TriggerCategory.Condition,
                r => TriggeringFurreStanding(r),
                "and the triggering furre is standing,");

            Add(TriggerCategory.Condition,
                r => TriggeringFurreSitting(r),
                "and the triggering furre is sitting,");

            Add(TriggerCategory.Condition,
                r => TriggeringFurreLaying(r),
                "and the triggering furre is laying,");

            Add(TriggerCategory.Condition,
                r => TriggeringFurreIsFacingDirection(r),
                "and the triggering furre is facing NE,");

            Add(TriggerCategory.Condition,
                r => FurreNamedStanding(r),
                "and the furre named {...} is standing,");

            Add(TriggerCategory.Condition,
                r => FurreNamedSitting(r),
                "and the furre named {...} is sitting,");

            Add(TriggerCategory.Condition,
                r => FurreNamedLaying(r),
                "and the furre named {...} is laying,");

            Add(TriggerCategory.Condition,
                r => FurreNamedFacingIsFacingDirection(r),
                "and the furre named {...} is facing direction #,");

            Add(TriggerCategory.Condition,
                 r => WhenAnyoneMoveInto(r),
                 "and the triggering furre moved into/is standing at (x,y)");

            Add(TriggerCategory.Condition,
                r => FurreNamedMoveInto(r),
                "and the furre named {...} moved into/is standing at (x,y),");

            Add(TriggerCategory.Condition,
                r => MoveFrom(r),
                "and the triggering furre moved from (x,y),");

            Add(TriggerCategory.Condition,
                r => FurreNamedMoveFrom(r),
                "and the furre named {...} moved from (x,y),");

            Add(TriggerCategory.Condition,
                r => AndTriggeringFurreStoodStill(r),
                "and the triggering furre tried to move but stood still,");

            Add(TriggerCategory.Effect,
                r => TriggeringFurreDescVar(r),
                "set variable %Variable to the Triggering furre's description.");

            Add(TriggerCategory.Effect,
                r => TriggeringFurreGenderVar(r),
                "set variable %Variable to the triggering furre's gender.");

            Add(TriggerCategory.Effect,
                r => TriggeringFurreSpeciesVar(r),
                "set variable %Variable to the triggering furre's species.");

            Add(TriggerCategory.Effect,
                r => TriggeringFurreColorsVar(r),
                "set variable %Variable to the triggering furre's colors.");

            Add(TriggerCategory.Effect,
                r => FurreNamedGenderVar(r),
                "set variable %Variable to the furre named {...}'s gender if they are in the dream.");

            Add(TriggerCategory.Effect,
                r => FurreNamedSpeciesVar(r),
                "set variable %Variable to the furre named {...}'s species, if they are in the dream.");

            Add(TriggerCategory.Effect,
                r => FurreNamedDescVar(r),
                "set variable %Variable to the furred named {...}'s description, if they are in the dream.");

            Add(TriggerCategory.Effect,
                r => FurreNamedColorsVar(r),
                "set variable %Variable to the furre named {...}'s colors, if they are in the dream.");

            Add(TriggerCategory.Effect,
                r => TriggeringFurreWingsVar(r),
                "set %Variable to the wings type, the triggering furre is wearing.");

            Add(TriggerCategory.Effect,
                r => FurreNamedWingsVar(r),
                "set %Variable to the type of wings, the furre named {...}, is wearing.");

            Add(TriggerCategory.Effect,
                r => TurnCW(r),
                "turn the bot clock-wise one space.");

            Add(TriggerCategory.Effect,
                r => TurnCCW(r),
                "turn the bot counter-clockwise one space.");

            Add(TriggerCategory.Effect,
                r => SetVariableToCordX(r),
                "set variable %Variable to the X coordinate where the triggering furre moved into/is at.");

            Add(TriggerCategory.Effect,
                r => SetVariableToCordY(r),
                "set variable %Variable to the Y coordinate where the triggering furre moved into/is at.");

            Add(TriggerCategory.Effect,
                r => FurreNamedSetCordX(r),
                "set variable %Variable to the X coordinate where the furre named {...} moved into/is at.");

            Add(TriggerCategory.Effect,
                r => FurreNamedSetCordY(r),
                "set variable %Variable to the Y coordinate where the furre named {...} moved into/is at.");

            Add(TriggerCategory.Effect,
                r => BotSit(r),
                "make the bot sit down.");

            Add(TriggerCategory.Effect,
                r => BotLie(r),
                "make the bot lay down.");

            Add(TriggerCategory.Effect,
                r => BotStand(r),
                "make the bot stand up.");

            Add(TriggerCategory.Effect,
                r => BotMoveSequence(r),
                "Move the bot in this sequence {...} (one, sw, three, se, seven, nw, nine, or ne)");
        }

        public override void Unload(Page page)
        {
        }

        #endregion Public Methods

        #region Private Methods

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        [TriggerStringParameter]
        private bool AndDescContains(TriggerReader reader)
        {
            if (string.IsNullOrEmpty(Player.FurreDescription))
            {
                Logger.Warn("Description not found. Try looking at the furre first");
                return false;
            }

            return Player.FurreDescription.Contains(reader.ReadString());
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        private bool AndFurreNamedDescContains(TriggerReader reader)
        {
            var Target = Furres.GetFurreByName(reader.ReadString());
            var Pattern = reader.ReadString();
            if (string.IsNullOrEmpty(Target.FurreDescription))
            {
                Logger.Warn("Description for {Target.Name}not found. Try looking at the furre first");
                return false;
            }

            return Target.FurreDescription.Contains(Pattern);
        }

        private bool AndFurreNamedIsGender(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        [TriggerStringParameter]
        private bool AndNotDescContains(TriggerReader reader)
        {
            if (string.IsNullOrEmpty(Player.FurreDescription))
            {
                Logger.Warn("Description not found. Try looking at the furre first");
                return false;
            }

            return !Player.FurreDescription.Contains(reader.ReadString());
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        private bool AndNotDescContainsFurreNamed(TriggerReader reader)
        {
            Furre Target = Furres.GetFurreByName(reader.ReadString());
            if (string.IsNullOrEmpty(Target.FurreDescription))
            {
                Logger.Warn("Description not found. Try looking at the furre first");
                return false;
            }

            return !Target.FurreDescription.Contains(reader.ReadString());
        }

        private bool AndTriggeringFurreIsGender(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool AndTriggeringFurreStoodStill(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool BotLie(TriggerReader reader)
        {
            return SendServer("`lie");
        }

        [TriggerStringParameter]
        private bool BotMoveSequence(TriggerReader reader)
        {
            // TODO: http://bugtraq.tsprojects.org/view.php?id=55
            // Queue System?
            var directions = reader.ReadString();
            var r = new Regex(RGEX_Mov_Steps, RegexOptions.Compiled);
            MatchCollection m = r.Matches(directions);
            bool ServerSend = false;
            foreach (Match n in m)
            {
                if (n.Value.ToLower() == "ne")
                {
                    ServerSend = SendServer("`m9");
                }
                else if (n.Value.ToLower() == "se")
                {
                    ServerSend = SendServer("`m3");
                }
                else if (n.Value.ToLower() == "nw")
                {
                    ServerSend = SendServer("`m7");
                }
                else if (n.Value.ToLower() == "sw")
                {
                    ServerSend = SendServer("`m1");
                }
                else
                {
                    switch (int.Parse(n.Value))
                    {
                        case 1:
                        case 7:
                        case 3:
                        case 9:

                            ServerSend = SendServer("`m" + n.Value);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(m.ToString());
                    }
                }

                if (!ServerSend)
                {
                    break;
                }
            }

            return ServerSend;
        }

        private bool BotSit(TriggerReader reader)
        {
            return SendServer("`sit");
        }

        private bool BotStand(TriggerReader reader)
        {
            return SendServer("`stand");
        }

        [TriggerNumberParameter]
        [TriggerStringParameter]
        private bool FurreNamedColorsVar(TriggerReader reader)
        {
            var Var = reader.ReadVariable(true);
            var name = reader.ReadString();
            var Target = Furres.GetFurreByName(name);
            Var.Value = Target.FurreColors.ToString();
            return Target.FurreID >= 0;
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        [TriggerVariableParameter]
        [TriggerStringParameter]
        private bool FurreNamedDescVar(TriggerReader reader)
        {
            var Var = reader.ReadVariable(true);
            var name = reader.ReadString();
            var Target = Furres.GetFurreByName(name);
            if (string.IsNullOrEmpty(Target.FurreDescription))
            {
                Logger.Warn("Description for {Target.Name}not found. Try looking at the furre first");
                return false;
            }

            Var.Value = Target.FurreDescription;
            return true;
        }

        private bool FurreNamedFacingIsFacingDirection(TriggerReader r)
        {
            throw new NotImplementedException();
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        [TriggerVariableParameter]
        [TriggerStringParameter]
        private bool FurreNamedGenderVar(TriggerReader reader)
        {
            var Var = reader.ReadVariable(true);
            var name = reader.ReadString();
            var Target = Furres.GetFurreByName(name);
            switch (Target.FurreColors.Gender.Value)
            {
                case -1:
                    Logger.Warn("Gender not found. Try looking at the furre first");
                    return false;

                case 0:
                case 1:
                    Var.Value = Target.FurreColors.Gender;
                    break;
            }
            return true;
        }

        private bool FurreNamedLaying(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool FurreNamedMoveFrom(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool FurreNamedMoveInto(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        [TriggerStringParameter]
        private bool FurreNamedNoWings(TriggerReader reader)
        {
            var Target = Furres.GetFurreByName(reader.ReadString());
            switch (Target.LastStat)
            {
                case -1:
                    Logger.Warn("Wings type not found. Try looking at the furre first");
                    return false;

                case 0:
                case 1:
                    return Target.FurreColors.Wings != reader.ReadNumber();
            }
            return false;
        }

        [TriggerStringParameter]
        private bool FurreNamedSetCordX(TriggerReader reader)
        {
            var Cord = reader.ReadVariable(true);
            var TargetFurre = Furres.GetFurreByName(reader.ReadString());
            Cord.Value = TargetFurre.Location.X;
            return true;
        }

        [TriggerVariableParameter]
        [TriggerStringParameter]
        private bool FurreNamedSetCordY(TriggerReader reader)
        {
            var Cord = reader.ReadVariable(true);
            var TargetFurre = Furres.GetFurreByName(reader.ReadString());
            Cord.Value = TargetFurre.Location.Y;
            return true;
        }

        private bool FurreNamedSitting(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool FurreNamedSpecies(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        [TriggerVariableParameter]
        [TriggerStringParameter]
        private bool FurreNamedSpeciesVar(TriggerReader reader)
        {
            var Var = reader.ReadVariable(true);
            var name = reader.ReadString();
            var TargetFurre = Furres.GetFurreByName(name);
            switch (TargetFurre.LastStat)
            {
                case -1:
                    Logger.Warn("Species not found. Try looking at the furre first");
                    return false;

                case 0:
                case 1:
                    Var.Value = TargetFurre.FurreColors.Species;
                    break;
            }
            return true;
        }

        private bool FurreNamedStanding(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool FurreNamedStoodStill(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        [TriggerStringParameter]
        private bool FurreNamedWings(TriggerReader reader)
        {
            var Target = Furres.GetFurreByName(reader.ReadString());
            switch (Target.LastStat)
            {
                case -1:
                    Logger.Warn("Wings type not found. Try looking at the furre first");
                    return false;

                case 0:
                case 1:
                    return true;
            }
            return false;
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        [TriggerVariableParameter]
        [TriggerStringParameter]
        private bool FurreNamedWingsVar(TriggerReader reader)
        {
            var Var = reader.ReadVariable(true);
            var TargetFurre = (Furre)Furres.GetFurreByName(reader.ReadString());
            switch (TargetFurre.LastStat)
            {
                case -1:
                    Logger.Warn("Wings type not found. Try looking at the furre first");
                    return false;

                case 0:
                case 1:
                    Var.Value = TargetFurre.FurreColors.Wings;
                    break;
            }
            return true;
        }

        [TriggerNumberParameter]
        [TriggerNumberParameter]
        private bool MoveFrom(TriggerReader reader)
        {
            ReadTriggeringFurreParams(reader);
            var X = reader.ReadNumber();
            var Y = reader.ReadNumber();

            return Player.LastPosition == new FurrePosition(X, Y);
        }

        [TriggerDescription("Set the specified variable to the X coordinate of the Triggering Furre")]
        [TriggerVariableParameter]
        private bool SetVariableToCordX(TriggerReader reader)
        {
            reader.ReadVariable(true).Value = Player.Location.X;
            return true;
        }

        [TriggerDescription("Set the specified variable to the Y coordinate of the Triggering Furre")]
        [TriggerVariableParameter]
        private bool SetVariableToCordY(TriggerReader reader)
        {
            reader.ReadVariable(true).Value = Player.Location.Y;
            return true;
        }

        [TriggerVariableParameter]
        private bool TriggeringFurreColorsVar(TriggerReader reader)
        {
            reader.ReadVariable(true).Value = Player.FurreColors.ToString();
            return true;
        }

        [TriggerDescription("This trigger only works after the bot has looked at the specified furre")]
        [TriggerVariableParameter]
        private bool TriggeringFurreDescVar(TriggerReader reader)
        {
            if (Player.FurreDescription == null)
            {
                Logger.Warn("Description not found, Try looking at the furre first.");
                return false;
            }

            reader.ReadVariable(true).Value = Player.FurreDescription;
            return true;
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        private bool TriggeringFurreGenderVar(TriggerReader reader)
        {
            if (Player.FurreColors.Gender == -1 || Player.LastStat == -1)
            {
                Logger.Warn("Wings type not found, Try looking at the furre first");
                return false;
            }

            var Var = reader.ReadVariable(true);
            if (Player.LastStat == 0 || Player.LastStat == 1)
            {
                Var.Value = Player.FurreColors.Gender;
            }

            return true;
        }

        private bool TriggeringFurreIsFacingDirection(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool TriggeringFurreLaying(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool TriggeringFurreNoWings(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool TriggeringFurreSitting(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        private bool TriggeringFurreSpecies(TriggerReader reader)
        {
            if (Player.FurreColors.Gender == -1 || Player.LastStat == -1)
            {
                Logger.Warn("Wings type not found, Try looking at the furre first");
                return false;
            }

            var Spec = reader.ReadNumber();
            if (Player.LastStat == 0 || Player.LastStat == 1)
            {
                return true;
            }

            return false;
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        private bool TriggeringFurreSpeciesVar(TriggerReader reader)
        {
            switch (Player.LastStat)
            {
                case -1:
                    Logger.Warn("Species not found. Try looking at the furre first");
                    return false;

                case 0:
                case 1:
                    reader.ReadVariable(true).Value = Player.FurreColors.Species;
                    break;
            }
            return true;
        }

        private bool TriggeringFurreStanding(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        private bool TriggeringFurreWings(TriggerReader reader)
        {
            throw new NotImplementedException();
        }

        [TriggerDescription(" This trigger only works after the bot has looked at the specified furre")]
        private bool TriggeringFurreWingsVar(TriggerReader reader)
        {
            var Var = reader.ReadVariable(true);
            switch (Player.LastStat)
            {
                case -1:
                    Logger.Warn("Wings type not found. Try looking at the furre first");
                    return false;

                case 0:
                    Var.Value = Player.FurreColors.Wings;
                    break;

                case 1:
                    if (Player.FurreColors.Wings == -1)
                    {
                        if (Player.FurreColors.Wings == -1)
                        {
                            Logger.Warn("Wings type not found, Try looking at the furre first");
                            return false;
                        }

                        Var.Value = Player.FurreColors.Wings;
                    }
                    else
                    {
                        Var.Value = Player.FurreColors.Wings;
                    }

                    break;
            }
            return true;
        }

        [TriggerDescription("Send turn counter clockwise command to the game server")]
        private bool TurnCCW(TriggerReader reader)
        {
            return SendServer("`<");
        }

        [TriggerDescription("Send turn clockwise command to the game server")]
        private bool TurnCW(TriggerReader reader)
        {
            return SendServer("`>");
        }

        [TriggerDescription("Triggers once for any one that enters the bots view.")]
        private bool WhenAnyoneEnterView(TriggerReader reader)
        {
            ReadTriggeringFurreParams(reader);
            return Player.HasEnteredRange;
        }

        [TriggerDescription("Triggers once for any one that leaves the bots view.")]
        private bool WhenAnyoneLeaveView(TriggerReader reader)
        {
            ReadTriggeringFurreParams(reader);
            return Player.HasLeftRange;
        }

        [TriggerDescription("Triggers once when the triggering furre moves into the specified coordinates.")]
        [TriggerNumberParameter]
        [TriggerNumberParameter]
        private bool WhenAnyoneMoveInto(TriggerReader reader)
        {
            var X = reader.ReadNumber();
            var Y = reader.ReadNumber();

            return Player.Location == new FurrePosition(X, Y);
        }

        [TriggerDescription("Triggers once when the specified Furre enters the bot view.")]
        [TriggerStringParameter]
        private bool WhenFurreNamedEnterView(TriggerReader reader)
        {
            return TriggeringFurreNameIsAndSetVariables(reader) && Player.HasEnteredRange;
        }

        [TriggerDescription("Triggers once when the specified Furre leaves the bot view.")]
        [TriggerStringParameter]
        private bool WhenFurreNamedLeaveView(TriggerReader reader)
        {
            return TriggeringFurreNameIsAndSetVariables(reader) && Player.HasLeftRange;
        }

        #endregion Private Methods
    }
}