﻿Imports MonkeyCore
Namespace Engine.Libraries
    Public Class MS_Repq
        Inherits MonkeySpeakLibrary



#Region "Public Constructors"

        Public Sub New()
            writer = New TextBoxWriter(Variables.TextBox1)

        End Sub

#End Region

    End Class
End Namespace