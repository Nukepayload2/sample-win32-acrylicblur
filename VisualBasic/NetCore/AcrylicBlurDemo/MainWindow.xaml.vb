Option Strict On

Imports System.Runtime.InteropServices
Imports System.Windows.Interop

Friend Enum AccentState
    ACCENT_DISABLED = 0
    ACCENT_ENABLE_GRADIENT = 1
    ACCENT_ENABLE_TRANSPARENTGRADIENT = 2
    ACCENT_ENABLE_BLURBEHIND = 3
    ACCENT_ENABLE_ACRYLICBLURBEHIND = 4
    ACCENT_INVALID_STATE = 5
End Enum

Friend Structure AccentPolicy
    Dim AccentState As AccentState
    Dim AccentFlags As Integer
    Dim GradientColor As UInteger
    Dim AnimationId As Integer
End Structure

Friend Structure WindowCompositionAttributeData
    Dim Attribute As WindowCompositionAttribute
    Dim Data As IntPtr
    Dim SizeOfData As Integer
End Structure

Friend Enum WindowCompositionAttribute
    ' ...
    WCA_ACCENT_POLICY = 19
    ' ...
End Enum

Class MainWindow

    Friend Declare Function SetWindowCompositionAttribute Lib "user32.dll" (hwnd As IntPtr, ByRef data As WindowCompositionAttributeData) As Integer

    Private _blurOpacity As UInteger
    Public Property BlurOpacity As Double
        Get
            Return _blurOpacity
        End Get
        Set(value As Double)
            _blurOpacity = CUInt(Fix(value))
            UpdateBlur()
        End Set
    End Property

    Private _blurBackgroundColor As UInteger = &H990000 ' BGR color format

    Friend Sub UpdateBlur()
        Dim windowHelper As New WindowInteropHelper(Me)

        Dim accent As New AccentPolicy With {
            .AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
            .AccentFlags = 2,
            .GradientColor = (_blurOpacity << 24) Or (_blurBackgroundColor And &HFFFFFFUI)
        }
        Dim hGc = GCHandle.Alloc(accent, GCHandleType.Pinned)
        Dim data As New WindowCompositionAttributeData With {
            .Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
            .SizeOfData = Marshal.SizeOf(accent),
            .Data = hGc.AddrOfPinnedObject
        }
        SetWindowCompositionAttribute(windowHelper.Handle, data)
        hGc.Free()
    End Sub

    Private Sub MainWindow_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        DragMove()
    End Sub

    Private Sub MainWindow_SourceInitialized(sender As Object, e As EventArgs) Handles Me.SourceInitialized
        UpdateBlur()
    End Sub

    Private Sub SldOpacity_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles SldOpacity.ValueChanged
        BlurOpacity = SldOpacity.Value
        UpdateBlur()
    End Sub
End Class
