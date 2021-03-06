﻿using Engine.BotSession;
using Furcadia.Logging;
using Furcadia.Net;
using Furcadia.Net.Utils.ChannelObjects;
using Furcadia.Net.Utils.ServerParser;
using IO;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using static Libraries.MsLibHelper;
using static SmEngineTests.Utilities;

namespace SmEngineTests
{
    [TestFixture]
    public class SilverMonkeyQueryTests
    {
        public const string GeroJoinBot = "<font color='query'><name shortname='gerolkae'>Gerolkae</name> requests permission to join your company. To accept the request, <a href='command://summon'>click here</a> or type `summon and press &lt;enter&gt;.</font>";
        public const string GeroFollowBot = "<font color='query'><name shortname='gerolkae'>Gerolkae</name> requests permission to follow you. To accept the request, <a href='command://lead'>click here</a> or type `lead and press &lt;enter&gt;.</font>";
        public const string GeroLeadBot = "<font color='query'><name shortname='gerolkae'>Gerolkae</name> requests permission to lead you. To accept the request, <a href='command://follow'>click here</a> or type `follow and press &lt;enter&gt;.</font>";
        public const string GeroSummonBot = "<font color='query'><name shortname='gerolkae'>Gerolkae</name> asks you to join their company in <b>the dream of Silver|Monkey</b>. To accept the request, <a href='command://join'>click here</a> or type `join and press &lt;enter&gt;.</font>";
        public const string GeroCuddleBot = "<font color='query'><name shortname='gerolkae'>Gerolkae</name> asks you to cuddle with them. To accept the request, <a href='command://cuddle'>click here</a> or type `cuddle and press &lt;enter&gt;.</font>";

        private static Bot Proxy;

        public string SettingsFile { get; private set; }
        public string BackupSettingsFile { get; private set; }
        public BotOptions Options { get; private set; }

        [OneTimeSetUp]
        public void Initialize()
        {
            var BotFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Silver Monkey.bini");
            var MsFile = Path.Combine(IO.Paths.SilverMonkeyDocumentsPath,
                "Bugreport 165 From Jake.ms");
            var CharacterFile = Path.Combine(Paths.FurcadiaCharactersFolder,
                "silvermonkey.ini");
            var MsEngineOption = new EngineOptoons()
            {
                MonkeySpeakScriptFile = MsFile,
                IsEnabled = false,
                BotController = @"Gerolkae"
            };
            Options = new BotOptions(BotFile)
            {
                Standalone = true,
                CharacterIniFile = CharacterFile,
                MonkeySpeakEngineOptions = MsEngineOption,
                ResetSettingTime = 10
            };

            Options.SaveBotSettings();

            Proxy = new Bot(Options);
            Proxy.Error += (e, o) => Logger.Error($"{e} {o}");
            BotHasConnected();
        }

        [TestCase(GeroJoinBot, "Gerolkae")]
        [TestCase(GeroFollowBot, "Gerolkae")]
        [TestCase(GeroLeadBot, "Gerolkae")]
        [TestCase(GeroSummonBot, "Gerolkae")]
        [TestCase(GeroCuddleBot, "Gerolkae")]
        public void ExpectedQueryCharacter(string testc, string ExpectedValue)
        {
            bool IsTested = false;
            Proxy.ProcessServerChannelData += (sender, Args) =>
            {
                if (!IsTested && sender is QueryChannelObject queryObject)
                {
                    Assert.That(queryObject.Player.ShortName,
                        Is.EqualTo(ExpectedValue.ToFurcadiaShortName()));
                    IsTested = true;
                }
            };

            Proxy.ParseServerChannel(testc, false);
            Proxy.ProcessServerChannelData -= (sender, Args) =>
            {
                if (!IsTested && sender is QueryChannelObject queryObject)
                {
                    Assert.That(queryObject.Player.ShortName,
                        Is.EqualTo(ExpectedValue.ToFurcadiaShortName()));
                    IsTested = true;
                }
            };
            Console.WriteLine($"ServerStatus: {Proxy.ServerStatus}");
            Console.WriteLine($"ClientStatus: {Proxy.ClientStatus}");
        }

        [TestCase(GeroJoinBot, QueryType.join)]
        [TestCase(GeroFollowBot, QueryType.follow)]
        [TestCase(GeroLeadBot, QueryType.lead)]
        [TestCase(GeroSummonBot, QueryType.summon)]
        [TestCase(GeroCuddleBot, QueryType.cuddle)]
        public void ChannelIsQueryOfType(string ChannelCode, QueryType ExpectedValue)
        {
            bool IsTested = false;
            Proxy.ProcessServerChannelData += (sender, Args) =>
            {
                if (!IsTested && sender is QueryChannelObject queryObject)
                {
                    IsTested = true;
                    Assert.Multiple(() =>
                    {
                        Assert.That(queryObject.Query,
                            Is.EqualTo(ExpectedValue));
                    });
                }
            };

            Proxy.ParseServerChannel(ChannelCode, false);
            Proxy.ProcessServerChannelData -= (sender, Args) =>
            {
                if (!IsTested && sender is QueryChannelObject queryObject)
                {
                    IsTested = true;
                    Assert.Multiple(() =>
                    {
                        Assert.That(queryObject.Query,
                            Is.EqualTo(ExpectedValue));
                    });
                }
            };
        }

        // TODO: Need MonkeySpeak Script
        public void QueryMonkeySpeakVariablesAreSet()
        {
            if (!Proxy.StandAlone)
                HaltFor(DreamEntranceDelay);

            Assert.Multiple(() =>
            {
                var Var = Proxy.MSpage.GetVariable(TriggeringFurreNameVariable);
                Assert.That(Var.Value,
                    !Is.EqualTo(null),
                    $"Constant Variable: '{Var}' ");
                Assert.That(Var.IsConstant,
                    Is.EqualTo(true),
                    $"Constant Variable: '{Var}' ");
                Assert.That(Var.Value.ToString(),
                    Is.EqualTo("Gerolkae"),
                    $"Constant Variable: '{Var}' ");

                Var = Proxy.MSpage.GetVariable(TriggeringFurreShortNameVariable);
                Assert.That(Var.Value,
                    !Is.EqualTo(null),
                    $"Constant Variable: '{Var}' ");
                Assert.That(Var.IsConstant,
                    Is.EqualTo(true),
                    $"Constant Variable: '{Var}' ");
                Assert.That(Var.Value.ToString(),
                    Is.EqualTo("gerolkae"),
                    $"Constant Variable: '{Var}' ");
            });
        }

        public void BotHasConnected(bool StandAlone = true)
        {
            Proxy.StandAlone = StandAlone;
            Task.Run(() => Proxy.ConnetAsync()).Wait();

            Assert.Multiple(() =>
            {
                Assert.That(Proxy.ServerStatus,
                    Is.EqualTo(ConnectionPhase.Connected),
                    $"Proxy.ServerStatus {Proxy.ServerStatus}");
                Assert.That(Proxy.IsServerSocketConnected,
                    Is.EqualTo(true),
                    $"Proxy.IsServerSocketConnected {Proxy.IsServerSocketConnected}");
                if (StandAlone)
                {
                    Assert.That(Proxy.ClientStatus,
                        Is.EqualTo(ConnectionPhase.Disconnected),
                         $"Proxy.ClientStatus {Proxy.ClientStatus}");
                    Assert.That(Proxy.IsClientSocketConnected,
                        Is.EqualTo(false),
                         $"Proxy.IsClientSocketConnected {Proxy.IsClientSocketConnected}");
                    Assert.That(Proxy.FurcadiaClientIsRunning,
                        Is.EqualTo(false),
                        $"Proxy.FurcadiaClientIsRunning {Proxy.FurcadiaClientIsRunning}");
                }
                else
                {
                    Assert.That(Proxy.ClientStatus,
                        Is.EqualTo(ConnectionPhase.Connected),
                        $"Proxy.ClientStatus {Proxy.ClientStatus}");
                    Assert.That(Proxy.IsClientSocketConnected,
                        Is.EqualTo(true),
                        $"Proxy.IsClientSocketConnected {Proxy.IsClientSocketConnected}");
                    Assert.That(Proxy.FurcadiaClientIsRunning,
                        Is.EqualTo(true),
                        $"Proxy.FurcadiaClientIsRunning {Proxy.FurcadiaClientIsRunning}");
                }
            });
        }

        public void BotHaseDisconnected()
        {
            Proxy.Disconnect();
            if (!Proxy.StandAlone)
                HaltFor(CleanupDelayTime);

            Assert.Multiple(() =>
            {
                Assert.That(Proxy.ServerStatus,
                     Is.EqualTo(ConnectionPhase.Disconnected),
                    $"Proxy.ServerStatus {Proxy.ServerStatus}");
                Assert.That(Proxy.IsServerSocketConnected,
                     Is.EqualTo(false),
                    $"Proxy.IsServerSocketConnected {Proxy.IsServerSocketConnected}");
                Assert.That(Proxy.ClientStatus,
                     Is.EqualTo(ConnectionPhase.Disconnected),
                     $"Proxy.ClientStatus {Proxy.ClientStatus}");
                Assert.That(Proxy.IsClientSocketConnected,
                     Is.EqualTo(false),
                     $"Proxy.IsClientSocketConnected {Proxy.IsClientSocketConnected}");
                Assert.That(Proxy.FurcadiaClientIsRunning,
                     Is.EqualTo(false),
                    $"Proxy.FurcadiaClientIsRunning {Proxy.FurcadiaClientIsRunning}");
            });
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            BotHaseDisconnected();
            Proxy.Error -= (e, o) => Logger.Error($"{e} {o}");

            Proxy.Dispose();
            Options = null;
        }
    }
}