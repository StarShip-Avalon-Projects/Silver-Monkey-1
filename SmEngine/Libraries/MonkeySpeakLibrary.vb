﻿Imports System.Threading
Imports Furcadia.Net
Imports Furcadia.Net.Dream
Imports Furcadia.Text.FurcadiaMarkup
Imports Furcadia.Util
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
            Dim msMsg = StripFurcadiaMarkup(reader.ReadString()).ToLower.Trim

            Dim msg = Player.Message
            msg = StripFurcadiaMarkup(msg)
            Return msg.Contains(msMsg.ToLower)

        End Function

        ''' <summary>
        ''' Set <see cref="Player"/> and <see cref="Dream"/> from
        ''' GetParametersOfType&lt;T&gt;
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <returns>true if any parameter was set; false otherwise</returns>
        Public Function ReadParams(reader As TriggerReader) As Boolean
            Dim ParamSet = False
            Dim dreamInfo As DREAM() = reader.GetParametersOfType(Of DREAM)
            Dim ActiveFurre As IFurre() = reader.GetParametersOfType(Of IFurre)
            If dreamInfo IsNot Nothing Then
                If String.IsNullOrWhiteSpace(dreamInfo(0).Name) Then
                    Throw New ArgumentException("DreamInfo not set")
                End If
                Dream = dreamInfo(0)

                UpdateCurrentDreamVariables(Dream, reader.Page)
                ParamSet = True
            End If
            If ActiveFurre IsNot Nothing Then
                Player = ActiveFurre(0)
                If ActiveFurre(0).FurreID = -1 Then
                    Throw New ArgumentException("Invalid ActivePlayer")
                End If
                UpdateTriggerigFurreVariables(Player, reader.Page)
                ParamSet = True

            End If
            Return ParamSet
        End Function

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
            Dim msMsg = StripFurcadiaMarkup(reader.ReadString())
            Dim msg = Player.Message
            msg = StripFurcadiaMarkup(msg)
            'Debug.Print("Msg = " & msg)
            Return msg.EndsWith(msMsg)

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

            Dim msg = StripFurcadiaMarkup(Player.Message).ToLower.Trim
            Dim test = StripFurcadiaMarkup(reader.ReadString()).ToLower.Trim
            Return Not FurcadiaSession.IsConnectedCharacter AndAlso msg = test

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

            Dim msMsg = StripFurcadiaMarkup(reader.ReadString())

            Dim msg = Player.Message
            msg = StripFurcadiaMarkup(msg)

            Return Not msg.EndsWith(msMsg)

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

            Dim msMsg = StripFurcadiaMarkup(reader.ReadString())
            Dim msg = Player.Message
            msg = StripFurcadiaMarkup(msg)

            Return msg.StartsWith(msMsg)
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
            ReadParams(reader)
            Dim TmpName = reader.ReadString()
            Return FurcadiaShortName(TmpName) = Player.ShortName

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
        Private _HasHelp As Boolean
        Private _SkillLevel As Integer
        Private FurcTimeTimer As Timer

#End Region

#Region "Public Fields"

        ''' <summary>
        ''' Reference to the Main Bot Session for the bot
        ''' </summary>
        Public WithEvents FurcadiaSession As BotSession

        ''' <summary>
        ''' Reference to the Main Engine in the bot
        ''' </summary>
        Public WithEvents MsEngine As MonkeyspeakEngine

        ''' <summary>
        ''' Current Dream the Bot is in
        ''' </summary>
        Public Property Dream As DREAM

        ''' <summary>
        ''' Current Triggering Furre
        ''' </summary>
        Public Property Player As Furre

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
        Public Function SendServer(ByRef message As String) As Boolean
            If FurcadiaSession.IsServerConnected Then
                FurcadiaSession.SendFormattedTextToServer(message)
                Return True
            Else
                Return False
            End If
        End Function

#End Region

#Region "Public Constructors"

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
            MyBase.New()
            If Session Is Nothing Then
                Throw New ArgumentException("Session cannot be null")
            End If
            _SkillLevel = 0
            _HasHelp = False
            FurcadiaSession = Session
            FurcTimeTimer = New Timer(AddressOf TimeUpdate, Nothing,
                                      TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500))
        End Sub

#End Region

#Region "Common Library Methods"

        ''' <summary>
        ''' Is the specified furre in the dream?
        ''' </summary>
        ''' <param name="Name">
        ''' Furre Name
        ''' </param>
        ''' <returns>
        ''' True if the furre is in the dream
        ''' </returns>
        Public Function InDream(ByRef Name As String) As Boolean
            Dim found As Boolean = False
            For Each Fur In Dream.Furres
                If Fur.ShortName = FurcadiaShortName(Name) Then
                    found = True
                    Exit For
                End If
            Next
            Return found
        End Function

        Public Overrides Sub Unload(page As Page)

        End Sub

        Public Overrides Sub Initialize(ParamArray args() As Object)

        End Sub

#End Region

    End Class

End Namespace