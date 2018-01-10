﻿Imports System.IO
Imports Furcadia.Net.Pounce
Imports MonkeyCore
Imports Monkeyspeak
Imports MsLibHelper

''' <summary>
''' Pounce Server interface with a list of furres contained in a simple text file. This system is styled after <see cref="MsMemberList"/>
''' </summary>
''' <remarks>
''' This classe is the Monkey Speak interface for using the Furcadia Pounce server. It does not read the online.ini located in the Furcadia App-Data Folder.
''' Instead it uses a text file With a list Of Furcadia Names(Long Or Short formats Don't matter). Furcadia.Net.Pounce is a HTTP(S)  Async Post Method
''' class that sends the requests to the server once every 30 seconds.
''' <para/>
''' Why 30 seconds? Because the Furcadia pounce server runs on a 30 second cron job, Therefore it makes sense to stick with it update time.
''' </remarks>
Public NotInheritable Class MsPounce
    Inherits MonkeySpeakLibrary
    Implements IDisposable

#Region "Fields"

    ''' <summary>
    ''' Pounce list
    ''' </summary>
    Private WithEvents OnlineFurres As IO.NameList

    ''' <summary>
    ''' Furcadia Pounce Server
    ''' </summary>
    Private WithEvents SmPounce As PounceClient

#End Region

#Region "Public Fields"

    ''' <summary>
    ''' Default File we use
    ''' </summary>
    Public Const ListFile As String = "onlineList.txt"

#End Region

#Region "Private Fields"

    ''' <summary>
    ''' Pounce List File Name
    ''' </summary>
    Private _onlineListFile As String

    ''' <summary>
    ''' Pounce Furre List
    ''' </summary>
    Private PounceFurres As List(Of MsPounceFurre)

#End Region

#Region "Public Properties"

    ''' <summary>
    ''' the File of the Friends List to Check
    ''' <para/>
    ''' Defaults to 'BotFolder\onlineList.txt"
    ''' </summary>
    ''' <returns>
    ''' </returns>
    Public ReadOnly Property OnlineListFile As String
        Get
            Return _onlineListFile
        End Get

    End Property

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' (5:951) add the furre named {...} to the smPounce list.
    ''' they aren't already on it.
    ''' </summary>
    ''' <param name="reader"> <see cref="TriggerReader"/>
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function AddFurreNamed(reader As TriggerReader) As Boolean
        'TODO: Fix to Pounce Reader

        If FurreNamedIsMember(reader) = False AndAlso FurreNamedIsNotMember(reader) Then
            Using sw As StreamWriter = New StreamWriter(_onlineListFile, True)
                sw.WriteLine(reader.ReadString)
            End Using
        End If
        Return True

    End Function

    ''' <summary>
    ''' (5:950) add the triggering furre to the smPounce List.
    ''' aren't already on it.
    ''' <para/>
    ''' Defaults to 'BotFolder\on-lineList.txt"
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function AddTrigFurre(reader As TriggerReader) As Boolean

        Dim Furre = reader.Page.GetVariable(TriggeringFurreNameVariable).Value.ToString
        If TrigFurreIsMember(reader) = False AndAlso TrigFurreIsNotMember(reader) Then
            Dim sw As StreamWriter = New StreamWriter(_onlineListFile, True)
            sw.WriteLine(Furre)
            sw.Close()
        End If
        Return True

    End Function

    '''' <summary>
    '''' MonkeySpeak Execute When a Furre Logged Off
    '''' <para>
    '''' (0:951) When a furre logs off,
    '''' </para>
    '''' <para>
    '''' (0:953) When the furre named {...} logs off,
    '''' </para>
    '''' </summary>
    '''' <param name="Furre">
    '''' PounceFurre Object
    '''' </param>
    '''' <param name="e">
    '''' <see cref="Eventargs.Empty"/>
    '''' </param>
    'Public Sub FurreLoggedOff(ByVal Furre As Object, e As EventArgs)
    '    Dim Furr = DirectCast(Furre, PounceFurre)
    '    DirectCast(ParentBotSession.MSpage.GetVariable("%NAME"), ConstantVariable).SetValue(Furr.Name)
    '    ParentBotSession.MSpage.Execute(951, 953)
    'End Sub

    '''' <summary>
    '''' MonkeySpeak Execute When a Furre Logged on
    '''' <para>
    '''' (0:950) When a furre logs on,
    '''' </para>
    '''' <para>
    '''' (0:952) When the furre named {...} logs on,
    '''' </para>
    '''' </summary>
    '''' <param name="Furre">
    '''' PounceFurre Object
    '''' </param>
    '''' <param name="e">
    '''' EventSrgs.Empty
    '''' </param>
    'Public Sub FurreLoggedOn(ByVal Furre As Object, e As EventArgs)
    '    Dim Furr = DirectCast(Furre, PounceFurre)
    '    DirectCast(ParentBotSession.MSpage.GetVariable("%NAME"), ConstantVariable).SetValue(Furr.Name)
    '    ParentBotSession.MSpage.Execute(950, 952)
    'End Sub

    ''' <summary>
    ''' (1:954) and the furre named {...} is on the smPounce list,
    ''' </summary>
    ''' <param name="reader"><see cref="TriggerReader"/>
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function FurreNamedIsMember(reader As Monkeyspeak.TriggerReader) As Boolean
        CheckonlineList()

        Dim Furre = reader.ReadString
        Dim f = File.ReadAllLines(_onlineListFile)
        For Each l In f
            If l.ToFurcadiaShortName() = Furre.ToFurcadiaShortName() Then Return True
        Next
        Return False

    End Function

    ''' <summary>
    ''' (1:955) and the furre named {...} is not on the smPounce list,
    ''' </summary>
    ''' <param name="reader"> <see cref="TriggerReader"/>
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function FurreNamedIsNotMember(reader As Monkeyspeak.TriggerReader) As Boolean
        Return Not FurreNamedIsMember(reader)
    End Function

    ''' <summary>
    ''' (1:951) and the furre named {...} is off-line,
    ''' </summary>
    ''' <param name="reader"><see cref="TriggerReader"/></param>
    ''' <returns></returns>
    Public Function FurreNamedNotOnline(reader As TriggerReader) As Boolean

        Dim TmpName = reader.ReadString()
        For Each Fur In PounceFurres
            If Fur.ShortName = TmpName.ToFurcadiaShortName() Then
                Return Not Fur.Online
            End If
        Next
        'add Machine Name parser
        Return False

    End Function

    ''' <summary>
    ''' (1:950) and the furre named {...} is on-line,
    ''' </summary>
    ''' <param name="reader"><see cref="TriggerReader"/></param>
    ''' <returns></returns>
    Public Function FurreNamedonline(reader As TriggerReader) As Boolean

        Dim TmpName = reader.ReadString()
        For Each Fur In PounceFurres
            If Fur.ShortName = TmpName.ToFurcadiaShortName() Then
                Return Fur.Online
            End If
        Next
        'add Machine Name parser
        Return False

    End Function

    Public Overrides Sub Initialize(ParamArray args() As Object)
        MyBase.Initialize(args)

        PounceFurres = New List(Of PounceFurre)
        'Setup our Default Objects
        _onlineListFile = Paths.CheckBotFolder(ListFile)
        OnlineFurres = New IO.NameList(_onlineListFile)
        ' (0:950) When a furre logs on,
        Add(TriggerCategory.Cause, 950,
        Function() True, "When a furre logs on,")

        '(0:951) When a furre logs off,
        Add(TriggerCategory.Cause, 951,
        Function() True, "When a furre logs off,")
        '(0:952) When the furre named {...} logs on,
        Add(TriggerCategory.Cause, 952,
            AddressOf NameIs, "When the furre named {...} logs on,")
        '(0:953) When the furre named {...} logs off,
        Add(TriggerCategory.Cause, 953,
            AddressOf NameIs, "When the furre named {...} logs off,")

        '(1;950) and the furre named {...} is on-line,
        Add(TriggerCategory.Condition, 950,
            AddressOf FurreNamedonline, "and the furre named {...} is on-line,")

        '(1:951) and the furre named {...} is off-line,
        Add(TriggerCategory.Condition, 951,
            AddressOf FurreNamedNotOnline, "and the furre named {...} is off-line,")

        '(1:952) and triggering furre is on the smPounce List,
        Add(TriggerCategory.Condition, 952,
            AddressOf TrigFurreIsMember, "and triggering furre is on the smPounce List,")
        '(1:953) and the triggering furre is not on the smPounce List,
        Add(TriggerCategory.Condition, 953,
            AddressOf TrigFurreIsNotMember, "and the triggering furre is not on the smPounce List,")

        '(1:954) and the furre named {...} is on the smPounce list,
        Add(TriggerCategory.Condition, 954,
            AddressOf FurreNamedIsMember, "and the furre named {...} is on the smPounce list,")

        '(1:955) and the furre named {...} is not on the smPounce list,
        Add(TriggerCategory.Condition, 955,
            AddressOf FurreNamedIsNotMember, "and the furre named {...} is not on the smPounce list,")

        '(5:950) add the triggering furre to the smPounce List.
        Add(TriggerCategory.Effect, 950,
            AddressOf AddTrigFurre, "add the triggering furre to the smPounce List.")
        '(5:951) add the furre named {...} to the smPounce list.
        Add(TriggerCategory.Effect, 951,
            AddressOf AddFurreNamed, "add the furre named {...} to the smPounce list.")
        '(5:952) remove the triggering furre from the smPounce list.
        Add(TriggerCategory.Effect, 952,
            AddressOf RemoveTrigFurre, "remove the triggering furre from the smPounce list.")
        '(5:953) remove the furre named {...} from the smPounce list.
        Add(TriggerCategory.Effect, 953,
            AddressOf RemoveFurreNamed, "remove the furre named {...} from the smPounce list.")
        '(5:954) use the file named {...} as the smPounce list.
        Add(TriggerCategory.Effect, 954,
            AddressOf UseMemberFile, "use the file named {...} as the smPounce list and start the Pounce Clinet Interface.")
    End Sub

    ''' <summary>
    ''' (5:953) remove the furre named {...} from the smPounce list.
    ''' </summary>
    ''' <param name="reader"><see cref="TriggerReader"/>
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function RemoveFurreNamed(reader As TriggerReader) As Boolean

        CheckonlineList()

        Dim Furre = reader.ReadString
        Dim line As String = Nothing
        Dim linesList = New List(Of String)(File.ReadAllLines(_onlineListFile))
        Using SR = New StreamReader(_onlineListFile)
            line = SR.ReadLine()
            For i = 0 To linesList.Count - 1
                If line.ToFurcadiaShortName() = Furre.ToFurcadiaShortName() Then
                    linesList.RemoveAt(i)
                    File.WriteAllLines(_onlineListFile, linesList.ToArray())
                    Return True
                End If
                line = SR.ReadLine()
            Next i
        End Using

        Return False
    End Function

    ''' <summary>
    ''' (5:952) remove the triggering furre from the smPounce list.
    ''' </summary>
    ''' <param name="reader"> <see cref="TriggerReader"/>
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function RemoveTrigFurre(reader As TriggerReader) As Boolean

        CheckonlineList()

        Dim Furre = reader.Page.GetVariable(TriggeringFurreNameVariable).Value.ToString
        Furre = Furre.ToFurcadiaShortName()
        Dim line As String = Nothing
        Dim linesList = New List(Of String)(File.ReadAllLines(_onlineListFile))
        Using SR As New StreamReader(_onlineListFile)
            line = SR.ReadLine()
            For i As Integer = 0 To linesList.Count - 1
                If line.ToFurcadiaShortName() = Furre Then
                    linesList.RemoveAt(i)
                    File.WriteAllLines(_onlineListFile, linesList.ToArray())
                    Return True
                End If
                line = SR.ReadLine()
            Next i
        End Using

        Return False
    End Function

    ''' <summary>
    ''' (1:952) and triggering furre is on the smPounce List,
    ''' </summary>
    ''' <param name="reader">
    ''' <see cref="TriggerReader"/>
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function TrigFurreIsMember(reader As Monkeyspeak.TriggerReader) As Boolean
        CheckonlineList()

        Dim Furre = reader.Page.GetVariable(TriggeringFurreNameVariable).Value.ToString
        Dim f = File.ReadAllLines(_onlineListFile)
        For Each line In f
            If line.ToFurcadiaShortName() = Furre.ToFurcadiaShortName() Then Return True
        Next
        Return False

    End Function

    ''' <summary>
    ''' (1:953) and the triggering furre is not on the smPounce List,
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function TrigFurreIsNotMember(reader As Monkeyspeak.TriggerReader) As Boolean
        Return Not TrigFurreIsMember(reader)
    End Function

    Public Overrides Sub Unload(page As Page)
        Dispose(True)
    End Sub

    ''' <summary>
    ''' (5:904) Use file {...} as the dream member list.
    ''' <para/>
    ''' Defaults to 'BotFolder\on-lineList.txt"
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' True on Success
    ''' </returns>
    Public Function UseMemberFile(reader As TriggerReader) As Boolean

        Dim FileList = reader.ReadString
        CheckonlineList()
        If SmPounce IsNot Nothing Then SmPounce.Dispose()
        SmPounce = New PounceClient(OnlineFurres.ToArray, Nothing)

        Return True
    End Function

#End Region

#Region "Private Methods"

    Private Sub CheckonlineList()
        _onlineListFile = Paths.CheckBotFolder(_onlineListFile)
        If File.Exists(_onlineListFile) = False Then
            Console.WriteLine("On-line List File: "+ _onlineListFile + "Doesn't Exist, Creating new file")
            Using sw = New StreamWriter(_onlineListFile, False)
                sw.Close()
            End Using
        End If
    End Sub

#End Region

#Region "IDisposable Support"

    Private disposedValue As Boolean ' To detect redundant calls

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)

    End Sub

    ' IDisposable
    Protected Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                If SmPounce IsNot Nothing Then SmPounce.Dispose()
            End If

        End If
        disposedValue = True
    End Sub

#End Region

End Class