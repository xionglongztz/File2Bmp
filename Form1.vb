Imports System.IO
Public Class Form1
    Dim Filename As String
    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If Char.IsDigit(e.KeyChar) Or e.KeyChar = Chr(8) Then '检测0-9，退格键
            e.Handled = False '处理
        Else
            e.Handled = True '程序认为已经处理过了，于是不会处理
        End If '进行检测处理，禁止输入0-9以及退格键以外的东西
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim FileSelect As New OpenFileDialog '定义打开对话框
        FileSelect.Title = "选择一个文件..." '设置标题
        FileSelect.Filter = "所有文件(*.*)|*.*" '设置过滤器
        FileSelect.InitialDirectory = Application.StartupPath '程序路径
        FileSelect.Multiselect = False '禁止多选
        FileSelect.ShowDialog() '显示对话框
        If FileSelect.FileName <> "" Then '所选文件不为空
            Filename = FileSelect.FileName
            Label3.Text = Filename
            Button3.Enabled = True
            Button5.Enabled = True
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim SaveFileDialog1 As New SaveFileDialog() '创建一个保存对话框
        SaveFileDialog1.Filter = "BMP Files(*.bmp)|*.bmp" '设置扩展名
        SaveFileDialog1.FileName = "新文件频谱图1.bmp" '默认文件名
        SaveFileDialog1.Title = "将图片保存在..." '设置保存窗口标题
        If SaveFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then '如果确定保存
            Dim fs As FileStream '定义文件流
            Dim br As BinaryReader '二进制值
            Dim everystr As String '读取到的字符
            Dim PicY As Integer '图片高度
            Dim PicX As Integer '图片宽度
            Dim Nx, Ny As Integer '当前位置
            Nx = 0
            Ny = 0 '初值为0
            Try
                fs = New FileStream(Filename, FileMode.Open, FileAccess.Read) '打开，读取文件
                br = New BinaryReader(fs) '二进制文件流
                PicX = Val(TextBox1.Text) '宽度为输入值
                PicY = (br.BaseStream.Length \ Val(TextBox1.Text)) + 1 '高度为计算值，已向上取整
                Dim Image1 As Bitmap = New Bitmap(PicX, PicY) '定义图形处理类
                Dim Confirm As Integer '对话框确认参量
                Confirm = MsgBox("总像素点数：" & br.BaseStream.Length & vbCrLf & "图像分辨率（不足的行数已向上取整）：" & PicX & "(宽度) * " & PicY & "(高度)" & vbCrLf & "请确认磁盘有足够的剩余空间，若文件较大，可能花费较长时间，请耐心等待。" & vbCrLf & "点击是开始操作，否取消操作", vbYesNo + vbInformation)
                If Confirm = vbNo Then
                    Exit Sub '如果为no取消输出
                End If
                Text = "写入中..."
                While br.BaseStream.Position < br.BaseStream.Length '如果没读完就进入循环
                    everystr = Hex(br.ReadByte()) '读取对应的十六进制字符串
                    If Nx < PicX Then '判断当前x是否小于最大值
                        Image1.SetPixel(Nx, Ny, Int2Color(Dec(everystr))) '填充像素
                        Nx += 1
                    Else
                        Nx = 0
                        Ny += 1 '换行
                    End If
                End While
                Image1.Save(SaveFileDialog1.FileName) '存入文件
                fs.Close()
                br.Close() '关闭每个流
                Text = "File2Bmp"
                MsgBox("写入完成！", vbInformation)
            Catch Ex As Exception
                MsgBox("发生错误，未能成功写入数据...错误信息如下：" & vbCrLf & vbCrLf & Ex.Message, vbCritical)
            End Try
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim SaveFileDialog3 As New SaveFileDialog() '创建一个保存对话框
        SaveFileDialog3.Filter = "Text Files(*.txt)|*.txt" '设置扩展名
        SaveFileDialog3.FileName = "新文件字符集.txt" '默认文件名
        SaveFileDialog3.Title = "将十六进制字符集保存在..." '设置保存窗口标题
        If SaveFileDialog3.ShowDialog() = System.Windows.Forms.DialogResult.OK Then '如果确定保存
            Dim fs As FileStream '定义文件流
            Dim br As BinaryReader '二进制值
            Dim sw As StreamWriter '写入流
            Dim everystr As String '读取到的字符
            Try
                fs = New FileStream(Filename, FileMode.Open, FileAccess.Read) '打开，读取文件
                br = New BinaryReader(fs) '二进制文件流
                sw = New StreamWriter(SaveFileDialog3.FileName) '要写入的文件
                Text = "写入中..."
                While br.BaseStream.Position < br.BaseStream.Length '如果没读完就进入循环
                    everystr = Hex(br.ReadByte()) '读取对应的十六进制字符串
                    If everystr = "0" Then
                        everystr = "00" '将一位0转化为2位
                    End If
                    sw.Write(everystr) '写入到文件
                End While
                sw.Close()
                fs.Close()
                br.Close() '关闭每个流
                Text = "File2Bmp"
                MsgBox("写入完成！", vbInformation)
            Catch Ex As Exception
                MsgBox("发生错误，未能成功写入数据...错误信息如下：" & vbCrLf & vbCrLf & Ex.Message, vbCritical)
            End Try
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Button3.Enabled = False
        Button5.Enabled = False '启动前禁用按钮
    End Sub
    Private Function Dec(Hex As String) '十六进制转十进制函数
        Return Val("&H" & Hex & “&”)
    End Function
    Private Function Int2Color(Int As Integer) '将数值映射到对应的颜色上
        Dim R As Integer
        Dim G As Integer
        Dim B As Integer '蓝色是00，红色是FF，然后将0到255的所有数值映射到对应的颜色，再返回
        If Int >= 0 And Int <= 63 Then
            R = 0
            G = 4 * Int
            B = 255
        End If
        If Int >= 64 And Int <= 127 Then
            R = 0
            G = 255
            B = 255 - (Int - 64) * 4
        End If
        If Int >= 128 And Int <= 191 Then
            R = (Int - 128) * 4
            G = 255
            B = 0
        End If
        If Int >= 192 And Int <= 255 Then
            R = 255
            G = 255 - (Int - 192) * 4
            B = 0
        End If
        Return Color.FromArgb(R, G, B)
    End Function
    Private Sub Form1_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Try '支持拖放操作
            Dim filePath() As String = e.Data.GetData(DataFormats.FileDrop)
            For Each file As String In filePath
                'txtFilePath.Text = file
                If IO.Directory.Exists(file) Then '如果是目录
                    Exit Sub
                End If
                Filename = file
            Next
            Label3.Text = Filename
            Button3.Enabled = True
            Button5.Enabled = True
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub
    Private Sub Form1_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub '拖放操作
End Class
