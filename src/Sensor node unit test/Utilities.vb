Imports System.Text
'Imports System.ServiceProcess
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.Data.SqlServerCe

Public Enum enumAlarmState
    NORMAL
    LOW
    HIGH
    FAILED
End Enum

Public Class Utilities

    Public Shared Char0Dots(,) As Byte = {{0, 0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 3, 3, 3}, {1, 2, 3, 4, 5, 0, 6, 0, 6, 1, 2, 3, 4, 5}}
    Public Shared Char1Dots(,) As Byte = {{0, 1, 2, 2, 2, 2, 2, 2, 2}, {4, 5, 0, 1, 2, 3, 4, 5, 6}}
    Public Shared Char2Dots(,) As Byte = {{0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3}, {0, 1, 4, 5, 0, 2, 6, 0, 3, 6, 0, 4, 5}}
    Public Shared Char3Dots(,) As Byte = {{0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3}, {1, 5, 0, 3, 6, 0, 3, 6, 1, 2, 4, 5}}
    Public Shared Char4Dots(,) As Byte = {{0, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 3}, {3, 4, 3, 5, 0, 1, 2, 3, 4, 5, 6, 3}}
    Public Shared Char5Dots(,) As Byte = {{0, 0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3}, {1, 3, 4, 5, 6, 0, 4, 6, 0, 4, 6, 1, 2, 3, 6}}
    Public Shared Char6Dots(,) As Byte = {{0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3}, {1, 2, 3, 4, 0, 3, 5, 0, 3, 6, 1, 2, 6}}
    Public Shared Char7Dots(,) As Byte = {{0, 1, 1, 1, 1, 1, 2, 2, 3, 3}, {6, 0, 1, 2, 3, 6, 4, 6, 5, 6}}
    Public Shared Char8Dots(,) As Byte = {{0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3}, {1, 2, 4, 5, 0, 3, 6, 0, 3, 6, 1, 2, 4, 5}}
    Public Shared Char9Dots(,) As Byte = {{0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3}, {4, 5, 0, 3, 6, 1, 3, 6, 2, 3, 4, 5}}
    Public Shared CharCommaDots(,) As Byte = {{0, 0, 1, 1}, {0, 2, 1, 2}}
    Public Shared CharDegreeCDots(,) As Byte = {{0, 0, 1, 1, 2, 2}, {1, 2, 0, 3, 0, 3}}
    Public Shared CharPercentDots(,) As Byte = {{0, 0, 1, 2, 2}, {1, 4, 2, 0, 3}}
    Public Shared CharDotDots(,) As Byte = {{0}, {1}}

    Structure strucDisplayFont
        Dim StartColumn As Integer
        Dim NextStartColumn As Integer
        Dim CharKey As Char
        Dim Dots(,) As Byte
        Public Sub New(ByVal key As Char, ByVal start As Integer)
            StartColumn = start
            CharKey = key
            Select Case CharKey
                Case "0"
                    Dots = Char0Dots.Clone
                    NextStartColumn = StartColumn + 5
                Case "1"
                    Dots = Char1Dots.Clone
                    NextStartColumn = StartColumn + 4
                Case "2"
                    Dots = Char2Dots.Clone
                    NextStartColumn = StartColumn + 5
                Case "3"
                    Dots = Char3Dots.Clone
                    NextStartColumn = StartColumn + 5
                Case "4"
                    Dots = Char4Dots.Clone
                    NextStartColumn = StartColumn + 5
                Case "5"
                    Dots = Char5Dots.Clone
                    NextStartColumn = StartColumn + 5
                Case "6"
                    Dots = Char6Dots.Clone
                    NextStartColumn = StartColumn + 5
                Case "7"
                    Dots = Char7Dots.Clone
                    NextStartColumn = StartColumn + 5
                Case "8"
                    Dots = Char8Dots.Clone
                    NextStartColumn = StartColumn + 5
                Case "9"
                    Dots = Char9Dots.Clone
                    NextStartColumn = StartColumn + 5
                Case ","
                    Dots = CharCommaDots.Clone
                    NextStartColumn = StartColumn + 3
                Case "c", "C"
                    Dots = CharDegreeCDots.Clone
                    NextStartColumn = StartColumn + 4
                Case "%"
                    Dots = CharPercentDots.Clone
                    NextStartColumn = StartColumn + 4
                Case "."
                    Dots = CharDotDots.Clone
                    NextStartColumn = StartColumn + 2
                Case Else
                    Dots = Nothing
                    NextStartColumn = StartColumn
            End Select
        End Sub
        Public Overrides Function ToString() As String
            Return CharKey.ToString
        End Function
    End Structure

    Public Shared Function GenerateDisplayDots(ByVal text As String) As String()
        Try
            Dim Dots(0 To 7, 0 To 47) As Byte
            Dim startCol As Integer = 0
            Try
                For Each c As String In text
                    If c.Equals(" ") Then
                        startCol += 2
                    Else
                        Dim charDots As New strucDisplayFont(c, startCol)
                        If Not charDots.Dots Is Nothing Then
                            For idot As Integer = 0 To charDots.Dots.Length / 2 - 1
                                Try : Dots(charDots.Dots(1, idot), startCol + charDots.Dots(0, idot)) = 1 : Catch : End Try
                            Next
                        End If
                        startCol = charDots.NextStartColumn
                    End If
                Next
            Catch ex As Exception
                'Do nothing
            End Try
            Dim retStr(0 To 3) As String
            Dim retSB As New StringBuilder
            For j As Integer = 0 To 1
                For d As Integer = 0 To 40 Step 8
                    Dim w As Integer = 128
                    Dim sum As Integer = 0
                    For i As Integer = d To d + 7
                        sum += Dots(j, i) * w
                        w = w / 2
                    Next
                    retSB.Append(sum.ToString("X2"))
                Next
            Next
            retStr(0) = retSB.ToString
            retSB.Remove(0, retSB.Length)
            For j As Integer = 2 To 3
                For d As Integer = 0 To 40 Step 8
                    Dim w As Integer = 128
                    Dim sum As Integer = 0
                    For i As Integer = d To d + 7
                        sum += Dots(j, i) * w
                        w = w / 2
                    Next
                    retSB.Append(sum.ToString("X2"))
                Next
            Next
            retStr(1) = retSB.ToString
            retSB.Remove(0, retSB.Length)
            For j As Integer = 4 To 5
                For d As Integer = 0 To 40 Step 8
                    Dim w As Integer = 128
                    Dim sum As Integer = 0
                    For i As Integer = d To d + 7
                        sum += Dots(j, i) * w
                        w = w / 2
                    Next
                    retSB.Append(sum.ToString("X2"))
                Next
            Next
            retStr(2) = retSB.ToString
            retSB.Remove(0, retSB.Length)
            For j As Integer = 6 To 7
                For d As Integer = 0 To 40 Step 8
                    Dim w As Integer = 128
                    Dim sum As Integer = 0
                    For i As Integer = d To d + 7
                        sum += Dots(j, i) * w
                        w = w / 2
                    Next
                    retSB.Append(sum.ToString("X2"))
                Next
            Next
            retStr(3) = retSB.ToString
            Return retStr
        Catch ex As Exception
            Return {"", "", "", ""}
        End Try
    End Function

#Region "Check database and path"

    Public Shared Sub WriteLog(ByVal text As String)
        'text = Me.Name & " - " & ex.Message 
        Dim path As String = My.Settings.SystemDataPath & "\" & My.Settings.LogPath & "\" & My.Application.Info.AssemblyName
        Dim FileName As String = Now.ToString("yyyy_MM_dd") & ".txt"
        Dim FullFileName As String = path & "\" & FileName
        Dim AllText As String = Now.ToString("yyyy MMM dd HH:mm:ss: ") & text & vbCrLf
        Try
            System.IO.File.AppendAllText(FullFileName, AllText)
        Catch ex As Exception
            CheckSystemDataAndPath()
            If Not System.IO.Directory.Exists(path) Then
                System.IO.Directory.CreateDirectory(path)
            End If
            System.IO.File.AppendAllText(FullFileName, AllText)
        End Try
    End Sub

    Public Shared Sub CheckSystemDataAndPath() ' ควรปรับใหม่สำหรับอ่าน config จากฐานข้อมูล
        Try
            '---------------------------------------------------------------
            ' Check system data directory
            ' D:\Drying Plant Control
            '---------------------------------------------------------------
            Dim path As String = My.Settings.SystemDataPath
            If Not System.IO.Directory.Exists(path) Then
                System.IO.Directory.CreateDirectory(path)
            End If

            '---------------------------------------------------------------
            ' Check log files directory
            ' D:\Drying Plant Control\Log
            '---------------------------------------------------------------
            path = My.Settings.SystemDataPath & "\" & My.Settings.LogPath
            If Not System.IO.Directory.Exists(path) Then
                System.IO.Directory.CreateDirectory(path)
            End If

            '---------------------------------------------------------------
            ' Check database directory
            ' D:\Drying Plant Control\Database
            '---------------------------------------------------------------
            path = My.Settings.SystemDataPath & "\" & My.Settings.DatabasePath
            If Not System.IO.Directory.Exists(path) Then
                System.IO.Directory.CreateDirectory(path)
            End If

            '---------------------------------------------------------------
            ' Check database file
            ' D:\Drying Plant Control\Database\DryingPlantControlDB.sdf
            '---------------------------------------------------------------
            path = My.Settings.SystemDataPath & "\" & My.Settings.DatabasePath & "\" & My.Settings.DatabaseFileName
            If Not System.IO.File.Exists(path) Then
                System.IO.File.Copy(My.Application.Info.DirectoryPath & "\" & My.Settings.BlankDatabaseFileName, path)
            End If

            ' Future more.
            ' D:\Drying Plant Control\Database\Kenmin.sdf

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#End Region

#Region "Devices interfacing"

    Public Shared LastIOException As Exception

#Region "Display unit connection for AD"

    Public Shared _displayP1Commu2DispADPort As System.IO.Ports.SerialPort
    Public Shared Function ConnectP1Commu2DispADPort() As Boolean
        Try
            Dim dbConfig As New ConfigDBClass
            Dim _portDispName As String

            Try
                If dbConfig Is Nothing Then dbConfig = New ConfigDBClass
                _portDispName = dbConfig.GetCharValue("DisplayPortName1", True)
            Catch ex As Exception
                LastIOException = ex
                Return False
            End Try

            If _displayP1Commu2DispADPort Is Nothing Then
                _displayP1Commu2DispADPort = New System.IO.Ports.SerialPort
                _displayP1Commu2DispADPort.PortName = _portDispName
            End If
            If _displayP1Commu2DispADPort.IsOpen Then _displayP1Commu2DispADPort.Close()
            _displayP1Commu2DispADPort.PortName = _portDispName
            _displayP1Commu2DispADPort.BaudRate = 9600
            _displayP1Commu2DispADPort.DataBits = 8
            _displayP1Commu2DispADPort.StopBits = IO.Ports.StopBits.One
            _displayP1Commu2DispADPort.Handshake = IO.Ports.Handshake.None
            _displayP1Commu2DispADPort.Parity = IO.Ports.Parity.None
            _displayP1Commu2DispADPort.Encoding = System.Text.Encoding.Default
            '_portDisp.WriteTimeout = 70
            _displayP1Commu2DispADPort.ReadTimeout = 20
            _displayP1Commu2DispADPort.RtsEnable = False
            _displayP1Commu2DispADPort.DtrEnable = False
            _displayP1Commu2DispADPort.Open()
            Threading.Thread.Sleep(500)
            WriteLog("Initalize serial port of " & _displayP1Commu2DispADPort.PortName)

            Return True
        Catch ex As Exception
            WriteLog(ex.Message)
            Return False
        End Try

    End Function
    Public Shared Sub DisconnectP1Commu2DispADPortPort()
        Try
            WriteLog("Closing " & _displayP1Commu2DispADPort.PortName)
            _displayP1Commu2DispADPort.Close()
            Threading.Thread.Sleep(500)
            WriteLog("Closed.")
        Catch ex As Exception
            WriteLog(ex.Message)
            Throw ex
        End Try
    End Sub

#End Region


#Region "Display unit connection for EJ"

    Public Shared _displayP1Commu3DispEJPort As System.IO.Ports.SerialPort
    Public Shared Function ConnectP1Commu3DispEJPort() As Boolean
        Try
            Dim dbConfig As New ConfigDBClass
            Dim _portDispName As String

            Try
                If dbConfig Is Nothing Then dbConfig = New ConfigDBClass
                _portDispName = dbConfig.GetCharValue("DisplayPortName2", True)
            Catch ex As Exception
                LastIOException = ex
                Return False
            End Try

            If _displayP1Commu3DispEJPort Is Nothing Then
                _displayP1Commu3DispEJPort = New System.IO.Ports.SerialPort
                _displayP1Commu3DispEJPort.PortName = _portDispName
            End If
            If _displayP1Commu3DispEJPort.IsOpen Then _displayP1Commu3DispEJPort.Close()
            _displayP1Commu3DispEJPort.PortName = _portDispName
            _displayP1Commu3DispEJPort.BaudRate = 9600
            _displayP1Commu3DispEJPort.DataBits = 8
            _displayP1Commu3DispEJPort.StopBits = IO.Ports.StopBits.One
            _displayP1Commu3DispEJPort.Handshake = IO.Ports.Handshake.None
            _displayP1Commu3DispEJPort.Parity = IO.Ports.Parity.None
            _displayP1Commu3DispEJPort.Encoding = System.Text.Encoding.Default
            '_displayP1Commu3DispEJPort.WriteTimeout = 70
            _displayP1Commu3DispEJPort.ReadTimeout = 20
            _displayP1Commu3DispEJPort.RtsEnable = False
            _displayP1Commu3DispEJPort.DtrEnable = False
            _displayP1Commu3DispEJPort.Open()
            Threading.Thread.Sleep(500)
            WriteLog("Initalize serial port of " & _displayP1Commu3DispEJPort.PortName)

            Return True
        Catch ex As Exception
            WriteLog(ex.Message)
            Return False
        End Try

    End Function
    Public Shared Sub DisconnectP1Commu3DispEJPortPort()
        Try
            WriteLog("Closing " & _displayP1Commu3DispEJPort.PortName)
            _displayP1Commu3DispEJPort.Close()
            Threading.Thread.Sleep(500)
            WriteLog("Closed.")
        Catch ex As Exception
            WriteLog(ex.Message)
            Throw ex
        End Try
    End Sub

#End Region

#Region "Control AD,EJ units connection"

    Public Shared _controlP1Commu1AJPort As System.IO.Ports.SerialPort
    Public Shared Function ConnectP1Commu1CtrlAJPort() As Boolean
        Try
            Dim dbConfig As New ConfigDBClass
            Dim _PortName As String

            Try
                If dbConfig Is Nothing Then dbConfig = New ConfigDBClass
                _PortName = dbConfig.GetCharValue("PortName", True)
            Catch ex As Exception
                LastIOException = ex
                Return False
            End Try

            If _controlP1Commu1AJPort Is Nothing Then
                _controlP1Commu1AJPort = New System.IO.Ports.SerialPort
                _controlP1Commu1AJPort.PortName = _PortName
            End If
            If _controlP1Commu1AJPort.IsOpen Then _controlP1Commu1AJPort.Close()
            _controlP1Commu1AJPort.PortName = _PortName
            _controlP1Commu1AJPort.BaudRate = 9600
            _controlP1Commu1AJPort.DataBits = 8
            _controlP1Commu1AJPort.StopBits = IO.Ports.StopBits.One
            _controlP1Commu1AJPort.Handshake = IO.Ports.Handshake.None
            _controlP1Commu1AJPort.Parity = IO.Ports.Parity.None
            _controlP1Commu1AJPort.Encoding = System.Text.Encoding.Default
            '_controlP1Commu1AJPort.WriteTimeout = 70
            _controlP1Commu1AJPort.ReadTimeout = 20
            _controlP1Commu1AJPort.RtsEnable = False
            _controlP1Commu1AJPort.DtrEnable = False
            _controlP1Commu1AJPort.Open()
            Threading.Thread.Sleep(500)
            Utilities.WriteLog("Initalize serial port of " & _controlP1Commu1AJPort.PortName)

            Return True

        Catch ex As Exception
            WriteLog(ex.Message)
            Return False
        End Try

    End Function
    Public Shared Function ConnectP1Commu1CtrlAJPort(ByVal portName As String) As Boolean
        Try
            Dim dbConfig As New ConfigDBClass
            Dim _PortName As String = ""

            Try
                If portName = "" Then
                    If dbConfig Is Nothing Then dbConfig = New ConfigDBClass
                    _PortName = dbConfig.GetCharValue("PortName", True)
                Else
                    _PortName = portName
                End If
            Catch ex As Exception
                LastIOException = ex
                Return False
            End Try

            If _controlP1Commu1AJPort Is Nothing Then
                _controlP1Commu1AJPort = New System.IO.Ports.SerialPort
                _controlP1Commu1AJPort.PortName = _PortName
            End If
            If _controlP1Commu1AJPort.IsOpen Then _controlP1Commu1AJPort.Close()
            _controlP1Commu1AJPort.PortName = _PortName
            _controlP1Commu1AJPort.BaudRate = 9600
            _controlP1Commu1AJPort.DataBits = 8
            _controlP1Commu1AJPort.StopBits = IO.Ports.StopBits.One
            _controlP1Commu1AJPort.Handshake = IO.Ports.Handshake.None
            _controlP1Commu1AJPort.Parity = IO.Ports.Parity.None
            _controlP1Commu1AJPort.Encoding = System.Text.Encoding.Default
            '_port.WriteTimeout = 70
            _controlP1Commu1AJPort.ReadTimeout = 20
            _controlP1Commu1AJPort.RtsEnable = False
            _controlP1Commu1AJPort.DtrEnable = False
            _controlP1Commu1AJPort.Open()
            Threading.Thread.Sleep(500)
            WriteLog("Initalize serial port of " & _controlP1Commu1AJPort.PortName)

            Return True
        Catch ex As Exception
            WriteLog(ex.Message)
            Return False
        End Try

    End Function
    Public Shared Sub DisconnectP1Commu1CtrlAJPort()
        Try
            WriteLog("Closing " & _controlP1Commu1AJPort.PortName)
            _controlP1Commu1AJPort.Close()
            Threading.Thread.Sleep(500)
            WriteLog("Closed.")
        Catch ex As Exception
            WriteLog(ex.Message)
            Throw ex
        End Try
    End Sub

#End Region

#Region "Control KM units connection"

    Public Shared _controlP2Commu1KMPort As System.IO.Ports.SerialPort
    Public Shared Function ConnectP2Commu1CtrlKMPort() As Boolean
        Try
            Dim dbConfig As New ConfigDBClass
            Dim _PortName As String

            Try
                If dbConfig Is Nothing Then dbConfig = New ConfigDBClass
                _PortName = dbConfig.GetCharValue("P2Commu1", True)
            Catch ex As Exception
                LastIOException = ex
                Return False
            End Try

            If _controlP2Commu1KMPort Is Nothing Then
                _controlP2Commu1KMPort = New System.IO.Ports.SerialPort
                _controlP2Commu1KMPort.PortName = _PortName
            End If
            If _controlP2Commu1KMPort.IsOpen Then _controlP2Commu1KMPort.Close()
            _controlP2Commu1KMPort.PortName = _PortName
            _controlP2Commu1KMPort.BaudRate = 9600
            _controlP2Commu1KMPort.DataBits = 8
            _controlP2Commu1KMPort.StopBits = IO.Ports.StopBits.One
            _controlP2Commu1KMPort.Handshake = IO.Ports.Handshake.None
            _controlP2Commu1KMPort.Parity = IO.Ports.Parity.None
            _controlP2Commu1KMPort.Encoding = System.Text.Encoding.Default
            '_controlP2Commu1KMPort.WriteTimeout = 70
            _controlP2Commu1KMPort.ReadTimeout = 20
            _controlP2Commu1KMPort.RtsEnable = False
            _controlP2Commu1KMPort.DtrEnable = False
            _controlP2Commu1KMPort.Open()
            Threading.Thread.Sleep(500)
            Utilities.WriteLog("Initalize serial port of " & _controlP2Commu1KMPort.PortName)

            Return True

        Catch ex As Exception
            WriteLog(ex.Message)
            Return False
        End Try

    End Function
    Public Shared Function ConnectP2Commu1CtrlKMPort(ByVal portName As String) As Boolean
        Try
            Dim dbConfig As New ConfigDBClass
            Dim _PortName As String = ""

            Try
                If portName = "" Then
                    If dbConfig Is Nothing Then dbConfig = New ConfigDBClass
                    _PortName = dbConfig.GetCharValue("PortName", True)
                Else
                    _PortName = portName
                End If
            Catch ex As Exception
                LastIOException = ex
                Return False
            End Try

            If _controlP2Commu1KMPort Is Nothing Then
                _controlP2Commu1KMPort = New System.IO.Ports.SerialPort
                _controlP2Commu1KMPort.PortName = _PortName
            End If
            If _controlP2Commu1KMPort.IsOpen Then _controlP2Commu1KMPort.Close()
            _controlP2Commu1KMPort.PortName = _PortName
            _controlP2Commu1KMPort.BaudRate = 9600
            _controlP2Commu1KMPort.DataBits = 8
            _controlP2Commu1KMPort.StopBits = IO.Ports.StopBits.One
            _controlP2Commu1KMPort.Handshake = IO.Ports.Handshake.None
            _controlP2Commu1KMPort.Parity = IO.Ports.Parity.None
            _controlP2Commu1KMPort.Encoding = System.Text.Encoding.Default
            '_controlP2Commu1KMPort.WriteTimeout = 70
            _controlP2Commu1KMPort.ReadTimeout = 20
            _controlP2Commu1KMPort.RtsEnable = False
            _controlP2Commu1KMPort.DtrEnable = False
            _controlP2Commu1KMPort.Open()
            Threading.Thread.Sleep(500)
            WriteLog("Initalize serial port of " & _controlP2Commu1KMPort.PortName)

            Return True
        Catch ex As Exception
            WriteLog(ex.Message)
            Return False
        End Try

    End Function
    Public Shared Sub DisconnectP2Commu1CtrlKMPort()
        Try
            WriteLog("Closing " & _controlP2Commu1KMPort.PortName)
            _controlP2Commu1KMPort.Close()
            Threading.Thread.Sleep(500)
            WriteLog("Closed.")
        Catch ex As Exception
            WriteLog(ex.Message)
            Throw ex
        End Try
    End Sub

#End Region

    Public Shared Function IsDayPeriod() As Boolean
        Try
            Dim dbConfig As New ConfigDBClass
            Dim sStartDay As String = dbConfig.GetCharValue("DayPeriodStartFrom")
            Dim sStartNight As String = dbConfig.GetCharValue("NightPeriodStartFrom")
            dbConfig.Dispose()
            Dim aStartDay() As String = sStartDay.Split(":")
            Dim aStartNight() As String = sStartNight.Split(":")
            Return DateTime.Now.TimeOfDay >= New TimeSpan(CInt(Val(aStartDay(0))), CInt(Val(aStartDay(1))), 0) AndAlso DateTime.Now.TimeOfDay <= New TimeSpan(CInt(Val(aStartNight(0))), CInt(Val(aStartNight(1))), 0)
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Shared Function PeriodCheck(ByRef PeriodType As String) As Long
        Try
            Dim dbConfig As New ConfigDBClass
            Dim sStartDay As String
            Try : sStartDay = dbConfig.GetCharValue("DayPeriodStartFrom") : Catch : sStartDay = "7:40" : End Try
            Dim sStartNight As String
            Try : sStartNight = dbConfig.GetCharValue("NightPeriodStartFrom") : Catch : sStartNight = "19:40" : End Try
            dbConfig.Dispose()

            Dim aStartDay() As String = sStartDay.Split(":")
            Dim aStartNight() As String = sStartNight.Split(":")

            Dim currentTimeOfDay As TimeSpan = DateTime.Now.TimeOfDay
            Dim startDay As TimeSpan = New TimeSpan(CInt(Val(aStartDay(0))), CInt(Val(aStartDay(1))), 0)
            Dim startNight As TimeSpan = New TimeSpan(CInt(Val(aStartNight(0))), CInt(Val(aStartNight(1))), 0)

            Dim PeriodID As Long = TimeSpan.FromTicks(DateTime.Today.Ticks).TotalDays
            PeriodType = ""
            If New TimeSpan(0, 0, 0) <= currentTimeOfDay AndAlso currentTimeOfDay < startDay Then 'Night 2/2
                PeriodType = "NIGHT"
                PeriodID -= 1
            ElseIf startDay <= currentTimeOfDay AndAlso currentTimeOfDay < startNight Then 'Day
                PeriodType = "DAY"
            ElseIf startNight <= currentTimeOfDay AndAlso currentTimeOfDay < New TimeSpan(23, 59, 59) Then
                PeriodType = "NIGHT"
            Else
                PeriodType = "NIGHT"
            End If
            Return PeriodID
        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Shared _resCode As Integer

    Shared ByteIncoming As Byte
    Shared iByte As Integer = 0
    Shared running As Boolean = True

    Shared lastDataID As Long

    Public Shared X_TheDateTime As DateTime
    Public Shared X_DateTimeText As String
    Public Shared X_TodayTotalDays As Long
    Public Shared X_Totalminutes As Double

    Public Shared Temperature, Humidity As Double
    Public Shared SetTemperatureHigh, SetHumidityHigh As Double
    Public Shared SetTemperatureLow, SetHumidityLow As Double
    Public Shared TemperatureAlarm As enumAlarmState = enumAlarmState.FAILED
    Public Shared HumidityAlarm As enumAlarmState = enumAlarmState.FAILED
    Public Shared ControlContact As Integer
    Public Shared DigitalInput As Integer
    Public Shared DigitalInputP2KM(0 To 3) As Integer

    Shared timeSpanArray() As TimeSpan = {New TimeSpan(0, 0, 0), New TimeSpan(2, 0, 0), New TimeSpan(4, 0, 0), New TimeSpan(6, 0, 0), New TimeSpan(8, 0, 0), New TimeSpan(10, 0, 0), New TimeSpan(12, 0, 0), New TimeSpan(14, 0, 0), New TimeSpan(16, 0, 0), New TimeSpan(18, 0, 0), New TimeSpan(20, 0, 0), New TimeSpan(22, 0, 0)}

    Shared sb As New StringBuilder

    Shared thisLock As New Object

    Const readDelay As Integer = 10
    Public Shared Sub ReadControlUnit(ByVal sUnit As String, ByVal sAddress485 As String, ByRef resCode As Integer)
        If sAddress485 Is Nothing Then Throw New Exception("There is no rs485 address!")
        Temperature = 0
        Humidity = 0
        SetTemperatureHigh = 0
        SetTemperatureLow = 0
        SetHumidityHigh = 0
        SetHumidityLow = 0
        ControlContact = 0
        resCode = 0

        Dim db1 As New ConfigDBClass
        Dim MaxTryLoop As Integer = 1
        Try
            MaxTryLoop = db1.GetIntValue("MaxTryReading")
        Catch ex As Exception
            WriteLog(ex.Message)
        End Try
        db1.Dispose()

        Dim TryLoop As Integer = 0
TRYLOOP:
        Try
            SyncLock thisLock

                '---------------------------------------------------------------------
                ' 1. Send environment state query command
                '---------------------------------------------------------------------
                _controlP1Commu1AJPort.DiscardInBuffer()
                _controlP1Commu1AJPort.Write(":" & sAddress485 & "1" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP1Commu1AJPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 12 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store environment state
                '---------------------------------------------------------------------
                Try
                    Dim strArray() As String = sb.ToString.Split(",")
                    Temperature = Val(strArray(0)) / 10
                    Humidity = Val(strArray(1)) / 10
                    ControlContact = CInt(Val(strArray(2)))
                Catch ex As Exception

                End Try

                '---------------------------------------------------------------------
                ' 2. Send setting query command
                '---------------------------------------------------------------------
                _controlP1Commu1AJPort.DiscardInBuffer()
                _controlP1Commu1AJPort.Write(":" & sAddress485 & "2" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running

                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP1Commu1AJPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 18 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store setting
                '---------------------------------------------------------------------
                Try
                    Dim strArray() As String = sb.ToString.Split(",")
                    SetTemperatureHigh = Val(strArray(0)) / 10
                    SetTemperatureLow = Val(strArray(1)) / 10
                    SetHumidityHigh = Val(strArray(2)) / 10
                    SetHumidityLow = Val(strArray(3)) / 10
                Catch ex As Exception

                End Try
                '-----------------------------------------------------------
                ' Log to text file
                '-----------------------------------------------------------
                Try
                    Dim db2 As New ConfigDBClass
                    Dim iLogReading As Integer = db2.GetIntValue("LogReading", True)
                    db2.Dispose()
                    If iLogReading > 0 Then
                        WriteLog(sUnit & ": " & Temperature.ToString("F1") & "," & Humidity.ToString("F1") & "," & SetTemperatureHigh.ToString("F1") & "," & SetTemperatureLow.ToString("F1") & "," & SetHumidityHigh.ToString("F1") & "," & SetHumidityLow.ToString("F1"))
                    End If

                Catch ex As Exception
                    WriteLog(ex.Message)
                    resCode = -2
                End Try

            End SyncLock

        Catch ex As Exception
            resCode = -3
            TryLoop += 1
            If TryLoop < MaxTryLoop Then : GoTo TRYLOOP '******CONFIG
            Else : Throw ex : End If
        End Try
    End Sub

    Public Shared Sub WriteSettingToControlUnit(ByVal sUnit As String, ByVal sAddress485 As String, ByVal tempHigh As String, ByVal tempLow As String, ByVal HumidHigh As String, ByVal HumidLow As String, ByRef resCode As Integer)
        resCode = 0
        Dim TryLoop As Integer = 0
TRYLOOP:
        Try
            SyncLock thisLock

                '---------------------------------------------------------------------
                ' 1. Send setting 1
                '---------------------------------------------------------------------
                _controlP1Commu1AJPort.DiscardInBuffer()
                _controlP1Commu1AJPort.Write(":" & sAddress485 & "3" & tempHigh & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running

                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP1Commu1AJPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 4 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' 2. Send setting 2
                '---------------------------------------------------------------------
                _controlP1Commu1AJPort.DiscardInBuffer()
                _controlP1Commu1AJPort.Write(":" & sAddress485 & "4" & tempLow & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running

                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP1Commu1AJPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 4 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' 3. Send setting 3
                '---------------------------------------------------------------------
                _controlP1Commu1AJPort.DiscardInBuffer()
                _controlP1Commu1AJPort.Write(":" & sAddress485 & "5" & HumidHigh & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running

                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP1Commu1AJPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 4 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' 4. Send setting 4
                '---------------------------------------------------------------------
                _controlP1Commu1AJPort.DiscardInBuffer()
                _controlP1Commu1AJPort.Write(":" & sAddress485 & "6" & HumidLow & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running

                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP1Commu1AJPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 4 Then Exit While

                End While

            End SyncLock

        Catch ex As Exception
            resCode = -1
            TryLoop += 1
            If TryLoop < 1 Then : GoTo TRYLOOP '******CONFIG
            Else : Throw ex : End If
        End Try
    End Sub

    Public Shared Sub WriteAlarmAJ(ByVal sAddress485 As String, ByVal data As String, ByRef resCode As Integer)
        resCode = 0
        Dim TryLoop As Integer = 0
TRYLOOP:
        Try

            SyncLock thisLock


                '---------------------------------------------------------------------
                ' 1. Send setting 1
                '---------------------------------------------------------------------
                _controlP1Commu1AJPort.DiscardInBuffer()
                _controlP1Commu1AJPort.Write(":" & sAddress485 & "1" & data & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP1Commu1AJPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 4 Then Exit While

                End While

            End SyncLock

        Catch ex As Exception
            resCode = -1
            TryLoop += 1
            If TryLoop < 1 Then : GoTo TRYLOOP '******CONFIG
            Else : Throw ex : End If
        End Try
    End Sub

    Public Shared Sub WriteAlarmKM(ByVal sAddress485 As String, ByVal data As String, ByRef resCode As Integer)
        resCode = 0
        Dim TryLoop As Integer = 0
TRYLOOP:
        Try

            SyncLock thisLock


                '---------------------------------------------------------------------
                ' 1. Send setting 1
                '---------------------------------------------------------------------
                _controlP2Commu1KMPort.DiscardInBuffer()
                _controlP2Commu1KMPort.Write(sAddress485 & "@3" & data & "#" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 32 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store environment state
                '---------------------------------------------------------------------
                Try
                    Dim m() As String = sb.ToString.Split("@")
                    If m(0).Equals("$" & sAddress485) Then
                        _controlP2Commu1KMPort.Write("$")
                    Else
                        _controlP2Commu1KMPort.Write("$")
                    End If
                Catch ex As Exception

                End Try

            End SyncLock

        Catch ex As Exception
            resCode = -1
            TryLoop += 1
            If TryLoop < 1 Then : GoTo TRYLOOP '******CONFIG
            Else : Throw ex : End If
        End Try
    End Sub

    Public Shared Sub RotateDisplay()
        Try
            '-------------------------------------------------------
            ' Connect
            '-------------------------------------------------------
            If _displayP1Commu2DispADPort Is Nothing Then ConnectP1Commu2DispADPort()
            If Not _displayP1Commu2DispADPort.IsOpen Then ConnectP1Commu2DispADPort()

            '-------------------------------------------------------
            ' tbPage
            '-------------------------------------------------------
            Dim dbPage As New PageDbClass

            Dim ds As DataSet = dbPage.SelectAll(True)
            Dim dt As DataTable = ds.Tables(0)

            '-------------------------------------------------------
            ' Send appear commmand
            '-------------------------------------------------------
            For Each r As DataRow In dt.Rows

                Dim en As Boolean : Try : en = (r("HasDisplay") = True) : Catch ex As Exception : Continue For : End Try
                Dim Addr485 As String = "25" : Try : Addr485 = r("Addr485") : Catch ex As Exception : End Try

                Dim theText As String = "--------"

                Dim CurrentProductName As String = "--------"
                Dim lDataID As Long
                Dim dtSensor As DataTable
                Dim dr As DataRow
                Dim Temp, Humidity As Double
                Dim TempHigh, TempLow As Double
                Dim HumidityHigh, HumidityLow As Double
                Dim iPageID As Integer = r("PageID")
                Dim iStateID As Integer = r("DisplayStateID")
                Dim newIStateId As Integer = iStateID + 1
                Dim sUnit As String = r("LinkUID")
                Dim aUnit() As String = sUnit.Split(",")
                Dim maxStateID As Integer = aUnit.Count

                If 0 < iStateID Then
                    Dim db As New TempMonitoringDBClass
                    Dim selectedUnit As String = aUnit(iStateID - 1)
                    lDataID = db.SelectMaxIDOf(selectedUnit, True) : dtSensor = db.SelectByDataID(lDataID, True).Tables(0) : dr = dtSensor.Rows(0)
                    Temp = dr("Temperature")
                    Humidity = dr("Humidity")
                    TempHigh = dr("SetTemperatureHigh")
                    TempLow = dr("SetTemperatureLow")
                    HumidityHigh = dr("SetHumidityHigh")
                    HumidityLow = dr("SetHumidityLow")
                    db.Dispose()
                Else
                    CurrentProductName = "--------" : Try : CurrentProductName = r("CurrentProductName") : Catch ex As Exception : End Try
                End If

                'The text
                Select Case iStateID
                    Case 0 : theText = CurrentProductName
                    Case 1 : theText = String.Format("1 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                    Case 2 : theText = String.Format("2 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                    Case 3 : theText = String.Format("3 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                    Case 4 : theText = String.Format("4 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                End Select

                'Display
                If en And iPageID = 1 Then

                    Try
                        If maxStateID < newIStateId Then newIStateId = 0
                        dbPage.EditStateID(iPageID, newIStateId, True)
                    Catch ex As Exception
                        WriteLog(ex.Message)
                    End Try

                    SyncLock thisLock

                        Dim dispType As String = dbPage.GetDisplayType(iPageID)

                        If dispType.ToUpper.Equals("A") Then 'อ่านชนิดตัวแสดง จาก tbpage 

                            '---------------------------------------------------------------------
                            ' 2.1 Send text
                            '---------------------------------------------------------------------
                            _displayP1Commu2DispADPort.DiscardInBuffer()
                            _displayP1Commu2DispADPort.Write(":" & Addr485 & "2" & theText & vbCr)

                            '---------------------------------------------------------------------
                            ' 2.2 Read response bytes
                            '---------------------------------------------------------------------
                            'System.Threading.Thread.Sleep(readDelay)
                            sb.Remove(0, sb.Length)
                            iByte = 0
                            running = True
                            While running
                                System.Threading.Thread.Sleep(readDelay)
                                Try
                                    ByteIncoming = 0
                                    ByteIncoming = _displayP1Commu2DispADPort.ReadByte()
                                Catch ex As Exception
                                    ByteIncoming = 0
                                    If 1 < iByte Then
                                        Exit While
                                    End If
                                End Try
                                iByte += 1

                                If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                                    sb.Append(Chr(ByteIncoming))
                                End If

                                If ByteIncoming = &HD Then
                                    Exit While
                                End If

                                If iByte > 4 Then Exit While

                            End While

                        End If

                        If dispType.ToUpper.Equals("B") Then

                            Dim sDispText() As String
                            Try
                                sDispText = theText.Split(",")
                            Catch ex As Exception
                                ReDim sDispText(1)
                                sDispText(0) = theText
                                sDispText(1) = theText
                            End Try


                            '---------------------------------------------------------------------
                            ' 2.1 Send text
                            '---------------------------------------------------------------------
                            If iStateID = 0 Then

                                _displayP1Commu2DispADPort.DiscardInBuffer()
                                _displayP1Commu2DispADPort.Write(":T00" & theText & vbCr)
                                Threading.Thread.Sleep(20)
                                _displayP1Commu2DispADPort.Write(":A" & vbCr)
                                'Threading.Thread.Sleep(100)

                            Else

                                _displayP1Commu2DispADPort.DiscardInBuffer()
                                _displayP1Commu2DispADPort.Write(":T00" & sDispText(0) & vbCr)
                                Threading.Thread.Sleep(20)
                                _displayP1Commu2DispADPort.Write(":E08  " & sDispText(1) & vbCr)
                                Threading.Thread.Sleep(20)
                                _displayP1Commu2DispADPort.Write(":A" & vbCr)

                            End If

                        End If

                        If dispType.ToUpper.Equals("C") Then 'อ่านชนิดตัวแสดง จาก tbpage 

                            Dim sDispText() As String
                            If iStateID > 0 Then
                                theText = theText.Replace(",", "")
                                sDispText = GenerateDisplayDots(theText)
                            End If

                            If iStateID = 0 Then
                                _displayP1Commu2DispADPort.DiscardInBuffer()
                                _displayP1Commu2DispADPort.Write(":" & Addr485 & "2" & theText & vbCr)
                                ReadOK(_displayP1Commu2DispADPort)

                            Else
                                If Not sDispText(0).Equals("") Then
                                    '---------------------------------------------------------------------
                                    ' Clear
                                    '---------------------------------------------------------------------
                                    _displayP1Commu2DispADPort.DiscardInBuffer()
                                    _displayP1Commu2DispADPort.Write(":" & Addr485 & "1" & vbCr)
                                    ReadOK(_displayP1Commu2DispADPort)

                                    '---------------------------------------------------------------------
                                    ' Row 0->1
                                    '---------------------------------------------------------------------
                                    _displayP1Commu2DispADPort.DiscardInBuffer()
                                    _displayP1Commu2DispADPort.Write(":" & Addr485 & "700" & sDispText(0) & vbCr)
                                    ReadOK(_displayP1Commu2DispADPort)

                                    '---------------------------------------------------------------------
                                    ' Row 2->3
                                    '---------------------------------------------------------------------
                                    _displayP1Commu2DispADPort.DiscardInBuffer()
                                    _displayP1Commu2DispADPort.Write(":" & Addr485 & "712" & sDispText(1) & vbCr)
                                    ReadOK(_displayP1Commu2DispADPort)

                                    '---------------------------------------------------------------------
                                    ' Row 4->5
                                    '---------------------------------------------------------------------
                                    _displayP1Commu2DispADPort.DiscardInBuffer()
                                    _displayP1Commu2DispADPort.Write(":" & Addr485 & "724" & sDispText(2) & vbCr)
                                    ReadOK(_displayP1Commu2DispADPort)

                                    '---------------------------------------------------------------------
                                    ' Row 6->7
                                    '---------------------------------------------------------------------
                                    _displayP1Commu2DispADPort.DiscardInBuffer()
                                    _displayP1Commu2DispADPort.Write(":" & Addr485 & "736" & sDispText(3) & vbCr)
                                    ReadOK(_displayP1Commu2DispADPort)

                                End If
                            End If

                        End If

                    End SyncLock

                End If

            Next

            dbPage.Dispose()

        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Public Shared Sub ReadMonitoringUnit(ByVal sUnit As String, ByVal sAddress485 As String, ByRef resCode As Integer)
        resCode = 0
        Temperature = 0
        Humidity = 0
        SetTemperatureHigh = 0
        SetTemperatureLow = 0
        SetHumidityHigh = 0
        SetHumidityLow = 0

        Dim db1 As New ConfigDBClass
        Dim MaxTryLoop As Integer = 1
        Try
            MaxTryLoop = db1.GetIntValue("MaxTryReading")
        Catch ex As Exception
            WriteLog(ex.Message)
        End Try
        db1.Dispose()

        Dim TryLoop As Integer = 0
TRYLOOP:
        Try
            '---------------------------------------------------------------------
            ' 1. Send environment state query command
            '---------------------------------------------------------------------
            _controlP1Commu1AJPort.DiscardInBuffer()
            _controlP1Commu1AJPort.Write(":" & sAddress485 & "1" & vbCr)

            '---------------------------------------------------------------------
            ' Read response bytes
            '---------------------------------------------------------------------
            'System.Threading.Thread.Sleep(readDelay)
            sb.Remove(0, sb.Length)
            iByte = 0
            running = True
            While running

                Try
                    ByteIncoming = 0
                    ByteIncoming = _controlP1Commu1AJPort.ReadByte()
                Catch ex As Exception
                    ByteIncoming = 0
                    If 1 < iByte Then
                        resCode = -1
                        Exit While
                    End If
                End Try
                iByte += 1

                If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                    sb.Append(Chr(ByteIncoming))
                End If

                If ByteIncoming = &HD Then
                    resCode = 0
                    Exit While
                End If

                If iByte > 20 Then Exit While

            End While

            '---------------------------------------------------------------------
            ' Analysis and store environment state
            '---------------------------------------------------------------------
            Try
                Dim strArray() As String = sb.ToString.Split(",")
                Temperature = Val(strArray(0)) / 10
                Humidity = Val(strArray(1)) / 10
                ControlContact = CInt(Val(strArray(2)))
            Catch ex As Exception

            End Try


            '-----------------------------------------------------------
            ' Log to text file
            '-----------------------------------------------------------
            Try
                Dim db2 As New ConfigDBClass
                Dim iLogReading As Integer = db2.GetIntValue("LogReading", True)
                db2.Dispose()
                If iLogReading > 0 Then
                    WriteLog(sUnit & ": " & Temperature.ToString("F1") & "," & Humidity.ToString("F1") & "," & SetTemperatureHigh.ToString("F1") & "," & SetTemperatureLow.ToString("F1") & "," & SetHumidityHigh.ToString("F1") & "," & SetHumidityLow.ToString("F1"))
                End If
            Catch ex As Exception
                WriteLog(ex.Message)
            End Try

        Catch ex As Exception
            TryLoop += 1
            If TryLoop < MaxTryLoop Then : GoTo TRYLOOP '******CONFIG
            Else : Throw ex : End If
        End Try
    End Sub

    Public Shared Sub ReadInputUnit(ByVal sUnit As String, ByVal sAddress485 As String, ByRef resCode As Integer)
        resCode = 0
        DigitalInput = -16

        Dim db1 As New ConfigDBClass
        Dim MaxTryLoop As Integer = 1
        Try
            MaxTryLoop = db1.GetIntValue("MaxTryReading")
        Catch ex As Exception
            WriteLog(ex.Message)
        End Try
        db1.Dispose()

        Dim TryLoop As Integer = 0
TRYLOOP:
        Try
            '---------------------------------------------------------------------
            ' 1. Send environment state query command
            '---------------------------------------------------------------------
            _controlP1Commu1AJPort.DiscardInBuffer()
            _controlP1Commu1AJPort.Write(":" & sAddress485 & "1" & vbCr)

            '---------------------------------------------------------------------
            ' Read response bytes
            '---------------------------------------------------------------------
            'System.Threading.Thread.Sleep(readDelay)
            sb.Remove(0, sb.Length)
            iByte = 0
            running = True
            While running
                System.Threading.Thread.Sleep(readDelay)
                Try
                    ByteIncoming = 0
                    ByteIncoming = _controlP1Commu1AJPort.ReadByte()
                Catch ex As Exception
                    ByteIncoming = 0
                    If 1 < iByte Then
                        resCode = -1
                        Exit While
                    End If
                End Try
                iByte += 1

                If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                    sb.Append(Chr(ByteIncoming))
                End If

                If ByteIncoming = &HD Then
                    resCode = 0
                    Exit While
                End If

                If iByte > 8 Then Exit While

            End While

            '---------------------------------------------------------------------
            ' Analysis and store environment state
            '---------------------------------------------------------------------
            Try
                DigitalInput = CInt(Val(sb.ToString))
            Catch ex As Exception
                DigitalInput = -16
            End Try

            '-----------------------------------------------------------
            ' Log to text file
            '-----------------------------------------------------------
            Try
                Dim db2 As New ConfigDBClass
                Dim iLogReading As Integer = db2.GetIntValue("LogReading", True)
                db2.Dispose()
                If iLogReading > 0 Then
                    WriteLog(sUnit & ": " & DigitalInput.ToString)
                End If
            Catch ex As Exception
                WriteLog(ex.Message)
            End Try

        Catch ex As Exception
            TryLoop += 1
            If TryLoop < MaxTryLoop Then : GoTo TRYLOOP '******CONFIG
            Else : Throw ex : End If
        End Try
    End Sub

    Public Shared Sub DoAllAlarm()
        Try
            Dim db As New UnitDBClass
            Dim dt As DataTable = db.SelectAlarmUnit.Tables(0)
            Dim sLinkUnit, sAddress485 As String
            For Each r As DataRow In dt.Rows
                Try
                    sLinkUnit = r("LinkUID")
                    sAddress485 = r("Address485")
                    DoAlarm(sLinkUnit, sAddress485)
                Catch ex1 As Exception
                    WriteLog(ex1.Message)
                End Try
            Next
            db.Dispose()
        Catch ex As Exception
            WriteLog(ex.Message)
        End Try
    End Sub

    Public Shared Sub DoAlarm(ByVal sLinkUnit As String, ByVal sAddress485 As String)
        Try
            Dim aLinkUnit() As String = sLinkUnit.Split(",")
            Try
                Dim lDataID As Long
                Dim dt As DataTable
                Dim dr As DataRow
                Dim Temp, Humidity As Double
                Dim TempHigh, TempLow As Double
                Dim HumidityHigh, HumidityLow As Double
                Dim TempAlarm, HumidityAlarm As enumAlarmState

                Dim dbTempMon As New TempMonitoringDBClass
                Dim iLogic As Integer = 0

                For Each s As String In aLinkUnit
                    Try
                        lDataID = dbTempMon.SelectMaxIDOf(s, True) : dt = dbTempMon.SelectByDataID(lDataID, True).Tables(0) : dr = dt.Rows(0)
                        Temp = dr("Temperature")
                        Humidity = dr("Humidity")
                        TempHigh = dr("SetTemperatureHigh")
                        TempLow = dr("SetTemperatureLow")
                        HumidityHigh = dr("SetHumidityHigh")
                        HumidityLow = dr("SetHumidityLow")
                        ControlContact = dr("ControlContact")
                        Try : DigitalInput = dr("Switches") : Catch ex As Exception : DigitalInput = 0 : End Try
                        TempAlarm = enumAlarmState.NORMAL
                        If Temp < TempLow Then TempAlarm = enumAlarmState.LOW
                        If TempHigh < Temp Then TempAlarm = enumAlarmState.HIGH
                        HumidityAlarm = enumAlarmState.NORMAL
                        If Humidity < HumidityLow Then HumidityAlarm = enumAlarmState.LOW
                        If HumidityHigh < Humidity Then HumidityAlarm = enumAlarmState.HIGH
                        If TempAlarm = enumAlarmState.HIGH Then : iLogic = iLogic Or 1 : End If
                        If TempAlarm = enumAlarmState.LOW Then : iLogic = iLogic Or 2 : End If
                        If HumidityAlarm = enumAlarmState.HIGH Then : iLogic = iLogic Or 4 : End If
                        If HumidityAlarm = enumAlarmState.LOW Then : iLogic = iLogic Or 8 : End If
                    Catch ex As Exception
                        WriteLog(ex.Message)
                    End Try
                Next

                _resCode = 0
                WriteAlarmAJ(sAddress485, iLogic.ToString("00"), _resCode)

            Catch ex As Exception
                Utilities.WriteLog(ex.Message)
            End Try
        Catch ex As Exception
            Utilities.WriteLog(ex.Message)
        End Try
    End Sub

    'Private Shared Sub ReadOK()
    '    Try
    '        'System.Threading.Thread.Sleep(readDelay)
    '        sb.Remove(0, sb.Length)
    '        iByte = 0
    '        running = True
    '        While running
    '            System.Threading.Thread.Sleep(readDelay)
    '            Try
    '                ByteIncoming = 0
    '                ByteIncoming = _controlPort.ReadByte()
    '            Catch ex As Exception
    '                ByteIncoming = 0
    '                If 1 < iByte Then
    '                    Exit While
    '                End If
    '            End Try
    '            iByte += 1

    '            If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
    '                sb.Append(Chr(ByteIncoming))
    '            End If

    '            If ByteIncoming = &HD Then
    '                Exit While
    '            End If

    '            If iByte > 4 Then Exit While

    '        End While
    '    Catch ex As Exception

    '    End Try
    'End Sub

    Private Shared Function ReadOK(ByRef _port As System.IO.Ports.SerialPort) As Boolean
        Try
            'System.Threading.Thread.Sleep(readDelay)
            sb.Remove(0, sb.Length)
            iByte = 0
            running = True
            While running
                System.Threading.Thread.Sleep(readDelay)
                Try
                    ByteIncoming = 0
                    ByteIncoming = _port.ReadByte()
                Catch ex As Exception
                    ByteIncoming = 0
                    If 1 < iByte Then
                        Return False
                    End If
                End Try
                iByte += 1

                If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                    sb.Append(Chr(ByteIncoming))
                End If

                If ByteIncoming = &HD Then
                    Return True
                End If

                If iByte > 4 Then Return False

            End While

            Return False

        Catch ex As Exception
            WriteLog(ex.Message)
            LastIOException = ex
            Return False
        End Try
    End Function

    Public Shared LastDoPollControlUnitsException As Exception
    Public Shared LastDoPollMonitorUnitsException As Exception
    Public Shared LastDoPollAlarmUnitsException As Exception
    Public Shared LastDoPollDisplayUnitsException As Exception
    Public Shared RetryLevel As Integer = 3

    Public Shared Function DoPollControlUnitsP1AJ(ByVal sProduct As String, ByVal sPeriod As String, ByVal sUnit As String, ByVal sAddress485 As String, _
                                              ByVal LinkDI As String, ByVal LinkAddress485 As String, _
                                              ByVal tempOffset As Double, ByVal humidityOffset As Double) As Boolean
        Try
            '-------------------------------------------------------
            ' Units which querying
            '-------------------------------------------------------
            Dim dbTempMon As New TempMonitoringDBClass

            Try

                For iRetry As Integer = 1 To RetryLevel
                    Temperature = 0
                    Humidity = 0
                    SetTemperatureHigh = 0
                    SetTemperatureLow = 0
                    SetHumidityHigh = 0
                    SetHumidityLow = 0
                    ControlContact = 0
                    DigitalInput = -16
                    TemperatureAlarm = enumAlarmState.FAILED
                    HumidityAlarm = enumAlarmState.FAILED

                    ReadControlUnit(sUnit, sAddress485, _resCode) 'INTERFACING!!!
                    If _resCode = 0 Then Exit For

                Next

                If _resCode <> 0 Then
                    Throw New Exception("Read CONTROL unit " & sUnit & " fault!")
                End If

                If Not LinkAddress485.Equals("") And Not LinkDI.Equals("") Then
                    For iRetry As Integer = 1 To RetryLevel
                        DigitalInput = -16
                        ReadInputUnit(LinkDI, LinkAddress485, _resCode) 'INTERFACING!!!
                        If _resCode = 0 Then Exit For
                    Next
                End If

                If _resCode <> 0 Then
                    Throw New Exception("Read SWITCHES state unit " & LinkDI & " fault!")
                End If

                TemperatureAlarm = enumAlarmState.NORMAL
                If Temperature < SetTemperatureLow Then TemperatureAlarm = enumAlarmState.LOW
                If SetTemperatureHigh < Temperature Then TemperatureAlarm = enumAlarmState.HIGH
                HumidityAlarm = enumAlarmState.NORMAL
                If Humidity < SetHumidityLow Then HumidityAlarm = enumAlarmState.LOW
                If SetHumidityHigh < Humidity Then HumidityAlarm = enumAlarmState.HIGH

            Catch ex As Exception
                WriteLog(ex.Message)
                LastDoPollControlUnitsException = ex
                Return False
            End Try

            '---------------------------------------------------------------------
            ' Update offset/calibration
            '---------------------------------------------------------------------            
            Temperature += tempOffset : Humidity += humidityOffset

            ''---------------------------------------------------------------------
            '' Insert database
            ''---------------------------------------------------------------------
            'X_TheDateTime = DateTime.Now
            'X_TodayTotalDays = TimeSpan.FromTicks(DateTime.Today.Ticks).TotalDays
            'X_Totalminutes = (X_TheDateTime - DateTime.Today).TotalMinutes
            'X_DateTimeText = X_TheDateTime.ToString("yyyy-MMM-dd HH:mm:ss")

            'dbTempMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
            '              Temperature, Humidity, ControlContact, _
            '              SetTemperatureHigh, SetTemperatureLow, SetHumidityHigh, SetHumidityLow, _
            '              "", TemperatureAlarm.ToString, HumidityAlarm.ToString, sProduct, sPeriod, DigitalInput, True)

            'dbTempMon.Dispose()

            Return True

        Catch ex As Exception
            WriteLog(ex.Message)
            LastDoPollControlUnitsException = ex
            Return False
        End Try

    End Function

    Public Shared Function DoPollMonitorUnitsP1AJ(ByVal sProduct As String, ByVal sPeriod As String, ByVal sUnit As String, ByVal sAddress485 As String) As Boolean
        Try
            '-------------------------------------------------------
            ' Units which querying
            '-------------------------------------------------------
            Dim dbTempMon As New TempMonitoringDBClass

            Try

                For iRetry As Integer = 1 To RetryLevel
                    Temperature = 0
                    Humidity = 0
                    SetTemperatureHigh = 0
                    SetTemperatureLow = 0
                    SetHumidityHigh = 0
                    SetHumidityLow = 0
                    ControlContact = 0
                    DigitalInput = -16
                    TemperatureAlarm = enumAlarmState.FAILED
                    HumidityAlarm = enumAlarmState.FAILED

                    ReadMonitoringUnit(sUnit, sAddress485, _resCode) 'INTERFACING!!!
                    If _resCode = 0 Then Exit For

                Next

                If _resCode <> 0 Then
                    Throw New Exception("Read CONTROL unit " & sUnit & " fault!")
                End If

            Catch ex As Exception
                WriteLog(ex.Message)
                LastDoPollControlUnitsException = ex
                Return False
            End Try

            ''---------------------------------------------------------------------
            '' Insert database
            ''---------------------------------------------------------------------
            'X_TheDateTime = DateTime.Now
            'X_TodayTotalDays = TimeSpan.FromTicks(DateTime.Today.Ticks).TotalDays
            'X_Totalminutes = (X_TheDateTime - DateTime.Today).TotalMinutes
            'X_DateTimeText = X_TheDateTime.ToString("yyyy-MMM-dd HH:mm:ss")

            'dbTempMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
            '              Temperature, Humidity, ControlContact, _
            '              SetTemperatureHigh, SetTemperatureLow, SetHumidityHigh, SetHumidityLow, _
            '              "", TemperatureAlarm.ToString, HumidityAlarm.ToString, sProduct, sPeriod, DigitalInput, True)

            'dbTempMon.Dispose()

            Return True

        Catch ex As Exception
            WriteLog(ex.Message)
            LastDoPollControlUnitsException = ex
            Return False
        End Try

    End Function

    Public Shared Function DoPollAlarmAJUnits(ByVal sLinkUnit As String, ByVal sAddress485 As String) As Boolean
        Try
            WriteLog("DoPollAlarmAJUnits (" & sLinkUnit & ", " & sAddress485 & ")")

            Dim aLinkUnit() As String = sLinkUnit.Split(",")

            Try
                Dim lDataID As Long
                Dim dt As DataTable
                Dim dr As DataRow
                Dim Temp, Humidity As Double
                Dim TempHigh, TempLow As Double
                Dim HumidityHigh, HumidityLow As Double
                Dim TempAlarm, HumidityAlarm As enumAlarmState

                Dim dbTempMon As New TempMonitoringDBClass
                Dim iLogic As Integer = 0

                For Each s As String In aLinkUnit 'For example <C1,C2,C3>
                    Try
                        lDataID = dbTempMon.SelectMaxIDOf(s, True) : dt = dbTempMon.SelectByDataID(lDataID, True).Tables(0) : dr = dt.Rows(0)
                        Temp = dr("Temperature")
                        Humidity = dr("Humidity")
                        TempHigh = dr("SetTemperatureHigh")
                        TempLow = dr("SetTemperatureLow")
                        HumidityHigh = dr("SetHumidityHigh")
                        HumidityLow = dr("SetHumidityLow")
                        ControlContact = dr("ControlContact")
                        Try : DigitalInput = dr("Switches") : Catch ex As Exception : DigitalInput = 0 : End Try
                        TempAlarm = enumAlarmState.NORMAL
                        If Temp < TempLow Then TempAlarm = enumAlarmState.LOW
                        If TempHigh < Temp Then TempAlarm = enumAlarmState.HIGH
                        HumidityAlarm = enumAlarmState.NORMAL
                        If Humidity < HumidityLow Then HumidityAlarm = enumAlarmState.LOW
                        If HumidityHigh < Humidity Then HumidityAlarm = enumAlarmState.HIGH
                        If TempAlarm = enumAlarmState.HIGH Then : iLogic = iLogic Or 1 : End If
                        If TempAlarm = enumAlarmState.LOW Then : iLogic = iLogic Or 2 : End If
                        If HumidityAlarm = enumAlarmState.HIGH Then : iLogic = iLogic Or 4 : End If
                        If HumidityAlarm = enumAlarmState.LOW Then : iLogic = iLogic Or 8 : End If
                    Catch ex2 As Exception
                        Throw ex2
                    End Try
                Next

                _resCode = 0
                WriteAlarmAJ(sAddress485, iLogic.ToString("00"), _resCode)

                If _resCode = 0 Then : Return True
                Else : Return False : End If

            Catch ex As Exception
                WriteLog(ex.Message)
                LastDoPollAlarmUnitsException = ex
                Return False
            End Try

        Catch ex1 As Exception
            WriteLog(ex1.Message)
            LastDoPollAlarmUnitsException = ex1
            Return False
        End Try

    End Function

    Public Shared Function DoPollAlarmKMUnits(ByVal sLinkUnit As String, ByVal sAddress485 As String) As Boolean
        Try
            WriteLog("DoPollAlarmKMUnits (" & sLinkUnit & ", " & sAddress485 & ")")

            Dim aLinkUnit() As String = sLinkUnit.Split(",")

            Try
                Dim lDataID As Long
                Dim dt As DataTable
                Dim dr As DataRow
                Dim Temp, Humidity As Double
                Dim TempHigh, TempLow As Double
                Dim HumidityHigh, HumidityLow As Double
                Dim TempAlarm, HumidityAlarm As enumAlarmState

                Dim dbTempMon As New TempMonitoringDBClass
                Dim iLogic As Integer = 0

                For Each s As String In aLinkUnit 'For example <C1,C2,C3>
                    Try
                        lDataID = dbTempMon.SelectMaxIDOf(s, True) : dt = dbTempMon.SelectByDataID(lDataID, True).Tables(0) : dr = dt.Rows(0)
                        Temp = dr("Temperature")
                        Humidity = dr("Humidity")
                        TempHigh = dr("SetTemperatureHigh")
                        TempLow = dr("SetTemperatureLow")
                        HumidityHigh = dr("SetHumidityHigh")
                        HumidityLow = dr("SetHumidityLow")
                        ControlContact = dr("ControlContact")
                        Try : DigitalInput = dr("Switches") : Catch ex As Exception : DigitalInput = 0 : End Try
                        TempAlarm = enumAlarmState.NORMAL
                        If Temp < TempLow Then TempAlarm = enumAlarmState.LOW
                        If TempHigh < Temp Then TempAlarm = enumAlarmState.HIGH
                        HumidityAlarm = enumAlarmState.NORMAL
                        If Humidity < HumidityLow Then HumidityAlarm = enumAlarmState.LOW
                        If HumidityHigh < Humidity Then HumidityAlarm = enumAlarmState.HIGH
                        If TempAlarm = enumAlarmState.HIGH Then : iLogic = iLogic Or 1 : End If
                        If TempAlarm = enumAlarmState.LOW Then : iLogic = iLogic Or 2 : End If
                        If HumidityAlarm = enumAlarmState.HIGH Then : iLogic = iLogic Or 4 : End If
                        If HumidityAlarm = enumAlarmState.LOW Then : iLogic = iLogic Or 4 : End If
                    Catch ex2 As Exception
                        Throw ex2
                    End Try
                Next

                _resCode = 0
                WriteAlarmKM(sAddress485, (0).ToString("000"), _resCode)
                WriteAlarmKM(sAddress485, iLogic.ToString("000"), _resCode)

                If _resCode = 0 Then : Return True
                Else : Return False : End If

            Catch ex As Exception
                WriteLog(ex.Message)
                LastDoPollAlarmUnitsException = ex
                Return False
            End Try

        Catch ex1 As Exception
            WriteLog(ex1.Message)
            LastDoPollAlarmUnitsException = ex1
            Return False
        End Try

    End Function

    Public Shared Function DoPollDisplayADUnits() As Boolean
        Try
            WriteLog("DoPollDisplayADUnits()")

            '-------------------------------------------------------
            ' tbPage
            '-------------------------------------------------------
            Dim dbPage As New PageDbClass

            Dim ds As DataSet = dbPage.SelectAll(True)
            Dim dt As DataTable = ds.Tables(0)

            '-------------------------------------------------------
            ' Send appear commmand
            '-------------------------------------------------------
            Dim resFlag As Boolean = False
            For Each r As DataRow In dt.Rows

                Dim en As Boolean : Try : en = (r("HasDisplay") = True) : Catch ex As Exception : Continue For : End Try
                If Not en Then Continue For
                Dim Addr485 As String = "25" : Try : Addr485 = r("Addr485") : Catch ex As Exception : Throw ex : End Try

                Dim theText As String = "--------"

                Dim CurrentProductName As String = "--------"
                Dim lDataID As Long
                Dim dtSensor As DataTable
                Dim dr As DataRow
                Dim Temp, Humidity As Double
                Dim TempHigh, TempLow As Double
                Dim HumidityHigh, HumidityLow As Double
                Dim iPageID As Integer = r("PageID")
                Dim iStateID As Integer = r("DisplayStateID")
                Dim newIStateId As Integer = iStateID + 1
                Dim sUnit As String = r("LinkUID")
                Dim aUnit() As String = sUnit.Split(",")
                Dim maxStateID As Integer = aUnit.Count

                If 0 < iStateID Then
                    Dim db As New TempMonitoringDBClass
                    Dim selectedUnit As String = aUnit(iStateID - 1)
                    lDataID = db.SelectMaxIDOf(selectedUnit, True) : dtSensor = db.SelectByDataID(lDataID, True).Tables(0) : dr = dtSensor.Rows(0)
                    Temp = dr("Temperature")
                    Humidity = dr("Humidity")
                    TempHigh = dr("SetTemperatureHigh")
                    TempLow = dr("SetTemperatureLow")
                    HumidityHigh = dr("SetHumidityHigh")
                    HumidityLow = dr("SetHumidityLow")
                    db.Dispose()
                Else
                    CurrentProductName = "--------" : Try : CurrentProductName = r("CurrentProductName") : Catch ex As Exception : End Try
                End If

                'The text
                Select Case iStateID
                    Case 0 : theText = CurrentProductName
                    Case 1 : theText = String.Format("1 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                    Case 2 : theText = String.Format("2 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                    Case 3 : theText = String.Format("3 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                    Case 4 : theText = String.Format("4 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                End Select

                'Display
                If en And iPageID = 1 Then

                    Try
                        If maxStateID < newIStateId Then newIStateId = 0
                        dbPage.EditStateID(iPageID, newIStateId, True)
                    Catch ex As Exception
                        Throw ex
                    End Try

                    Dim dispType As String = dbPage.GetDisplayType(iPageID)

                    If dispType.ToUpper.Equals("A") Then 'อ่านชนิดตัวแสดง จาก tbpage 

                        resFlag = False

                        '---------------------------------------------------------------------
                        ' 2.1 Send text
                        '---------------------------------------------------------------------
                        _displayP1Commu2DispADPort.DiscardInBuffer()
                        _displayP1Commu2DispADPort.Write(":" & Addr485 & "2" & theText & vbCr)

                        '---------------------------------------------------------------------
                        ' 2.2 Read response bytes
                        '---------------------------------------------------------------------
                        If ReadOK(_displayP1Commu2DispADPort) Then
                            resFlag = True
                        Else
                            resFlag = False
                        End If

                    End If

                    If dispType.ToUpper.Equals("B") Then

                        resFlag = False

                        Dim sDispText() As String
                        Try
                            sDispText = theText.Split(",")
                        Catch ex As Exception
                            ReDim sDispText(1)
                            sDispText(0) = theText
                            sDispText(1) = theText
                        End Try


                        '---------------------------------------------------------------------
                        ' 2.1 Send text
                        '---------------------------------------------------------------------
                        If iStateID = 0 Then

                            _displayP1Commu2DispADPort.DiscardInBuffer()
                            _displayP1Commu2DispADPort.Write(":T00" & theText & vbCr)
                            Threading.Thread.Sleep(20)
                            _displayP1Commu2DispADPort.Write(":A" & vbCr)
                            'Threading.Thread.Sleep(100)

                        Else

                            _displayP1Commu2DispADPort.DiscardInBuffer()
                            _displayP1Commu2DispADPort.Write(":T00" & sDispText(0) & vbCr)
                            Threading.Thread.Sleep(20)
                            _displayP1Commu2DispADPort.Write(":E08  " & sDispText(1) & vbCr)
                            Threading.Thread.Sleep(20)
                            _displayP1Commu2DispADPort.Write(":A" & vbCr)

                        End If

                        resFlag = True

                    End If

                    If dispType.ToUpper.Equals("C") Then 'อ่านชนิดตัวแสดง จาก tbpage 

                        resFlag = False

                        Dim sDispText() As String
                        If iStateID > 0 Then
                            theText = theText.Replace(",", "")
                            sDispText = GenerateDisplayDots(theText)
                        End If

                        If iStateID = 0 Then
                            _displayP1Commu2DispADPort.DiscardInBuffer()
                            _displayP1Commu2DispADPort.Write(":" & Addr485 & "2" & theText & vbCr)
                            If ReadOK(_displayP1Commu2DispADPort) Then
                                resFlag = True
                            Else
                                resFlag = False
                            End If

                        Else
                            If Not sDispText(0).Equals("") Then
                                '---------------------------------------------------------------------
                                ' Clear
                                '---------------------------------------------------------------------
                                _displayP1Commu2DispADPort.DiscardInBuffer()
                                _displayP1Commu2DispADPort.Write(":" & Addr485 & "1" & vbCr)
                                If ReadOK(_displayP1Commu2DispADPort) Then
                                    '---------------------------------------------------------------------
                                    ' Row 0->1
                                    '---------------------------------------------------------------------
                                    _displayP1Commu2DispADPort.DiscardInBuffer()
                                    _displayP1Commu2DispADPort.Write(":" & Addr485 & "700" & sDispText(0) & vbCr)
                                    If ReadOK(_displayP1Commu2DispADPort) Then
                                        '---------------------------------------------------------------------
                                        ' Row 2->3
                                        '---------------------------------------------------------------------
                                        _displayP1Commu2DispADPort.DiscardInBuffer()
                                        _displayP1Commu2DispADPort.Write(":" & Addr485 & "712" & sDispText(1) & vbCr)
                                        If ReadOK(_displayP1Commu2DispADPort) Then
                                            '---------------------------------------------------------------------
                                            ' Row 4->5
                                            '---------------------------------------------------------------------
                                            _displayP1Commu2DispADPort.DiscardInBuffer()
                                            _displayP1Commu2DispADPort.Write(":" & Addr485 & "724" & sDispText(2) & vbCr)
                                            If ReadOK(_displayP1Commu2DispADPort) Then
                                                '---------------------------------------------------------------------
                                                ' Row 6->7
                                                '---------------------------------------------------------------------
                                                _displayP1Commu2DispADPort.DiscardInBuffer()
                                                _displayP1Commu2DispADPort.Write(":" & Addr485 & "736" & sDispText(3) & vbCr)
                                                If ReadOK(_displayP1Commu2DispADPort) Then
                                                    resFlag = True
                                                Else
                                                    resFlag = False
                                                End If
                                            Else
                                                resFlag = False
                                            End If
                                        Else
                                            resFlag = False
                                        End If
                                    Else
                                        resFlag = False
                                    End If
                                Else
                                    resFlag = False
                                End If
                            End If
                        End If
                    End If
                End If

                If Not resFlag Then Exit For

            Next

            dbPage.Dispose()

            Return resFlag

        Catch ex0 As Exception
            WriteLog(ex0.Message)
            LastDoPollDisplayUnitsException = ex0
            Return False
        End Try

    End Function

    Public Shared Function DoPollDisplayEJUnits() As Boolean
        Try
            WriteLog("DoPollDisplayEJUnits()")

            '-------------------------------------------------------
            ' tbPage
            '-------------------------------------------------------
            Dim dbPage As New PageDbClass

            Dim ds As DataSet = dbPage.SelectAll(True)
            Dim dt As DataTable = ds.Tables(0)

            '-------------------------------------------------------
            ' Send appear commmand
            '-------------------------------------------------------
            Dim resFlag As Boolean = True
            For Each r As DataRow In dt.Rows

                Dim en As Boolean : Try : en = (r("HasDisplay") = True) : Catch ex As Exception : Continue For : End Try
                If Not en Then Continue For
                Dim Addr485 As String = "25" : Try : Addr485 = r("Addr485") : Catch ex As Exception : Throw ex : End Try

                Dim theText As String = "--------"

                Dim CurrentProductName As String = "--------"
                Dim lDataID As Long
                Dim dtSensor As DataTable
                Dim dr As DataRow
                Dim Temp, Humidity As Double
                Dim TempHigh, TempLow As Double
                Dim HumidityHigh, HumidityLow As Double
                Dim iPageID As Integer = r("PageID")
                Dim iStateID As Integer = r("DisplayStateID")
                Dim newIStateId As Integer = iStateID + 1
                Dim sUnit As String = r("LinkUID")
                Dim aUnit() As String = sUnit.Split(",")
                Dim maxStateID As Integer = aUnit.Count

                If 0 < iStateID Then
                    Dim db As New TempMonitoringDBClass
                    Dim selectedUnit As String = aUnit(iStateID - 1)
                    lDataID = db.SelectMaxIDOf(selectedUnit, True) : dtSensor = db.SelectByDataID(lDataID, True).Tables(0) : dr = dtSensor.Rows(0)
                    Temp = dr("Temperature")
                    Humidity = dr("Humidity")
                    TempHigh = dr("SetTemperatureHigh")
                    TempLow = dr("SetTemperatureLow")
                    HumidityHigh = dr("SetHumidityHigh")
                    HumidityLow = dr("SetHumidityLow")
                    db.Dispose()
                Else
                    CurrentProductName = "--------" : Try : CurrentProductName = r("CurrentProductName") : Catch ex As Exception : End Try
                End If

                'The text
                Select Case iStateID
                    Case 0 : theText = CurrentProductName
                    Case 1 : theText = String.Format("1 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                    Case 2 : theText = String.Format("2 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                    Case 3 : theText = String.Format("3 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                    Case 4 : theText = String.Format("4 {0:00.0}c,{1:00.0}%", Temp, Humidity)
                End Select

                'Display
                If en And iPageID = 2 Then

                    Try
                        If maxStateID < newIStateId Then newIStateId = 0
                        dbPage.EditStateID(iPageID, newIStateId, True)
                    Catch ex As Exception
                        Throw ex
                    End Try

                    Dim dispType As String = dbPage.GetDisplayType(iPageID)

                    If dispType.ToUpper.Equals("A") Then 'อ่านชนิดตัวแสดง จาก tbpage 

                        resFlag = False

                        '---------------------------------------------------------------------
                        ' 2.1 Send text
                        '---------------------------------------------------------------------
                        _displayP1Commu3DispEJPort.DiscardInBuffer()
                        _displayP1Commu3DispEJPort.Write(":" & Addr485 & "2" & theText & vbCr)

                        '---------------------------------------------------------------------
                        ' 2.2 Read response bytes
                        '---------------------------------------------------------------------
                        If ReadOK(_displayP1Commu3DispEJPort) Then
                            resFlag = True
                        Else
                            resFlag = False
                        End If

                    End If

                    If dispType.ToUpper.Equals("B") Then

                        resFlag = False

                        Dim sDispText() As String
                        Try
                            sDispText = theText.Split(",")
                        Catch ex As Exception
                            ReDim sDispText(1)
                            sDispText(0) = theText
                            sDispText(1) = theText
                        End Try


                        '---------------------------------------------------------------------
                        ' 2.1 Send text
                        '---------------------------------------------------------------------
                        If iStateID = 0 Then

                            _displayP1Commu3DispEJPort.DiscardInBuffer()
                            _displayP1Commu3DispEJPort.Write(":T00" & theText & vbCr)
                            Threading.Thread.Sleep(20)
                            _displayP1Commu3DispEJPort.Write(":A" & vbCr)
                            'Threading.Thread.Sleep(100)

                        Else

                            _displayP1Commu3DispEJPort.DiscardInBuffer()
                            _displayP1Commu3DispEJPort.Write(":T00" & sDispText(0) & vbCr)
                            Threading.Thread.Sleep(20)
                            _displayP1Commu3DispEJPort.Write(":E08  " & sDispText(1) & vbCr)
                            Threading.Thread.Sleep(20)
                            _displayP1Commu3DispEJPort.Write(":A" & vbCr)

                        End If

                        resFlag = True

                    End If

                    If dispType.ToUpper.Equals("C") Then 'อ่านชนิดตัวแสดง จาก tbpage 

                        resFlag = False

                        Dim sDispText() As String
                        If iStateID > 0 Then
                            theText = theText.Replace(",", "")
                            sDispText = GenerateDisplayDots(theText)
                        End If

                        If iStateID = 0 Then
                            _displayP1Commu3DispEJPort.DiscardInBuffer()
                            _displayP1Commu3DispEJPort.Write(":" & Addr485 & "2" & theText & vbCr)
                            If ReadOK(_displayP1Commu3DispEJPort) Then
                                resFlag = True
                            Else
                                resFlag = False
                            End If

                        Else
                            If Not sDispText(0).Equals("") Then
                                '---------------------------------------------------------------------
                                ' Clear
                                '---------------------------------------------------------------------
                                _displayP1Commu3DispEJPort.DiscardInBuffer()
                                _displayP1Commu3DispEJPort.Write(":" & Addr485 & "1" & vbCr)
                                If ReadOK(_displayP1Commu3DispEJPort) Then
                                    '---------------------------------------------------------------------
                                    ' Row 0->1
                                    '---------------------------------------------------------------------
                                    _displayP1Commu3DispEJPort.DiscardInBuffer()
                                    _displayP1Commu3DispEJPort.Write(":" & Addr485 & "700" & sDispText(0) & vbCr)
                                    If ReadOK(_displayP1Commu3DispEJPort) Then
                                        '---------------------------------------------------------------------
                                        ' Row 2->3
                                        '---------------------------------------------------------------------
                                        _displayP1Commu3DispEJPort.DiscardInBuffer()
                                        _displayP1Commu3DispEJPort.Write(":" & Addr485 & "712" & sDispText(1) & vbCr)
                                        If ReadOK(_displayP1Commu3DispEJPort) Then
                                            '---------------------------------------------------------------------
                                            ' Row 4->5
                                            '---------------------------------------------------------------------
                                            _displayP1Commu3DispEJPort.DiscardInBuffer()
                                            _displayP1Commu3DispEJPort.Write(":" & Addr485 & "724" & sDispText(2) & vbCr)
                                            If ReadOK(_displayP1Commu3DispEJPort) Then
                                                '---------------------------------------------------------------------
                                                ' Row 6->7
                                                '---------------------------------------------------------------------
                                                _displayP1Commu3DispEJPort.DiscardInBuffer()
                                                _displayP1Commu3DispEJPort.Write(":" & Addr485 & "736" & sDispText(3) & vbCr)
                                                If ReadOK(_displayP1Commu3DispEJPort) Then
                                                    resFlag = True
                                                Else
                                                    resFlag = False
                                                End If
                                            Else
                                                resFlag = False
                                            End If
                                        Else
                                            resFlag = False
                                        End If
                                    Else
                                        resFlag = False
                                    End If
                                Else
                                    resFlag = False
                                End If
                            End If
                        End If
                    End If
                End If

                If Not resFlag Then Exit For

            Next

            dbPage.Dispose()

            Return resFlag

        Catch ex0 As Exception
            WriteLog(ex0.Message)
            LastDoPollDisplayUnitsException = ex0
            Return False
        End Try

    End Function

    Public Shared Sub DoCC1(ByVal sUnit As String, ByVal iPageId As Integer, ByVal sProduct As String, ByVal createCC As Boolean, ByRef _lastCCID As Long)
        Dim logText As New StringBuilder
        Try
            Dim sPeriod As String
            Dim lPeriodID As Long
            Try
                lPeriodID = PeriodCheck(sPeriod)
            Catch ex As Exception
                sPeriod = "NONE"
            End Try
            Dim lastDataID As Long
            Dim lastCCID As Long = -99
            If (sUnit.ToUpper.Contains("C") Or sUnit.ToUpper.Contains("M")) Then
                Dim dbMon As New MonitoringDBClass
                lastDataID = dbMon.SelectMaxIDOf(sUnit)
                logText.Append("CC :" & sUnit & " DataID:" & lastDataID.ToString)
                Try
                    Using conn As New SqlCeConnection(dbMon.GetMonitoringDbConnString)
                        conn.Open()
                        Dim tx As SqlCeTransaction
                        tx = conn.BeginTransaction
                        Try
                            '----------------------------------
                            'Control condition Header
                            '----------------------------------
                            If lastCCID < 0 Then
                                dbMon.InsertControlConditionsHeader(X_TheDateTime, X_TodayTotalDays, X_DateTimeText, X_TheDateTime.ToString("HH:mm"), lPeriodID, True, conn, tx)
                                lastCCID = dbMon.SelectMaxCCIDOf(True, conn)
                            End If
                            logText.Append(" CCID: " & lastCCID.ToString)
                            _lastCCID = lastCCID

                            '----------------------------------
                            'Control condition for each page
                            '----------------------------------
                            If createCC Then
                                dbMon.InsertControlConditions(lastCCID, iPageId, sProduct, True, conn, tx)
                            End If

                            '----------------------------------
                            'Control condition detail each unit
                            '----------------------------------
                            If sUnit.ToUpper.Contains("C") Then dbMon.InsertControlConditionsDetail(lastCCID, sUnit, True, conn, tx)

                            '----------------------------------
                            'Update monitoirng record
                            '----------------------------------
                            dbMon.UpdateMonitoringCCID(lastDataID, lastCCID, True, conn, tx)

                            '----------------------------------
                            ' Commit
                            '----------------------------------
                            tx.Commit()
                            logText.Append(" Commited.")
                        Catch ex As Exception
                            tx.Rollback()
                            logText.Append(" Rollback: " & lastCCID.ToString)
                        End Try
                        conn.Close()
                    End Using

                    dbMon.Dispose()

                Catch excc As Exception
                    logText.Append(" Error!: " & excc.Message)
                End Try

                WriteLog(logText.ToString)

            End If

        Catch ex As Exception
            WriteLog("CONTROL CONDITION: " & ex.ToString & " -- " & logText.ToString)
        End Try
    End Sub

    Public Shared Sub DoCC2(ByVal sUnit As String, ByVal iPageId As Integer, ByVal sProduct As String, ByVal createCC As Boolean, ByVal _lastCCID As Long)
        Dim logText As New StringBuilder
        Try
            Dim sPeriod As String
            Dim lPeriodID As Long
            Try
                lPeriodID = PeriodCheck(sPeriod)
            Catch ex As Exception
                sPeriod = "NONE"
            End Try
            Dim lastDataID As Long
            Dim lastCCID As Long = _lastCCID
            If (sUnit.ToUpper.Contains("C") Or sUnit.ToUpper.Contains("M")) Then
                Dim dbMon As New MonitoringDBClass
                lastDataID = dbMon.SelectMaxIDOf(sUnit)
                logText.Append("CC :" & sUnit & " DataID:" & lastDataID.ToString)
                Try
                    Using conn As New SqlCeConnection(dbMon.GetMonitoringDbConnString)
                        conn.Open()
                        Dim tx As SqlCeTransaction
                        tx = conn.BeginTransaction
                        Try
                            '----------------------------------
                            'Control condition for each page
                            '----------------------------------
                            If createCC Then
                                dbMon.InsertControlConditions(lastCCID, iPageId, sProduct, True, conn, tx)
                            End If

                            '----------------------------------
                            'Control condition detail each unit
                            '----------------------------------
                            If sUnit.ToUpper.Contains("C") Then dbMon.InsertControlConditionsDetail(lastCCID, sUnit, True, conn, tx)

                            '----------------------------------
                            'Update monitoirng record
                            '----------------------------------
                            dbMon.UpdateMonitoringCCID(lastDataID, lastCCID, True, conn, tx)

                            '----------------------------------
                            ' Commit
                            '----------------------------------
                            tx.Commit()
                            logText.Append(" Commited.")
                        Catch ex As Exception
                            tx.Rollback()
                            logText.Append(" Rollback: " & lastCCID.ToString)
                        End Try
                        conn.Close()
                    End Using

                    dbMon.Dispose()

                Catch excc As Exception
                    logText.Append(" Error!: " & excc.Message)
                End Try

                WriteLog(logText.ToString)

            End If

        Catch ex As Exception
            WriteLog("CONTROL CONDITION: " & ex.ToString & " -- " & logText.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Save specific data from unit to tbMonitoring.
    ''' By default, 5.0 minutes interval timer trigs this.
    ''' </summary>
    ''' <param name="sUnit"></param>
    ''' <param name="sUType"></param>
    ''' <param name="sProduct"></param>
    ''' <param name="sPeriod"></param>
    ''' <param name="lPeriodID"></param>
    ''' <remarks></remarks>
    Public Shared Sub SaveMonitoringFrom(ByVal sUnit As String, ByVal sUType As String, ByVal sProduct As String, ByVal sPeriod As String, ByVal lPeriodID As Long)
        Try
            '-------------------------------------------------------
            ' Database callback
            '-------------------------------------------------------
            Dim dbMon As New MonitoringDBClass

            Try
                Select Case sUType
                    Case "C"
                        '---------------------------------------------------------------------
                        ' Insert database
                        '---------------------------------------------------------------------
                        dbMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
                                      Temperature, Humidity, ControlContact, _
                                      SetTemperatureHigh, SetTemperatureLow, SetHumidityHigh, SetHumidityLow, _
                                      "", TemperatureAlarm.ToString, HumidityAlarm.ToString, sProduct, sPeriod, DigitalInput, lPeriodID, True)

                    Case "M"
                        '---------------------------------------------------------------------
                        ' Insert database
                        '---------------------------------------------------------------------
                        dbMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
                                      Temperature, Humidity, ControlContact, _
                                      SetTemperatureHigh, SetTemperatureLow, SetHumidityHigh, SetHumidityLow, _
                                      "", TemperatureAlarm.ToString, HumidityAlarm.ToString, sProduct, sPeriod, DigitalInput, lPeriodID, True)

                End Select

                WriteLog("MONITORING UPDATING DONE." & Environment.NewLine)

            Catch ex As Exception
                WriteLog(ex.Message)
            End Try

            '---------------------------------------------------------------------
            ' Dispose database
            '---------------------------------------------------------------------
            dbMon.Dispose()

        Catch ex As Exception
            WriteLog(ex.Message)
            Throw ex
        End Try

    End Sub

#Region "Phase-II K-M"

    Public Shared Sub ReadControlUnitP2KM(ByVal sUnit As String, ByVal sAddress485 As String, ByRef resCode As Integer)
        If sAddress485 Is Nothing Then Throw New Exception("There is no rs485 address!")
        Temperature = 0
        Humidity = 0
        SetTemperatureHigh = 0
        SetTemperatureLow = 0
        SetHumidityHigh = 0
        SetHumidityLow = 0
        ControlContact = 0
        resCode = 0

        Dim db1 As New ConfigDBClass
        Dim MaxTryLoop As Integer = 1
        Try
            MaxTryLoop = db1.GetIntValue("MaxTryReading")
        Catch ex As Exception
            WriteLog(ex.Message)
        End Try
        db1.Dispose()

        Dim TryLoop As Integer = 0
TRYLOOP:
        Try
            SyncLock thisLock

                '---------------------------------------------------------------------
                ' 1. Send environment state query command
                '---------------------------------------------------------------------
                _controlP2Commu1KMPort.DiscardInBuffer()
                _controlP2Commu1KMPort.Write(sAddress485 & "?0#" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 32 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store environment state
                '---------------------------------------------------------------------
                Try
                    Dim m() As String = sb.ToString.Split("?")
                    If m(0).Equals("$" & sAddress485) Then
                        _controlP2Commu1KMPort.Write("$")
                        Dim strArray() As String = m(1).ToString.Split(",")
                        Temperature = Val(strArray(0)) / 10
                        Humidity = Val(strArray(1)) / 10
                        ControlContact = CInt(Val(strArray(2)))
                    End If
                Catch ex As Exception

                End Try

                '---------------------------------------------------------------------
                ' 2. Send setting query command
                '---------------------------------------------------------------------
                _controlP2Commu1KMPort.DiscardInBuffer()
                _controlP2Commu1KMPort.Write(sAddress485 & "@12#" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running

                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 32 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store setting
                '---------------------------------------------------------------------
                Try
                    Dim m() As String = sb.ToString.Split("@")
                    If m(0).Equals("$" & sAddress485) Then
                        _controlP2Commu1KMPort.Write("$")
                        Dim strArray() As String = m(1).ToString.Split(",")
                        SetTemperatureHigh = Val(strArray(0)) / 10
                        SetTemperatureLow = Val(strArray(1)) / 10
                        SetHumidityHigh = Val(strArray(2)) / 10
                        SetHumidityLow = Val(strArray(3)) / 10
                    End If
                Catch ex As Exception

                End Try
                '-----------------------------------------------------------
                ' Log to text file
                '-----------------------------------------------------------
                Try
                    Dim db2 As New ConfigDBClass
                    Dim iLogReading As Integer = db2.GetIntValue("LogReading", True)
                    db2.Dispose()
                    If iLogReading > 0 Then
                        WriteLog(sUnit & ": " & Temperature.ToString("F1") & "," & Humidity.ToString("F1") & "," & SetTemperatureHigh.ToString("F1") & "," & SetTemperatureLow.ToString("F1") & "," & SetHumidityHigh.ToString("F1") & "," & SetHumidityLow.ToString("F1"))
                    End If

                Catch ex As Exception
                    WriteLog(ex.Message)
                    resCode = -2
                End Try

            End SyncLock

        Catch ex As Exception
            resCode = -3
            TryLoop += 1
            If TryLoop < MaxTryLoop Then : GoTo TRYLOOP '******CONFIG
            Else : Throw ex : End If
        End Try
    End Sub

    Public Shared Sub ReadInputUnitP2KM(ByVal sUnit As String, ByVal sAddress485 As String, ByRef resCode As Integer)
        resCode = 0
        DigitalInput = -16

        Dim db1 As New ConfigDBClass
        Dim MaxTryLoop As Integer = 1
        Try
            MaxTryLoop = db1.GetIntValue("MaxTryReading")
        Catch ex As Exception
            WriteLog(ex.Message)
        End Try
        db1.Dispose()

        Dim TryLoop As Integer = 0
TRYLOOP:
        Try
            '---------------------------------------------------------------------
            ' 1. Send environment state query command
            '---------------------------------------------------------------------
            _controlP2Commu1KMPort.DiscardInBuffer()
            _controlP2Commu1KMPort.Write(sAddress485 & "?0#" & vbCr)

            '---------------------------------------------------------------------
            ' Read response bytes
            '---------------------------------------------------------------------
            'System.Threading.Thread.Sleep(readDelay)
            sb.Remove(0, sb.Length)
            iByte = 0
            running = True
            While running
                System.Threading.Thread.Sleep(readDelay)
                Try
                    ByteIncoming = 0
                    ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                Catch ex As Exception
                    ByteIncoming = 0
                    If 1 < iByte Then
                        resCode = -1
                        Exit While
                    End If
                End Try
                iByte += 1

                If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                    sb.Append(Chr(ByteIncoming))
                End If

                If ByteIncoming = &HD Then
                    resCode = 0
                    Exit While
                End If

                If iByte > 32 Then Exit While

            End While

            '---------------------------------------------------------------------
            ' Analysis and store environment state
            '---------------------------------------------------------------------
            Try
                Dim m() As String = sb.ToString.Split("?")
                If m(0).Equals("$" & sAddress485) Then
                    _controlP2Commu1KMPort.Write("$")
                    Dim di() As String = m(1).ToString.Split(",")
                    DigitalInputP2KM(0) = CInt(Val(di(0).ToString))
                    DigitalInputP2KM(1) = CInt(Val(di(1).ToString))
                    DigitalInputP2KM(2) = CInt(Val(di(2).ToString))
                    DigitalInputP2KM(3) = CInt(Val(di(3).ToString))
                End If
            Catch ex As Exception

            End Try

            '-----------------------------------------------------------
            ' Log to text file
            '-----------------------------------------------------------
            Try
                Dim db2 As New ConfigDBClass
                Dim iLogReading As Integer = db2.GetIntValue("LogReading", True)
                db2.Dispose()
                If iLogReading > 0 Then
                    WriteLog(sUnit & ": " & DigitalInput.ToString)
                End If
            Catch ex As Exception
                WriteLog(ex.Message)
            End Try

        Catch ex As Exception
            TryLoop += 1
            If TryLoop < MaxTryLoop Then : GoTo TRYLOOP '******CONFIG
            Else : Throw ex : End If
        End Try
    End Sub

    Public Shared Sub WriteSettingToControlUnitP2KM(ByVal sUnit As String, ByVal sAddress485 As String, ByVal tempHigh As String, ByVal tempLow As String, ByVal HumidHigh As String, ByVal HumidLow As String, ByRef resCode As Integer)
        resCode = 0
        Dim TryLoop As Integer = 0
TRYLOOP:
        Try
            SyncLock thisLock

                '---------------------------------------------------------------------
                ' 0. ERASE
                '---------------------------------------------------------------------
                System.Threading.Thread.Sleep(300)
                _controlP2Commu1KMPort.DiscardInBuffer()
                _controlP2Commu1KMPort.Write(sAddress485 & "@10#" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 32 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store environment state
                '---------------------------------------------------------------------
                Try
                    Dim m() As String = sb.ToString.Split("@")
                    If m(0).Equals("$" & sAddress485) Then
                        _controlP2Commu1KMPort.Write("$")
                    Else
                        _controlP2Commu1KMPort.Write("$")
                    End If
                Catch ex As Exception

                End Try

                '---------------------------------------------------------------------
                ' 1. Send setting 1
                '---------------------------------------------------------------------
                System.Threading.Thread.Sleep(300)
                _controlP2Commu1KMPort.DiscardInBuffer()
                _controlP2Commu1KMPort.Write(sAddress485 & "@510" & tempHigh & "#" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 32 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store environment state
                '---------------------------------------------------------------------
                Try
                    Dim m() As String = sb.ToString.Split("@")
                    If m(0).Equals("$" & sAddress485) Then
                        _controlP2Commu1KMPort.Write("$")
                    End If
                Catch ex As Exception

                End Try

                '---------------------------------------------------------------------
                ' 2. Send setting 2
                '---------------------------------------------------------------------
                System.Threading.Thread.Sleep(300)
                _controlP2Commu1KMPort.DiscardInBuffer()
                _controlP2Commu1KMPort.Write(sAddress485 & "@511" & tempLow & "#" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 32 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store environment state
                '---------------------------------------------------------------------
                Try
                    Dim m() As String = sb.ToString.Split("@")
                    If m(0).Equals("$" & sAddress485) Then
                        _controlP2Commu1KMPort.Write("$")
                    End If
                Catch ex As Exception

                End Try

                '---------------------------------------------------------------------
                ' 3. Send setting 3
                '---------------------------------------------------------------------
                System.Threading.Thread.Sleep(300)
                _controlP2Commu1KMPort.DiscardInBuffer()
                _controlP2Commu1KMPort.Write(sAddress485 & "@512" & HumidHigh & "#" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 32 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store environment state
                '---------------------------------------------------------------------
                Try
                    Dim m() As String = sb.ToString.Split("@")
                    If m(0).Equals("$" & sAddress485) Then
                        _controlP2Commu1KMPort.Write("$")
                    End If
                Catch ex As Exception

                End Try

                '---------------------------------------------------------------------
                ' 4. Send setting 4
                '---------------------------------------------------------------------
                System.Threading.Thread.Sleep(300)
                _controlP2Commu1KMPort.DiscardInBuffer()
                _controlP2Commu1KMPort.Write(sAddress485 & "@513" & HumidLow & "#" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 32 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store environment state
                '---------------------------------------------------------------------
                Try
                    Dim m() As String = sb.ToString.Split("@")
                    If m(0).Equals("$" & sAddress485) Then
                        _controlP2Commu1KMPort.Write("$")
                    End If
                Catch ex As Exception

                End Try

                '---------------------------------------------------------------------
                ' 5. Test
                '---------------------------------------------------------------------
                System.Threading.Thread.Sleep(300)
                _controlP2Commu1KMPort.DiscardInBuffer()
                _controlP2Commu1KMPort.Write(sAddress485 & "@12#" & vbCr)

                '---------------------------------------------------------------------
                ' Read response bytes
                '---------------------------------------------------------------------
                'System.Threading.Thread.Sleep(readDelay)
                sb.Remove(0, sb.Length)
                iByte = 0
                running = True
                While running
                    System.Threading.Thread.Sleep(readDelay)
                    Try
                        ByteIncoming = 0
                        ByteIncoming = _controlP2Commu1KMPort.ReadByte()
                    Catch ex As Exception
                        ByteIncoming = 0
                        If 1 < iByte Then
                            resCode = -1
                            Exit While
                        End If
                    End Try
                    iByte += 1

                    If 20 <= ByteIncoming And ByteIncoming <= &H7E Then
                        sb.Append(Chr(ByteIncoming))
                    End If

                    If ByteIncoming = &HD Then
                        resCode = 0
                        Exit While
                    End If

                    If iByte > 32 Then Exit While

                End While

                '---------------------------------------------------------------------
                ' Analysis and store environment state
                '---------------------------------------------------------------------
                Try
                    Dim m() As String = sb.ToString.Split("@")
                    If m(0).Equals("$" & sAddress485) Then
                        _controlP2Commu1KMPort.Write("$")
                    End If
                Catch ex As Exception

                End Try

                System.Threading.Thread.Sleep(300)

            End SyncLock

        Catch ex As Exception
            resCode = -1
            TryLoop += 1
            If TryLoop < 1 Then : GoTo TRYLOOP '******CONFIG
            Else : Throw ex : End If
        End Try
    End Sub

    Public Shared Function DoPollControlUnitsP2KM(ByVal sProduct As String, ByVal sPeriod As String, ByVal sUnit As String, ByVal sAddress485 As String, _
                                              ByVal LinkDI As String, ByVal LinkAddress485 As String, _
                                              ByVal tempOffset As Double, ByVal humidityOffset As Double) As Boolean
        Try
            '-------------------------------------------------------
            ' Units which querying
            '-------------------------------------------------------
            Dim dbTempMon As New TempMonitoringDBClass

            Try

                For iRetry As Integer = 1 To RetryLevel
                    Temperature = 0
                    Humidity = 0
                    SetTemperatureHigh = 0
                    SetTemperatureLow = 0
                    SetHumidityHigh = 0
                    SetHumidityLow = 0
                    ControlContact = 0
                    DigitalInput = -16
                    TemperatureAlarm = enumAlarmState.FAILED
                    HumidityAlarm = enumAlarmState.FAILED

                    ReadControlUnitP2KM(sUnit, sAddress485, _resCode) 'INTERFACING!!!
                    If _resCode = 0 Then Exit For

                Next

                If _resCode <> 0 Then
                    Throw New Exception("Read CONTROL unit " & sUnit & " fault!")
                End If

                ' GoTo N1 '<--- CAUTION

                If Not LinkAddress485.Equals("") And Not LinkDI.Equals("") Then
                    For iRetry As Integer = 1 To RetryLevel
                        DigitalInputP2KM(0) = -16
                        DigitalInputP2KM(1) = -16
                        DigitalInputP2KM(2) = -16
                        DigitalInputP2KM(3) = -16

                        ReadInputUnitP2KM(LinkDI, LinkAddress485, _resCode) 'INTERFACING!!!
                        If _resCode = 0 Then Exit For
                    Next
                End If

                'If _resCode <> 0 Then
                '    Throw New Exception("Read SWITCHES state unit " & LinkDI & " fault!")
                'End If

                ' Update Adaptive DI
                If DigitalInputP2KM(0) = -16 Then DigitalInputP2KM(0) = 255
                If DigitalInputP2KM(1) = -16 Then DigitalInputP2KM(0) = 255
                If DigitalInputP2KM(2) = -16 Then DigitalInputP2KM(0) = 255
                If DigitalInputP2KM(3) = -16 Then DigitalInputP2KM(0) = 255
                Dim iBit, nBit As Integer
                Select Case sUnit
                    Case "C7"
                        DigitalInput = 0
                        iBit = 1 : nBit = 1
                        DigitalInput += IIf(((DigitalInputP2KM(0) And iBit) = iBit), nBit, 0)
                        iBit = 2 : nBit = 2
                        DigitalInput += IIf(((DigitalInputP2KM(0) And iBit) = iBit), nBit, 0)
                        iBit = 4 : nBit = 4
                        DigitalInput += IIf(((DigitalInputP2KM(0) And iBit) = iBit), nBit, 0)
                        iBit = 16 : nBit = 8
                        DigitalInput += IIf(((DigitalInputP2KM(0) And iBit) = iBit), nBit, 0)
                        iBit = 32 : nBit = 16
                        DigitalInput += IIf(((DigitalInputP2KM(0) And iBit) = iBit), nBit, 0)
                        iBit = 64 : nBit = 32
                        DigitalInput += IIf(((DigitalInputP2KM(0) And iBit) = iBit), nBit, 0)

                    Case "C8"
                        DigitalInput = 0
                        iBit = 128 : nBit = 1
                        DigitalInput += IIf(((DigitalInputP2KM(0) And iBit) = iBit), nBit, 0)
                        iBit = 1 : nBit = 2
                        DigitalInput += IIf(((DigitalInputP2KM(1) And iBit) = iBit), nBit, 0)
                        iBit = 2 : nBit = 4
                        DigitalInput += IIf(((DigitalInputP2KM(1) And iBit) = iBit), nBit, 0)
                        iBit = 4 : nBit = 8
                        DigitalInput += IIf(((DigitalInputP2KM(1) And iBit) = iBit), nBit, 0)
                        iBit = 16 : nBit = 16
                        DigitalInput += IIf(((DigitalInputP2KM(1) And iBit) = iBit), nBit, 0)
                        iBit = 32 : nBit = 32
                        DigitalInput += IIf(((DigitalInputP2KM(1) And iBit) = iBit), nBit, 0)

                    Case "C9"
                        DigitalInput = 0
                        iBit = 64 : nBit = 1
                        DigitalInput += IIf(((DigitalInputP2KM(1) And iBit) = iBit), nBit, 0)
                        iBit = 128 : nBit = 2
                        DigitalInput += IIf(((DigitalInputP2KM(1) And iBit) = iBit), nBit, 0)
                        iBit = 1 : nBit = 4
                        DigitalInput += IIf(((DigitalInputP2KM(2) And iBit) = iBit), nBit, 0)
                        iBit = 2 : nBit = 8
                        DigitalInput += IIf(((DigitalInputP2KM(2) And iBit) = iBit), nBit, 0)
                        iBit = 4 : nBit = 16
                        DigitalInput += IIf(((DigitalInputP2KM(2) And iBit) = iBit), nBit, 0)
                        iBit = 16 : nBit = 32
                        DigitalInput += IIf(((DigitalInputP2KM(2) And iBit) = iBit), nBit, 0)

                End Select

N1:

                ' Update alarm state
                TemperatureAlarm = enumAlarmState.NORMAL
                If Temperature < SetTemperatureLow Then TemperatureAlarm = enumAlarmState.LOW
                If SetTemperatureHigh < Temperature Then TemperatureAlarm = enumAlarmState.HIGH
                HumidityAlarm = enumAlarmState.NORMAL
                If Humidity < SetHumidityLow Then HumidityAlarm = enumAlarmState.LOW
                If SetHumidityHigh < Humidity Then HumidityAlarm = enumAlarmState.HIGH

            Catch ex As Exception
                WriteLog(ex.Message)
                LastDoPollControlUnitsException = ex
                Return False
            End Try

            '---------------------------------------------------------------------
            ' Update offset/calibration
            '---------------------------------------------------------------------            
            Temperature += tempOffset : Humidity += humidityOffset

            ''---------------------------------------------------------------------
            '' Insert database
            ''---------------------------------------------------------------------
            'X_TheDateTime = DateTime.Now
            'X_TodayTotalDays = TimeSpan.FromTicks(DateTime.Today.Ticks).TotalDays
            'X_Totalminutes = (X_TheDateTime - DateTime.Today).TotalMinutes
            'X_DateTimeText = X_TheDateTime.ToString("yyyy-MMM-dd HH:mm:ss")

            'dbTempMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
            '              Temperature, Humidity, ControlContact, _
            '              SetTemperatureHigh, SetTemperatureLow, SetHumidityHigh, SetHumidityLow, _
            '              "", TemperatureAlarm.ToString, HumidityAlarm.ToString, sProduct, sPeriod, DigitalInput, True)

            'dbTempMon.Dispose()

            Return True

        Catch ex As Exception
            WriteLog(ex.Message)
            LastDoPollControlUnitsException = ex
            Return False
        End Try

    End Function

    Public Shared Function DoPollMonitoringP2KM(ByVal sProduct As String, ByVal sPeriod As String, ByVal sUnit As String, ByVal sAddress485 As String) As Boolean
        Try
            '-------------------------------------------------------
            ' Units which querying
            '-------------------------------------------------------            
            Try

                For iRetry As Integer = 1 To RetryLevel
                    Temperature = 0
                    Humidity = 0
                    SetTemperatureHigh = 0
                    SetTemperatureLow = 0
                    SetHumidityHigh = 0
                    SetHumidityLow = 0
                    ControlContact = 0
                    DigitalInput = -16
                    TemperatureAlarm = enumAlarmState.FAILED
                    HumidityAlarm = enumAlarmState.FAILED

                    ReadControlUnitP2KM(sUnit, sAddress485, _resCode) 'INTERFACING!!!
                    If _resCode = 0 Then Exit For

                Next

                If _resCode <> 0 Then
                    Throw New Exception("Read Monitoring unit: " & sUnit & " fault!")
                End If

            Catch ex As Exception
                WriteLog(ex.Message)
                LastDoPollControlUnitsException = ex
                Return False
            End Try

            ''---------------------------------------------------------------------
            '' Insert database
            ''---------------------------------------------------------------------
            'X_TheDateTime = DateTime.Now
            'X_TodayTotalDays = TimeSpan.FromTicks(DateTime.Today.Ticks).TotalDays
            'X_Totalminutes = (X_TheDateTime - DateTime.Today).TotalMinutes
            'X_DateTimeText = X_TheDateTime.ToString("yyyy-MMM-dd HH:mm:ss")

            'Dim dbTempMon As New TempMonitoringDBClass

            'dbTempMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
            '              Temperature, Humidity, ControlContact, _
            '              SetTemperatureHigh, SetTemperatureLow, SetHumidityHigh, SetHumidityLow, _
            '              "", TemperatureAlarm.ToString, HumidityAlarm.ToString, sProduct, sPeriod, DigitalInput, True)

            'dbTempMon.Dispose()

            Return True

        Catch ex As Exception
            WriteLog(ex.Message)
            LastDoPollControlUnitsException = ex
            Return False
        End Try

    End Function

#End Region

#End Region

#Region "Background process"

    Public Shared Sub ConnectDPCSBackground()
        Try
            Dim db As New ConfigDBClass
            db.UpdateCharValue("DPCSStatus", "Starting")
            db.Dispose()
            For Each p As Process In Process.GetProcesses
                If String.Compare(p.ProcessName.ToUpper, "DRY PLANT CONTROL BACKGROUND", True) = 0 Then
                    p.Kill()
                    Exit For
                End If
            Next
            Shell("Dry Plant Control Background.exe")
        Catch
        End Try
    End Sub

    Public Shared Sub DisconnectDPCSBackground()
        Try
            For Each p As Process In Process.GetProcesses
                If String.Compare(p.ProcessName.ToUpper, "DRY PLANT CONTROL BACKGROUND", True) = 0 Then
                    p.Kill()
                    Exit For
                End If
            Next
            Dim db As New ConfigDBClass
            db.UpdateCharValue("DPCSStatus", "Stopped", True)
            db.Dispose()
            Dim dbTemp As New TempMonitoringDBClass
            dbTemp.DeleteAll()
            dbTemp.Dispose()
        Catch ex As Exception
            'Throw ex
        End Try
    End Sub

    Public Shared Function DPCSBackgroundStatus() As String
        Try
            Dim ret As String = "NONE"
            Dim db As New ConfigDBClass
            ret = db.GetCharValue("DPCSStatus", True)
            db.Dispose()
            Return ret
        Catch ex As Exception
            Return "NONE"
        End Try
    End Function

#End Region

End Class

