﻿Imports Monkeyspeak
Imports System.Text.RegularExpressions
Imports System.Collections.Generic
Imports MonkeyCore
Imports MonkeyCore.IO
Imports SilverMonkey

Public Class MS_Pounce
    Inherits Libraries.AbstractBaseLibrary
    Private writer As TextBoxWriter = Nothing

    Private Shared _OnlineList As String
    Public Shared Property OnlineList As String
        Get
            If String.IsNullOrEmpty(_OnlineList) Then
                _OnlineList = Paths.CheckBotFolder("OnlineList.txt")
            End If
            Return _OnlineList
        End Get
        Set(value As String)

        End Set
    End Property

    Sub New()
        writer = New TextBoxWriter(Variables.TextBox1)

        ' (0:950) When a furre logs on,
        Add(TriggerCategory.Cause, 950,
            Function()
                Return True
            End Function, "(0:950) When a furre logs on,")


        '(0:951) When a furre logs off,
        Add(TriggerCategory.Cause, 951,
            Function()
                Return True
            End Function, "(0:951) When a furre logs off,")
        '(0:952) When the furre named {...} logs on,
        Add(TriggerCategory.Cause, 952, AddressOf NameIs, "(0:952) When the furre named {...} logs on,")
        '(0:953) When the furre named {...} logs off,
        Add(TriggerCategory.Cause, 953, AddressOf NameIs, "(0:953) When the furre named {...} logs off,")



        '(1;950) and the furre named {...} is online,  
        Add(New Trigger(TriggerCategory.Condition, 950), AddressOf FurreNamedOnline, "(1:950) and the furre named {...} is online,")

        '(1:951) and the furre named {...} is offline,
        Add(New Trigger(TriggerCategory.Condition, 951), AddressOf FurreNamedNotOnline, "(1:951) and the furre named {...} is offline,")

        '(1:952) and triggering furre is on the smPounce List,
        Add(New Trigger(TriggerCategory.Condition, 952), AddressOf TrigFurreIsMember, "(1:952) and triggering furre is on the smPounce List,")
        '(1:953) and the triggering furre is not on the smPounce List,
        Add(New Trigger(TriggerCategory.Condition, 953), AddressOf TrigFurreIsNotMember, "(1:953) and the triggering furre is not on the smPounce List,")

        '(1:954) and the furren named {...} is on the smpounce list,
        Add(New Trigger(TriggerCategory.Condition, 954), AddressOf FurreNamedIsMember, "(1:954) and the furren named {...} is on the smpounce list,")

        '(1:955) and the furre named {...} is not on the smPounce list,
        Add(New Trigger(TriggerCategory.Condition, 955), AddressOf FurreNamedIsNotMember, "(1:955) and the furre named {...} is not on the smPounce list,")

        '(5:950) add the triggering furre to the smPounce List.
        Add(New Trigger(TriggerCategory.Effect, 950), AddressOf AddTrigFurre, "(5:950) add the triggering furre to the smPounce List.")
        '(5:951) add the furre named {...} to the smPounce list.
        Add(New Trigger(TriggerCategory.Effect, 951), AddressOf AddFurreNamed, "(5:951) add the furre named {...} to the smPounce list.")
        '(5:952) remove the triggering furre from the smPounce list.
        Add(New Trigger(TriggerCategory.Effect, 952), AddressOf RemoveTrigFurre, "(5:952) remove the triggering furre from the smPounce list.")
        '(5:953) remove the furre named {...} from the smPounce list.
        Add(New Trigger(TriggerCategory.Effect, 953), AddressOf RemoveFurreNamed, "(5:953) remove the furre named {...} from the smPounce list.")
        '(5:954) use the file named {...} as the smPounce list.
        Add(New Trigger(TriggerCategory.Effect, 954), AddressOf UseMemberFile, "(5:954) use the file named {...} as the smPounce list.")
    End Sub

    Function NameIs(reader As TriggerReader) As Boolean
        Try
            Dim TmpName As String = reader.ReadString()
            Dim tname As Variable = MainMSEngine.MSpage.GetVariable(MS_Name)
            'add Machine Name parser
            Return MainMSEngine.ToFurcShortName(TmpName) = MainMSEngine.ToFurcShortName(tname.Value.ToString)

        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function

    Function FurreNamedOnline(reader As TriggerReader) As Boolean
        Try
            Dim TmpName As String = reader.ReadString()
            Dim Furr As Main.pFurre
            For Each Fur As KeyValuePair(Of String, Main.pFurre) In callbk.FurreList
                If MainMSEngine.ToFurcShortName(Fur.Key) = MainMSEngine.ToFurcShortName(TmpName) Then
                    Furr = Fur.Value
                    Return Furr.Online
                End If
            Next
            'add Machine Name parser
            Return False
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function
    Function FurreNamedNotOnline(reader As TriggerReader) As Boolean
        Try
            Dim TmpName As String = reader.ReadString()
            Dim Furr As Main.pFurre
            For Each Fur As KeyValuePair(Of String, Main.pFurre) In callbk.FurreList
                If MainMSEngine.ToFurcShortName(Fur.Key) = MainMSEngine.ToFurcShortName(TmpName) Then
                    Furr = Fur.Value
                    Return Not Furr.Online
                End If
            Next
            'add Machine Name parser
            Return False
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
    End Function
    '(1:900) and the triggering furre is on my dream Member List,
    Private Function TrigFurreIsMember(reader As Monkeyspeak.TriggerReader) As Boolean
        CheckCemberList()
        Dim Furre As String = Nothing
        Dim f() As String
        Try
            Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString
            f = File.ReadAllLines(OnlineList)
            For Each l As String In f
                If MainMSEngine.ToFurcShortName(l) = MainMSEngine.ToFurcShortName(Furre) Then Return True
            Next
            Return False
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

    End Function
    '(1:901) and the furre named {...} is on my Dream Member list.
    Private Function FurreNamedIsMember(reader As Monkeyspeak.TriggerReader) As Boolean
        CheckCemberList()
        Dim Furre As String = Nothing
        Dim f() As String
        Try
            Furre = reader.ReadString
            f = File.ReadAllLines(OnlineList)
            For Each l As String In f
                If MainMSEngine.ToFurcShortName(l) = MainMSEngine.ToFurcShortName(Furre) Then Return True
            Next
            Return False
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

    End Function
    '(1:902) and the triggering furre is not on my Dream Member list.
    Private Function TrigFurreIsNotMember(reader As Monkeyspeak.TriggerReader) As Boolean
        Return Not TrigFurreIsMember(reader)
    End Function
    '(1:903) and the furre named {...} is not on my Dream Member list.
    Private Function FurreNamedIsNotMember(reader As Monkeyspeak.TriggerReader) As Boolean
        Return Not FurreNamedIsMember(reader)
    End Function
    '(1:900) add the triggering furre to my Dream Member list if they aren't already on it.
    Private Function AddTrigFurre(reader As TriggerReader) As Boolean
        Dim Furre As String = Nothing

        Try
            Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString
            If TrigFurreIsMember(reader) = False And TrigFurreIsNotMember(reader) Then
                Dim sw As StreamWriter = New StreamWriter(OnlineList, True)
                sw.WriteLine(Furre)
                sw.Close()
            End If
            Return True

        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

    End Function


    '(5:901) add the furre named {...} to my Dream Member list if they aren't already on it.
    Private Function AddFurreNamed(reader As TriggerReader) As Boolean
        Dim Furre As String = Nothing

        Try
            Furre = reader.ReadString
            If FurreNamedIsMember(reader) = False And FurreNamedIsNotMember(reader) Then
                Dim sw As StreamWriter = New StreamWriter(OnlineList, True)
                sw.WriteLine(Furre)
                sw.Close()
            End If
            Return True

        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try

    End Function
    '(5:902) remove the triggering furre to my Dream Member list if they are on it.
    Private Function RemoveTrigFurre(reader As TriggerReader) As Boolean
        Dim Furre As String = Nothing
        CheckCemberList()
        Try
            Furre = MainMSEngine.MSpage.GetVariable(MS_Name).Value.ToString
            Furre = Regex.Replace(Furre.ToLower(), MainMSEngine.REGEX_NameFilter, "")
            Dim line As String = Nothing
            Dim linesList As New List(Of String)(File.ReadAllLines(OnlineList))
            Dim SR As New StreamReader(OnlineList)
            line = SR.ReadLine()
            For i As Integer = 0 To linesList.Count - 1
                If Regex.Replace(line.ToLower(), MainMSEngine.REGEX_NameFilter, "") = Furre Then
                    SR.Dispose()
                    SR.Close()
                    linesList.RemoveAt(i)
                    File.WriteAllLines(OnlineList, linesList.ToArray())
                    Return True
                End If
                line = SR.ReadLine()
            Next i
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function
    '(5:903) remove the furre named {...} from my Dream Member list if they are on it.
    Private Function RemoveFurreNamed(reader As TriggerReader) As Boolean
        Dim Furre As String = Nothing
        CheckCemberList()
        Try
            Furre = reader.ReadString
            Furre = Regex.Replace(Furre.ToLower(), MainMSEngine.REGEX_NameFilter, "")
            Dim line As String = Nothing
            Dim linesList As New List(Of String)(File.ReadAllLines(OnlineList))
            Dim SR As New StreamReader(OnlineList)
            line = SR.ReadLine()
            For i As Integer = 0 To linesList.Count - 1
                If Regex.Replace(line.ToLower(), MainMSEngine.REGEX_NameFilter, "") = Furre Then
                    SR.Dispose()
                    SR.Close()
                    linesList.RemoveAt(i)
                    File.WriteAllLines(OnlineList, linesList.ToArray())
                    Return True
                End If
                line = SR.ReadLine()
            Next i
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return False
    End Function

    '(5:904) Use file {...} as the dream member list.  
    Private Function UseMemberFile(reader As TriggerReader) As Boolean

        Try
            OnlineList = reader.ReadString
            CheckCemberList()
        Catch ex As Exception
            MainMSEngine.LogError(reader, ex)
            Return False
        End Try
        Return True
    End Function
    Private Sub CheckCemberList()
        OnlineList = Paths.CheckBotFolder(OnlineList)
        If File.Exists(OnlineList) = False Then
            Dim sw As StreamWriter = New StreamWriter(OnlineList, False)
            sw.Close()
        End If
    End Sub
End Class
