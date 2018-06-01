﻿'MIT License

'Copyright(c) 2016-2018 Peter Kirmeier

'Permission Is hereby granted, free Of charge, to any person obtaining a copy
'of this software And associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, And/Or sell
'copies of the Software, And to permit persons to whom the Software Is
'furnished to do so, subject to the following conditions:

'The above copyright notice And this permission notice shall be included In all
'copies Or substantial portions of the Software.

'THE SOFTWARE Is PROVIDED "AS IS", WITHOUT WARRANTY Of ANY KIND, EXPRESS Or
'IMPLIED, INCLUDING BUT Not LIMITED To THE WARRANTIES Of MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE And NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS Or COPYRIGHT HOLDERS BE LIABLE For ANY CLAIM, DAMAGES Or OTHER
'LIABILITY, WHETHER In AN ACTION Of CONTRACT, TORT Or OTHERWISE, ARISING FROM,
'OUT OF Or IN CONNECTION WITH THE SOFTWARE Or THE USE Or OTHER DEALINGS IN THE
'SOFTWARE.

Imports System.IO
Imports System.Xml.Serialization

'<Serializable()> Public Class TestType
'    Public test As String
'End Class

Public Class SaveModule(Of t) ' (Of t As {New})
    Private _Filename As String
    Private xml As XmlSerializer

    Public Sub New(Filename As String)
        _Filename = Filename
        xml = New XmlSerializer(GetType(t))
    End Sub

    Public Function ReadXML(Optional Filename As String = Nothing) As t
        ReadXML = Nothing
        If Filename Is Nothing Then Filename = _Filename
        Try
            Dim file As New StreamReader(Filename)
            ReadXML = CType(xml.Deserialize(file), t)
            file.Close()
        Catch ex As Exception
            If Not ex.HResult = -2147024894 Then
                MessageBox.Show(ex.Message & vbNewLine & "==> Using defaults", "Error loading settings!", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            Exit Function
        End Try
    End Function

    Public Function WriteXML(data As t) As Boolean
        WriteXML = False
        Try
            Dim file As New StreamWriter(_Filename)
            xml.Serialize(file, data)
            file.Close()
            WriteXML = True
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error writing settings!", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Function
        End Try
    End Function
End Class
