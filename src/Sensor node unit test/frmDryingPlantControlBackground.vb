Imports DB_Callback
Imports System.Threading
Imports System.Text

Public Class frmDryingPlantControlBackground

    Public Delegate Sub SetHideDelegate()

    Public Delegate Sub SetText(ByVal text As String)

    Private Sub SetLabelText(ByVal text As String)
        If lbStatus.InvokeRequired Then
            lbStatus.Invoke(New SetText(AddressOf SetLabelText), text)
        Else
            lbStatus.Text = text
            Application.DoEvents()
        End If
    End Sub

    Private Sub SetHide()
        If Me.InvokeRequired Then
            Me.Invoke(New SetHideDelegate(AddressOf SetHide))
        Else
            Me.Hide()
            Application.DoEvents()
        End If
    End Sub

#Region "Backup time analysis"

    '---------------------------------------------------------------------
    ' Backup
    '---------------------------------------------------------------------
    'Private Sub BackupTimeAnalysis()
    '    Try
    '        If Not DoBackupFlag Then

    '            Dim dbMon As New MonitoringDBClass
    '            Dim dbConfig As New ConfigDBClass

    '            Dim currentDay As String = DateTime.Now.DayOfWeek.ToString
    '            Dim currentTimeOfDay As TimeSpan = DateTime.Now.TimeOfDay
    '            Dim ti2 As TimeSpan
    '            Dim minDiff As Integer
    '            Try : minDiff = CInt(dbConfig.GetIntValue("MonitoringInterval", True) / 100) : Catch : minDiff = 5 : End Try
    '            Dim sWeeklyBackupTime As String
    '            Try : sWeeklyBackupTime = dbConfig.GetCharValue("WeeklyBackupTime", True) : Catch : sWeeklyBackupTime = "19:40" : End Try
    '            Dim sWeeklyBackupDay As String
    '            Try : sWeeklyBackupDay = dbConfig.GetCharValue("WeeklyBackupDay", True) : Catch : sWeeklyBackupDay = "Sunday" : End Try

    '            dbConfig.Dispose()

    '            If sWeeklyBackupDay.ToUpper.Equals(currentDay.ToUpper) Then
    '                Dim asTime() As String = sWeeklyBackupTime.Split(":")
    '                Dim tbk As TimeSpan = New TimeSpan(CInt(asTime(0)), CInt(asTime(1)), 0)
    '                ti2 = tbk.Add(New TimeSpan(0, minDiff, 10)) 'less than 1 minute
    '                If tbk <= currentTimeOfDay AndAlso currentTimeOfDay <= ti2 Then
    '                    ' Check last analysis time 
    '                    If LastBackupTimeSpan < tbk Then
    '                        LastBackupTimeSpan = tbk
    '                        DoBackupFlag = True
    '                    Else
    '                        DoBackupFlag = False
    '                    End If
    '                End If
    '            End If

    '            dbMon.Dispose()

    '        End If

    '        System.Threading.Thread.Sleep(1000) 'for threading

    '    Catch ex As Exception
    '        Utilities.WriteLog("Backup time analysis: " & ex.Message)
    '    End Try
    'End Sub

    Private Sub DoBackup()
        Try
            If DoBackupFlag Then

                DoBackupFlag = False

                Dim dbMon As New MonitoringDBClass
                Dim dbConfig As New ConfigDBClass

                Dim sourceDbFile As String
                Try : sourceDbFile = dbConfig.GetCharValue("MonitoringDbFullFileName", True) : Catch : sourceDbFile = "C:\Drying Plant Control\Database\DryingPlantControlDB.sdf" : End Try
                Dim targetWeeklyBkPath As String
                Try : targetWeeklyBkPath = dbConfig.GetCharValue("WeeklyBackupPath", True) : Catch : targetWeeklyBkPath = "C:\Drying Plant Control\WeeklyBackup" : End Try
                Dim targetBigBkPath As String
                Try : targetBigBkPath = dbConfig.GetCharValue("BigSizeBackupPath", True) : Catch : targetBigBkPath = "C:\Drying Plant Control\BigSizeBackup" : End Try
                dbConfig.Dispose()

                'Small backup
                dbMon.WeeklyBackup(sourceDbFile, targetWeeklyBkPath, False)

                ' Big size backup and drop data
                Dim infoReader As System.IO.FileInfo
                infoReader = My.Computer.FileSystem.GetFileInfo(sourceDbFile)
                If 3700000000L < infoReader.Length Then
                    dbMon.BigSizeBackup(sourceDbFile, targetBigBkPath, False)
                End If

                dbMon.Dispose()
            End If

        Catch ex As Exception
            Utilities.WriteLog("Do backup: " & ex.Message)
        End Try
    End Sub

#End Region

#Region "Monitoring and Control Condition time analysis"

    Private timeSpanArray() As TimeSpan = {New TimeSpan(0, 0, 0), New TimeSpan(2, 0, 0), New TimeSpan(4, 0, 0), New TimeSpan(6, 0, 0), New TimeSpan(8, 0, 0), New TimeSpan(10, 0, 0), New TimeSpan(12, 0, 0), New TimeSpan(14, 0, 0), New TimeSpan(16, 0, 0), New TimeSpan(18, 0, 0), New TimeSpan(20, 0, 0), New TimeSpan(22, 0, 0)}
    Dim DoMonitoringFlag As Boolean = False
    Private DoCCFlag As Boolean = False
    Private LastCCTimeSpan As TimeSpan
    Private DoBackupFlag As Boolean = False
    Private LastBackupTimeSpan As TimeSpan


    '-------------------------------------------------------
    ' Time span array
    '-------------------------------------------------------
    Private Sub DefineCCTimeSpanArray()
        Try
            Dim dbConfig As New ConfigDBClass
            Dim sTimeSpanArray() As String = dbConfig.GetCharValue("TimeSpanArray", True).Split(",")
            ReDim timeSpanArray(sTimeSpanArray.Length - 1)
            Dim itime As Integer = 0
            For Each st As String In sTimeSpanArray
                Dim a() As String = st.Split(":")
                timeSpanArray(itime) = New TimeSpan(Convert.ToInt32(a(0)), Convert.ToInt32(a(1)), 0)
                itime += 1
            Next
            dbConfig.Dispose()
        Catch ex As Exception
            timeSpanArray = {New TimeSpan(0, 0, 0), New TimeSpan(2, 0, 0), New TimeSpan(4, 0, 0), New TimeSpan(6, 0, 0), New TimeSpan(8, 0, 0), New TimeSpan(10, 0, 0), New TimeSpan(12, 0, 0), New TimeSpan(14, 0, 0), New TimeSpan(16, 0, 0), New TimeSpan(18, 0, 0), New TimeSpan(20, 0, 0), New TimeSpan(22, 0, 0)}
        End Try
    End Sub

    Private Sub CCTimeAnalysis()
        Try
            While Not StopFlag

                If Not DoCCFlag Then

                    Dim currentTimeOfDay As TimeSpan = DateTime.Now.TimeOfDay
                    Dim ti2 As TimeSpan

                    For Each ti As TimeSpan In timeSpanArray
                        ti2 = ti.Add(New TimeSpan(0, 1, 0)) 'less than 1 minute
                        If ti <= currentTimeOfDay AndAlso currentTimeOfDay <= ti2 Then
                            If LastCCTimeSpan < ti Then
                                LastCCTimeSpan = ti
                                DoCCFlag = True

#If DEBUG Then
                                Debug.Print("CC Flag -> TRUE" & Environment.NewLine)
#End If

                                Exit For
                            Else
                                DoCCFlag = False

#If DEBUG Then
                                Debug.Print("CC Flag -> FALSE" & Environment.NewLine)
#End If

                            End If
                        End If
                    Next
                End If

                Thread.Sleep(900) ' For threading

            End While

        Catch ex0 As Exception
            Utilities.WriteLog("CC time analysis: " & ex0.Message)
        End Try

    End Sub

    Private Sub BackupTimeAnalysis()
        Try
            Dim dbConfig As New ConfigDBClass

            Dim currentDay As String = DateTime.Now.DayOfWeek.ToString
            Dim currentTimeOfDay As TimeSpan = DateTime.Now.TimeOfDay
            Dim ti2 As TimeSpan
            Dim minDiff As Integer
            Try : minDiff = CInt(dbConfig.GetIntValue("MonitoringInterval", True) / 100) : Catch : minDiff = 5 : End Try
            Dim sWeeklyBackupTime As String
            Try : sWeeklyBackupTime = dbConfig.GetCharValue("WeeklyBackupTime", True) : Catch : sWeeklyBackupTime = "19:40" : End Try
            Dim sWeeklyBackupDay As String
            Try : sWeeklyBackupDay = dbConfig.GetCharValue("WeeklyBackupDay", True) : Catch : sWeeklyBackupDay = "Sunday" : End Try

            dbConfig.Dispose()

            While Not StopFlag

                If Not DoBackupFlag Then

                    If sWeeklyBackupDay.ToUpper.Equals(currentDay.ToUpper) Then
                        Dim asTime() As String = sWeeklyBackupTime.Split(":")
                        Dim ti As TimeSpan = New TimeSpan(CInt(asTime(0)), CInt(asTime(1)), 0)
                        ti2 = ti.Add(New TimeSpan(0, 1, 0)) 'less than 1 minute
                        If ti <= currentTimeOfDay AndAlso currentTimeOfDay <= ti2 Then
                            ' Check last analysis time 
                            If LastBackupTimeSpan < ti Then
                                LastBackupTimeSpan = ti
                                DoBackupFlag = True
#If DEBUG Then
                                Debug.Print("Backup -> TRUE" & Environment.NewLine)
#End If

                            Else
                                DoBackupFlag = False
#If DEBUG Then
                                Debug.Print("Backup -> FALSE" & Environment.NewLine)
#End If

                            End If
                        End If
                    End If

                End If

                Thread.Sleep(900) ' For threading

            End While

        Catch ex0 As Exception
            Utilities.WriteLog("CC time analysis: " & ex0.Message)
        End Try

    End Sub

    Private Sub MonitoringTimeAnalysis()
        Try
            Dim dbConfig As New ConfigDBClass
            Dim minDiff As Integer
            Try : minDiff = CInt(dbConfig.GetIntValue("MonitoringInterval", True) / 100) : Catch : minDiff = 5 : End Try

            While Not StopFlag

                If Not DoMonitoringFlag Then

                    For sec As Integer = 1 To minDiff * 60
                        Thread.Sleep(1000)

#If DEBUG Then
                        Debug.Print("Monitoring tick" & Environment.NewLine)
#End If

                    Next

#If DEBUG Then
                    Debug.Print("Monitoring flag -> TRUE" & Environment.NewLine)
#End If

                    DoMonitoringFlag = True
                    Continue While

                End If

                Thread.Sleep(1000)

#If DEBUG Then
                Debug.Print("Wait for monitoring updating..." & Environment.NewLine)
#End If

            End While

        Catch ex0 As Exception
            Utilities.WriteLog("CC time analysis: " & ex0.Message)
        End Try

    End Sub

#End Region

#Region "New interfacing"

    Private StopFlag As Boolean
    Friend P1Commu1CtrlAJConnecting, P1Commu1CtrlAJRunning As Boolean
    Friend P1Commu2DispADConnecting, P1Commu2DispADRunning As Boolean
    Friend P1Commu3DispEJConnecting, P1Commu3DispEJRunning As Boolean
    Friend P2Commu1CtrlKMConnecting, P2Commu1CtrlKMRunning As Boolean
    Friend tP1Commu1CtrlAJRecon, tP1Commu2DispADRecon, tP1Commu3DispEJRecon, tP2Commu1CtrlKMRecon As Thread

    Private Sub ReconnectionTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
        If TypeOf sender Is System.Windows.Forms.Timer Then
            Dim t As System.Windows.Forms.Timer = sender
            t.Enabled = False
        End If
    End Sub

    Private Sub ConnectToP1Commu1CtrlAJ()
        P1Commu1CtrlAJConnecting = True

        Dim ReconnectionTimer As New System.Windows.Forms.Timer
        AddHandler ReconnectionTimer.Tick, AddressOf ReconnectionTimer_Tick
        ReconnectionTimer.Interval = 1000
        Dim Attempts = 10

        For i As Integer = 1 To Attempts
            If Utilities.ConnectP1Commu1CtrlAJPort() Then
                P1Commu1CtrlAJConnecting = False
                P1Commu1CtrlAJRunning = True
                Return
            End If

            ' Reset port timer
            ReconnectionTimer.Enabled = True
            While ReconnectionTimer.Enabled
                If StopFlag Then
                    P1Commu1CtrlAJConnecting = False
                    Return
                End If
                Application.DoEvents()
                Thread.Sleep(1)
            End While

        Next

        P1Commu1CtrlAJConnecting = False

    End Sub

    Private Sub ConnectToP1Commu2DispAD()
        P1Commu2DispADConnecting = True

        Dim ReconnectionTimer As New System.Windows.Forms.Timer
        AddHandler ReconnectionTimer.Tick, AddressOf ReconnectionTimer_Tick
        ReconnectionTimer.Interval = 1000
        Dim Attempts = 10

        For i As Integer = 1 To Attempts
            If Utilities.ConnectP1Commu2DispADPort() Then
                P1Commu2DispADConnecting = False
                P1Commu2DispADRunning = True
                Return
            End If

            ' Reset port timer
            ReconnectionTimer.Enabled = True
            While ReconnectionTimer.Enabled
                If StopFlag Then
                    P1Commu2DispADConnecting = False
                    Return
                End If
                Application.DoEvents()
                Thread.Sleep(1)
            End While

        Next

        P1Commu2DispADConnecting = False

    End Sub

    Private Sub ConnectToP1Commu3DispEJ()
        P1Commu3DispEJConnecting = True

        Dim ReconnectionTimer As New System.Windows.Forms.Timer
        AddHandler ReconnectionTimer.Tick, AddressOf ReconnectionTimer_Tick
        ReconnectionTimer.Interval = 1000
        Dim Attempts = 10

        For i As Integer = 1 To Attempts
            If Utilities.ConnectP1Commu3DispEJPort() Then
                P1Commu3DispEJConnecting = False
                P1Commu3DispEJRunning = True
                Return
            End If

            ' Reset port timer
            ReconnectionTimer.Enabled = True
            While ReconnectionTimer.Enabled
                If StopFlag Then
                    P1Commu3DispEJConnecting = False
                    Return
                End If
                Application.DoEvents()
                Thread.Sleep(1)
            End While

        Next

        P1Commu3DispEJConnecting = False

    End Sub

    Private Sub ConnectToP2Commu1CtrlKM()
        P2Commu1CtrlKMConnecting = True

        Dim ReconnectionTimer As New System.Windows.Forms.Timer
        AddHandler ReconnectionTimer.Tick, AddressOf ReconnectionTimer_Tick
        ReconnectionTimer.Interval = 1000
        Dim Attempts = 10

        For i As Integer = 1 To Attempts
            If Utilities.ConnectP2Commu1CtrlKMPort() Then
                P2Commu1CtrlKMConnecting = False
                P2Commu1CtrlKMRunning = True
                Return
            End If

            ' Reset port timer
            ReconnectionTimer.Enabled = True
            While ReconnectionTimer.Enabled
                If StopFlag Then
                    P2Commu1CtrlKMConnecting = False
                    Return
                End If
                Application.DoEvents()
                Thread.Sleep(1)
            End While

        Next

        P2Commu1CtrlKMConnecting = False

    End Sub

    Private swPause As New Stopwatch
    Private Sub Pause(ByVal ms As Integer)
        swPause.Reset()
        swPause.Start()
        Do
            If StopFlag Then Return
            Application.DoEvents()
        Loop Until swPause.ElapsedMilliseconds > ms
        swPause.Stop()
    End Sub

    Private Sub ReconnectP1Commu1CtrlAJ()
        While Not P1Commu1CtrlAJRunning
            ConnectToP1Commu1CtrlAJ()
            Pause(500)
            If StopFlag Then Return
        End While
    End Sub

    Private Sub ReconnectP1Commu2DispAD()
        While Not P1Commu2DispADRunning
            ConnectToP1Commu2DispAD()
            Pause(500)
            If StopFlag Then Return
        End While
    End Sub

    Private Sub ReconnectP1Commu3DispEJ()
        While Not P1Commu3DispEJRunning
            ConnectToP1Commu3DispEJ()
            Pause(500)
            If StopFlag Then Return
        End While
    End Sub

    Private Sub ReconnectP2Commu1CtrlKM()
        While Not P2Commu1CtrlKMRunning
            ConnectToP2Commu1CtrlKM()
            Pause(500)
            If StopFlag Then Return
        End While
    End Sub

    Private Sub MainLoop()
        Try
            Dim TEST As Boolean = False

            SetLabelText("Connecting...")

            '---------------------------------------------------------------
            ' Check utilities data and path
            ' D:\Drying Plant Control
            ' D:\Drying Plant Control\Log
            ' D:\Drying Plant Control\Database
            ' D:\Drying Plant Control\Database\DryingPlantControlDB.sdf
            ' D:\Drying Plant Control\Database\Kenmin.sdf
            '---------------------------------------------------------------
            Utilities.CheckSystemDataAndPath()

            '---------------------------------------------------------------
            ' Initial startup flag
            '---------------------------------------------------------------
            StopFlag = False
            P1Commu1CtrlAJRunning = False
            P1Commu2DispADRunning = False
            P1Commu3DispEJRunning = False
            P2Commu1CtrlKMRunning = False

            '---------------------------------------------------------------
            ' Connect to ports: Start communication
            '---------------------------------------------------------------
            If TEST Then GoTo A

            ConnectToP1Commu1CtrlAJ()
            ConnectToP1Commu2DispAD()
            ConnectToP1Commu3DispEJ()
            ConnectToP2Commu1CtrlKM()

            '---------------------------------------------------------------
            ' Check connection fault and EXIT THREAD
            '---------------------------------------------------------------
            If (Not P1Commu1CtrlAJRunning Or P1Commu1CtrlAJConnecting) AndAlso _
                (Not P1Commu2DispADRunning Or P1Commu2DispADConnecting) AndAlso _
                (Not P1Commu3DispEJRunning Or P1Commu3DispEJConnecting) AndAlso _
                (Not P2Commu1CtrlKMRunning Or P2Commu1CtrlKMConnecting) Then

                Try
                    Dim dbConfigA As New ConfigDBClass
                    dbConfigA.UpdateCharValue("DPCSStatus", "Stopped", True)
                    dbConfigA.Dispose()
                Catch ex2 As Exception
                    Utilities.WriteLog(ex2.Message)
                End Try
                MsgBox("Cannot connect to both control units and display units! Background process going to exit thread.", MsgBoxStyle.Exclamation, "Background process")
                Utilities.WriteLog("Cannot connect to both control units and display units! Background process going to exit thread.")
                Application.ExitThread()

            End If

A:

            '---------------------------------------------------------------
            ' Initial time parameters
            '---------------------------------------------------------------
            Dim dbConfig As New ConfigDBClass
            dbConfig.UpdateCharValue("DPCSStatus", "Runing", True)
            dbConfig.Dispose()

            ' Set hidden
            Dim HideFlag As Boolean
            Dim db As New ConfigDBClass
            If db.GetCharValue("HideBackground") = "Y" Then HideFlag = True Else HideFlag = False
            db.Dispose()
            If HideFlag And Me.Visible = True Then SetHide()

            ' Polling loop
            Dim dbUnit As New UnitDBClass
            Dim dbPage As New PageDbClass
            Dim dbDailyCCTime As New DailyCCTimeDBClass 'ฐานข้อมูล CC Config สำหรับจดจำและปรับปรุงสถานะการบันทึก 2hr

            Dim dtUnit As DataTable = dbUnit.SelectAll(True).Tables(0)
            Dim dtAlarmUnit As DataTable = dbUnit.SelectAlarmUnit(True).Tables(0)
            Dim dtPages As DataTable = dbPage.SelectAll(True).Tables(0)
            dbPage.Dispose()

            Dim sProduct, sPeriod, sUnit, sUType, sAddress485, LinkDI, LinkAddress485 As String
            Dim iUnit, iPageID As Integer
            Dim lPeriodID As Long
            Dim tempOffset As Double, humidityOffset As Double

            Dim CanSaveMonitoring As Boolean = False

            Dim HasP1Commu1CtrlAJFault As Boolean = False
            Dim HasP2Commu1CtrlKMFault As Boolean = False

            Dim StatM3Temperature As New Numerical.MovingStatistics
            StatM3Temperature.StdevLimit = 3
            StatM3Temperature.Period = 10
            Dim StatM3Humidity As New Numerical.MovingStatistics
            StatM3Humidity.StdevLimit = 13
            StatM3Humidity.Period = 10

            While Not StopFlag

                '---------------------------------------------------------------------------------------------------------------------------------------------------------------------
                ' CONTROL AND MONITORING UNITS:-
                ' if running and !connecting and !doPoll for control units
                ' -> running = false;
                ' -> start reconnect thread
                '---------------------------------------------------------------------------------------------------------------------------------------------------------------------
                HasP1Commu1CtrlAJFault = False 'ใช้จำความผิดพลาดของชุดควบคุม A-J
                HasP2Commu1CtrlKMFault = False 'ใช้จำความผิดพลาดของชุดควบคุม K-M

                '-------------------------------------------------------
                ' Period id for all query
                ' ใช้ส่งข้อมูลให้ฟังก์ชันอื่น ๆ
                '-------------------------------------------------------
                Try
                    lPeriodID = Utilities.PeriodCheck(sPeriod)
                Catch ex As Exception
                    sPeriod = "NONE"
                End Try

                '-------------------------------------------------------
                ' Query each unit
                ' 
                '-------------------------------------------------------
                Dim CcPageRecord As List(Of Integer) = New List(Of Integer)

                '-------------------------------------------------------
                ' 17 ก.พ. 2559
                ' เพิ่ม CanMonitoring / รอให้ MonitoringTimeAnalysis เปิดให้ทำงาน
                '-------------------------------------------------------
                If DoMonitoringFlag Then
                    CanSaveMonitoring = True
                Else
                    CanSaveMonitoring = False
                End If

                '-------------------------------------------------------
                ' START Query each unit
                '-------------------------------------------------------
                For Each r As DataRow In dtUnit.Rows

                    If r("Enable") = "Y" Then

                        '-------------------------------------
                        ' Load unit type
                        '-------------------------------------
                        Try
                            sUnit = r("Type")
                            sUType = sUnit.ToString.Substring(0, 1)
                            iUnit = CInt(r("Type").ToString.Substring(1, 1))
                            sAddress485 = r("Address485")
                            If sUType = "C" Then
                                sProduct = r("CurrentProduct")
                                LinkDI = r("LinkUID")
                                LinkAddress485 = dbUnit.SelectByType(LinkDI).Tables(0).Rows(0)("Address485")
                            Else
                                sProduct = ""
                                LinkDI = ""
                                LinkAddress485 = ""
                            End If
                            Try
                                iPageID = r("PageID")
                            Catch ex As Exception
                                iPageID = -1
                            End Try
                            tempOffset = 0 : humidityOffset = 0
                            dbUnit.GetMonitoringOffset(sUnit, tempOffset, humidityOffset)
                        Catch ex As Exception
                            Throw ex
                        End Try

                        ' ------------------------------------------------------------------
                        ' Do poll control and monitoring
                        ' ------------------------------------------------------------------
                        If TEST Then GoTo B

                        Select Case iPageID
                            Case 1, 2 'A-D, E-J
                                '-------------------------------------
                                ' Query data from devices
                                '-------------------------------------
                                Select Case sUType.ToUpper
                                    Case "C"
                                        If P1Commu1CtrlAJRunning AndAlso Not P1Commu1CtrlAJConnecting AndAlso Not Utilities.DoPollControlUnitsP1AJ(sProduct, sPeriod, sUnit, sAddress485, LinkDI, LinkAddress485, tempOffset, humidityOffset) Then
                                            'HasP1Commu1CtrlAJFault = True
                                            If Not StopFlag Then
                                                P1Commu1CtrlAJRunning = False
                                                tP1Commu1CtrlAJRecon = New Thread(AddressOf ReconnectP1Commu1CtrlAJ)
                                                tP1Commu1CtrlAJRecon.Start()
                                            End If
                                        End If

                                    Case "M"
                                        If P1Commu1CtrlAJRunning AndAlso Not P1Commu1CtrlAJConnecting AndAlso Not Utilities.DoPollMonitorUnitsP1AJ(sProduct, sPeriod, sUnit, sAddress485) Then
                                            HasP1Commu1CtrlAJFault = True
                                            If Not StopFlag Then
                                                P1Commu1CtrlAJRunning = False
                                                tP1Commu1CtrlAJRecon = New Thread(AddressOf ReconnectP1Commu1CtrlAJ)
                                                tP1Commu1CtrlAJRecon.Start()
                                            End If
                                        End If

                                End Select

                            Case 3 'K-M
                                '-------------------------------------
                                ' Query data from devices
                                '-------------------------------------
                                Select Case sUType.ToUpper
                                    Case "C"
                                        If P2Commu1CtrlKMRunning AndAlso Not P2Commu1CtrlKMConnecting AndAlso Not Utilities.DoPollControlUnitsP2KM(sProduct, sPeriod, sUnit, sAddress485, LinkDI, LinkAddress485, tempOffset, humidityOffset) Then
                                            'HasP2Commu1CtrlKMFault = True
                                            If Not StopFlag Then
                                                P2Commu1CtrlKMRunning = False
                                                tP2Commu1CtrlKMRecon = New Thread(AddressOf ReconnectP2Commu1CtrlKM)
                                                tP2Commu1CtrlKMRecon.Start()
                                            End If
                                        End If

                                    Case "M"
                                        If P2Commu1CtrlKMRunning AndAlso Not P2Commu1CtrlKMConnecting AndAlso Not Utilities.DoPollMonitoringP2KM(sProduct, sPeriod, sUnit, sAddress485) Then
                                            'HasP2Commu1CtrlKMFault = True
                                            If Not StopFlag Then
                                                P2Commu1CtrlKMRunning = False
                                                tP2Commu1CtrlKMRecon = New Thread(AddressOf ReconnectP2Commu1CtrlKM)
                                                tP2Commu1CtrlKMRecon.Start()
                                            End If
                                        End If

                                End Select

                        End Select

B:

                        ' ------------------------------------------------------------------
                        ' Determine statistics of control and monitoring
                        ' ------------------------------------------------------------------
                        Dim X_TheDateTime As DateTime
                        Dim X_DateTimeText As String
                        Dim X_TodayTotalDays As Long
                        Dim X_Totalminutes As Double

                        X_TheDateTime = DateTime.Now
                        X_TodayTotalDays = TimeSpan.FromTicks(DateTime.Today.Ticks).TotalDays
                        X_Totalminutes = (X_TheDateTime - DateTime.Today).TotalMinutes
                        X_DateTimeText = X_TheDateTime.ToString("yyyy-MMM-dd HH:mm:ss")

                        Utilities.X_TheDateTime = X_TheDateTime
                        Utilities.X_TodayTotalDays = X_TodayTotalDays
                        Utilities.X_Totalminutes = X_Totalminutes
                        Utilities.X_DateTimeText = X_DateTimeText

                        '---------------------------------------------------------------------
                        ' Insert database
                        '---------------------------------------------------------------------
                        Dim dbTempMon As New TempMonitoringDBClass
                        Select Case sUnit
                            Case "C1", "C2", "C3", "C4", "C5", "C6"
                                dbTempMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
                                              Utilities.Temperature, Utilities.Humidity, Utilities.ControlContact, _
                                              Utilities.SetTemperatureHigh, Utilities.SetTemperatureLow, Utilities.SetHumidityHigh, Utilities.SetHumidityLow, _
                                              "", Utilities.TemperatureAlarm.ToString, Utilities.HumidityAlarm.ToString, sProduct, sPeriod, Utilities.DigitalInput, True)

                                Utilities.WriteLog("Insert temporary monitoring data." & Environment.NewLine)

                            Case "M1", "M2"
                                dbTempMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
                                              Utilities.Temperature, Utilities.Humidity, Utilities.ControlContact, _
                                              Utilities.SetTemperatureHigh, Utilities.SetTemperatureLow, Utilities.SetHumidityHigh, Utilities.SetHumidityLow, _
                                              "", Utilities.TemperatureAlarm.ToString, Utilities.HumidityAlarm.ToString, sProduct, sPeriod, Utilities.DigitalInput, True)

                                Utilities.WriteLog("Insert temporary monitoring data." & Environment.NewLine)

                            Case "C7", "C8", "C9"
                                dbTempMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
                                              Utilities.Temperature, Utilities.Humidity, Utilities.ControlContact, _
                                              Utilities.SetTemperatureHigh, Utilities.SetTemperatureLow, Utilities.SetHumidityHigh, Utilities.SetHumidityLow, _
                                              "", Utilities.TemperatureAlarm.ToString, Utilities.HumidityAlarm.ToString, sProduct, sPeriod, Utilities.DigitalInput, True)

                                Utilities.WriteLog("Insert temporary monitoring data." & Environment.NewLine)

                                'Case "M3"
                                '    'Utilities.WriteLog("Statistics..." & Environment.NewLine)
                                '    'StatM3Temperature.Push(Utilities.Temperature)
                                '    'StatM3Humidity.Push(Utilities.Humidity)
                                '    'Utilities.WriteLog(StatM3Temperature.ToString & Environment.NewLine)
                                '    'Utilities.WriteLog(StatM3Humidity.ToString & Environment.NewLine)

                                '    dbTempMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
                                '                  StatM3Temperature.LastData, StatM3Humidity.LastData, Utilities.ControlContact, _
                                '                  Utilities.SetTemperatureHigh, Utilities.SetTemperatureLow, Utilities.SetHumidityHigh, Utilities.SetHumidityLow, _
                                '                  "", Utilities.TemperatureAlarm.ToString, Utilities.HumidityAlarm.ToString, sProduct, sPeriod, Utilities.DigitalInput, True)

                                '    Utilities.WriteLog("Insert temporary monitoring data." & Environment.NewLine)

                            Case "M3", "M4"
                                dbTempMon.InsertData(sUnit, X_TheDateTime, X_DateTimeText, X_TodayTotalDays, X_Totalminutes, _
                                              Utilities.Temperature, Utilities.Humidity, Utilities.ControlContact, _
                                              Utilities.SetTemperatureHigh, Utilities.SetTemperatureLow, Utilities.SetHumidityHigh, Utilities.SetHumidityLow, _
                                              "", Utilities.TemperatureAlarm.ToString, Utilities.HumidityAlarm.ToString, sProduct, sPeriod, Utilities.DigitalInput, True)

                                Utilities.WriteLog("Insert temporary monitoring data." & Environment.NewLine)

                        End Select

                        dbTempMon.Dispose()

                        '-------------------------------------
                        ' บันทึกตังแต่ตัวแรก ทำทีละตัว ไปเรื่อยๆ
                        '-------------------------------------
                        If CanSaveMonitoring Then
                            '-------------------------------------
                            ' Update to MONITORING
                            '-------------------------------------
                            Utilities.WriteLog("MONITORING UPDATING..." & Environment.NewLine)
                            Utilities.SaveMonitoringFrom(sUnit, sUType, sProduct, sPeriod, lPeriodID)

                            '-------------------------------------
                            ' Update to CC
                            '-------------------------------------
                            Dim CCModel As CCConfigModel = dbDailyCCTime.SelectNext

                            '-------------------------------------
                            ' เมื่อข้อมูลพึ่งจะถูกเริ่มเขียนจะยังไม่มีข้อมูลต้นขั้ว จึงต้อง update ข้อมูล
                            '-------------------------------------
                            If CCModel.LAST_LOG_STATE = 0 Then
                                CCModel.PERIOD_ID = lPeriodID
                                CCModel.PERIOD = sPeriod
                                Dim ccid As Long
                                Dim dbccid As New ConfigDBClass
                                Try
                                    ccid = dbccid.GetIntValue("CCID")
                                Catch ex As Exception
                                    ccid = 0
                                End Try
                                dbccid.Dispose()
                                CCModel.CCID = ccid
                            End If

                            '-------------------------------------
                            ' ตรวจสอบค่าเวลาจริงกับค่าตั้ง เพื่อ
                            '    (1) บันทึก CC ลงตาราง ค่อยๆ ทำไปจนครบทุก sensor
                            '    (2) ปรับปรุงค่า ฐานข้อมูล CCConfig เพื่อให้จำค่าไว้ในฐานข้อมูล
                            ' ถ้าเวลายังไม่ถึงเวลาตั้งก็ไม่มีการบันทึกและปรับปรุงดังกล่าว
                            '-------------------------------------
                            Dim currentTimeOfDay As TimeSpan = X_TheDateTime.TimeOfDay
                            Dim aTIME_ID() As String = CCModel.TIME_ID.Split(":")
                            Dim ccTime As TimeSpan = New TimeSpan(CInt(Val(aTIME_ID(0))), CInt(Val(aTIME_ID(1))), 0)
                            If ccTime <= currentTimeOfDay Then
                                ' --------------------------------------------------------------
                                ' บันทึกตามดัชนี LAST_LOG_STATE = 
                                ' {0-5} ==> {C1-C6}, 
                                ' {6}   ==> {M2}, 
                                ' {7-9} ==> {C7-C9}
                                ' --------------------------------------------------------------
                                Dim IsCCUpdated As Boolean = False
                                Select Case CCModel.LAST_LOG_STATE
                                    Case 0 'C1
                                        If sUnit = "C1" Then Debug.WriteLine("CC:C1") : IsCCUpdated = True : Utilities.WriteLog("CC:C1")
                                    Case 1 'C2
                                        If sUnit = "C2" Then Debug.WriteLine("CC:C2") : IsCCUpdated = True : Utilities.WriteLog("CC:C2")
                                    Case 2 'C3
                                        If sUnit = "C3" Then Debug.WriteLine("CC:C3") : IsCCUpdated = True : Utilities.WriteLog("CC:C3")
                                    Case 3 'C4
                                        If sUnit = "C4" Then Debug.WriteLine("CC:C4") : IsCCUpdated = True : Utilities.WriteLog("CC:C4")
                                    Case 4 'C5
                                        If sUnit = "C5" Then Debug.WriteLine("CC:C5") : IsCCUpdated = True : Utilities.WriteLog("CC:C5")
                                    Case 5 'C6
                                        If sUnit = "C6" Then Debug.WriteLine("CC:C6") : IsCCUpdated = True : Utilities.WriteLog("CC:C6")
                                    Case 6 'M2
                                        If sUnit = "M2" Then Debug.WriteLine("CC:M2") : IsCCUpdated = True : Utilities.WriteLog("CC:M2")
                                    Case 7 'C7
                                        If sUnit = "C7" Then Debug.WriteLine("CC:C7") : IsCCUpdated = True : Utilities.WriteLog("CC:C7")
                                    Case 8 'C8
                                        If sUnit = "C8" Then Debug.WriteLine("CC:C8") : IsCCUpdated = True : Utilities.WriteLog("CC:C8")
                                    Case 9 'C9
                                        If sUnit = "C9" Then Debug.WriteLine("CC:C9") : IsCCUpdated = True : Utilities.WriteLog("CC:C9")
                                End Select

                                ' --------------------------------------------------------------
                                ' ปรับปรุงข้อมูลของตาราง tbCCConfig
                                ' --------------------------------------------------------------
                                If IsCCUpdated Then
                                    CCModel.LAST_LOG_STATE += 1
                                    dbDailyCCTime.UpdateCC(CCModel)
                                End If
                            End If

                        End If ' CanSaveMonitoring

                    End If ' If r("Enable") = "Y" Then

                Next 'Each control unit

                '' Restart Control Network
                'If HasP1Commu1CtrlAJFault Then
                '    If Not StopFlag Then
                '        P1Commu1CtrlAJRunning = False
                '        tP1Commu1CtrlAJRecon = New Thread(AddressOf ReconnectP1Commu1CtrlAJ)
                '        tP1Commu1CtrlAJRecon.Start()
                '    End If
                'End If

                '' Restart Control Network
                'If HasP2Commu1CtrlKMFault Then
                '    If Not StopFlag Then
                '        P2Commu1CtrlKMRunning = False
                '        tP2Commu1CtrlKMRecon = New Thread(AddressOf ReconnectP2Commu1CtrlKM)
                '        tP2Commu1CtrlKMRecon.Start()
                '    End If
                'End If

                If CanSaveMonitoring Then
                    ' Reset monitoring updating flag
                    DoMonitoringFlag = False

                End If

                '----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                ' ALARM UNITS:-
                ' if running and !connecting and !doPoll for ALARM UNITS
                ' -> running = false;
                ' -> start reconnect thread
                '----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                For Each r As DataRow In dtAlarmUnit.Rows
                    Try
                        LinkDI = r("LinkUID")
                        sAddress485 = r("Address485")
                    Catch ex As Exception
                        Throw ex
                    End Try

                    If CInt(r("PageID")) = 1 Or CInt(r("PageID")) = 2 Then

                        If P1Commu1CtrlAJRunning AndAlso Not P1Commu1CtrlAJConnecting AndAlso Not Utilities.DoPollAlarmAJUnits(LinkDI, sAddress485) Then
                            If Not StopFlag Then
                                P1Commu1CtrlAJRunning = False
                                tP1Commu1CtrlAJRecon = New Thread(AddressOf ReconnectP1Commu1CtrlAJ)
                                tP1Commu1CtrlAJRecon.Start()
                            End If
                        End If
                    End If

                    If CInt(r("PageID")) = 3 Then
                        If P2Commu1CtrlKMRunning AndAlso Not P2Commu1CtrlKMConnecting AndAlso Not Utilities.DoPollAlarmKMUnits(LinkDI, sAddress485) Then
                            If Not StopFlag Then
                                P2Commu1CtrlKMRunning = False
                                tP2Commu1CtrlKMRecon = New Thread(AddressOf ReconnectP2Commu1CtrlKM)
                                tP2Commu1CtrlKMRecon.Start()
                            End If
                        End If
                    End If
                Next

                '----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                ' DISPLAY UNITS:-
                ' if running and !connecting and !doPoll for DISPLAY UNITS
                ' -> running = false;
                ' -> start reconnect thread
                '----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                If P1Commu2DispADRunning AndAlso Not P1Commu2DispADConnecting AndAlso Not Utilities.DoPollDisplayADUnits Then
                    If Not StopFlag Then
                        P1Commu2DispADRunning = False
                        tP1Commu2DispADRecon = New Thread(AddressOf ReconnectP1Commu2DispAD)
                        tP1Commu2DispADRecon.Start()
                    End If
                End If
                If P1Commu3DispEJRunning AndAlso Not P1Commu3DispEJConnecting AndAlso Not Utilities.DoPollDisplayEJUnits Then
                    If Not StopFlag Then
                        P1Commu3DispEJRunning = False
                        tP1Commu3DispEJRecon = New Thread(AddressOf ReconnectP1Commu3DispEJ)
                        tP1Commu3DispEJRecon.Start()
                    End If
                End If


                '----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                ' BACKUP
                '----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                If DoBackupFlag Then
                    DoBackupFlag = False
                    DoBackup()
                End If

            End While '-------------------------------------------------------------------------------------------------------------- END WHILE ---------------------------------

        Catch ex As Exception
            Utilities.WriteLog(ex.Message)
            Try
                Dim dbConfig As New ConfigDBClass
                dbConfig.UpdateCharValue("DPCSStatus", ex.Message, True)
                dbConfig.Dispose()
            Catch ex2 As Exception
                Utilities.WriteLog(ex2.Message)
            End Try

            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Background main loop error")
            End ' TERMINATE
        End Try

        Try
            Dim dbConfigA As New ConfigDBClass
            dbConfigA.UpdateCharValue("DPCSStatus", "Stopped", True)
            dbConfigA.Dispose()
        Catch ex2 As Exception
            Utilities.WriteLog(ex2.Message)
        End Try

        Utilities.WriteLog("Mainloop has been stopped!")
        End ' TERMINATE

    End Sub

#End Region

    Private Sub AskAndQuit()
        Try
            If MsgBox("Do you want to quit program?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, "Quit") = MsgBoxResult.Yes Then
                Close()
            End If
        Catch ex As Exception
            'Do nothing
        End Try
    End Sub

    Private Sub pn_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pn.DoubleClick
        AskAndQuit()
    End Sub

    Private Sub lbStatus_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbStatus.DoubleClick
        AskAndQuit()
    End Sub

    Private Sub lbTitle_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbTitle.DoubleClick
        AskAndQuit()
    End Sub

    Private Sub frmDryingPlantControlBackground_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            StopFlag = True
        Catch ex As Exception

        End Try
    End Sub

    Dim tMainLoop As System.Threading.Thread
    Dim tMonitoringAnalysis As System.Threading.Thread
    Dim tCCTimeAnalysis As System.Threading.Thread
    Dim tBackupAnalysis As System.Threading.Thread

    Private Sub frmDryingPlantControlBackground_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Show()

        tMainLoop = New System.Threading.Thread(AddressOf MainLoop)
        tMainLoop.Start()

        tMonitoringAnalysis = New System.Threading.Thread(AddressOf MonitoringTimeAnalysis)
        tMonitoringAnalysis.Start()

        'DefineCCTimeSpanArray()     --------------------------------------------------BUG
        'tCCTimeAnalysis = New System.Threading.Thread(AddressOf CCTimeAnalysis)
        'tCCTimeAnalysis.Start()

        tBackupAnalysis = New System.Threading.Thread(AddressOf BackupTimeAnalysis)
        tBackupAnalysis.Start()
    End Sub

End Class


