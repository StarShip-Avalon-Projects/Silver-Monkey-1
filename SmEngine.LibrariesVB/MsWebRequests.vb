﻿Imports Furcadia.Net.Web

Imports Monkeyspeak
Imports SilverMonkey.Engine.Libraries.Web

''' <summary>
''' Provides web interface for getting a list of Variables from a web server
''' <para>
''' Does support HTTPS connections
''' </para>
''' <para>
''' Effects: (5:10) - (5:60)
''' </para>
''' </summary>
Public NotInheritable Class MsWebRequests
    Inherits MonkeySpeakLibrary

#Region "Private Fields"

    Private Webrequest As Web.WebRequests()
    Private WebStack As New List(Of Monkeyspeak.IVariable)()
    Private WebURL As Uri

#End Region

#Region "Public Constructors"

    Public Overrides ReadOnly Property BaseId As Integer
        Get
            Return 70
        End Get
    End Property

    Public Overrides Sub Initialize(ParamArray args() As Object)
        MyBase.Initialize(args)

        WebURL = Nothing
        '(0:70) When the bot receives a variable list by sending the Web-Cache.
        Add(TriggerCategory.Cause,
        Function()
            Return True
        End Function, "When the bot receives a variable list by sending the Web-Cache.")

        '(1:30) and Web-Cache setting {...} is equal to {...},
        Add(TriggerCategory.Condition,
            AddressOf WebArrayEqualTo,
            "and Web-Cache setting {...} is equal to {...},")

        '(1:31) and Web-Cache setting {...} is not equal to {...},
        Add(TriggerCategory.Condition,
            AddressOf WebArrayNotEqualTo,
            "and Web-Cache setting {...} is not equal to {...},")

        '(1:32) and the Web-Cache contains field named {...},
        Add(TriggerCategory.Condition,
            AddressOf WebArrayContainArrayField,
            "and the Web-Cache contains field named {...},")

        '(1:33) and the Web-Cache doesn't contain field named {...},
        Add(TriggerCategory.Condition,
            AddressOf WebArrayNotContainArrayField,
            "and the Web-Cache doesn't contain field named {...},")

        '(5:9) remove variable %Variable from the Web-Cache
        Add(TriggerCategory.Effect,
            AddressOf RemoveWebStack,
            "remove variable %Variable from the Web-Cache.")

        '(5:10)  Set the web URL to {...}
        Add(TriggerCategory.Effect,
            AddressOf SetURL,
            " Set the web URL to {...},")

        '(5:11)  remember setting {...} from Web-Cache and store it into variable %Variable.
        Add(TriggerCategory.Effect,
            AddressOf RememberSetting,
            " remember setting {...} from Web-Cache and store it into variable %Variable.")

        '(5:16) send GET request to send the Web-Cache to URL.
        Add(TriggerCategory.Effect,
            AddressOf SendGetWebStack,
            "send GET request to send the Web-Cache to URL.")

        '(5:17) store variable %Variable to the Web-Cache
        Add(TriggerCategory.Effect,
            AddressOf StoreWebStack,
            "store variable %Variable to the Web-Cache.")
        '(5:18) send post request to send the Web-Cache to the web host.
        Add(TriggerCategory.Effect,
            AddressOf SendWebStack,
            "send POST request to send the Web-Cache to URL.")
        '(5:19) clear the Web-Cache.
        Add(TriggerCategory.Effect,
            AddressOf ClearWebStack,
            "clear the Web-Cache.")

    End Sub

    Public Overrides Sub Unload(page As Page)

    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' (5:19) clear the Web-Cache.
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' True Always
    ''' </returns>
    Public Function ClearWebStack(reader As TriggerReader) As Boolean
        If Not IsNothing(WebStack) AndAlso WebStack.Count > 0 Then WebStack.Clear()
        Return True
    End Function

    ''' <summary>
    ''' (5:11) remember setting {...} from Web-Cache and store it into
    ''' variable %Variable.
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function RememberSetting(reader As TriggerReader) As Boolean

        Dim setting = reader.Page.SetVariable(reader.ReadString, Nothing, False)

        Dim var = reader.ReadVariable(True)
        If WebStack.Contains(setting) Then
            var.Value = WebStack(WebStack.IndexOf(setting))
        End If
        Return True

    End Function

    ''' <summary>
    ''' (5:60) remove variable %Variable from the Web-Cache
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function RemoveWebStack(reader As TriggerReader) As Boolean
        Dim var = reader.ReadVariable()
        WebStack.Remove(var)
        Return True
    End Function

    ''' <summary>
    ''' (5:16) send GET request to send the Web-Cache to URL.
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function SendGetWebStack(reader As TriggerReader) As Boolean

        Dim ws As New Web.WebRequests(WebURL, reader)

        Dim WebPage = ws.WGet(WebStack)
        WebStack = WebPage.WebStack
        If WebPage.ReceivedPage Then
            reader.Page.Execute(70)
        End If

        If WebPage.Status <> 0 Then Throw New WebException(WebPage.ErrMsg)

        Return WebPage.Status = 0
    End Function

    ''' <summary>
    ''' (5:18) send post request to send the Web-Cache to the web host.
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function SendWebStack(reader As TriggerReader) As Boolean

        Dim WebPage As New WebData
        Dim ws As New Web.WebRequests(WebURL, reader)

        SyncLock Me
            WebPage = ws.WPost(WebStack)
            WebStack = WebPage.WebStack
            If WebPage.ReceivedPage Then
                reader.Page.ExecuteAsync(70)
            End If
        End SyncLock
        If WebPage.Status <> 0 Then Throw New WebException(WebPage.ErrMsg, WebPage)

        Return WebPage.Status = 0
    End Function

    ''' <summary>
    ''' (5:10) Set the web URL to {...}.
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' True Always
    ''' </returns>
    Public Function SetURL(reader As TriggerReader) As Boolean

        WebURL = New Uri(reader.ReadString)
        Return True
    End Function

    ''' <summary>
    ''' (5:17) store variable %Variable to the Web-Cache
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function StoreWebStack(reader As TriggerReader) As Boolean

        Dim var = reader.ReadVariable()
        If var IsNot Nothing Then
            If WebStack.Contains(var) Then
                WebStack(WebStack.IndexOf(var)) = var
            Else
                WebStack.Add(var)
            End If
        End If
        Return True

    End Function

    ''' <summary>
    ''' (1:32) and the Web-Cache contains field named {...},
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <returns></returns>
    Public Function WebArrayContainArrayField(reader As TriggerReader) As Boolean
        Dim var = reader.Page.SetVariable(reader.ReadString, Nothing, False)
        Return WebStack.Contains(var)

    End Function

    ''' <summary>
    ''' (1:30) and Web-Cache setting {...} is equal to {...},
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function WebArrayEqualTo(reader As TriggerReader) As Boolean

        Dim setting As String
        Try
            setting = WebStack.Item(WebStack.IndexOf(reader.ReadVariable)).Value
        Catch
            setting = ""
        End Try
        Return setting = reader.ReadString

    End Function

    ''' <summary>
    ''' (1:33) and the Web-Cache doesn't contain field named {...},
    ''' </summary>
    ''' <param name="reader"><see cref="TriggerReader"/></param>
    ''' <returns></returns>
    Public Function WebArrayNotContainArrayField(reader As TriggerReader) As Boolean

        Return Not WebStack.Contains(reader.Page.SetVariable(reader.ReadString, Nothing, False))

    End Function

    ''' <summary>
    ''' (1:31) and Web-Cache setting {...} is not equal to {...},
    ''' </summary>
    ''' <param name="reader">
    ''' </param>
    ''' <returns>
    ''' </returns>
    Public Function WebArrayNotEqualTo(reader As TriggerReader) As Boolean

        Dim setting As New MsWebVariable(reader.ReadString)
        If WebStack.Contains(setting) Then
            setting = WebStack.Item(WebStack.IndexOf(setting))
        End If
        Return setting.Value <> reader.ReadString
    End Function

#End Region

End Class