using Timer = System.Windows.Forms.Timer;

namespace AsyncTaskExample;

public partial class MainForm : Form
{
    public bool LongTaskProcessed { get; private set; }

    public MainForm()
    {
        InitializeComponent();


        // Настройка таймера для часов
       var _clockTimer = new Timer();
        _clockTimer.Interval = 1000; // Обновление каждую секунду
        _clockTimer.Tick += (s, e) =>
        {
            if (lblClock != null)
                lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
        };
        _clockTimer.Start();

        UpdateTaskStatus("Нет активной задачи"); // Изначальный статус
        
        btnStartWork.Enabled = false; // Изначально кнопка неактивна
        
        UpdateTaskStateLabel(null); // Сброс цвета метки
        this.Shown += async (s, e) => await StartMainTaskAsync();
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }



    private async void btnStartTask_Click(object sender, EventArgs e)
    {
        await StartMainTaskAsync();
    }
    private async Task StartMainTaskAsync()
    {
        if (LongTaskProcessed)
        {
            MessageBox.Show("Задача уже выполняется!");
            return;
        }

        var cts = new CancellationTokenSource();
        UpdateTaskStatus("Задача выполняется...");
        btnStartWork.Enabled = false; // Кнопка становится неактивной
        UpdateTaskStateLabel(null); // Сброс цвета метки

        using var progressForm = new ProgressForm(cts);

        progressForm.Show();

        try
        {
            bool result = await LongTaskAsync(progressForm, cts);

            if (result)
            {
                UpdateTaskStatus("Задача завершена.");
                MessageBox.Show("Задача завершена успешно!");
                btnStartWork.Enabled = true; // Кнопка активируется
                UpdateTaskStateLabel(Color.LightGreen); // Метка окрашивается в салатовый
            }
            else
            {
                UpdateTaskStatus("Задача завершена с ошибкой.");
                MessageBox.Show("Задача завершена с ошибкой.");
                UpdateTaskStateLabel(Color.Red); // Метка окрашивается в красный
            }
        }
        catch (OperationCanceledException)
        {
            UpdateTaskStatus("Задача прервана.");
            btnStartWork.Enabled = false; // Кнопка остаётся неактивной
            UpdateTaskStateLabel(Color.Red); // Метка окрашивается в красный
            MessageBox.Show("Задача была прервана.");

        }
        finally
        {
            if (LongTaskProcessed)
                LongTaskProcessed = !LongTaskProcessed;

            cts.Dispose();
        }

    }
    private async Task<bool> LongTaskAsync(ProgressForm progressForm, CancellationTokenSource cts)
    {
        try
        {
            LongTaskProcessed = true;
            for (int i = 1; i <= 100; i++)
            {
               
                if (LongTaskProcessed==false && cts.Token.IsCancellationRequested ==false)
                {
                   cts.Cancel();
                }
                cts.Token.ThrowIfCancellationRequested();
                progressForm.UpdateProgress(i); //  обновление прогресса
                await Task.Delay(100, cts.Token); // Имитация ввода/вывода
            }
            // Возвращаем результат выполнения задачи
            return new Random().Next(0, 2) == 1; // Случайный результат: true или false
        }
        finally
        {
            LongTaskProcessed = false;
        }
    }

    private void BtnCancelTask_Click(object sender, EventArgs e)
    {
        if (LongTaskProcessed)
            LongTaskProcessed= false;
    }

    private void UpdateTaskStatus(string status) // Новый метод
    {
        if (lblTaskStatus.InvokeRequired)
        {
            lblTaskStatus.Invoke(new Action(() => lblTaskStatus.Text = status));
        }
        else
        {
            lblTaskStatus.Text = status;
        }
    }

    private void UpdateTaskStateLabel(Color? color) // Новый метод
    {
        if (lblTaskState.InvokeRequired)
        {
            lblTaskState.Invoke(new Action(() =>
            {
                lblTaskState.BackColor = color ?? Color.Transparent;
            }));
        }
        else
        {
            lblTaskState.BackColor = color ?? Color.Transparent;
        }
    }
}
partial class MainForm
{
    //private System.ComponentModel.IContainer components = null;
    private Button btnStartTask;
    private Button btnCancelTask;
    private Label lblClock; //  часы
    private Label lblTaskStatus; //  статус задачи
    private Button btnStartWork; //"Начать работу"
    private Label lblTaskState; //  метка состояния

    private void InitializeComponent()
    {
        this.btnStartTask = new System.Windows.Forms.Button();
        this.btnCancelTask = new System.Windows.Forms.Button();
        this.lblClock = new System.Windows.Forms.Label();
        this.lblTaskStatus = new System.Windows.Forms.Label();
        this.btnStartWork = new System.Windows.Forms.Button();
        this.lblTaskState = new System.Windows.Forms.Label();
        this.SuspendLayout();

        // 
        // btnStartTask
        // 
        this.btnStartTask.Location = new System.Drawing.Point(12, 12);
        this.btnStartTask.Name = "btnStartTask";
        this.btnStartTask.Size = new System.Drawing.Size(150, 30);
        this.btnStartTask.TabIndex = 0;
        this.btnStartTask.Text = "Запустить задачу";
        this.btnStartTask.UseVisualStyleBackColor = true;
        this.btnStartTask.Click += new System.EventHandler(this.btnStartTask_Click);

        // 
        // btnCancelTask
        // 
        this.btnCancelTask.Location = new System.Drawing.Point(12, 50);
        this.btnCancelTask.Name = "btnCancelTask";
        this.btnCancelTask.Size = new System.Drawing.Size(150, 30);
        this.btnCancelTask.TabIndex = 1;
        this.btnCancelTask.Text = "Отменить задачу";
        this.btnCancelTask.UseVisualStyleBackColor = true;
        this.btnCancelTask.Click += new System.EventHandler(this.BtnCancelTask_Click);


        // lblClock
        // 
        this.lblClock.Location = new System.Drawing.Point(12, 90);
        this.lblClock.Name = "lblClock";
        this.lblClock.Size = new System.Drawing.Size(260, 20);
        this.lblClock.TabIndex = 2;
        this.lblClock.Text = "00:00:00"; // Изначальное значение часов

        // 
        // lblTaskStatus
        // 
        this.lblTaskStatus.Location = new System.Drawing.Point(12, 120); // Новый элемент ниже часов
        this.lblTaskStatus.Name = "lblTaskStatus";
        this.lblTaskStatus.Size = new System.Drawing.Size(260, 20);
        this.lblTaskStatus.TabIndex = 3;
        this.lblTaskStatus.Text = "Нет активной задачи";

        // 
        // btnStartWork
        // 
        this.btnStartWork.Location = new System.Drawing.Point(12, 150); // Размещение ниже статуса задачи
        this.btnStartWork.Name = "btnStartWork";
        this.btnStartWork.Size = new System.Drawing.Size(150, 30);
        this.btnStartWork.TabIndex = 4;
        this.btnStartWork.Text = "Начать работу";
        this.btnStartWork.UseVisualStyleBackColor = true;
        this.btnStartWork.Enabled = false; // Изначально кнопка неактивна

        // 
        // lblTaskState
        // 
        this.lblTaskState.Location = new System.Drawing.Point(12, 190); // Метка ниже кнопки
        this.lblTaskState.Name = "lblTaskState";
        this.lblTaskState.Size = new System.Drawing.Size(260, 30);
        this.lblTaskState.TabIndex = 5;
        this.lblTaskState.Text = " ";
        this.lblTaskState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

        // 
        // MainForm
        // 
        this.ClientSize = new System.Drawing.Size(284, 250);
        this.Controls.Add(this.lblTaskState); // Добавлена метка состояния
        this.Controls.Add(this.btnStartWork);
        this.Controls.Add(this.lblTaskStatus);
        this.Controls.Add(this.lblClock);
        this.Controls.Add(this.btnCancelTask);
        this.Controls.Add(this.btnStartTask);
        this.Name = "MainForm";
        this.Text = "Главная форма";
        this.ResumeLayout(false);
    }
}

