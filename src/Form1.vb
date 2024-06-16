Imports System
Imports System.IO
Public Class Form1

    Private Path As String = String.Empty

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.Items.Add(GetUSB)
        ComboBox1.SelectedIndex = 0
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If RadioButton1.Checked = True Then
            Path = TextBox1.Text
        Else
            Path = ComboBox1.SelectedItem.ToString
        End If
        Try
            Cursor.Current = Cursors.WaitCursor
            ScanFiles(Path)
            Cursor.Current = Cursors.Default
            ProgressBar1.Value = 0
            MessageBox.Show("Đã thực hiện xong !")
        Catch ex As Exception
            MessageBox.Show("Không thể thực hiện. Mã lỗi {0}", ex.Message)
        End Try
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ComboBox1.Items.Clear()
        ComboBox1.Items.Add(GetUSB)
        ComboBox1.SelectedIndex = 0
    End Sub
    ''' <summary>
    ''' Tìm kiếm thiết bị USB
    ''' </summary>
    ''' <returns></returns>
    Private Function GetUSB() As String
        Dim result As String = String.Empty
        Try
            Dim drv As IEnumerable(Of DriveInfo) = From dr In DriveInfo.GetDrives() Where dr.IsReady = True AndAlso dr.DriveType = DriveType.Removable Select dr
            For Each dinfo As DriveInfo In drv
                result = dinfo.Name
            Next
        Catch ex As Exception
        End Try
        Return result
    End Function
    ''' <summary>
    ''' Xóa bỏ thuộc tính ẩn
    ''' </summary>
    ''' <param name="fsi"></param>
    Private Sub ResetAttribute(ByVal fsi As FileSystemInfo)
        Try
            'Thay đổi giá trị thuộc tính
            fsi.Attributes = fsi.Attributes And Not FileAttributes.Hidden
            fsi.Attributes = fsi.Attributes And Not FileAttributes.System
            fsi.Attributes = fsi.Attributes And Not FileAttributes.ReadOnly
            ProgressBar1.Value += 1
        Catch ex As Exception
            Return
        End Try
    End Sub
    Private Sub ScanFiles(ByVal currDir As String)
        If Not Directory.Exists(currDir) Then Return
        'Thiết lập giá trị tối đa cho progressbar
        ProgressBar1.Maximum += Directory.GetFileSystemEntries(currDir).Count()

        'Tìm kiếm tệp trong thư mục và xóa bỏ thuộc tính ẩn
        For Each ff As String In Directory.GetFiles(currDir)
            ResetAttribute(TryCast((New FileInfo(ff)), FileSystemInfo))
        Next

        For Each dr As String In Directory.GetDirectories(currDir)
            If Not Directory.Exists(dr) Then Return
            'Xóa bỏ thuộc tính ẩn của thư mục
            ResetAttribute(TryCast((New DirectoryInfo(dr)), FileSystemInfo))
            ScanFiles(dr) ' Tìm kiếm thư mục
        Next
    End Sub

End Class
