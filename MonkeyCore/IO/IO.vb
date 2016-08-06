﻿Imports System.IO
Imports System.Text


Public Class IO
    Private _KeysIni As IniFile
    Private _MS_KeysIni As IniFile
    Friend Shared ReadOnly File As Object

    Public Sub New()

    End Sub

    Public Property KeysIni As IniFile
        Get
            Return _KeysIni
        End Get
        Set(value As IniFile)
            _KeysIni = value
        End Set
    End Property

    Public Property MS_KeysIni As IniFile
        Get
            Return _MS_KeysIni
        End Get
        Set(value As IniFile)
            _MS_KeysIni = value
        End Set
    End Property


    ''' <summary>
    ''' News the ds file.
    ''' </summary>
    ''' <returns>Generic Preformatted DragonSpeak file</returns>
    Public Shared Function NewDSFile() As String
        Dim str As New StringBuilder
        str.AppendLine(Settings.KeysIni.GetKeyValue("MS-General", "Header"))
        Dim t As String = " "
        Dim n As Integer = 0
        While t <> ""
            t = Settings.KeysIni.GetKeyValue("MS-General", "H" + n.ToString)
            If t <> "" Then str.AppendLine(t)
            n += 1
        End While
        For i = 0 To CInt(Settings.KeysIni.GetKeyValue("MS-General", "InitLineSpaces"))
            str.AppendLine("")
        Next
        str.Append(Settings.KeysIni.GetKeyValue("MS-General", "Footer"))
        Return str.ToString
    End Function

    ''' <summary>
    ''' News the ms file.
    ''' </summary>
    ''' <returns>Generic Preformated MonkeySpeak file</returns>
    Public Shared Function NewMSFile() As String
        Dim str As New StringBuilder
        str.AppendLine(Settings.MS_KeysIni.GetKeyValue("MS-General", "Header"))
        Dim t As String = " "
        Dim n As Integer = 0
        While t <> ""
            t = Settings.MS_KeysIni.GetKeyValue("MS-General", "H" + n.ToString)
            If t <> "" Then str.AppendLine(t)
            n += 1
        End While
        For i = 0 To CInt(Settings.MS_KeysIni.GetKeyValue("MS-General", "InitLineSpaces"))
            str.AppendLine("")
        Next
        str.Append(Settings.MS_KeysIni.GetKeyValue("MS-General", "Footer"))
        Return str.ToString
    End Function


    ''' <summary>
    ''' News the dm script.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function NewDMScript() As String
        Dim str As New StringBuilder
        str.AppendLine(Settings.KeysIni.GetKeyValue("DM-Script", "Header"))
        Dim t As String = " "
        Dim n As Integer = 0
        While Not String.IsNullOrEmpty(t)
            t = Settings.KeysIni.GetKeyValue("DM-Script", "H" + n.ToString)
            If Not String.IsNullOrEmpty(t) Then str.AppendLine(t)
            n += 1
        End While
        For i = 0 To CInt(Settings.KeysIni.GetKeyValue("DM-Script", "InitLineSpaces"))
            str.AppendLine("")
        Next
        str.Append(Settings.KeysIni.GetKeyValue("DM-Script", "Footer"))
        Return str.ToString
    End Function

End Class

