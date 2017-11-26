﻿Imports System.Threading
Imports Furcadia.Net
Imports Furcadia.Net.Dream
Imports Monkeyspeak
Imports Monkeyspeak.Libraries
Imports SilverMonkeyEngine.MsLibHelper

Namespace Engine.Libraries

    ''' <summary>
    ''' The base library in which all Silver Monkey's Monkey Speak Libraries
    ''' are built on. This Library contains the commonly used functions for
    ''' all the other libraries
    ''' </summary>
    Public Class MonkeySpeakLibrary
        Inherits BaseLibrary

#Region "Protected Methods"

        ''' <summary>
        ''' <para>
        ''' Comparisons are done with Fucadia Markup Stripped
        ''' </para>
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' True if the %MESSAGE system variable contains the specified string
        ''' </returns>
        Protected Overridable Function MsgContains(reader As TriggerReader) As Boolean
            ReadParams(reader)
            Dim msMsg = reader.ReadString().ToStrippedFurcadiaMarkupString().ToLower

            Dim msg = Player.Message
            Return msg.Contains(msMsg.ToStrippedFurcadiaMarkupString().ToLower)

        End Function

        ''' <summary>
        ''' Set <see cref="Player"/> and <see cref="Dream"/> from
        ''' GetParametersOfType&lt;T&gt;
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <returns>true if any parameter was set; false otherwise</returns>
        Public Function ReadParams(reader As TriggerReader) As Boolean
            Dim ParamSet = False
            Dim dreamInfo = reader.GetParametersOfType(Of DREAM)
            Dim ActiveFurre = reader.GetParametersOfType(Of IFurre)
            If dreamInfo IsNot Nothing AndAlso dreamInfo.Count > 0 Then
                If String.IsNullOrWhiteSpace(dreamInfo(0).Name) Then
                    Throw New ArgumentException("DreamInfo not set")
                End If
                Dream = dreamInfo(0)
                ParamSet = True
                UpdateCurrentDreamVariables(Dream, reader.Page)
            End If

            If ActiveFurre IsNot Nothing AndAlso ActiveFurre.Count > 0 Then
                If ActiveFurre(0).ShortName = "unknown" Then
                    Throw New ArgumentException("Invalid ActivePlayer")
                End If
                Player = ActiveFurre(0)
                ParamSet = True

            End If
            If Player IsNot Nothing Then UpdateTriggerigFurreVariables(Player, reader.Page)
            Return ParamSet
        End Function

        Public ConnectedFurre As Furre

        ''' <summary>
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' true if the System %MESSAGE varible ends with the specified string
        ''' </returns>
        Protected Overridable Function MsgEndsWith(reader As TriggerReader) As Boolean
            ReadParams(reader)
            Dim msMsg = reader.ReadString().ToStrippedFurcadiaMarkupString()
            Dim msg = Player.Message.ToStrippedFurcadiaMarkupString()
            'Debug.Print("Msg = " & msg)
            Return msg.ToLower().EndsWith(msMsg.ToLower()) And Not IsConnectedCharacter(Player)

        End Function

        ''' <summary>
        ''' the Main Message is Comparason function
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' true on success
        ''' </returns>
        Protected Overridable Function MsgIs(reader As TriggerReader) As Boolean
            ReadParams(reader)

            Dim msg = Player.Message.ToStrippedFurcadiaMarkupString()
            Dim test = reader.ReadString().ToStrippedFurcadiaMarkupString()
            Return Not IsConnectedCharacter(Player) AndAlso
                msg.ToLower() = test.ToLower()

        End Function

        ''' <summary>
        ''' (1:14) and triggering furre's message doesn't end with {.},
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Protected Function MsgNotEndsWith(reader As TriggerReader) As Boolean
            ReadParams(reader)

            Dim msMsg = reader.ReadString().ToStrippedFurcadiaMarkupString().ToLower()
            Dim msg = Player.Message.ToStrippedFurcadiaMarkupString().ToLower

            Return Not msg.ToLower().EndsWith(msMsg.ToLower()) And Not IsConnectedCharacter(Player)

        End Function

        ''' <summary>
        ''' (1:11) and triggering furre's message starts with {.},
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Protected Function MsgStartsWith(reader As TriggerReader) As Boolean

            ReadParams(reader)

            Dim msMsg = reader.ReadString().ToStrippedFurcadiaMarkupString().ToLower()
            Dim msg = Player.Message.ToStrippedFurcadiaMarkupString().ToLower
            Return msg.ToLower().StartsWith(msMsg.ToLower()) And Not IsConnectedCharacter(Player)
        End Function

        ''' <summary>
        ''' Generic base Furre named {...} is Triggering Furre
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' True on Name match
        ''' </returns>
        ''' <remarks>
        ''' any name is acepted and converted to Furcadia Machine name
        ''' (ShortName version, lowercase with special characters stripped)
        ''' </remarks>
        Protected Overridable Function NameIs(reader As TriggerReader) As Boolean

            Return reader.ReadString.ToFurcadiaShortName() = Player.ShortName And Not IsConnectedCharacter(Player)

        End Function

#End Region

#Region "Public Properties"

        ''' <summary>
        ''' Current Furcadia Standard Time (fst)
        ''' </summary>
        ''' <returns>
        ''' Furcadia Time Object in Furcadia Standard Time (fst)
        ''' </returns>
        Public ReadOnly Property FurcTime As DateTime
            Get
                Return _FurcTime
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Furcadia Clock updater
        ''' </summary>
        ''' <param name="obj">
        ''' Nothing
        ''' </param>
        Private Sub TimeUpdate(obj As Object)
            _FurcTime = Now
        End Sub

#End Region

#Region "Private Fields"

        Private _FurcTime As DateTime
        Private FurcTimeTimer As Timer

#End Region

#Region "Public Fields"

        ''' <summary>
        ''' Reference to the Main Bot Session for the bot
        ''' </summary>
        Public WithEvents FurcadiaSession As BotSession

        ''' <summary>
        ''' Current Dream the Bot is in
        ''' <para/>
        ''' Referenced as a Monkeyspeak Parameter.
        ''' <para/>
        ''' Updates when ever Monkey Speak needs it through <see cref="Monkeyspeak.Page.Execute(Integer(), Object())"/>
        ''' </summary>
        Public Dream As DREAM

        ''' <summary>
        ''' Current Triggering Furre
        ''' <para/>
        ''' Referenced as a Monkeyspeak Parameter.
        ''' <para/>
        ''' Updates when ever Monkey Speak needs it through <see cref="Monkeyspeak.Page.Execute(Integer(), Object())"/>
        ''' </summary>
        Public Player As Furre

#End Region

#Region "Public Delegates"

        ''' <summary>
        ''' Send a raw instruction to the client
        ''' </summary>
        ''' <param name="message">
        ''' Message sring
        ''' </param>
        Public Sub SendClientMessage(ByRef message As String)
            FurcadiaSession.SendToClient(message)
        End Sub

        ''' <summary>
        ''' Send Formated Text to Server
        ''' </summary>
        ''' <param name="message">
        ''' Client to server instruction
        ''' </param>
        ''' <returns>
        ''' True is the Server is Connected
        ''' </returns>
        Public Function SendServer(message As String) As Boolean
            If FurcadiaSession.IsServerSocketConnected Then
                FurcadiaSession.SendFormattedTextToServer(message)
                Return True
            Else
                Return False
            End If
        End Function

        Private Async Function SendServerAsync(message As String) As Task(Of Boolean)
            Return Await Task.Run(Function() SendServer(message))
        End Function

#End Region

#Region "Public Constructors"

        Sub New()
            MyBase.New()
            FurcTimeTimer = New Timer(AddressOf TimeUpdate, Nothing,
               TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500))
        End Sub

        ''' <summary>
        ''' Constructor for Unit Testing prposes
        ''' </summary>
        ''' <param name="BotFurre"></param>
        Sub New(ByRef BotFurre As IFurre)
            Me.New()
            If BotFurre Is Nothing Then
                Throw New ArgumentException("Session cannot be null")
            End If
            ConnectedFurre = BotFurre
        End Sub

        ''' <summary>
        ''' Default Constructor
        ''' <para>
        ''' References the main components cfrom <see cref="BotSession"/>
        ''' </para>
        ''' </summary>
        ''' <exception cref="ArgumentException">
        ''' Thrown when Session is not provided
        ''' </exception>
        Sub New(ByRef Session As BotSession)
            Me.New(Session.ConnectedFurre)
            FurcadiaSession = Session

        End Sub

#End Region

#Region "Common Library Methods"

        ''' <summary>
        ''' checks the <see cref="Furcadia.Net.Dream.FurreList"/>
        ''' in the <see cref="MonkeySpeakLibrary.Dream">Dream Parameter</see>
        ''' for the Target Furre.
        ''' </summary>
        ''' <param name="TargetFurre">
        ''' Target Furre
        ''' </param>
        ''' <returns>
        ''' True if the furre is in the dream <see cref="Furcadia.Net.Dream.FurreList"/>
        ''' </returns>
        Public Function InDream(TargetFurre As Furre) As Boolean
            Dim found = False
            For Each Fur In Dream.Furres
                If Fur = TargetFurre Then
                    found = True
                    Exit For
                End If
            Next
            Return found
        End Function

        ''' <summary>
        ''' Seperate function for unit testing
        ''' </summary>
        ''' <param name="Furr"></param>
        ''' <returns></returns>
        Public Function IsConnectedCharacter(Furr As Furre) As Boolean
            If FurcadiaSession IsNot Nothing Then
                Return FurcadiaSession.IsConnectedCharacter(Furr)
            End If
            Return ConnectedFurre = Furr
        End Function

        Public Overrides Sub Unload(page As Page)
            FurcTimeTimer.Dispose()
        End Sub

        Public Overrides Sub Initialize(ParamArray args() As Object)

        End Sub

#End Region

    End Class

End Namespace