﻿Imports System.IO
Imports Furcadia.Util
Imports MonkeyCore
Imports Monkeyspeak
Imports SilverMonkeyEngine.SmConstants

Namespace Engine.Libraries

    ''' <summary>
    ''' Dream Member List management
    ''' <para>
    ''' a Simple dream administration system using a text file to contain a
    ''' list of Furre as staff
    ''' </para>
    ''' <para>
    ''' NOTE: The BotController is considered to be on the list even if the
    '''       furres name is not in the text file
    ''' </para>
    ''' <para>
    ''' Default Member-List file: <see cref="Paths.SilverMonkeyBotPath"/>\MemberList.txt
    ''' </para>
    ''' <conceptualLink target="d1358c3d-d6d3-4063-a0ef-259e13752a0f" />
    ''' <para/>
    ''' Credits: Drake for assistance with designing this system
    ''' </summary>
    Public NotInheritable Class MsMemberList
        Inherits MonkeySpeakLibrary

#Region "Private Fields"

        ''' <summary>
        ''' Member List file path
        ''' <para/>
        ''' Defaults to <see cref="MonkeyCore.Paths.SilverMonkeyBotPath"/>\MemberList.txt
        ''' </summary>
        ''' <returns></returns>
        Public Property MemberList As String

#End Region

#Region "Public Constructors"

        Public Sub New(ByRef Session As BotSession)
            MyBase.New(Session)
            MemberList = Paths.CheckBotFolder("MemberList.txt")

            '(1:900) and the triggering furre is on my Dream Member List,
            Add(New Trigger(TriggerCategory.Condition, 900), AddressOf TrigFurreIsMember, "(1:900) and the triggering furre is on my dream Member List,")
            '(1:901) and the furre named {...} is on my Dream Member list.
            Add(New Trigger(TriggerCategory.Condition, 901), AddressOf FurreNamedIsMember, "(1:901) and the furre named {...} is on my Dream Member list,")

            '(1:902) and the triggering furre is not on my Dream Member list.
            Add(New Trigger(TriggerCategory.Condition, 902), AddressOf TrigFurreIsNotMember, "(1:902) and the triggering furre is not on my Dream Member list,")
            '(1:903) and the furre named {...} is not on my Dream Member list.
            Add(New Trigger(TriggerCategory.Condition, 903), AddressOf FurreNamedIsNotMember, "(1:903) and the furre named {...} is not on my Dream Member list,")

            '(1:900) add the triggering furre to my Dream Member list if they aren't already on it.
            Add(New Trigger(TriggerCategory.Effect, 900), AddressOf AddTrigFurre, "(5:900) add the triggering furre to my Dream Member list if they aren't already on it.")
            '(5:901) add the furre named {...} to my Dream Member list if they aren't already on it.
            Add(New Trigger(TriggerCategory.Effect, 901), AddressOf AddFurreNamed, "(5:901) add the furre named {...} to my Dream Member list if they aren't already on it.")

            '(5:902) remove the triggering furre to my Dream Member list if they are on it.
            Add(New Trigger(TriggerCategory.Effect, 902), AddressOf RemoveTrigFurre, "(5:902) remove the triggering furre to my Dream Member list if they are on it.")
            '(5:903) remove the furre named {...} from my Dream Member list if they are on it.
            Add(New Trigger(TriggerCategory.Effect, 903), AddressOf RemoveFurreNamed, "(5:903) remove the furre named {...} from my Dream Member list if they are on it.")

            '(5:904) Use file {...} as the dream member list.
            Add(New Trigger(TriggerCategory.Effect, 904), AddressOf UseMemberFile, "(5:904) Use file {...} as the dream member list.")
            '(5:905) store member list to variable %Variable.
            Add(New Trigger(TriggerCategory.Effect, 905), AddressOf ListToVariable, "(5:905) store member list to variable %Variable.")

        End Sub

#End Region

#Region "Private Methods"

        ''' <summary>
        ''' (5:901) add the furre named {...} to my Dream Member list if
        ''' they aren't already on it.
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Public Function AddFurreNamed(reader As TriggerReader) As Boolean

            Try
                Dim Furre = reader.ReadString
                If FurreNamedIsNotMember(reader) Then
                    Using sw = New StreamWriter(MemberList, True)
                        sw.WriteLine(Furre)
                    End Using
                End If
                Return True
            Catch ex As Exception
                Throw New MonkeyspeakException("A problem occurred checking the member-list", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' (1:900) add the triggering furre to my Dream Member list if they
        ''' aren't already on it.
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Private Function AddTrigFurre(reader As TriggerReader) As Boolean

            Try
                Dim Furre = reader.Page.GetVariable(MS_Name).Value.ToString
                If TrigFurreIsMember(reader) = False And TrigFurreIsNotMember(reader) Then
                    Dim sw = New StreamWriter(MemberList, True)
                    sw.WriteLine(Furre)
                    sw.Close()
                End If
                Return True
            Catch ex As Exception
                Throw New MonkeyspeakException("A problem occurred checking the member-list", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Check for the member list file, If it doesn't exist created it.
        ''' <para>
        ''' Checks default <see cref="Paths.SilverMonkeyBotPath"/>
        ''' </para>
        ''' </summary>
        Public Sub CheckMemberList()
            MemberList = Paths.CheckBotFolder(MemberList)
            If Not File.Exists(MemberList) Then
                Using f As New StreamWriter(MemberList)
                    f.WriteLine("")
                End Using
            End If
        End Sub

        ''' <summary>
        ''' (1:901) and the furre named {...} is on my Dream Member list.
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Public Function FurreNamedIsMember(reader As TriggerReader) As Boolean

            Dim Furre = reader.ReadString
            Try
                CheckMemberList()
                Dim f = File.ReadAllLines(MemberList)
                For Each l As String In f
                    If FurcadiaShortName(l) = FurcadiaShortName(Furre) Then
                        Return True
                    End If
                Next
                Return FurcadiaSession.IsBotController
            Catch ex As Exception
                Throw New MonkeyspeakException("A problem occurred checking the member-list", ex)
            End Try

            Return False
        End Function

        ''' <summary>
        ''' (1:903) and the furre named {...} is not on my Dream Member list.
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Public Function FurreNamedIsNotMember(reader As TriggerReader) As Boolean
            Return Not FurreNamedIsMember(reader)
        End Function

        ''' <summary>
        ''' (5:905) store member list to variable %Variable.
        ''' </summary>
        ''' <param name="reader"></param>
        ''' <returns></returns>
        Public Function ListToVariable(reader As TriggerReader) As Boolean

            Try
                CheckMemberList()
                Dim Furre = reader.ReadVariable(True)

                Dim f = New List(Of String)
                f.AddRange(File.ReadAllLines(MemberList))
                Furre.Value = String.Join(" ", f.ToArray)

                Return True
            Catch ex As Exception
                Throw New MonkeyspeakException("A problem occurred checking the member-list", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' (5:903) remove the furre named {...} from my Dream Member list
        ''' if they are on it.
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Public Function RemoveFurreNamed(reader As TriggerReader) As Boolean

            Try
                CheckMemberList()

                Dim Furre = reader.ReadString
                Dim line As String
                Dim linesList = New List(Of String)(File.ReadAllLines(MemberList))
                Using SR = New StreamReader(MemberList)
                    While SR.Peek() <> -1
                        line = SR.ReadLine()
                        For i As Integer = 0 To linesList.Count - 1
                            If FurcadiaShortName(line) = FurcadiaShortName(Furre) Then
                                linesList.RemoveAt(i)
                                File.WriteAllLines(MemberList, linesList.ToArray())
                                Exit For
                            End If
                        Next i
                    End While
                End Using

                Return True
            Catch ex As Exception
                Throw New MonkeyspeakException("A problem occurred checking the member-list", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' (5:902) remove the triggering furre to my Dream Member list if
        ''' they are on it.
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Public Function RemoveTrigFurre(reader As TriggerReader) As Boolean

            Try
                CheckMemberList()

                Dim Furre = reader.Page.GetVariable(MS_Name).Value.ToString

                Dim linesList As New List(Of String)(File.ReadAllLines(MemberList))
                Using SR As New StreamReader(MemberList)
                    While SR.Peek() <> -1
                        Dim line = SR.ReadLine()
                        For i As Integer = 0 To linesList.Count - 1
                            If FurcadiaShortName(line) = FurcadiaShortName(Furre) Then
                                linesList.RemoveAt(i)
                                File.WriteAllLines(MemberList, linesList.ToArray())
                                Exit For
                            End If
                        Next i
                    End While
                End Using

                Return True
            Catch ex As Exception
                Throw New MonkeyspeakException("A problem occurred checking the member-list", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' (1:900) and the triggering furre is on my dream Member List,
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Public Function TrigFurreIsMember(reader As TriggerReader) As Boolean

            Try
                CheckMemberList()

                Dim Furre = reader.Page.GetVariable(MS_Name).Value.ToString

                Dim f = New List(Of String)
                f.AddRange(File.ReadAllLines(MemberList))
                For Each l As String In f
                    If FurcadiaShortName(l) = FurcadiaShortName(Furre) Then Return True
                Next

                Return FurcadiaSession.IsBotController
            Catch ex As Exception
                Throw New MonkeyspeakException("A problem occurred checking the member-list", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' (1:902) and the triggering furre is not on my Dream Member list.
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Public Function TrigFurreIsNotMember(reader As TriggerReader) As Boolean
            Return Not TrigFurreIsMember(reader)
        End Function

        ''' <summary>
        ''' (5:904) Use file {...} as the dream member list.
        ''' </summary>
        ''' <param name="reader">
        ''' <see cref="TriggerReader"/>
        ''' </param>
        ''' <returns>
        ''' </returns>
        Public Function UseMemberFile(reader As TriggerReader) As Boolean
            Try
                MemberList = reader.ReadString
                CheckMemberList()
                Return True
            Catch ex As Exception
                Throw New MonkeyspeakException("A problem occurred checking the member-list", ex)
                Return False
            End Try
        End Function

        Public Overrides Sub Unload(page As Page)

        End Sub

#End Region

    End Class

End Namespace