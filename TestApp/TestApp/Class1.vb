Imports System

Public Class Class1

    Public Class SomeEventArgs
        Inherits EventArgs

    End Class

    Public Event SomeOtherEvent As EventHandler(Of SomeEventArgs)

    Public Sub AddValueChangedHandler(handler As EventHandler(Of SomeEventArgs))
        AddHandler SomeOtherEvent, handler
    End Sub

End Class

